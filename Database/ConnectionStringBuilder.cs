using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MySql.Data.MySqlClient;

namespace Gemini.Database
{
    public static class ConnectionStringBuilder
    {
        public static string BuildMySQL(string database, string ip)
        {
            var connectionString = new MySqlConnectionStringBuilder
            {
                ConnectionTimeout = 10,
                Database = database,
                DefaultCommandTimeout = 30,
                Logging = false,
                MaximumPoolSize = 10,
                MinimumPoolSize = 1,
                Pooling = true,
                Server = ip,
                Port = 3306,
                UserID = "proteger",
                Password = "Escelsa2016@",
                AllowZeroDateTime = true,
                ConvertZeroDateTime = true,
                SslMode = 0
            };
            return connectionString.ToString();
        }

        public static string BuildAccess()
        {
            return @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=topo.mdb";
        }

        public static string BuildQualityAccess()
        {
            return @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=quali.mdb";
        }
    }
}
