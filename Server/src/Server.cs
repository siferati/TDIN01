using Common;
using Common.src;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using static Common.Order;


namespace Server
{
    /// <summary>
    /// Represents the server.
    /// </summary>
    public class Server : MarshalByRefObject, IServer
    {

        /// <summary>
        /// Server entry point.
        /// </summary>
        /// <param name="args">Arguments passed through the terminal.</param>
        public static void Main(string[] args)
        {

            Console.WriteLine("Starting...");
            

            // register the channel
            TcpChannel ch = new TcpChannel(9000);
            ChannelServices.RegisterChannel(ch, false);

            // register remote object
            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(Server), "Server.Rem",
                WellKnownObjectMode.Singleton
            );
            RemotingServices.Connect(typeof(Server), "tcp://localhost:" + 9000 + "/Server.Rem");
            Console.WriteLine("Waiting for a client to connect...");
            Console.WriteLine("Pess any key to shutdown the server.");
            Console.ReadLine();
        }


        /* --- ATTRIBUTES --- */

        /// <summary>
        /// Path to the database creation script (.sql).
        /// </summary>
        public const string DB_INPATH = "../../db/db.sql";

        /// <summary>
        /// Path of the database file (.sqlite).
        /// </summary>
        public const string DB_OUTPATH = "../../db/db.sqlite";

        /// <summary>
        /// The database.
        /// </summary>
        private DB db;

        /// <summary>
        /// The current quote of diginotes.
        /// </summary>
        public double Quote { get; set; }

        /// <summary>
        /// The current users logged
        /// </summary>
        Hashtable users_logged = new Hashtable();

        /* --- METHODS --- */

        /// <summary>
        /// Returns an instance of Server.
        /// </summary>
        public Server()
        {
            Log("Waking up...");

            // init database
            db = new DB(DB_INPATH, DB_OUTPATH, this);
            db.restoreAvailable();      // in case server crashs, orders my be suspended forever
            Quote = db.GetQuote();
        }


        /// <summary>
        /// Free resources once object is about to be destroyed.
        /// </summary>
        ~Server()
        {
            Log("Shutting off...");
            db.Dispose();
        }

        
        /// <summary>
        /// Setter.
        /// </summary>
        /// <param name="quote"></param>
        /// <returns></returns>
        public bool SetQuote(double quote, OrderType type)
        {
            Log("Updated quote from " + Quote + " to " + quote);

            

            foreach (DictionaryEntry pair in users_logged)
            {
                IClient rem = (IClient)RemotingServices.Connect(typeof(IClient), pair.Value.ToString());
                rem.updateQuote(quote);
            }

            if( (type == OrderType.Selling && quote < this.Quote) || (type == OrderType.Purchase && quote > this.Quote))
                Task.Run(() => ReviewPendingOrders(quote, type));

            this.Quote = quote;

            return db.InsertQuote(quote);
        }


        private System.Threading.Timer timer;

        /// <summary>
        /// When quote is changed, pending orders quote must be updated
        /// </summary>
        /// <param name="quote">New quote</param>
        /// <param name="type">Selling or Purchase</param>
        /// <returns></returns>
        private void ReviewPendingOrders(double quote, OrderType type)
        {
            Console.WriteLine("All pending orders of " + (type == OrderType.Selling ? "selling" : "purchase") + " must be updated");
            
            List<Order> orders = db.GetPendingOrders(false);

            foreach(Order order in orders)
            {
                if(order.Type == type)
                {
                    long available = order.Available + 1;

                    db.UpdateAvailableOrder(type, order.Id, available);

                    // notify user
                    if (users_logged.ContainsKey("" + order.UserId))
                    {
                        IClient rem = (IClient)RemotingServices.Connect(typeof(IClient),users_logged["" + order.UserId].ToString());
                        rem.pendingOrderSuspended(type, "" + order.Id);

                        this.timer = new System.Threading.Timer(x =>
                        {
                            this.handlePendingOrdersTimeOut(type, order.Id);
                        }, null, 60*1000, Timeout.Infinite);
                    }


                    
                        
                     
                }

            }
            

        }


        private void handlePendingOrdersTimeOut(OrderType type, long orderId)
        {
            Console.WriteLine("Passou um segundo na ordem: " + orderId);
            List<Order> orders = db.GetPendingOrders( false);
            foreach (Order order in orders)
            {
                if(order.Id == orderId && order.Type == type)
                {
                    Console.WriteLine("Ordem: " + orderId + "  available: " + order.Available);
                    long available = order.Available - 1;
                    if (available < 0) available = 0;
                    db.UpdateAvailableOrder(type, order.Id, available);

                    // tell the user that his order is no longer suspended
                    if (available == 0) {
                        IClient rem = (IClient)RemotingServices.Connect(typeof(IClient), users_logged["" + order.UserId].ToString());
                        rem.pendingOrderNotSuspended(type, "" + orderId);     
                    }

                }
            }

            
        }


