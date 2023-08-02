using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace Database
{
    /// <summary>
    /// Useful set of Extension methods for Data Access purposes.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Transform object into Identity data type (integer 64).
        /// </summary>
        /// <param name="item">The object to be transformed.</param>
        /// <param name="defaultId">Optional default value is -1.</param>
        /// <returns>Identity value.</returns>
        public static long AsId(this object item, long defaultId = -1)
        {
            if (item == null)
                return defaultId;

            if (!long.TryParse(item.ToString(), out var result))
                return defaultId;

            return result;
        }

        /// <summary>
        /// Transform object into integer 32 data type.
        /// </summary>
        /// <param name="item">The object to be transformed.</param>
        /// <param name="defaultInt">Optional default value is default(int).</param>
        /// <returns>The integer 32 value.</returns>
        public static int AsInt(this object item, int defaultInt = default)
        {
            if (item == null)
                return defaultInt;

            if (!int.TryParse(item.ToString(), out var result))
                return defaultInt;

            return result;
        }

        /// <summary>
        /// Transform object into integer 64 data type.
        /// </summary>
        /// <param name="item">The object to be transformed.</param>
        /// <param name="defaultInt">Optional default value is default(int).</param>
        /// <returns>The integer 64 value.</returns>
        public static long AsLong(this object item, long defaultInt = default)
        {
            if (item == null)
                return defaultInt;

            if (!long.TryParse(item.ToString(), out var result))
                return defaultInt;

            return result;
        }

        /// <summary>
        /// Transform object into double data type.
        /// </summary>
        /// <param name="item">The object to be transformed.</param>
        /// <param name="defaultDouble">Optional default value is default(double).</param>
        /// <returns>The double value.</returns>
        public static double AsDouble(this object item, double defaultDouble = default)
        {
            if (item == null)
                return defaultDouble;

            if (!double.TryParse(item.ToString(), out var result))
                return defaultDouble;

            return result;
        }

        /// <summary>
        /// Transform object into string data type.
        /// </summary>
        /// <param name="item">The object to be transformed.</param>
        /// <param name="defaultString">Optional default value is default(string).</param>
        /// <returns>The string value.</returns>
        public static string AsString(this object item, string defaultString = default(string))
        {
            defaultString = string.Empty;
            if (item == null || item.Equals(System.DBNull.Value))
                return defaultString;

            return item.ToString().Trim();
        }

        /// <summary>
        /// Transform object into boolean data type.
        /// </summary>
        /// <param name="item">The object to be transformed.</param>
        /// <param name="defaultBool">Optional default value is false.</param>
        /// <returns>The boolean value.</returns>
        public static bool AsBoolean(this object item, bool defaultBool = false)
        {
            if (item == null)
                return defaultBool;

            if (!bool.TryParse(item.ToString(), out var result))
                return defaultBool;

            return result;
        }

        public static string AsSortingOrder(this string item, string defaultString = "ASC")
        {
            if (item == null)
                return defaultString;

            if (string.Equals(item, "Ascending"))
                return "ASC";

            if (string.Equals(item, "Descending"))
                return "DESC";

            return defaultString;
        }

        public static void SetParameters(this SqlCommand command, object[] parms)
        {
            if (parms != null && parms.Length > 0)
            {
                // NOTE: Processes a name/value pair at each iteration
                for (int i = 0; i < parms.Length; i += 2)
                {
                    string name = parms[i].ToString();

                    // No empty strings to the database
                    if (parms[i + 1] is string && (string)parms[i + 1] == "")
                        parms[i + 1] = null;

                    // If null, set to DbNull
                    object value = parms[i + 1] ?? DBNull.Value;

                    SqlParameter dbParameter;
                    if (name.IndexOf("JSON", StringComparison.InvariantCulture) < 0)
                    {
                        dbParameter = command.CreateParameter();
                        dbParameter.ParameterName = name;
                        dbParameter.Value = value;
                    }
                    else
                    {
                        dbParameter = new SqlParameter(name, SqlDbType.Text) { Value = value };//TODO: Check if works with text instead of json
                    }

                    command.Parameters.Add(dbParameter);
                }
            }
        }

        public static string SanitizeSqlIdentifier(this string value)
        {
            value = value.Trim().Replace(" ", "").Replace("\\", "").Replace("'", "").Replace("-", "");

            return value;
        }

        public static string ToSqlIdentifier(this string value)
        {
            return " quote_ident(" + value + ")";
        }

        public static object[] ToParams(this Dictionary<string, string> value)
        {
            object[] result = new object[value.Count * 2];
            int index = 0;
            foreach (var param in value)
            {
                result[index] = param.Key;
                index++;
                result[index] = param.Value;
                index++;
            }
            return result;
        }
    }
}