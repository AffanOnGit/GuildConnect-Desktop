using System;
using System.Data.SqlClient;

namespace GuildConnect_Desktop.DataAccess
{
    /// <summary>
    /// Centralized database connection manager to ensure low Coupling Between Objects (CBO).
    /// </summary>
    public class DatabaseContext
    {
        private readonly string _connectionString = @"Server=localhost\SQLEXPRESS01;Database=GuildConnectDB;Trusted_Connection=True;TrustServerCertificate=True;";

        /// <summary>
        /// Retrieves a new SQL connection instance.
        /// </summary>
        public SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
