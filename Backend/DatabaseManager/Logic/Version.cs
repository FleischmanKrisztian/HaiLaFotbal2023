using Database;
using System.Globalization;

namespace Versioning.Logic
{
    internal class Version
    {
        public string Name { get; }

        public string FilePath { get; }

        public string VersionNumber { get; }

        public int Major { get; }

        public int Minor { get; }

        public int Revision { get; }

        public Version(string version)
        {
            var entireVersionNumber = version.Split(new string[] { "_" }, StringSplitOptions.None);

            Major = int.Parse(entireVersionNumber[0]);
            Minor = int.Parse(entireVersionNumber[1]);
            Revision = int.Parse(entireVersionNumber[2]);

            VersionNumber = Major.ToString(CultureInfo.InvariantCulture) + "_" + Minor.ToString(CultureInfo.InvariantCulture) + "_" + Revision.ToString(CultureInfo.InvariantCulture);
        }

        public Version(string version, string name, string filePath) : this(version)
        {
            Name = name;
            FilePath = filePath;
        }

        public static bool operator >(Version v1, Version v2)
        {
            if (v1.Major < v2.Major)
                return false;

            if (v1.Minor < v2.Minor)
                return false;

            if (v1.Revision < v2.Revision)
                return false;

            if (v1.Major == v2.Major && v1.Minor == v2.Minor && v1.Revision == v2.Revision)
                return false;

            return true;
        }

        public static bool operator >=(Version v1, Version v2)
        {
            if (v1.Major < v2.Major)
                return false;

            if (v1.Minor < v2.Minor)
                return false;

            if (v1.Revision < v2.Revision)
                return false;

            return true;
        }

        public static bool operator <(Version v1, Version v2)
        {
            if (v1.Major > v2.Major)
                return false;

            if (v1.Minor > v2.Minor)
                return false;

            if (v1.Revision > v2.Revision)
                return false;

            if (v1.Major == v2.Major && v1.Minor == v2.Minor && v1.Revision == v2.Revision)
                return false;

            return true;
        }

        public static bool operator <=(Version v1, Version v2)
        {
            if (v1.Major > v2.Major)
                return false;

            if (v1.Minor > v2.Minor)
                return false;

            if (v1.Revision > v2.Revision)
                return false;

            return true;
        }

        public string GetContent()
        {
            return "USE " + Config.Get("Database:Name") + " " + File.ReadAllText(FilePath);
        }
    }
}