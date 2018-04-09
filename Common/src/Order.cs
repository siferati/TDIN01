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
        /// The id of this order
        /// </summary>
        public long Id { get; }

        /// <summary>
        /// The timestamp this order was issued.
        /// </summary>
        public DateTime Timestamp { get; }

        /// <summary>
        /// The amount of diginotes to buy / sell.
        /// </summary>
        public long Amount { get; }

        /// <summary>
        /// Type of this order. Either selling or buying.
        /// </summary>
        public OrderType Type { get; }


        /* --- METHODS --- */

        /// <summary>
        /// Base constructor called by subclasses.
        /// </summary>
        /// <param name="id">Id of this order.</param>
        /// <param name="amount">Type of this order.</param>
        /// <param name="timestamp">Timestamp this order was issued.</param>
        /// <param name="amount">Amount of diginotes to buy / sell.</param>
        public Order(long id, OrderType type, DateTime timestamp, long amount)
        {
            this.Id = id;
            this.Type = type;
            this.Timestamp = timestamp;
            this.Amount = amount;
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

            str += " #" + Id + " Goal Amount: " + Amount;

            return str;
        }
    }
}
