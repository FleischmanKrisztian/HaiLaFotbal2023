using Database;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System.Data;
using System.Text.RegularExpressions;

namespace Versioning.Logic
{
    internal class VersionManager
    {
        private readonly Db _dbHelper;
        private readonly Server _server;

        public VersionManager(string connectionString)
        {
            _dbHelper = new Db(connectionString);

            SqlConnection connection = new SqlConnection(connectionString);
            Server server = new Server(new ServerConnection(connection));
            _server = server;
        }

        public IReadOnlyList<string> ExecuteMigrations(bool forceLastVersionUpdate)
        {
            var result = new List<string>();

            string currentVersion = GetCurrentVersion();

            result.Add("Current DB schema version is " + currentVersion);

            IReadOnlyList<Version> versions = GetNewVersions(currentVersion, forceLastVersionUpdate);
            result.Add(versions.Count + " version(s) found");

            string duplicateVersion = GetDuplicatedVersions(versions);
            if (!string.IsNullOrEmpty(duplicateVersion))
            {
                result.Add("Non-unique version found: " + duplicateVersion);

                return result;
            }

            foreach (var version in versions)
            {
                _server.ConnectionContext.ExecuteNonQuery(version.GetContent());
                UpdateVersion(version.VersionNumber);
                result.Add("Executed version: " + version.Name);
            }

            if (!versions.Any())
            {
                result.Add("No updates for the current schema version");
            }

            return result;
        }

        private string GetCurrentVersion()
        {
            if (!DoesSettingsTableExists())
            {
                CreateSettingsTable();
                return "0_0_0";
            }

            return GetCurrentVersionFromSettingsTable();
        }

        private bool DoesSettingsTableExists()
        {
            string query = @"   USE " + Config.Get("Database:Name") + " SELECT CAST(COUNT(1) AS BIT) FROM information_schema.tables WHERE table_name = 'Settings'";

            return _dbHelper.ExecuteScalar<bool>(query);
        }

        private void CreateSettingsTable()
        {
            string query = @"
                            USE " + Config.Get("Database:Name") + @"
                            CREATE TABLE Settings
                            (
                                ""name"" character varying(200) NOT NULL,
                                ""value"" character varying(200),
                                 CONSTRAINT pk_updateserversettings_name PRIMARY KEY(""name"")
                            )

                            INSERT INTO Settings(name, value)
                            VALUES('Version', '0_0_0')";

            _dbHelper.ExecuteNonQuery(query);
        }

        private string GetCurrentVersionFromSettingsTable()
        {
            return _dbHelper.ExecuteScalar<string>("USE " + Config.Get("Database:Name") + " select value from Settings where name='Version'");
        }

        private IReadOnlyList<Version> GetNewVersions(string currentVersion, bool forceLastVersionUpdate = false)
        {
            var regex = new Regex(@"^(\d)*_(\d)*_(\d)*_(.*)(sql)$");

            var queryVersions = new DirectoryInfo(@"Versions\")
                .GetFiles()
                .Where(x => regex.IsMatch(x.Name))
                .Select(x => new Version(x.Name, x.FullName, x.FullName));

            queryVersions = forceLastVersionUpdate ? queryVersions.Where(x => x >= new Version(currentVersion)) : queryVersions.Where(x => x > new Version(currentVersion));

            return queryVersions.OrderBy(x => x.VersionNumber).ToList();
        }

        private string GetDuplicatedVersions(IReadOnlyList<Version> versions)
        {
            string duplicatedVersion = versions.GroupBy(x => x.VersionNumber)
                .Where(x => x.Count() > 1)
                .Select(x => x.Key)
                .FirstOrDefault();

            return string.IsNullOrEmpty(duplicatedVersion) ? string.Empty : duplicatedVersion;
        }

        private void UpdateVersion(string newVersion)
        {
            object[] prams = { "version", newVersion };

            _dbHelper.ExecuteNonQuery("USE " + Config.Get("Database:Name") + " UPDATE Settings SET value = @version where name='Version'", CommandType.Text, prams);
        }
    }
}