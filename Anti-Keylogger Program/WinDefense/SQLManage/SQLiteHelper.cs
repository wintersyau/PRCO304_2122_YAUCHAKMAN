using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinDefense.SQLManage
{
    public class SQLiteHelper
    {
        public static string connectionString = string.Empty;

        /// <summary>
        /// Sets the connection string based on the data source, password, version number.
        /// </summary>
        /// <param name="datasource">datasource。</param>
        /// <param name="password">pw。</param>
        /// <param name="version">version（lost 3）。</param>
        public static void SetConnectionString(string datasource, string password, int version = 3)
        {
            if (string.IsNullOrWhiteSpace(password))
                connectionString = string.Format("Data Source={0};Version={1};", datasource, version, password);
            else
                connectionString = string.Format("Data Source={0};Version={1};password={2}", datasource, version, password);
        }

        /// <summary>
        /// Create a database file. if using the same name will overwritten.
        /// </summary>
        /// <param name="dbName">Table name</param>
        /// <param name="password">password</param>
        /// <exception cref="Exception"></exception>
        public static void CreateDB(string dbName)
        {
            if (!string.IsNullOrEmpty(dbName))
            {
                try { SQLiteConnection.CreateFile(dbName); }
                catch (Exception) { throw; }
            }
        }

        /// <summary> 
        /// Performs an addition deletion or alteration on SQLite database and return the number of rows affected. 
        /// </summary> 
        /// <param name="sql">The SQL to be added, deleted or altered.</param> 
        /// <param name="parameters">The parameters required to execute the addition, deletion, or alteration must be in the order in the SQL statement.
</param> 
        /// <returns></returns> 
        /// <exception cref="Exception"></exception>
        public static int ExecuteNonQuery(string sql, string dataWxid = "", params SQLiteParameter[] parameters)
        {
            int affectedRows = 0;
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    try
                    {
                        connection.Open();
                        command.CommandText = sql;
                        if (parameters.Length != 0)
                        {
                            command.Parameters.AddRange(parameters);
                        }
                        affectedRows = command.ExecuteNonQuery();

                        command.Connection.Close(); command.Dispose();
                    }
                    catch (Exception) { throw; }
                }
            }
            return affectedRows;
        }

        /// <summary>
        /// Batch process data manipulation statements.
        /// </summary>
        /// <param name="list">SQL statements。</param>
        /// <exception cref="Exception"></exception>
        public static void ExecuteNonQueryBatch(List<KeyValuePair<string, SQLiteParameter[]>> list, string dataWxid = "")
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try { conn.Open(); }
                catch { throw; }
                using (SQLiteTransaction tran = conn.BeginTransaction())
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                    {
                        try
                        {
                            foreach (var item in list)
                            {
                                cmd.CommandText = item.Key;
                                if (item.Value != null)
                                {
                                    cmd.Parameters.AddRange(item.Value);
                                }
                                cmd.ExecuteNonQuery();
                            }
                            tran.Commit();
                        }
                        catch (Exception) { tran.Rollback(); throw; }
                    }
                }
            }
        }

        public static void ExecuteNonQueryBatch(Dictionary<string, SQLiteParameter[]> list, string dataWxid = "")
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try { conn.Open(); }
                catch { throw; }
                using (SQLiteTransaction tran = conn.BeginTransaction())
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                    {
                        try
                        {
                            foreach (var item in list)
                            {
                                cmd.CommandText = item.Key;
                                if (item.Value != null)
                                {
                                    cmd.Parameters.AddRange(item.Value);
                                }
                                cmd.ExecuteNonQuery();
                            }
                            tran.Commit();
                        }
                        catch (Exception) { tran.Rollback(); throw; }
                    }
                }
            }
        }

        public static void ExecuteNonQueryBatch(List<string> list, string dataWxid = "")
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try { conn.Open(); }
                catch { throw; }
                using (SQLiteTransaction tran = conn.BeginTransaction())
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                    {
                        try
                        {
                            foreach (var item in list)
                            {
                                cmd.CommandText = item;
                                cmd.ExecuteNonQuery();
                            }
                            tran.Commit();
                        }
                        catch (Exception) { tran.Rollback(); throw; }
                    }
                }
            }
        }

        /// <summary>
        /// Executes the query statement and returns the first result.
        /// </summary>
        /// <param name="sql">Query</param>
        /// <returns>returns。</returns>
        /// <exception cref="Exception"></exception>
        public static object ExecuteScalar(string sql, string dataWxid = "", params SQLiteParameter[] parameters)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    try
                    {
                        conn.Open();
                        cmd.CommandText = sql;
                        if (parameters.Length != 0)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }
                        object obj = cmd.ExecuteScalar();
                        cmd.Connection.Close(); cmd.Dispose();
                        return obj;
                    }
                    catch (Exception) { throw; }
                }
            }
        }

        public static DataTable ExecuteQuery(string sql, string dataWxid = "", params SQLiteParameter[] parameters)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                {
                    if (parameters.Length != 0)
                    {
                        command.Parameters.AddRange(parameters);
                    }
                    SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
                    DataTable data = new DataTable();
                    try { adapter.Fill(data); adapter.Dispose(); command.Connection.Close(); command.Dispose(); }
                    catch (Exception) { throw; }
                    return data;
                }
            }
        }

        public static SQLiteDataReader ExecuteReader(string sql, string dataWxid = "", params SQLiteParameter[] parameters)
        {
            SQLiteConnection connection = new SQLiteConnection(connectionString);
            SQLiteCommand command = new SQLiteCommand(sql, connection);
            try
            {
                if (parameters.Length != 0)
                {
                    command.Parameters.AddRange(parameters);
                }
                connection.Open();
                return command.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception) { throw; }
        }

        /// <summary> 
        /// Queries all information data
        /// </summary> 
        /// <returns></returns> 
        /// <exception cref="Exception"></exception>
        public static DataTable GetSchema()
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    return connection.GetSchema("TABLES");
                }
                catch (Exception) { throw; }
            }
        }

        public static bool InitSqliteConfig(string DbName)
        {
            bool returnStatus = false;

            string config_dir = System.AppDomain.CurrentDomain.BaseDirectory + "Sqlite";

            if (!Directory.Exists(config_dir))
            {
                Directory.CreateDirectory(config_dir);
            }

            string DataPath = config_dir + @"\" + DbName + ".db";

            if (!File.Exists(DataPath))
            {
                CreateDB(DataPath);
                returnStatus = true;
            }

            SetConnectionString(DataPath, "");

            return returnStatus;
        }
    }
}
