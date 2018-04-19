using Common.src;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class RemMessage : MarshalByRefObject, IClient
    {
       
        private MainPage win;

        public override object InitializeLifetimeService()
        {
            return null;
        }
        
        public void PutMyForm(MainPage form)
        {
            win = form;
        }


        /// <summary>
        /// Updates Quote
        /// </summary>
        /// <param name="quote">Current Quote</param>
        public void updateQuote(double quote)
        {
            Console.WriteLine("SERVER UPDATE QUOTE WITH: " + quote);
            win.updateQuote("" + quote);
        }

        public void pendingOrderSuspended(Common.Order.OrderType type, string orderId)
        {
            Console.WriteLine("Server wants to suspend order: " + orderId + " of type: " + (type == Common.Order.OrderType.Purchase ? "Purchase" : "Selling"));
            win.pendingOrderSuspended(type, orderId);

        }

        public void pendingOrderNotSuspended(Common.Order.OrderType type, string orderId)
        {
            Console.WriteLine("Server wants to unsuspend order: " + orderId + " of type: " + (type == Common.Order.OrderType.Purchase ? "Purchase" : "Selling"));
            win.pendingOrderNotSuspended(type, orderId);
        }


        public void updateUserInterface()
        {
            win.inicialize_wallet();
            win.inicialize_purchase();
            win.inicialize_selling();
            
        }
    }
}
