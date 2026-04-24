using Npgsql;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace BookingSystem.Data
{
    public class DapperContext
    {
        private readonly string _connectionString;

        public DapperContext(IConfiguration configuration)
        {
            var rawUrl = Environment.GetEnvironmentVariable("DATABASE_URL")
                ?? configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string not found.");

            _connectionString = ConvertDatabaseUrl(rawUrl);
        }

        public IDbConnection CreateConnection()
            => new NpgsqlConnection(_connectionString);

        private static string ConvertDatabaseUrl(string url)
        {
            // If it's already a proper connection string, return as-is
            if (!url.StartsWith("postgresql://") && !url.StartsWith("postgres://"))
                return url;

            var uri = new Uri(url);
            var userInfo = uri.UserInfo.Split(':');
            var username = userInfo[0];
            var password = userInfo.Length > 1 ? userInfo[1] : "";
            var host = uri.Host;
            var port = uri.Port > 0 ? uri.Port : 5432;
            var database = uri.AbsolutePath.TrimStart('/');

            return $"Host={host};Port={port};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true";
        }
    }
}