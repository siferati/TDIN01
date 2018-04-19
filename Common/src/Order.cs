using System;

namespace Common
{
    /// <summary>
    /// Represents an order (either selling or buying).
    /// </summary>
    public class Order
    {
        /// <summary>
        /// The type of order.
        /// </summary>
        public enum OrderType { Selling, Purchase };

        /* --- ATTRIBUTES --- */

        /// <summary>
        /// The id of this order.
        /// </summary>
        public long Id { get; }

        /// <summary>
        /// The id of owner of this order.
        /// </summary>
        public long UserId { get; }

        /// <summary>
        /// The timestamp this order was issued.
        /// </summary>
        public DateTime Timestamp { get; }

        /// <summary>
        /// The amount of diginotes to buy / sell.
        /// </summary>
        public long Amount { get; set;  }

        /// <summary>
        /// The current amount of diginotes bought / sold.
        /// </summary>
        public long CurrentAmount { get; set; }

        /// <summary>
        /// Type of this order. Either selling or buying.
        /// </summary>
        public OrderType Type { get; }

        /// <summary>
        /// Whether order is supended or not
        /// </summary>
        public long Available { get; set; }


        /* --- METHODS --- */

        /// <summary>
        /// Base constructor called by subclasses.
        /// </summary>
        /// <param name="id">Id of this order.</param>
        /// <param name="userId">User id of this order.</param>
        /// <param name="amount">Type of this order.</param>
        /// <param name="timestamp">Timestamp this order was issued.</param>
        /// <param name="amount">Amount of diginotes to buy / sell.</param>
        /// <param name="currentAmount">Amount of diginotes bought / sold.</param>
        public Order(long id, long userId, OrderType type, DateTime timestamp, long amount, long available = 0, long currentAmount = 0)
        {
            this.Id = id;
            this.UserId = userId;
            this.Type = type;
            this.Timestamp = timestamp;
            this.Amount = amount;
            this.CurrentAmount = currentAmount;
            this.Available = available;
        }


        public override string ToString()
        {
            string str = "";

            if (Type == OrderType.Purchase)
            {
                str += "Purchase Order";
            }
            else if (Type == OrderType.Selling)
            {
                str += "Selling Order";
            }

            str += " #" + Id + " Current Amount: " + CurrentAmount + "/" + Amount;

            return str;
        }
    }
}
