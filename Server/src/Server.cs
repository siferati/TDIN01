using Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Security.Cryptography;
using System.Text;
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


        /* --- METHODS --- */

        /// <summary>
        /// Returns an instance of Server.
        /// </summary>
        public Server()
        {
            Log("Waking up...");

            // init database
            db = new DB(DB_INPATH, DB_OUTPATH, this);

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
        public bool SetQuote(double quote)
        {
            Log("Updated quote from " + Quote + " to " + quote);

            this.Quote = quote;

            return db.InsertQuote(quote);
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
        public string Login(string username, string password)
        {
            Log("Logging in user...");

            User user = db.GetUser(username, HashPassword(password));

            if (user != null)
            {
                Log("Login successful.");
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
            List<Order> orders = db.GetPendingOrders(userId);

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
    }
}
