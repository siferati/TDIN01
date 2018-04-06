using System;
using System.Data.SQLite;
using System.IO;

namespace Server
{
    class DB
    {
        private static SQLiteConnection conn;
        private static SQLiteCommand com;
        private static SQLiteTransaction trans;
        private static SQLiteDataReader reader;

        public const string DB_PATH = "../../db/db.sqlite";
        public const string SQL_PATH = "../../db/db.sql";


        public static void init(bool overwrite = false)
        {

            if (!File.Exists(DB_PATH))
            {
                SQLiteConnection.CreateFile(DB_PATH);
                overwrite = true;
            }

            else if (overwrite)
            {
                File.Delete(DB_PATH);
            }

            /* created db connection*/
            conn = new SQLiteConnection("Data Source=" + DB_PATH + ";Version=3;foreign keys=true;");
            conn.Open();

            /* creates sql command for future use */
            com = new SQLiteCommand(conn);

            /* if new db was created */
            if (overwrite)
            {
                /* reads sql script */
                com.CommandText = File.ReadAllText(SQL_PATH);

                /* executes sql script */
                try
                {
                    trans = conn.BeginTransaction();
                    com.ExecuteNonQuery();
                    trans.Commit();
                }
                catch (SQLiteException e)
                {
                    trans.Rollback();
                    Console.WriteLine(e.StackTrace);
                }
            }

        }

        public static bool insertUser(string UserName, string NickName, string Password)
        {



            com.CommandText = "INSERT INTO User(  username, nickname, password, balance) VALUES (  @user, @nick, @pass, @balance)";

            com.Parameters.Add(new SQLiteParameter("@user", UserName));
            com.Parameters.Add(new SQLiteParameter("@nick", NickName));
            com.Parameters.Add(new SQLiteParameter("@pass", Password));
            com.Parameters.Add(new SQLiteParameter("@balance", 0.0));


            try
            {
                trans = conn.BeginTransaction();
                int rows = com.ExecuteNonQuery();
                trans.Commit();
                if (rows > 0)
                    return true;
                return false;

            }
            catch (SQLiteException e)
            {
                trans.Rollback();
                Console.WriteLine(e.StackTrace);
            }

            return false;

        }


        public static bool login(string Nick, string Password)
        {
            com.CommandText = "SELECT * FROM User WHERE nickname = @user AND password = @pass";
            com.Parameters.Add(new SQLiteParameter("@user", Nick));
            com.Parameters.Add(new SQLiteParameter("@pass", Password));


            try
            {
                reader = com.ExecuteReader();
            }
            catch (SQLiteException e)
            {
                Console.WriteLine(e.StackTrace);
                return false;
            }


            if (reader.Read())
            {
                reader.Close();
                return true;
            }
            reader.Close();
            return false;

        }


        public static void logout()
        {

        }

        public static void registerUser()
        {

        }
    }
}
