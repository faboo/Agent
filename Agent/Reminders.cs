using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlServerCe;

namespace Agent {
    public class Reminders {
        public Reminders() {
            if (!DataStore.TableExists("reminders"))
                MakeTable();
        }

        private void MakeTable() {
            var command = DataStore.Connection.CreateCommand();

            command.CommandText = "CREATE TABLE [reminders] "
                + "([id] INT NOT NULL IDENTITY (1, 1) PRIMARY KEY, [name] NVARCHAR(50) NOT NULL, [text] NVARCHAR(256) NULL, [time] TIME NOT NULL, [frequency] NVARCHAR(10))";

            command.ExecuteNonQuery();
        }

        public void Add(string name, string text, DateTime time, Frequency freq) {

        }

        /*public void Load() {
            var command = DataStore.Connection.CreateCommand();
            SqlCeDataReader reader = null;

            command.CommandText = "SELECT  FROM [pad] ORDER BY [id]";

            Lines.Clear();

            reader = command.ExecuteReader();

            while (reader.Read())
                Lines.Add(new Line { Text = reader.GetString(0) });

            if (Lines.Last().Text != "")
                Lines.Add(new Line { Text = "" });

            reader.Dispose();
            command.Dispose();
        }*/

    }
}
