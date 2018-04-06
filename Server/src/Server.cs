using Common;
using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

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
            DB.Init();

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


        /* --- METHODS --- */

        /// <summary>
        /// Returns an instance of Server.
        /// </summary>
        public Server()
        {

            Log("Waking up...");
        }


        /// <summary>
        /// Clean up once the object is about to be destroyed
        /// </summary>
        ~Server()
        {
            Log("Shutting off...");
        }


        /// <summary>
        /// Logs the user into the system.
        /// </summary>
        /// <param name="username">Username.</param>
        /// <param name="password">Password.</param>
        /// <returns>TRUE if username and passsword match, FALSE otherwise.</returns>
        public bool Login(string username, string password)
        {
            Log("Client is trying to login...");

            if (DB.Login(username, password))
            {
                Log("Password and username match. User allowed to login.");
                return true;
            }
            else
            {
                Log("Password and username don't match. User not allowed to login.");
                return false;
            }
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

            if (DB.InsertUser(name, username, password))
            {
                Log("New user created.");
                return true;
            }
            else
            {
                Log("User already exists.");
                return false;
            }
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
