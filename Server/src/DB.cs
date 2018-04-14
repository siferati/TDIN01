using Common;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using static Common.Order;

namespace Server
{
    /// <summary>
    /// Represents a database.
    /// </summary>
    class DB : IDisposable
    {
        /* --- ATTRIBUTES --- */

        /// <summary>
        /// Connection to the database.
        /// </summary>
        private SQLiteConnection connection;

        /// <summary>
        /// Path to the database creation script (.sql).
        /// </summary>
        private string inpath;

        /// <summary>
        /// Path to the database file (.sqlite).
        /// </summary>
        private string outpath;


        /* --- METHODS --- */

        /// <summary>
        /// Returns an instance of DB.
        /// </summary>
        /// <param name="inpath">Path to the database creation script (.sql).</param>
        /// <param name="outpath">Path to the database file (.sqlite).</param>
        public DB(string inpath, string outpath)
        {
            this.inpath = inpath;
            this.outpath = outpath;

            // true if database didn't exist yet
            bool newDB = false;

            if (!File.Exists(outpath))
            {
                // create database file
                SQLiteConnection.CreateFile(outpath);

                newDB = true;
            }            

            // create and open connection to database
            connection = new SQLiteConnection("Data Source=" + outpath + ";Version=3;foreign keys=true;");
            connection.Open();

            if (newDB)
            {
                // fetch creation script from inpath (.sql)
                string sql = File.ReadAllText(inpath);

                // initialize database
                SQLiteCommand cmd = new SQLiteCommand(sql, connection);
                cmd.ExecuteNonQuery();
            }           
        }


        /// <summary>
        /// Free resources once object is no longer needed.
        /// </summary>
        public void Dispose()
        {
            connection.Dispose();
        }


        /// <summary>
        /// Insert a new user in the database.
        /// </summary>
        /// <param name="name">Name of the user.</param>
        /// <param name="username">Username.</param>
        /// <param name="password">Password.</param>
        /// <returns>TRUE if the insert was successful, FALSE otherwise.</returns>
        public bool InsertUser(string name, string username, string password)
        {
            string sql = @"
                INSERT INTO Users (name, username, password)
                VALUES (@name, @username, @password)
            ";

            SQLiteCommand cmd = new SQLiteCommand(sql, connection);

            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@password", password);

            try
            {
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (SQLiteException e)
            {
                Console.WriteLine(e);
                return false;
            }
        }


        /// <summary>
        /// Checks if the given username and password match.
        /// </summary>
        /// <param name="username">Username.</param>
        /// <param name="password">Password.</param>
        /// <returns>User if login was successful, null otherwise.</returns>
        public User GetUser(string username, string password)
        {
            string sql = @"
                SELECT id, name, username, money FROM Users
                WHERE username = @username
                AND password = @password
            ";

            SQLiteCommand cmd = new SQLiteCommand(sql, connection);

            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@password", password);

            SQLiteDataReader reader = cmd.ExecuteReader();

            User user = null;

            if (reader.Read())
            {
                user =  new User(
                    (long) reader["id"],
                    (string) reader["name"],
                    (string) reader["username"],
                    (double) reader["money"],
                    new List<Diginote>()
                );

                sql = @"
                    SELECT id
                    FROM Diginotes
                    WHERE Diginotes.userId = @userId
                ";

                cmd = new SQLiteCommand(sql, connection);

                cmd.Parameters.AddWithValue("@userId", user.Id);

                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    user.Wallet.Add(new Diginote((long) reader["id"]));
                }
            }

            return user;
        }


        /// <summary>
        /// Returns the diginotes given user has.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <returns>List of diginotes this user owns.</returns>
        public List<Diginote> GetWallet(long userId)
        {
            string sql = @"
                    SELECT id
                    FROM Diginotes
                    WHERE Diginotes.userId = @userId
                ";

            SQLiteCommand cmd = new SQLiteCommand(sql, connection);

            cmd.Parameters.AddWithValue("@userId", userId);

            SQLiteDataReader reader = cmd.ExecuteReader();

            List<Diginote> wallet = new List<Diginote>();

            while (reader.Read())
            {
                wallet.Add(new Diginote((long)reader["id"]));
            }

            return wallet;
        }


