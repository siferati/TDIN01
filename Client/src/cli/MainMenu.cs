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
        /// Index of emit selling order menu entry.
        /// </summary>
        public const int SELLING_ORDER = 1;

        /// <summary>
        /// Index of exit menu entry.
        /// </summary>
        public const int EXIT = 2;


        /* --- METHODS --- */

        public MainMenu(Client client) : base(client, "Main Menu",
            new string[] {"Logout", "Emit Selling Order", "Exit" })
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
                            client.Logout();
                            return new InitialMenu(client);
                        }

                    case SELLING_ORDER:
                        {
                            Console.Write("Amount: ");
                            long amount = Convert.ToInt64(Console.ReadLine());

                            // TODO Order = server.addOrder(User, amount)
                            return this;
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
