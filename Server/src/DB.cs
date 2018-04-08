using Common;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

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
            catch (SQLiteException)
            {
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
                SELECT id, name, username FROM Users
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
    }
}
