using Client.Cli;
using Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Windows.Forms;
using static Common.Order;

namespace Client
{
    /// <summary>
    /// Represents a client.
    /// </summary>
    public class Client
    {
        /// <summary>
        /// Client entry point.
        /// </summary>
        /// <param name="args">Arguments passed through the terminal.</param>
        public static void Main(string[] args)
        {
            Console.WriteLine("");

            // start client
            Client client = new Client();

            if (args.Length == 1 && args[0] == "-nogui")
            {
                // start cli
                CLI cli = new CLI();
                cli.Launch(client);
            }
            else
            {
                //start gui
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1(client));
            }
        }


        /* --- ATTRIBUTES --- */

        /// <summary>
        /// Server proxy.
        /// </summary>
        private IServer server;

        /// <summary>
        /// The user that's currently logged into the system.
        /// </summary>
        public User User { get; set; }


        /* --- METHODS --- */

        /// <summary>
        /// Returns an instance of Client.
        /// </summary>
        public Client()
        {
            User = null;

            Log("Creating server proxy...");
            server = (IServer)RemotingServices.Connect(typeof(IServer), "tcp://localhost:9000/Server.rem");
            Log("Server proxy created.");
        }


        /// <summary>
        /// Clean up once the object is about to be destroyed
        /// </summary>
        ~Client()
        {
            Log("Shutting off...");
        }


        /// <summary>
        /// Logs the user into the system.
        /// </summary>
        /// <param name="username">Username.</param>
        /// <param name="password">Password.</param>
        /// <returns>TRUE if user was able to login, FALSE otherwise.</returns>
        public bool Login(string username, string password)
        {
            Log("Logging in...");

            User = JsonConvert.DeserializeObject<User>(server.Login(username, password));

            if (User != null)
            {
                Log("Login successful.");
                return true;
            }
            else
            {
                Log("Login failed: username and password don't match.");
                return false;
            }
        }

        /// <summary>
        /// Updates the user.
        /// </summary>
        public void UpdateUser()
        {
            if (User != null)
            {
                User.Wallet = JsonConvert.DeserializeObject<List<Diginote>>(server.GetWallet(User.Id));
                User.Money = server.GetMoney(User.Id);
            }            
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

            bool status = server.DeleteOrder(type, orderId);

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
        /// Adds a new order.
        /// </summary>
        /// <param name="type">Type of order to add (buying or selling).</param>
        /// <param name="amount">Amount of diginotes to buy / sell.</param>
        /// <returns>Status.</returns>
        public Info AddOrder(OrderType type, long amount)
        {
            Log("Emitting new order...");

            UpdateUser();

            List<Order> pendingOrders = GetPendingOrders();
            long sellingAmount = 0;
            long purchaseAmount = 0;

            foreach (Order order in pendingOrders)
            {
                if (order.Type == OrderType.Selling)
                {
                    sellingAmount += (order.Amount - order.CurrentAmount);
                }
                else if (order.Type == OrderType.Purchase)
                {
                    purchaseAmount += (order.Amount - order.CurrentAmount);
                }
            }

            if (type == OrderType.Selling && User.Wallet.Count < (amount + sellingAmount))
            {
                Log("Emission failed: not enough diginotes to proceed with sale.");
                return Info.Failed;
            }
            else if (type == OrderType.Purchase && User.Money < ((amount + purchaseAmount) * GetQuote()))
            {
                Log("Emission failed: not enough money to proceed with purchase.");
                return Info.Failed;
            }
            
            Info status = JsonConvert.DeserializeObject<Info>(
                server.AddOrder(type, User.Id, amount)
            );

            if (status == Info.Failed)
            {
                Log("Emission failed: order already exists.");
            }
            else
            {
                Log("Emition successful: " + status);
            }

            return status;
        }


        /// <summary>
        /// Logs the user out of the system.
        /// </summary>
        public void Logout()
        {
            Log("Logging out...");
            User = null;
        }


        /// <summary>
        /// Registers a new user into the system.
        /// </summary>
        /// <param name="name">Name of the user.</param>
        /// <param name="username">Username.</param>
        /// <param name="password">Password.</param>
        /// <returns>TRUE if user was able to register, FALSE otherwise.</returns>
        public bool Register(String name, String username, String password)
        {
            Log("Registering a new user...");

            if (server.Register(name, username, password))
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
        /// Get the current quote of diginotes.
        /// </summary>
        /// <returns>The current quote.</returns>
        public double GetQuote()
        {
            return server.Quote;
        }


        /// <summary>
        /// Adds a new quote.
        /// </summary>
        /// <param name="quote">New quote.</param>
        /// <returns>TRUE if insert was successful, FALSE otherwise.</returns>
        public bool AddQuote(double quote)
        {
            return server.SetQuote(quote);
        }


        /// <summary>
        /// Add money.
        /// </summary>
        /// <param name="amount">Amount to add.</param>
        /// <returns>TRUE if insert was successful, FALSE otherwise.</returns>
        public bool AddMoney(long amount)
        {
            return server.AddMoney(User.Id, amount);
        }


        /// <summary>
        /// Returns the list of pending orders.
        /// </summary>
        /// <returns>List of pending orders.</returns>
        public List<Order> GetPendingOrders()
        {
            List<Order> orders = JsonConvert.DeserializeObject<List<Order>>(
                server.GetPendingOrders(User.Id)
            );

            return orders;
        }


        /// <summary>
        /// Logs the given string.
        /// </summary>
        /// <param name="str">String to log.</param>
        private void Log(string str)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter("log.txt", true))
            {
                file.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ") + "[Client] " + str);
            }
        }
    }
}
