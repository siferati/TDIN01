using System.Collections.Generic;

namespace Common
{
    /// <summary>
    /// Represents a user.
    /// </summary>
    public class User
    {
        /* --- ATTRIBUTES --- */

        /// <summary>
        /// The user's id.
        /// </summary>
        public long Id { get; }

        /// <summary>
        /// Name of this user.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Username.
        /// </summary>
        public string Username { get; }

        /// <summary>
        /// The user's wallet. It's a list of diginotes owned by this user.
        /// </summary>
        public List<Diginote> Wallet { get; }

        /// <summary>
        /// List of selling orders emitted by this user.
        /// </summary>
        public List<Order> SellingOrders { get; set; }

        /// <summary>
        /// List of purchase orders emitted by this user.
        /// </summary>
        public List<Order> PurchaseOrsers { get; set; }

        /* --- METHODS --- */

        /// <summary>
        /// Returns an instace of User.
        /// </summary>
        /// <param name="id">The user's id.</param>
        /// <param name="name">The user's name.</param>
        /// <param name="username">Username.</param>
        /// <param name="wallet">Diginotes owned by this user.</param>
        public User(long id, string name, string username, List<Diginote> wallet)
        {
            this.Id = id;
            this.Name = name;
            this.Username = username;
            this.Wallet = wallet;
            SellingOrders = new List<Order>();
            PurchaseOrsers = new List<Order>();
        }
    }
}
