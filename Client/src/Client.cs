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
            Log("Attempting to login...");

            User = JsonConvert.DeserializeObject<User>(server.Login(username, password));

            if (User != null)
            {
                Log("Login successful!");
                return true;
            }
            else
            {
                Log("Failted to login!");
                return false;
            }
        }

        /// <summary>
        /// Updates the user.
        /// </summary>
        public void UpdateUser()
        {
            User.Wallet = JsonConvert.DeserializeObject<List<Diginote>>(server.GetWallet(User.Id));
        }


        /// <summary>
        /// Adds a new order.
        /// </summary>
        /// <param name="type">Type of order to add (buying or selling).</param>
        /// <param name="amount">Amount of diginotes to buy / sell.</param>
        /// <returns>TRUE if the order was added, FALSE otherwise.</returns>
        public bool AddOrder(OrderType type, long amount)
        {
            Log("Attempting to emit a new order...");

            UpdateUser();
            if (User.Wallet.Count < amount)
            {
                Log("Emition failed: not enough diginotes to proceed with sale.");
                return false;
            }
            
            Order order = JsonConvert.DeserializeObject<Order>(
                server.AddOrder(type, User.Id, amount)
            );

            if (order != null)
            {
                Log("Emition successful!");
                return true;
            }
            else
            {
                Log("Failted to emit new order!");
                return false;
            }
        }


        /// <summary>
        /// Logs the user out of the system.
        /// </summary>
        public void Logout()
        {
            Log("Attempting to logout...");
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
            Log("Attempting to register a new user...");

            if (server.Register(name, username, password))
            {
                Log("Registration successful!");
                return true;
            }
            else
            {
                Log("Failted to register!");
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
        /// Returns the list of pending orders.
        /// </summary>
        /// <returns>List of pending orders.</returns>
        public List<Order> GetPendingOrders()
        {
            Log("Attempting to fetch pending orders...");

            List<Order> orders = JsonConvert.DeserializeObject<List<Order>>(
                server.GetPendingOrders(User.Id)
            );

            if (orders != null)
            {
                Log("Retrieval of pending orders was successful!");
            }
            else
            {
                Log("Failted to retrieve list of pending orders!");
            }

            return orders;
        }


        /// <summary>
        /// Logs the given string.
        /// </summary>
        /// <param name="str">String to log.</param>
        private void Log(string str)
        {
            Console.WriteLine("[Client]: " + str);
        }
    }
}
