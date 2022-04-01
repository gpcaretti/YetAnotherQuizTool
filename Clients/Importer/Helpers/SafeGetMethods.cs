using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatenteN.Importer.Helpers {
    /// <summary>
    ///     Safe get methods
    /// </summary>
    internal static class SafeGetMethods {

        public static string SafeGetString(this OdbcDataReader reader, int colIndex) {
            if (!reader.IsDBNull(colIndex))
                return reader.GetString(colIndex);
            return string.Empty;
        }
        public static int SafeGetInt16(this OdbcDataReader reader, int colIndex) {
            if (!reader.IsDBNull(colIndex))
                return reader.GetInt16(colIndex);
            return 0;
        }
        public static int SafeGetInt32(this OdbcDataReader reader, int colIndex) {
            if (!reader.IsDBNull(colIndex))
                return reader.GetInt32(colIndex);
            return 0;
        }
        public static long SafeGetInt64(this OdbcDataReader reader, int colIndex) {
            if (!reader.IsDBNull(colIndex))
                return reader.GetInt64(colIndex);
            return 0;
        }
        public static double SafeGetDouble(this OdbcDataReader reader, int colIndex) {
            if (!reader.IsDBNull(colIndex))
                return reader.GetDouble(colIndex);
            return 0;
        }
        public static DateTime SafeGetDate(this OdbcDataReader reader, int colIndex) {
            if (!reader.IsDBNull(colIndex))
                return reader.GetDateTime(colIndex);
            return new DateTime();
        }
        public static bool SafeGetBoolean(this OdbcDataReader reader, int colIndex) {
            if (!reader.IsDBNull(colIndex))
                return reader.GetBoolean(colIndex);
            return false;

        }
    }
}
