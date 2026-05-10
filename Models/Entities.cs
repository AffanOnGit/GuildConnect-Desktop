using System;

namespace GuildConnect_Desktop.Models
{
    // Author: Affan Hameed
    // Co-Authors: Saim, Yahya
    
    /// <summary>
    /// Represents a system user across all three roles: Student, Society Head, and Admin.
    /// </summary>
    public class User
    {
        public int UserID { get; set; }
        public string RollNumber { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Represents a university society.
    /// </summary>
    public class Society
    {
        public int SocietyID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int HeadID { get; set; }
        public string Status { get; set; }
    }
    
    /// <summary>
    /// Represents an assigned task for a society member.
    /// </summary>
    public class SocietyTask
    {
        public int TaskID { get; set; }
        public int SocietyID { get; set; }
        public int AssignedToStudentID { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; }
    }

    /// <summary>
    /// Represents a membership relationship between a student and a society.
    /// </summary>
    public class Membership
    {
        public int MembershipID { get; set; }
        public int StudentID { get; set; }
        public int SocietyID { get; set; }
        public string Status { get; set; }
        public string RoleInSociety { get; set; }
    }

    /// <summary>
    /// Represents an event organized by a society.
    /// </summary>
    public class Event
    {
        public int EventID { get; set; }
        public int SocietyID { get; set; }
        public string Title { get; set; }
        public DateTime EventDate { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
    }

    /// <summary>
    /// Represents a student's registration (ticket) for an event.
    /// </summary>
    public class EventRegistration
    {
        public int RegistrationID { get; set; }
        public int EventID { get; set; }
        public int StudentID { get; set; }
        public DateTime RegistrationDate { get; set; }
    }
}