        /// <summary>
        /// Get current quote for diginotes.
        /// </summary>
        /// <returns>Current quote.</returns>
        public double GetQuote()
        {
            string sql = @"
                SELECT value
                FROM Quote
                ORDER BY timestamp DESC
                LIMIT 1
            ";

            SQLiteCommand cmd = new SQLiteCommand(sql, connection);

            SQLiteDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return (double) reader["value"];
            }
            else
            {
                return 0;
            }
        }


        /// <summary>
        /// Inserts new quote value.
        /// </summary>
        /// <param name="quote">New quote value.</param>
        /// <returns>TRUE if insert was successful, FALSE otherwise.</returns>
        public bool InsertQuote(double quote)
        {
            string sql = @"
                INSERT INTO Quote (value)
                VALUES (@quote)
            ";

            SQLiteCommand cmd = new SQLiteCommand(sql, connection);

            cmd.Parameters.AddWithValue("@quote", quote);

            return (cmd.ExecuteNonQuery() > 0);
        }


        /// <summary>
        /// Get amount of money user has.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <returns>Current quote.</returns>
        public double GetUserMoney(long userId)
        {
            string sql = @"
                SELECT money
                FROM Users
                WHERE id = @userId
            ";

            SQLiteCommand cmd = new SQLiteCommand(sql, connection);

            cmd.Parameters.AddWithValue("@userId", userId);

            SQLiteDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return (double)reader["money"];
            }
            else
            {
                return 0;
            }
        }


        /// <summary>
        /// Inserts a new order in the database.
        /// </summary>
        /// <param name="type">Type of order to insert (buying or selling).</param>
        /// <param name="userId">Id of user whom order belongs to.</param>
        /// <param name="amount">Amount of diginotes to buy / sell.</param>
        /// <returns>This operation status.</returns>
        public Info InsertOrder(OrderType type, long userId, long amount)
        {
            string table = "";

            if (type == OrderType.Purchase)
            {
                table = "PurchaseOrders";
            }
            else if (type == OrderType.Selling)
            {
                table = "SellingOrders";
            }

            string sql = @"
                INSERT INTO " + table + @" (userId, amount)
                VALUES (@userId, @amount)
            ";

            SQLiteCommand cmd = new SQLiteCommand(sql, connection);

            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@amount", amount);

            try
            {
                // insert order
                cmd.ExecuteNonQuery();
                
                // get id from inserted order
                sql = @"
                    SELECT id
                    FROM " + table + @"
                    WHERE userId = @userId
                    AND amount = @amount
                    ORDER BY timestamp DESC Limit 1
                ";

                cmd = new SQLiteCommand(sql, connection);

                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@amount", amount);

                SQLiteDataReader reader = cmd.ExecuteReader();
                
                if (reader.Read())
                {
                    Order order = GetOrder(type, (long)reader["id"]);

                    // get all pending orders
                    List<Order> pendingOrders = GetPendingOrders();

                    // true if order was parcially completed
                    bool parcial = false;
                    foreach (Order pendingOrder in pendingOrders)
                    {
                        long pendingOrderNeededAmount = pendingOrder.Amount - pendingOrder.CurrentAmount;
                        long orderNeededAmount = order.Amount - order.CurrentAmount;

                        // ignore orders of same type or same owner
                        if (type == pendingOrder.Type || userId == pendingOrder.UserId)
                        {
                            continue;
                        }

                        // pending order has enough to satisfy order
                        if (orderNeededAmount <= pendingOrderNeededAmount)
                        {
                            // insert completed order
                            InsertCompletedOrder(order, pendingOrder, orderNeededAmount);

                            return Info.OrderCompleted;
                        }
                        // one pending order isn't enough
                        else
                        {
                            InsertCompletedOrder(order, pendingOrder, pendingOrderNeededAmount);

                            // update order amount
                            order.CurrentAmount += pendingOrderNeededAmount;
                            parcial = true;
                        }
                    }

                    if (parcial)
                    {
                        return Info.OrderParciallyCompleted;
                    }
                    else
                    {
                        return Info.OrderPending;
                    }
                }

                return Info.Failed;

            }
            catch (SQLiteException e)
            {
                Console.WriteLine(e);
                return Info.Failed;
            }
        }


        /// <summary>
        /// Updates the amount of money the user has.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <param name="amount">Amount to add / remove.</param>
        /// <returns>TRUE if updat was successful, FALSE otherwise.</returns>
        public bool UpdateUserMoney(long userId, double amount)
        {
            string sql = @"
                UPDATE Users
                SET money = money + @amount
                WHERE id = @userId
            ";

            SQLiteCommand cmd = new SQLiteCommand(sql, connection);

            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@amount", amount);

            try
            {
                return (cmd.ExecuteNonQuery() > 0);
            }
            catch (SQLiteException e)
            {
                Console.WriteLine(e);
                return false;
            }
        }


        /// <summary>
        /// Inserts a completed order in the databse.
        /// </summary>
        /// <param name="order1">First order.</param>
        /// <param name="order2">Second order.</param>
        /// <param name="amount">Amount transactioned.</param>
        /// <returns>TRUE if order was inserted, FALSE otherwise.</returns>
        public bool InsertCompletedOrder(Order order1, Order order2, long amount)
        {
            string sql = @"
                INSERT INTO CompletedOrders (sellingOrderId, purchaseOrderId, amount)
                VALUES (@sellingId, @buyingId, @amount)
            ";

            SQLiteCommand cmd = new SQLiteCommand(sql, connection);

            if (order1.Type == OrderType.Selling)
            {
                cmd.Parameters.AddWithValue("@sellingId", order1.Id);
                cmd.Parameters.AddWithValue("@buyingId", order2.Id);
            }
            else if (order1.Type == OrderType.Purchase)
            {
                cmd.Parameters.AddWithValue("@sellingId", order2.Id);
                cmd.Parameters.AddWithValue("@buyingId", order1.Id);
            }
            cmd.Parameters.AddWithValue("@amount", amount);

            try
            {
                cmd.ExecuteNonQuery();

                double quote = GetQuote();
                double money = quote * amount;

                // update users money
                if (order1.Type == OrderType.Selling)
                {
                    UpdateUserMoney(order1.UserId, money);
                    UpdateUserMoney(order2.UserId, -1 * money);
                }
                else if (order1.Type == OrderType.Purchase)
                {
                    UpdateUserMoney(order2.UserId, money);
                    UpdateUserMoney(order1.UserId, -1 * money);
                }


                // TODO update user wallets here


                return true;
            }
            catch (SQLiteException e)
            {
                Console.WriteLine(e);
                return false;
            }
        }


        /// <summary>
        /// Returns the order of given type and given id.
        /// </summary>
        /// <param name="type">Type of order (either buying or selling).</param>
        /// <param name="id">Id of order to return.</param>
        /// <returns>The order.</returns>
        public Order GetOrder(OrderType type, long id)
        {
            string table = "";
            if (type == OrderType.Purchase)
            {
                table = "PurchaseOrders";
            }
            else if (type == OrderType.Selling)
            {
                table = "SellingOrders";
            }

            string sql = @"
                SELECT timestamp, amount, userId
                FROM " + table + @"
                WHERE id = @id
            ";

            SQLiteCommand cmd = new SQLiteCommand(sql, connection);

            cmd.Parameters.AddWithValue("@id", id);

            SQLiteDataReader reader = cmd.ExecuteReader();

            Order order = null;

            if (reader.Read())
            {
                order = new Order(
                    id,
                    (long)reader["userId"],
                    type,
                    (DateTime) reader["timestamp"],
                    (long) reader["amount"]
                );
            }

            return order;
        }


        /// <summary>
        /// Returns a list of pending orders belonging to the given user.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <returns>List of pending orders.</returns>
        public List<Order> GetPendingOrders(long userId)
        {
            // pending purchase orders
            string sql = @"
                SELECT purchaseOrderId as pId, PurchaseOrders.amount AS goalAmount, SUM(CompletedOrders.amount) AS currentAmount, PurchaseOrders.timestamp, PurchaseOrders.userId
                FROM PurchaseOrders, CompletedOrders, SellingOrders
                WHERE PurchaseOrders.userId = @userId
                AND PurchaseOrders.id = purchaseOrderId
                AND SellingOrders.id = sellingOrderId
                GROUP BY pId
                HAVING goalAmount > currentAmount
                UNION
                SELECT id AS pId, amount AS goalAmount, 0 AS currentAmount, timestamp, userId
                FROM PurchaseOrders
                WHERE NOT EXISTS (
	                SELECT *
                    FROM CompletedOrders
                    WHERE PurchaseOrders.id = purchaseOrderId
                )
                AND userId = @userId
            ";

            SQLiteCommand cmd = new SQLiteCommand(sql, connection);

            cmd.Parameters.AddWithValue("@userId", userId);

            SQLiteDataReader reader = cmd.ExecuteReader();

            List <Order> orders = new List<Order>();

            while (reader.Read())
            {
                orders.Add(new Order(
                    (long) reader["pId"],
                    (long) reader["userId"],
                    OrderType.Purchase,
                    (DateTime) reader["timestamp"],
                    (long) reader["goalAmount"],
                    (long)reader["currentAmount"]
                ));
            }

            // pending selling orders
            sql = @"
                SELECT sellingOrderId as sId, SellingOrders.amount AS goalAmount, SUM(CompletedOrders.amount) AS currentAmount, SellingOrders.timestamp, SellingOrders.userId
                FROM PurchaseOrders, CompletedOrders, SellingOrders
                WHERE SellingOrders.userId = @userId
                AND PurchaseOrders.id = purchaseOrderId
                AND SellingOrders.id = sellingOrderId
                GROUP BY sId
                HAVING goalAmount > currentAmount
                UNION
                SELECT id AS sId, amount AS goalAmount, 0 AS currentAmount, timestamp, userId
                FROM SellingOrders
                WHERE NOT EXISTS (
	                SELECT *
                    FROM CompletedOrders
                    WHERE SellingOrders.id = sellingOrderId
                )
                AND userId = @userId
            ";

            cmd = new SQLiteCommand(sql, connection);

            cmd.Parameters.AddWithValue("@userId", userId);

            reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                orders.Add(new Order(
                    (long)reader["sId"],
                    (long) reader["userId"],
                    OrderType.Selling,
                    (DateTime)reader["timestamp"],
                    (long)reader["goalAmount"],
                    (long)reader["currentAmount"]
                ));
            }

            return orders;
        }

        /// <summary>
        /// Returns an ordererd list of all pending orders of all users.
        /// </summary>
        /// <returns>List of pending orders.</returns>
        public List<Order> GetPendingOrders()
        {
            string sql = @"
                SELECT id
                FROM Users
            ";

            SQLiteCommand cmd = new SQLiteCommand(sql, connection);

            SQLiteDataReader reader = cmd.ExecuteReader();

            List<Order> orders = new List<Order>();

            while (reader.Read())
            {
                orders.AddRange(GetPendingOrders((long) reader["id"]));
            }

            // sort orders
            orders.Sort((x, y) => x.Timestamp.CompareTo(y.Timestamp));

            return orders;
        }
    }
}