        /// <summary>
        /// Hashes the given password through sha256.
        /// </summary>
        /// <param name="password">Password to hash.</param>
        /// <returns>The hashed password.</returns>
        private string HashPassword(string password)
        {
            StringBuilder sb = new StringBuilder();

            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                Byte[] result = hash.ComputeHash(enc.GetBytes(password));

                foreach (Byte b in result)
                    sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }


        /// <summary>
        /// Registers a new user into the system.
        /// </summary>
        /// <param name="name">Name of the user.</param>
        /// <param name="username">Username.</param>
        /// <param name="password">Password.</param>
        /// <returns>TRUE if new user was created, FALSE otherwise.</returns>
        public bool Register(string name, string username, string password)
        {
            Log("Registering a new user...");

            if (db.InsertUser(name, username, HashPassword(password)))
            {
                Log("Registration successful.");
                return true;
            }
            else
            {
                Log("Registration failed: username already exists.");
                return false;
            }
        }


        /// <summary>
        /// Logs the user into the system.
        /// </summary>
        /// <param name="username">Username.</param>
        /// <param name="password">Password.</param>
        /// <returns>Serialized user that logged in.</returns>
        public string Login(string username, string password, string address)
        {
            Log("Logging in user...");

            User user = db.GetUser(username, HashPassword(password));

            if (user != null)
            {
                Log("Login successful.");
                users_logged.Add("" + user.Id, address);
            }
            else
            {
                Log("Login failed: username and password don't match.");
            }

            return JsonConvert.SerializeObject(user);
        }


        /// <summary>
        /// Get user's wallet.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <returns>Serialized user wallet.</returns>
        public string GetWallet(long userId)
        {
            List<Diginote> wallet = db.GetWallet(userId);

            return JsonConvert.SerializeObject(wallet);
        }


        /// <summary>
        /// Adds a new order to the given user.
        /// </summary>
        /// <param name="type">Type of order to add (buying or selling).</param>
        /// <param name="user">User id.</param>
        /// <param name="amount">Amount of diginotes to buy / sell.</param>
        /// <returns>Serialized order that was added.</returns>
        public string AddOrder(OrderType type, long userId, long amount)
        {
            Log("Emitting new order...");

            Info status = db.InsertOrder(type, userId, amount);
            
            if (status == Info.Failed)
            {
                Log("Emission failed: order already exists.");
            }
            else
            {
                Log("Emition successful: " + status);
            }

            return JsonConvert.SerializeObject(status);
        }


        /// <summary>
        /// Returns the list of pending orders for given user.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <returns>List of pending orders.</returns>
        public string GetPendingOrders(long userId)
        {
            List<Order> orders = db.GetPendingOrders(userId, false);

            return JsonConvert.SerializeObject(orders);
        }


        /// <summary>
        /// Add money to given user.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <param name="amount">Amount to add.</param>
        /// <returns>TRUE if the adition was succesful, FALSE otherwise.</returns>
        public bool AddMoney(long userId, long amount)
        {
            return db.UpdateUserMoney(userId, amount);
        }


        /// <summary>
        /// Get user's money.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <returns>Amount of money user has.</returns>
        public double GetMoney(long userId)
        {
            return db.GetUserMoney(userId);
        }


        /// <summary>
        /// Deletes an order.
        /// </summary>
        /// <param name="type">Type of order to remove (buying or selling).</param>
        /// <param name="orderId">Order to remove.</param>
        /// <returns>TRUE if order was removed, FALSE otherwise.</returns>
        public bool DeleteOrder(OrderType type, long orderId)
        {
            Log("Deleting order #" + orderId + " ...");

            bool status = db.DeleteOrder(type, orderId);

            if (status)
            {
                Log("Deletion successful.");
            }
            else
            {
                Log("Deletion failed: order is already parcially completed.");
            }

            return status;
        }


        /// <summary>
        /// Logs the given string.
        /// </summary>
        /// <param name="str">String to log.</param>
        public void Log(string str)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter("log.txt", true))
            {
                file.WriteLine(DateTime.Now.ToString("[HH:mm:ss] [Server] ") + str);
            }
        }

        public void Logout(string id)
        {
            users_logged.Remove(id);
        }

        public bool ConfirmOrder(OrderType type, long orderId)
        {
            return db.UpdateAvailableOrder(type, orderId, 0);
        }

        public void UpdateUser(long userId)
        {
            if(users_logged.ContainsKey("" + userId)) { 
                IClient rem = (IClient)RemotingServices.Connect(typeof(IClient), users_logged["" + userId].ToString());
                rem.updateUserInterface();
            }
        }

    }
}
