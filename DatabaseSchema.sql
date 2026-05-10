-- Create the dedicated database
CREATE DATABASE GuildConnectDB;
GO
USE GuildConnectDB;
GO

-- 1. Users Table (Handles Students, Society Heads, and Admins)
CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    RollNumber NVARCHAR(20) UNIQUE NULL, -- e.g., 22I-2582
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(256) NOT NULL,
    Role NVARCHAR(20) CHECK (Role IN ('Student', 'SocietyHead', 'Admin')) NOT NULL,
    IsActive BIT DEFAULT 1
);

-- 2. Societies Table
CREATE TABLE Societies (
    SocietyID INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) UNIQUE NOT NULL,
    Description NVARCHAR(500),
    HeadID INT FOREIGN KEY REFERENCES Users(UserID),
    Status NVARCHAR(20) CHECK (Status IN ('Pending', 'Approved', 'Suspended')) DEFAULT 'Pending'
);

-- 3. Memberships Table (Join Table for Students and Societies)
CREATE TABLE Memberships (
    MembershipID INT IDENTITY(1,1) PRIMARY KEY,
    StudentID INT FOREIGN KEY REFERENCES Users(UserID),
    SocietyID INT FOREIGN KEY REFERENCES Societies(SocietyID),
    Status NVARCHAR(20) CHECK (Status IN ('Pending', 'Approved', 'Rejected')) DEFAULT 'Pending',
    RoleInSociety NVARCHAR(50) DEFAULT 'Member'
);

-- 4. Events Table
CREATE TABLE Events (
    EventID INT IDENTITY(1,1) PRIMARY KEY,
    SocietyID INT FOREIGN KEY REFERENCES Societies(SocietyID),
    Title NVARCHAR(100) NOT NULL,
    EventDate DATETIME NOT NULL,
    Description NVARCHAR(500),
    Status NVARCHAR(20) CHECK (Status IN ('PendingApproval', 'Approved', 'Cancelled')) DEFAULT 'PendingApproval'
);

-- 5. Event Registrations (Tickets/Passes)
CREATE TABLE EventRegistrations (
    RegistrationID INT IDENTITY(1,1) PRIMARY KEY,
    EventID INT FOREIGN KEY REFERENCES Events(EventID),
    StudentID INT FOREIGN KEY REFERENCES Users(UserID),
    RegistrationDate DATETIME DEFAULT GETDATE()
);

-- 6. Tasks Table
CREATE TABLE SocietyTasks (
    TaskID INT IDENTITY(1,1) PRIMARY KEY,
    SocietyID INT FOREIGN KEY REFERENCES Societies(SocietyID),
    AssignedToStudentID INT FOREIGN KEY REFERENCES Users(UserID),
    Description NVARCHAR(500) NOT NULL,
    DueDate DATETIME NOT NULL,
    Status NVARCHAR(20) CHECK (Status IN ('Pending', 'InProgress', 'Completed')) DEFAULT 'Pending'
);
