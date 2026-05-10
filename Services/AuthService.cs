using System;
using System.Data.SqlClient;
using GuildConnect_Desktop.Models;
using GuildConnect_Desktop.DataAccess;

namespace GuildConnect_Desktop.Services
{
    /// <summary>
    /// Handles user authentication and registration logic.
    /// Helps keep Cyclomatic Complexity low in UI classes.
    /// </summary>
    public class AuthService
    {
        private readonly DatabaseContext _dbContext;

        public AuthService()
        {
            _dbContext = new DatabaseContext();
        }

        /// <summary>
        /// Authenticates a user based on email and password.
        /// </summary>
        public User Login(string email, string password)
        {
            using (var conn = _dbContext.GetConnection())
            {
                conn.Open();
                string query = "SELECT UserID, RollNumber, FullName, Email, Role, IsActive FROM Users WHERE Email = @Email AND PasswordHash = @Password AND IsActive = 1";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    // In a real scenario, use hashed passwords. Using plain text or simple hash here for the assignment.
                    cmd.Parameters.AddWithValue("@Password", password);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                UserID = Convert.ToInt32(reader["UserID"]),
                                RollNumber = reader["RollNumber"]?.ToString(),
                                FullName = reader["FullName"].ToString(),
                                Email = reader["Email"].ToString(),
                                Role = reader["Role"].ToString(),
                                IsActive = Convert.ToBoolean(reader["IsActive"])
                            };
                        }
                    }
                }
            }
            return null; // Login failed
        }

        /// <summary>
        /// Registers a new student user.
        /// </summary>
        public bool Register(string rollNumber, string fullName, string email, string password)
        {
            using (var conn = _dbContext.GetConnection())
            {
                conn.Open();
                string query = "INSERT INTO Users (RollNumber, FullName, Email, PasswordHash, Role, IsActive) VALUES (@Roll, @Name, @Email, @Password, 'Student', 1)";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Roll", rollNumber);
                    cmd.Parameters.AddWithValue("@Name", fullName);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", password);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}
