using System;
using System.Data;
using System.Data.SqlClient;
using GuildConnect_Desktop.DataAccess;

namespace GuildConnect_Desktop.Services
{
    /// <summary>
    /// Handles admin-specific data operations (user management, approvals).
    /// </summary>
    public class AdminService
    {
        private readonly DatabaseContext _dbContext;

        public AdminService()
        {
            _dbContext = new DatabaseContext();
        }

        /// <summary>
        /// Returns all users for admin management.
        /// </summary>
        public DataTable GetAllUsers()
        {
            using (var conn = _dbContext.GetConnection())
            {
                conn.Open();
                string query = "SELECT UserID, RollNumber, FullName, Email, Role, IsActive FROM Users ORDER BY Role, FullName";
                using (var adapter = new SqlDataAdapter(query, conn))
                {
                    var dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }

        /// <summary>
        /// Toggles a user's IsActive status.
        /// </summary>
        public bool ToggleUserStatus(int userId)
        {
            using (var conn = _dbContext.GetConnection())
            {
                conn.Open();
                string query = "UPDATE Users SET IsActive = CASE WHEN IsActive = 1 THEN 0 ELSE 1 END WHERE UserID = @ID";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", userId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// Approves a pending society.
        /// </summary>
        public bool ApproveSociety(int societyId)
        {
            using (var conn = _dbContext.GetConnection())
            {
                conn.Open();
                string query = "UPDATE Societies SET Status = 'Approved' WHERE SocietyID = @ID AND Status = 'Pending'";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", societyId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// Approves a pending event.
        /// </summary>
        public bool ApproveEvent(int eventId)
        {
            using (var conn = _dbContext.GetConnection())
            {
                conn.Open();
                string query = "UPDATE Events SET Status = 'Approved' WHERE EventID = @ID AND Status = 'PendingApproval'";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", eventId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// Creates a new society with a specified head user.
        /// </summary>
        public bool CreateSociety(string name, string description, int headId)
        {
            using (var conn = _dbContext.GetConnection())
            {
                conn.Open();
                string query = "INSERT INTO Societies (Name, Description, HeadID, Status) VALUES (@Name, @Desc, @HeadID, 'Pending')";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Desc", description);
                    cmd.Parameters.AddWithValue("@HeadID", headId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// Suspends an active/approved society.
        /// </summary>
        public bool SuspendSociety(int societyId)
        {
            using (var conn = _dbContext.GetConnection())
            {
                conn.Open();
                string query = "UPDATE Societies SET Status = 'Suspended' WHERE SocietyID = @ID";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", societyId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// Deletes a society and all its associated memberships, events, registrations, and tasks.
        /// </summary>
        public bool DeleteSociety(int societyId)
        {
            using (var conn = _dbContext.GetConnection())
            {
                conn.Open();
                // Must delete in order due to FK constraints
                using (var cmd = new SqlCommand("DELETE FROM EventRegistrations WHERE EventID IN (SELECT EventID FROM Events WHERE SocietyID = @ID)", conn))
                { cmd.Parameters.AddWithValue("@ID", societyId); cmd.ExecuteNonQuery(); }

                using (var cmd = new SqlCommand("DELETE FROM Events WHERE SocietyID = @ID", conn))
                { cmd.Parameters.AddWithValue("@ID", societyId); cmd.ExecuteNonQuery(); }

                using (var cmd = new SqlCommand("DELETE FROM SocietyTasks WHERE SocietyID = @ID", conn))
                { cmd.Parameters.AddWithValue("@ID", societyId); cmd.ExecuteNonQuery(); }

                using (var cmd = new SqlCommand("DELETE FROM Memberships WHERE SocietyID = @ID", conn))
                { cmd.Parameters.AddWithValue("@ID", societyId); cmd.ExecuteNonQuery(); }

                using (var cmd = new SqlCommand("DELETE FROM Societies WHERE SocietyID = @ID", conn))
                {
                    cmd.Parameters.AddWithValue("@ID", societyId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}
