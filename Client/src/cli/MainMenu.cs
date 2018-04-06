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
        /// Index of login menu entry.
        /// </summary>
        public const int LOGIN = 0;

        /// <summary>
        /// Index of register menu entry.
        /// </summary>
        public const int REGISTER = 1;

        /// <summary>
        /// Index of exit menu entry.
        /// </summary>
        public const int EXIT = 2;


        /* --- METHODS --- */

        public MainMenu(Client client) : base(client, "Main Menu", new string[] { "Login", "Register", "Exit" })
        {

        }


        public override Menu ProcessInput(string input)
        {
            int i;
            if (int.TryParse(input, out i) && i > 0 && i <= options.Length)
            {
                switch (--i)
                {
                    case LOGIN:
                        {

                            Console.Write("Username: ");
                            string username = Console.ReadLine();
                            Console.Write("Password: ");
                            string password = Console.ReadLine();

                            // login
                            client.Login(username, password);

                            // TODO return next menu
                            break;
                        }

                    case REGISTER:
                        {

                            Console.Write("Name: ");
                            string name = Console.ReadLine();
                            Console.Write("Username: ");
                            string username = Console.ReadLine();
                            Console.Write("Password: ");
                            string password = Console.ReadLine();

                            // register
                            client.Register(name, username, password);

                            // TODO return next menu

                            break;

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
