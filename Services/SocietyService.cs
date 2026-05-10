using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using GuildConnect_Desktop.Models;
using GuildConnect_Desktop.DataAccess;

namespace GuildConnect_Desktop.Services
{
    /// <summary>
    /// Handles all society-related data operations.
    /// Used by Student and SocietyHead dashboards.
    /// </summary>
    public class SocietyService
    {
        private readonly DatabaseContext _dbContext;

        public SocietyService()
        {
            _dbContext = new DatabaseContext();
        }

        /// <summary>
        /// Returns all approved societies for student browsing.
        /// </summary>
        public DataTable GetApprovedSocieties()
        {
            using (var conn = _dbContext.GetConnection())
            {
                conn.Open();
                string query = @"SELECT s.SocietyID, s.Name, s.Description, u.FullName AS [Society Head], s.Status 
                                 FROM Societies s 
                                 LEFT JOIN Users u ON s.HeadID = u.UserID 
                                 WHERE s.Status = 'Approved'";
                using (var adapter = new SqlDataAdapter(query, conn))
                {
                    var dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }

        /// <summary>
        /// Returns all societies (any status) for admin oversight.
        /// </summary>
        public DataTable GetAllSocieties()
        {
            using (var conn = _dbContext.GetConnection())
            {
                conn.Open();
                string query = @"SELECT s.SocietyID, s.Name, s.Description, u.FullName AS [Society Head], s.Status 
                                 FROM Societies s 
                                 LEFT JOIN Users u ON s.HeadID = u.UserID";
                using (var adapter = new SqlDataAdapter(query, conn))
                {
                    var dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }

        /// <summary>
        /// Returns the society managed by a specific head user.
        /// </summary>
        public DataTable GetSocietyByHead(int headUserId)
        {
            using (var conn = _dbContext.GetConnection())
            {
                conn.Open();
                string query = @"SELECT SocietyID, Name, Description, Status FROM Societies WHERE HeadID = @HeadID";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@HeadID", headUserId);
                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        /// <summary>
        /// Returns upcoming approved events for students.
        /// </summary>
        public DataTable GetUpcomingEvents()
        {
            using (var conn = _dbContext.GetConnection())
            {
                conn.Open();
                string query = @"SELECT e.EventID, e.Title, s.Name AS [Society], e.EventDate, e.Description, e.Status 
                                 FROM Events e 
                                 INNER JOIN Societies s ON e.SocietyID = s.SocietyID 
                                 WHERE e.Status = 'Approved' AND CAST(e.EventDate AS DATE) >= CAST(GETDATE() AS DATE)
                                 ORDER BY e.EventDate";
                using (var adapter = new SqlDataAdapter(query, conn))
                {
                    var dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }

        /// <summary>
        /// Returns all events (any status) for admin oversight.
        /// </summary>
        public DataTable GetAllEvents()
        {
            using (var conn = _dbContext.GetConnection())
            {
                conn.Open();
                string query = @"SELECT e.EventID, e.Title, s.Name AS [Society], e.EventDate, e.Status 
                                 FROM Events e 
                                 INNER JOIN Societies s ON e.SocietyID = s.SocietyID
                                 ORDER BY e.EventDate DESC";
                using (var adapter = new SqlDataAdapter(query, conn))
                {
                    var dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }

        /// <summary>
        /// Returns events belonging to a specific society.
        /// </summary>
        public DataTable GetEventsBySociety(int societyId)
        {
            using (var conn = _dbContext.GetConnection())
            {
                conn.Open();
                string query = @"SELECT EventID, Title, EventDate, Description, Status FROM Events WHERE SocietyID = @SocietyID ORDER BY EventDate DESC";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SocietyID", societyId);
                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        /// <summary>
        /// Returns tasks assigned to a specific student.
        /// </summary>
        public DataTable GetTasksByStudent(int studentId)
        {
            using (var conn = _dbContext.GetConnection())
            {
                conn.Open();
                string query = @"SELECT t.TaskID, s.Name AS [Society], t.Description, t.DueDate, t.Status 
                                 FROM SocietyTasks t 
                                 INNER JOIN Societies s ON t.SocietyID = s.SocietyID 
                                 WHERE t.AssignedToStudentID = @StudentID
                                 ORDER BY t.DueDate";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@StudentID", studentId);
                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        /// <summary>
        /// Returns tasks belonging to a specific society.
        /// </summary>
        public DataTable GetTasksBySociety(int societyId)
        {
            using (var conn = _dbContext.GetConnection())
            {
                conn.Open();
                string query = @"SELECT t.TaskID, u.FullName AS [Assigned To], t.Description, t.DueDate, t.Status 
                                 FROM SocietyTasks t 
                                 INNER JOIN Users u ON t.AssignedToStudentID = u.UserID 
                                 WHERE t.SocietyID = @SocietyID
                                 ORDER BY t.DueDate";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SocietyID", societyId);
                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        /// <summary>
        /// Returns all members/membership requests for a specific society.
        /// </summary>
        public DataTable GetMembersBySociety(int societyId)
        {
            using (var conn = _dbContext.GetConnection())
            {
                conn.Open();
                string query = @"SELECT m.MembershipID, u.FullName, u.RollNumber, u.Email, m.Status, m.RoleInSociety 
                                 FROM Memberships m 
                                 INNER JOIN Users u ON m.StudentID = u.UserID 
                                 WHERE m.SocietyID = @SocietyID";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SocietyID", societyId);
                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        /// <summary>
        /// Submits a join request (Pending membership) for a student to a society.
        /// </summary>
        public bool RequestToJoin(int studentId, int societyId)
        {
            using (var conn = _dbContext.GetConnection())
            {
                conn.Open();
                // Check if already a member or pending
                string checkQuery = "SELECT COUNT(*) FROM Memberships WHERE StudentID = @StudentID AND SocietyID = @SocietyID";
                using (var checkCmd = new SqlCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@StudentID", studentId);
                    checkCmd.Parameters.AddWithValue("@SocietyID", societyId);
                    if (Convert.ToInt32(checkCmd.ExecuteScalar()) > 0)
                        throw new InvalidOperationException("You have already requested or joined this society.");
                }

                string query = "INSERT INTO Memberships (StudentID, SocietyID, Status, RoleInSociety) VALUES (@StudentID, @SocietyID, 'Pending', 'Member')";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@StudentID", studentId);
                    cmd.Parameters.AddWithValue("@SocietyID", societyId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// Registers a student for an event.
        /// </summary>
        public bool RegisterForEvent(int studentId, int eventId)
        {
            using (var conn = _dbContext.GetConnection())
            {
                conn.Open();
                // Check if already registered
                string checkQuery = "SELECT COUNT(*) FROM EventRegistrations WHERE StudentID = @StudentID AND EventID = @EventID";
                using (var checkCmd = new SqlCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@StudentID", studentId);
                    checkCmd.Parameters.AddWithValue("@EventID", eventId);
                    if (Convert.ToInt32(checkCmd.ExecuteScalar()) > 0)
                        throw new InvalidOperationException("You are already registered for this event.");
                }

                string query = "INSERT INTO EventRegistrations (EventID, StudentID) VALUES (@EventID, @StudentID)";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EventID", eventId);
                    cmd.Parameters.AddWithValue("@StudentID", studentId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// Updates a membership status (Approve/Reject).
        /// </summary>
        public bool UpdateMembershipStatus(int membershipId, string status)
        {
            using (var conn = _dbContext.GetConnection())
            {
                conn.Open();
                string query = "UPDATE Memberships SET Status = @Status WHERE MembershipID = @ID";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Status", status);
                    cmd.Parameters.AddWithValue("@ID", membershipId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// Creates a new event for a society.
        /// </summary>
        public bool CreateEvent(int societyId, string title, DateTime eventDate, string description)
        {
            using (var conn = _dbContext.GetConnection())
            {
                conn.Open();
                string query = "INSERT INTO Events (SocietyID, Title, EventDate, Description, Status) VALUES (@SocietyID, @Title, @Date, @Desc, 'PendingApproval')";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SocietyID", societyId);
                    cmd.Parameters.AddWithValue("@Title", title);
                    cmd.Parameters.AddWithValue("@Date", eventDate);
                    cmd.Parameters.AddWithValue("@Desc", description);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// Assigns a task to a student in a society.
        /// </summary>
        public bool AssignTask(int societyId, int studentId, string description, DateTime dueDate)
        {
            using (var conn = _dbContext.GetConnection())
            {
                conn.Open();
                string query = "INSERT INTO SocietyTasks (SocietyID, AssignedToStudentID, Description, DueDate, Status) VALUES (@SocietyID, @StudentID, @Desc, @Due, 'Pending')";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SocietyID", societyId);
                    cmd.Parameters.AddWithValue("@StudentID", studentId);
                    cmd.Parameters.AddWithValue("@Desc", description);
                    cmd.Parameters.AddWithValue("@Due", dueDate);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// Updates an existing event's title, description, and date.
        /// </summary>
        public bool UpdateEvent(int eventId, string title, DateTime eventDate, string description)
        {
            using (var conn = _dbContext.GetConnection())
            {
                conn.Open();
                string query = "UPDATE Events SET Title = @Title, EventDate = @Date, Description = @Desc WHERE EventID = @ID";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Title", title);
                    cmd.Parameters.AddWithValue("@Date", eventDate);
                    cmd.Parameters.AddWithValue("@Desc", description);
                    cmd.Parameters.AddWithValue("@ID", eventId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// Cancels an event by setting its status to 'Cancelled'.
        /// </summary>
        public bool CancelEvent(int eventId)
        {
            using (var conn = _dbContext.GetConnection())
            {
                conn.Open();
                string query = "UPDATE Events SET Status = 'Cancelled' WHERE EventID = @ID";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ID", eventId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// Returns all event registrations (tickets/passes) for a student.
        /// </summary>
        public DataTable GetStudentRegistrations(int studentId)
        {
            using (var conn = _dbContext.GetConnection())
            {
                conn.Open();
                string query = @"SELECT er.RegistrationID, e.Title AS [Event], s.Name AS [Society], 
                                 e.EventDate, er.RegistrationDate AS [Registered On], e.Status AS [Event Status]
                                 FROM EventRegistrations er
                                 INNER JOIN Events e ON er.EventID = e.EventID
                                 INNER JOIN Societies s ON e.SocietyID = s.SocietyID
                                 WHERE er.StudentID = @StudentID
                                 ORDER BY e.EventDate DESC";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@StudentID", studentId);
                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        /// <summary>
        /// Returns the membership status for all societies a student has joined or requested.
        /// </summary>
        public DataTable GetStudentMemberships(int studentId)
        {
            using (var conn = _dbContext.GetConnection())
            {
                conn.Open();
                string query = @"SELECT m.MembershipID, s.Name AS [Society], m.Status, m.RoleInSociety AS [Role]
                                 FROM Memberships m
                                 INNER JOIN Societies s ON m.SocietyID = s.SocietyID
                                 WHERE m.StudentID = @StudentID";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@StudentID", studentId);
                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }
    }
}
