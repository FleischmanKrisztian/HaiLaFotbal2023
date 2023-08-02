using Microsoft.Extensions.Configuration;

namespace Database
{
    public static class Config
    {
        private static readonly IConfiguration configuration;

        static Config()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            configuration = builder.Build();
        }

        public static string Get(string name)
        {
            return configuration[name];
        }
    }
}