using Common;
using System;
using System.Collections.Generic;
using static Common.Order;

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
        /// Index of emit purchase order menu entry.
        /// </summary>
        public const int PURCHASE_ORDER = 2;

        /// <summary>
        /// Index of list pending orders menu entry.
        /// </summary>
        public const int PENDING_ORDERS = 3;

        /// <summary>
        /// Index of exit menu entry.
        /// </summary>
        public const int EXIT = 4;


        /* --- METHODS --- */

        public MainMenu(Client client) : base(client, "Main Menu",
            new string[] {"Logout", "Emit Selling Order", "Emit Purchase Order", "List Pending Orders", "Exit" })
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
                            client.AddOrder(OrderType.Selling, amount);

                            break;
                        }

                    case PURCHASE_ORDER:
                        {
                            Console.Write("Amount: ");
                            long amount = Convert.ToInt64(Console.ReadLine());
                            client.AddOrder(OrderType.Purchase, amount);

                            break;
                        }

                    case PENDING_ORDERS:
                        {
                            List<Order> orders = client.GetPendingOrders();

                            Console.WriteLine("\n----------");
                            Console.WriteLine("Pending Orders");
                            foreach (Order order in orders)
                            {
                                Console.WriteLine(order);
                            }
                            Console.WriteLine("----------");

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

            // same menu (prob invalid input)
            return this;
        }
    }
}
