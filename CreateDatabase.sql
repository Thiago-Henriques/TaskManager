-- Create database
CREATE DATABASE TaskManagerDb;
GO

USE TaskManagerDb;
GO

-- Users table
CREATE TABLE Users (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(256) NOT NULL
);

-- TaskStatus table
CREATE TABLE TaskStatus (
    Id INT PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL
);

-- Seed TaskStatus values
INSERT INTO TaskStatus (Id, Name) VALUES (0, 'Pending');
INSERT INTO TaskStatus (Id, Name) VALUES (1, 'InProgress');
INSERT INTO TaskStatus (Id, Name) VALUES (2, 'Done');

-- Tasks table
CREATE TABLE Tasks (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX),
    DueDate DATETIME NOT NULL,
    StatusId INT NOT NULL FOREIGN KEY REFERENCES TaskStatus(Id),
    UserId UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES Users(Id)
);

-- Insert demo user
INSERT INTO Users (Id, Name, Email, PasswordHash)
VALUES (NEWID(), 'Demo User', 'demo@taskmanager.com', '123456');

-- Insert demo task
INSERT INTO Tasks (Id, Title, Description, DueDate, StatusId, UserId)
VALUES (NEWID(), 'First Task', 'Description of the first task', GETDATE(), 0, (SELECT TOP 1 Id FROM Users));
