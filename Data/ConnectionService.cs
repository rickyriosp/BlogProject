using Microsoft.Extensions.Configuration;
using Npgsql;
using System;

namespace BlogProject.Data
{
    public class ConnectionService
    {
        public static string GetConnectionString(IConfiguration configuration)
        {
            return "postgres://omwtdsqxdeqzcm:662ee77a5d646a16cb9ec01f2660c68de70f03c876de25a2484f8374e28e3cac@ec2-44-199-26-122.compute-1.amazonaws.com:5432/d1q9uliav4qs58";
            //var connectionString = configuration.GetConnectionString("DefaultConnection");
            //var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
            //return string.IsNullOrWhiteSpace(databaseUrl) ? connectionString : BuildConnectionString(databaseUrl);
        }
        private static string BuildConnectionString(string databaseUrl)
        {
            var databaseUri = new Uri(databaseUrl);
            var userInfo = databaseUri.UserInfo.Split(':');

            return new NpgsqlConnectionStringBuilder()
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = databaseUri.LocalPath.TrimStart('/'),
                SslMode = SslMode.Prefer,
                TrustServerCertificate = true,
            }.ToString();
        }
    }
}
