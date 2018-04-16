﻿using static Common.Order;

namespace Common
{
    /// <summary>
    /// Exposes the server methods to the client.
    /// </summary>
    public interface IServer
    {
        /// <summary>
        /// The current quote of diginotes.
        /// </summary>
        double Quote { get; set; }

        /// <summary>
        /// Setter.
        /// </summary>
        /// <param name="quote"></param>
        /// <returns></returns>
        bool SetQuote(double quote);

        /// <summary>
        /// Logs the user into the system.
        /// </summary>
        /// <param name="username">Username.</param>
        /// <param name="password">Password.</param>
        /// <returns>TRUE if username and passsword match, FALSE otherwise.</returns>
        string Login(string username, string password);

        /// <summary>
        /// Registers a new user into the system.
        /// </summary>
        /// <param name="name">Name of the user.</param>
        /// <param name="username">Username.</param>
        /// <param name="password">Password.</param>
        /// <returns>TRUE if new user was created, FALSE otherwise.</returns>
        bool Register(string name, string username, string password);

        /// <summary>
        /// Adds a new order to the given user.
        /// </summary>
        /// <param name="type">Type of order to add (buying or selling).</param>
        /// <param name="userId">User id.</param>
        /// <param name="amount">Amount of diginotes to buy / sell.</param>
        /// <returns>Serialized order that was added.</returns>
        string AddOrder(OrderType type, long userId, long amount);

        /// <summary>
        /// Adds a new order to the given user.
        /// </summary>
        /// <param name="type">Type of order to remove (buying or selling).</param>
        /// <param name="orderId">Order to remove.</param>
        /// <returns>TRUE if order was removed, FALSE otherwise.</returns>
        bool DeleteOrder(OrderType type, long orderId);

        /// <summary>
        /// Returns the list of pending orders for given user.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <returns>List of pending orders.</returns>
        string GetPendingOrders(long userId);

        /// <summary>
        /// Get user's wallet.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <returns>Serialized user wallet.</returns>
        string GetWallet(long userId);

        /// <summary>
        /// Get user's money.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <returns>Amount of money user has.</returns>
        double GetMoney(long userId);


        /// <summary>
        /// Add money to given user.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <param name="amount">Amount to add.</param>
        /// <returns>TRUE if the adition was succesful, FALSE otherwise.</returns>
        bool AddMoney(long userId, long amount);
    }
}
