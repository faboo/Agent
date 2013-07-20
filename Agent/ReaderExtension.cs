using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlServerCe;

namespace Agent {
    public static class ReaderExtension {
        public static string GetString(this SqlCeDataReader reader, string name) {
            return reader.GetString(reader.GetOrdinal(name));
        }

        public static DateTime GetDateTime(this SqlCeDataReader reader, string name) {
            return reader.GetDateTime(reader.GetOrdinal(name));
        }

        public static T GetEnum<T>(this SqlCeDataReader reader, string name) {
            return (T)Enum.Parse(typeof(T), reader.GetString(reader.GetOrdinal(name)), true);
        }
    }
}
