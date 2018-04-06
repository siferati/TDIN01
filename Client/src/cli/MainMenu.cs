using System;

namespace Client.Cli
{
    /// <summary>
    /// The main menu of the interface.
    /// </summary>
    class MainMenu : Menu
    {
        /* --- ATTRIBUTES --- */

        /// <summary>
        /// Index of logout menu entry.
        /// </summary>
        public const int LOGOUT = 0;

        /// <summary>
        /// Index of exit menu entry.
        /// </summary>
        public const int EXIT = 1;


        /* --- METHODS --- */

        public MainMenu(Client client) : base(client, "Main Menu", new string[] { "Logout", "Exit" })
        {

        }


        public override Menu ProcessInput(string input)
        {
            int i;
            if (int.TryParse(input, out i) && i > 0 && i <= options.Length)
            {
                switch (--i)
                {
                    case LOGOUT:
                        {

                            Console.Write("Logged out.");
                            return new InitialMenu(client);
                        }
                        
                    case EXIT:
                        {
                            return null;
                        }


                    // Invalid
                    default:
                        break;
                }
            }

            // invalid input
            return this;
        }
    }
}
