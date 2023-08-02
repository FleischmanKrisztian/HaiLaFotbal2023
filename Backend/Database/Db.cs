using Microsoft.Data.SqlClient;
using System.Data;

namespace Database
{
    public class Db
    {
        private readonly string _connectionString;

        public Db()
        {
            _connectionString = Config.Get("ConnectionStrings:Connection");
        }

        public Db(string connectionString)
        {
            _connectionString = connectionString;
        }

        public T ExecuteScalar<T>(string commandText, CommandType commandType = CommandType.Text, params object[] parameters)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand();

            cmd.Connection = conn;
            cmd.CommandText = commandText;
            cmd.CommandType = commandType;
            cmd.SetParameters(parameters);

            T result = default(T);

            try
            {
                conn.Open();
                var rawResult = cmd.ExecuteScalar();

                if (rawResult != null)
                    result = (T)rawResult;
            }
            catch
            {
                throw;
            }

            return result;
        }

        public void ExecuteReader(string commandText, Action<SqlDataReader> mappingToBusinessData, CommandType commandType = CommandType.Text, params SqlParameter[] parameters)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = commandText;
                    cmd.CommandType = commandType;

                    foreach (var sqlParameter in parameters)
                    {
                        cmd.Parameters.Add(sqlParameter);
                    }

                    mappingToBusinessData(cmd.ExecuteReader());
                }
            }
        }

        public List<T> Execute<T>(string sql, Func<IDataReader, T> make, params object[] parameters)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.ConnectionString = _connectionString;

                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandText = sql;
                    cmd.CommandType = CommandType.Text;

                    cmd.SetParameters(parameters);

                    var list = new List<T>();
                    try
                    {
                        connection.Open();
                        //connection.TypeMapper.UseJsonNet(); //TODO: Krisz Maybe this is useless

                        var reader = cmd.ExecuteReader();

                        while (reader.Read())
                            list.Add(make(reader));
                    }
                    catch
                    {
                        throw;
                    }

                    return list;
                }
            }
        }

        public void ExecuteNonQuery(string commandText, params SqlParameter[] parameters)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = commandText;
                    cmd.CommandType = CommandType.Text;

                    foreach (var sqlParameter in parameters)
                    {
                        cmd.Parameters.Add(sqlParameter);
                    }

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void ExecuteNonQuery(string commandText, CommandType commandType = CommandType.Text, params object[] parameters)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand();

            cmd.SetParameters(parameters); //TODO: Krisztian
            cmd.Connection = conn;
            cmd.CommandText = commandText;
            cmd.CommandType = commandType;

            try
            {
                conn.Open();
                //transaction = conn.BeginTransaction();

                cmd.ExecuteNonQuery();

                //transaction.Commit();
            }
            catch
            {
                //transaction?.Rollback();
                throw;
            }
        }
    }
}