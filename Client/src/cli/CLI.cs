using System;

namespace Client.Cli
{
    /// <summary>
    /// Represents the command line interface.
    /// </summary>
    class CLI
    {
        /* --- ATTRIBUTES --- */

        /// <summary>
        /// Current active menu;
        /// </summary>
        private Menu menu;


        /* --- METHODS --- */

        /// <summary>
        /// Returns an instance of CLI.
        /// </summary>
        public CLI()
        {

        }

        /// <summary>
        /// Launch the command line interface.
        /// </summary>
        /// <param name="client">The client object.</param>
        public void Launch(Client client)
        {
            // initial menu
            menu = new InitialMenu(client);

            while (menu != null)
            {
                Console.Write(menu);
                string input = Console.ReadLine();
                menu = menu.ProcessInput(input);
            }
        }
    }
}
