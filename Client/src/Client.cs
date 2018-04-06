using Client.Cli;
using Common;
using System;
using System.Runtime.Remoting;

namespace Client
{
    /// <summary>
    /// Represents a client.
    /// </summary>
    class Client
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

            // start interface
            CLI cli = new CLI();
            cli.Launch(client);
        }


        /* --- ATTRIBUTES --- */

        /// <summary>
        /// Server proxy.
        /// </summary>
        private IServer server;


        /* --- METHODS --- */

        /// <summary>
        /// Returns an instance of Client.
        /// </summary>
        public Client()
        {

            Log("Creating server proxy...");
            server = (IServer)RemotingServices.Connect(typeof(IServer), "tcp://localhost:9000/Server.rem");
            Log("Server proxy created.");
        }


        /// <summary>
        /// Logs the user into the system.
        /// </summary>
        /// <param name="username">Username.</param>
        /// <param name="password">Password.</param>
        /// <returns>TRUE if username and passsword match, FALSE otherwise.</returns>
        public void Login(string username, string password)
        {
            Log("Attempting to login...");

            if (server.Login(username, password))
            {
                Log("Login successful!");
            }
            else
            {
                Log("Failted to login!");
            }
        }


        /// <summary>
        /// Clean up once the object is about to be destroyed
        /// </summary>
        ~Client()
        {
            Log("Shutting off...");
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
