using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlServerCe;

namespace Agent {
    public class Reminders {
        private List<Reminder> reminders = new List<Reminder>();

        public Reminders() {
            if (!DataStore.TableExists("reminders"))
                MakeTable();

            Load();
        }

        private void MakeTable() {
            var command = DataStore.Connection.CreateCommand();

            command.CommandText = "CREATE TABLE [reminders] "
                + "([id] INT NOT NULL IDENTITY (1, 1) PRIMARY KEY, [name] NVARCHAR(50) NOT NULL, [text] NVARCHAR(256) NULL, [time] DATETIME NOT NULL, [frequency] NVARCHAR(10))";

            command.ExecuteNonQuery();
            command.Dispose();
        }

        public void Add(string name, string text, DateTime time, Frequency freq) {
            var command = DataStore.Connection.CreateCommand();
            var reminder = new Reminder(this) {
                Name = name,
                Text = text,
                Time = time,
                Frequency = freq
            };

            command.CommandText = "INSERT INTO [reminders] ([name], [text], [time], [frequency]) VALUES (?, ?, ?, ?)";
            command.Parameters.AddWithValue("name", name);
            command.Parameters.AddWithValue("text", text ?? "");
            command.Parameters.AddWithValue("time", time);
            command.Parameters.AddWithValue("freq", freq.ToString());

            command.ExecuteNonQuery();
            command.Dispose();

            reminders.Add(reminder);
            reminder.Init();
        }

        public void Remove(Reminder rem) {
            var command = DataStore.Connection.CreateCommand();

            reminders.Remove(rem);

            command.CommandText = "DELETE FROM [reminders] WHERE [name] = ?";
            command.Parameters.AddWithValue("name", rem.Name);

            command.ExecuteNonQuery();
            command.Dispose();
        }

        public void Load() {
            var command = DataStore.Connection.CreateCommand();
            SqlCeDataReader reader = null;

            command.CommandText = "SELECT * FROM [reminders] ORDER BY [time]";

            reader = command.ExecuteReader();

            while(reader.Read()) {
                Reminder rem = new Reminder(this);

                rem.Name = reader.GetString("name");
                rem.Text = reader.GetString("text");
                rem.Time = reader.GetDateTime("time");
                rem.Frequency = reader.GetEnum<Frequency>("frequency");

                reminders.Add(rem);
                rem.Init();
            }

            reader.Dispose();
            command.Dispose();
        }
    }
}
