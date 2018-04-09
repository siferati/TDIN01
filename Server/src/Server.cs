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
        public double Quote { get; }


        /* --- METHODS --- */

        /// <summary>
        /// Returns an instance of Server.
        /// </summary>
        public Server()
        {
            Log("Waking up...");

            // init database
            db = new DB(DB_INPATH, DB_OUTPATH);

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
            Log("Client is trying to register a new user...");

            if (db.InsertUser(name, username, HashPassword(password)))
            {
                Log("New user created.");
                return true;
            }
            else
            {
                Log("Failed to create new user: username already exists.");
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
            Log("Client is trying to login...");

            User user = db.GetUser(username, HashPassword(password));

            if (user != null)
            {
                Log("Login successful.");
            }
            else
            {
                Log("Failed to login: username and password don't match.");
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
            Log("Client is trying to get user wallet...");

            List<Diginote> wallet = db.GetWallet(userId);

            if (wallet != null)
            {
                Log("Success.");
            }
            else
            {
                Log("Failed.");
            }

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
            Log("Client is trying to emit new order...");

            Order order = db.InsertOrder(type, userId, amount);
            
            if (order != null)
            {
                Log("Order was emited successfully.");
            }
            else
            {
                Log("Failed to emit order.");
            }

            return JsonConvert.SerializeObject(order);
        }


        /// <summary>
        /// Returns the list of pending orders for given user.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <returns>List of pending orders.</returns>
        public string GetPendingOrders(long userId)
        {
            Log("Client is trying to get pending orders...");

            List<Order> orders = db.GetPendingOrders(userId);

            if (orders != null)
            {
                Log("Pending orders were retreived successfully.");
            }
            else
            {
                Log("Failed to retrieve pending orders.");
            }

            return JsonConvert.SerializeObject(orders);
        }


        /// <summary>
        /// Logs the given string.
        /// </summary>
        /// <param name="str">String to log.</param>
        private void Log(string str)
        {
            Console.WriteLine("[Server]: " + str);
        }
    }
}
