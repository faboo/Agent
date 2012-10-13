using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data.SqlServerCe;

namespace Agent {
    public class Memories {
        public Memories() {
            if (!DataStore.TableExists("memories"))
                MakeTable();
        }

        private void MakeTable() {
            var command = DataStore.Connection.CreateCommand();

            command.CommandText = "CREATE TABLE [memories] "
                + "([id] INT NOT NULL IDENTITY (1, 1) PRIMARY KEY, [name] NVARCHAR(50) NOT NULL, [text] NVARCHAR(256) NULL, [when] DATETIME NULL)";

            command.ExecuteNonQuery();
        }

        public void Add(string name, string memory) {
            var command = DataStore.Connection.CreateCommand();

            command.CommandText = "INSERT INTO [memories] ([name], [text]) VALUES (?, ?)";
            command.Parameters.Add(new SqlCeParameter("name", name));
            command.Parameters.Add(new SqlCeParameter("text", memory));

            command.ExecuteNonQuery();
            command.Dispose();
        }

        public void Add(string name, string memory, string when) {
            var command = DataStore.Connection.CreateCommand();

            command.CommandText = "INSERT INTO [memories] ([name], [text], [when]) VALUES (?, ?, ?)";
            command.Parameters.Add(new SqlCeParameter("name", name));
            command.Parameters.Add(new SqlCeParameter("text", memory));
            command.Parameters.Add(new SqlCeParameter("when", DateTime.Parse(when)));

            command.ExecuteNonQuery();
            command.Dispose();
        }

        public IEnumerable<string> Get(string name) {
            var command = DataStore.Connection.CreateCommand();
            SqlCeDataReader reader = null;
            List<string> text = new List<string>();

            command.CommandText = "SELECT [text] FROM [memories] WHERE [name] = ?";
            command.Parameters.Add(new SqlCeParameter("name", name));

            reader = command.ExecuteReader();

            while (reader.Read())
                text.Add(reader.GetString(0));
                
            command.Dispose();

            return text;
        }

        public IEnumerable<string> Get(DateTime when) {
            var command = DataStore.Connection.CreateCommand();
            SqlCeDataReader reader = null;
            List<string> text = new List<string>();

            command.CommandText = "SELECT [text] FROM [memories] WHERE [when] = ?";
            command.Parameters.Add(new SqlCeParameter("when", when));

            reader = command.ExecuteReader();

            while (reader.Read())
                text.Add(reader.GetString(0));

            command.Dispose();

            return text;
        }

        public IEnumerable<string> GetTop(int top) {
            var command = DataStore.Connection.CreateCommand();
            SqlCeDataReader reader = null;
            List<string> text = new List<string>();

            command.CommandText = "SELECT TOP (?) [name] FROM [memories]";
            command.Parameters.Add(new SqlCeParameter("top", top));

            reader = command.ExecuteReader();

            while(reader.Read())
                text.Add(reader.GetString(0));

            command.Dispose();

            return text;
        }
    }
}
