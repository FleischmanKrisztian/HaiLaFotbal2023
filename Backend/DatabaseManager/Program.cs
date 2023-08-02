using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Versioning.Logic;

namespace Versioning
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var forceLastVersionUpdate = true;//DoForceInstallOfLatestVersion(args);

            string connectionString = GetConnectionString();

            if (string.IsNullOrEmpty(connectionString))
                return;

            if (!IsDatabase())
                SetupDatabase();
            try
            {
                IReadOnlyList<string> output = new VersionManager(connectionString).ExecuteMigrations(forceLastVersionUpdate);

                foreach (string str in output)
                {
                    Console.WriteLine(str);
                }

                Console.WriteLine("Current DB schema is up to date.");
            }
            catch (Exception e)
            {
                Console.WriteLine("There was an error running the sql migrations files because: " + e.Message + ". The stack error trace is: " + Environment.NewLine + e.StackTrace);
            }
        }

        private static string GetConnectionString()
        {
            var configuration = GetConfiguration();

            return configuration["Database:Connection"];
        }

        private static bool IsDatabase()
        {
            bool result = false;

            using (var connection = new SqlConnection(GetConnectionString()))
            {
                var databaseExists = new SqlCommand("select 1 from sys.databases where name = '" + GetDatabaseName() + "';", connection);
                connection.Open();

                var databaseExistsResult = databaseExists.ExecuteScalar();

                if (databaseExistsResult != null)
                {
                    if ((int)databaseExistsResult == 1)
                        result = true;
                }
            }

            return result;
        }

        private static string GetDatabaseName()
        {
            var configuration = GetConfiguration();

            return configuration["Database:Name"];
        }

        private static IConfigurationRoot GetConfiguration()
        {
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            return configuration;
        }

        private static void SetupDatabase()
        {
            var connectionString = GetConnectionString();

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    var queryCommand = new SqlCommand(
                        " CREATE DATABASE " + GetDatabaseName(), connection);
                    connection.Open();
                    queryCommand.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Database could not be created because: " + e.Message + ". The stack error trace is: " + Environment.NewLine + e.StackTrace);
            }
        }

        private static bool DoForceInstallOfLatestVersion(string[] args)
        {
            if (args.Length > 0)
            {
                string forceLastVersionUpdate = args[0];
                if (forceLastVersionUpdate == "forceLastVersionUpdate")
                    return true;
            }

            return false;
        }
    }
}