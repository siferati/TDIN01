using System;

namespace Client.Cli
{
    /// <summary>
    /// The main menu of the interface.
    /// </summary>
    class MainMenu : Menu
    {
        /* --- ATTRIBUTES --- */


        /* --- METHODS --- */

        public MainMenu(Client client) : base(client, "Main Menu", new String[] { "Login", "Exit" })
        {

        }


        public override Menu ProcessInput(String input)
        {
            int i;
            if (int.TryParse(input, out i) && i > 0 && i <= options.Length)
            {
                switch (--i)
                {
                    // Login
                    case 0:
                        {
                            Console.Write("Username: ");
                            String username = Console.ReadLine();
                            Console.Write("Password: ");
                            String password = Console.ReadLine();

                            // login
                            client.Login(username, password);

                            // TODO return next menu
                            break;
                        }

                    // Exit
                    case 1:
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
