using System;
using System.Collections.Generic;
using System.Text;

namespace Common.src
{
    public interface IClient
    {
        /// <summary>
        /// Setter. Updates current quote
        /// </summary>
        /// <param name="quote"></param>
        /// <returns></returns>
        void updateQuote(double quote);

        /// <summary>
        ///  Suspend an order 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="orderId"></param>
        void pendingOrderSuspended(Common.Order.OrderType type, string orderId);

        /// <summary>
        /// Cancels suspension of an order
        /// </summary>
        /// <param name="type"></param>
        /// <param name="orderId"></param>
        void pendingOrderNotSuspended(Common.Order.OrderType type, string orderId);

        /// <summary>
        /// When an order is completed, user interface must be updated
        /// </summary>
        /// <param name="type"></param>
        void updateUserInterface();


    }
}
