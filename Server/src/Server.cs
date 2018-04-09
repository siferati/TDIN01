﻿using Common;
using Newtonsoft.Json;
using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Security.Cryptography;
using System.Text;

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
        /// <returns>TRUE if username and passsword match, FALSE otherwise.</returns>
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
        /// Logs the given string.
        /// </summary>
        /// <param name="str">String to log.</param>
        private void Log(string str)
        {
            Console.WriteLine("[Server]: " + str);
        }
    }
}
