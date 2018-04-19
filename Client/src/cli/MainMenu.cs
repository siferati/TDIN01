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
        /// Index of money menu entry.
        /// </summary>
        public const int MONEY = 1;

        /// <summary>
        /// Index of edit quote menu entry.
        /// </summary>
        public const int EDIT_QUOTE = 2;

        /// <summary>
        /// Index of emit selling order menu entry.
        /// </summary>
        public const int SELLING_ORDER = 3;

        /// <summary>
        /// Index of emit purchase order menu entry.
        /// </summary>
        public const int PURCHASE_ORDER = 4;

        /// <summary>
        /// Index of list pending orders menu entry.
        /// </summary>
        public const int PENDING_ORDERS = 5;

        /// <summary>
        /// Index of exit menu entry.
        /// </summary>
        public const int EXIT = 6;


        /* --- METHODS --- */

        public MainMenu(Client client) : base(client, "Main Menu",
            new string[] {"Logout", "Add Money", "Edit current quote", "Emit Selling Order", "Emit Purchase Order", "List Pending Orders", "Exit" })
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

                    case MONEY:
                        {
                            Console.Write("Amount: ");
                            long amount = Convert.ToInt64(Console.ReadLine());
                            client.AddMoney(amount);

                            break;
                        }

                    case EDIT_QUOTE:
                        {
                            List<Order> orders = client.GetPendingOrders();
                            if (orders.Count == 0)
                            {
                                break;
                            }
                            bool selling = false;
                            bool purchase = false;
                            foreach(Order order in orders)
                            {
                                if (!selling && order.Type == OrderType.Selling)
                                {
                                    selling = true;
                                }
                                else if (!purchase && order.Type == OrderType.Purchase)
                                {
                                    purchase = true;
                                }

                                if (purchase && selling)
                                {
                                    break;
                                }
                            }

                            double currentQuote = client.GetQuote();
                            Console.WriteLine("Current Quote: " + currentQuote);

                            double quote = 0;
                            if (selling && purchase)
                            {
                                Console.Write("New Quote: ");
                                quote = Convert.ToDouble(Console.ReadLine());
                            }
                            else if (selling)
                            {
                                do
                                {
                                    Console.Write("New Quote (less or equal than current quote): ");
                                    quote = Convert.ToDouble(Console.ReadLine());
                                } while (quote > currentQuote);
                            }
                            else if (purchase)
                            {
                                do
                                {
                                    Console.Write("New Quote (higher or equal than current quote): ");
                                    quote = Convert.ToDouble(Console.ReadLine());
                                } while (quote < currentQuote);
                            }

                            client.AddQuote(quote, OrderType.Purchase);

                            break;
                        }

                    case SELLING_ORDER:
                        {
                            Console.Write("Amount: ");
                            long amount = Convert.ToInt64(Console.ReadLine());
                            Info status = client.AddOrder(OrderType.Selling, amount);

                            if (status == Info.OrderParciallyCompleted || status == Info.OrderPending)
                            {
                                double currentQuote = client.GetQuote();
                                Console.WriteLine("Current Quote: " + currentQuote);
                                
                                double quote = 0;
                                do
                                {
                                    Console.Write("New Quote (less or equal than current quote): ");
                                    quote = Convert.ToDouble(Console.ReadLine());
                                } while (quote > currentQuote );
                                
                                client.AddQuote(quote, OrderType.Purchase);
                            }

                            break;
                        }

                    case PURCHASE_ORDER:
                        {
                            Console.Write("Amount: ");
                            long amount = Convert.ToInt64(Console.ReadLine());
                            Info status = client.AddOrder(OrderType.Purchase, amount);

                            if (status == Info.OrderParciallyCompleted || status == Info.OrderPending)
                            {
                                double currentQuote = client.GetQuote();
                                Console.WriteLine("Current Quote: " + currentQuote);

                                double quote = 0;
                                do
                                {
                                    Console.Write("New Quote (higher or equal than current quote): ");
                                    quote = Convert.ToDouble(Console.ReadLine());
                                } while (quote < currentQuote);

                                client.AddQuote(quote, OrderType.Purchase);
                            }
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
