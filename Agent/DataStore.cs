using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlServerCe;
using System.Windows;
using System.IO;

namespace Agent {
    public static class DataStore {
        static private SqlCeConnection connection;

        public static SqlCeConnection Connection {
            get { return connection; }
        }

        private static void CreateDatabase() {
            string appDir = Application.Current.GetUserAppDataPath();
            string connString = String.Format("Data Source={0}agent.sdf", appDir);

            try {
                var engine = new SqlCeEngine();

                if(!Directory.Exists(appDir))
                    Directory.CreateDirectory(appDir);

                engine.LocalConnectionString = connString;
                engine.CreateDatabase();
            }
            catch(Exception exception) {
                MessageBox.Show("Failed creating database: \n" + exception.Message, "Agent Error");
            }

            try{
                connection = new SqlCeConnection();
                connection.ConnectionString = connString;
                connection.Open();
            }
            catch(Exception exception) {
                MessageBox.Show("Failed opening database: \n" + exception.Message, "Agent Error");
            }
        }

        public static void Initialize() {
            try {
                string appDir = Application.Current.GetUserAppDataPath();
                string connString = String.Format("Data Source={0}agent.sdf", appDir);

                connection = new SqlCeConnection();
                connection.ConnectionString = connString;
                connection.Open();
            }
            catch {
                CreateDatabase();
            }
        }

        public static bool TableExists(string name) {
            var command = connection.CreateCommand();
            bool exists = false;

            command.CommandText = "SELECT COUNT(TABLE_NAME) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = ?";
            command.Parameters.Add(new SqlCeParameter("table", name));

            exists = (int)command.ExecuteScalar() == 1;
            command.Dispose();

            return exists;
        }
    }
}
