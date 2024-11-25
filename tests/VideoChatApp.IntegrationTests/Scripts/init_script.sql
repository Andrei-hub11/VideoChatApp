-- Create ApplicationUsers table first since it's referenced by other tables
CREATE TABLE ApplicationUsers (
    Id NVARCHAR(450) PRIMARY KEY NOT NULL,
    UserName NVARCHAR(120) NOT NULL,
    Email NVARCHAR(256) NOT NULL,
    ProfileImage VARBINARY(MAX) NOT NULL DEFAULT (0x),
    ProfileImagePath NVARCHAR(MAX) NOT NULL DEFAULT '',
    CreatedAt DATETIME DEFAULT GETDATE() NOT NULL,
    UpdatedAt DATETIME DEFAULT GETDATE() NOT NULL,
    CONSTRAINT UQ_ApplicationUsers_Email UNIQUE (Email)
);
GO

-- Create trigger for ApplicationUsers updates
CREATE TRIGGER TriggerApplicationUserUpdate
ON ApplicationUsers
AFTER UPDATE
AS
BEGIN
    IF UPDATE(UserName) OR UPDATE(Email) OR UPDATE(ProfileImage) OR UPDATE(ProfileImagePath)
    BEGIN
        UPDATE ApplicationUsers
        SET UpdatedAt = GETDATE()
        FROM ApplicationUsers
        INNER JOIN inserted ON ApplicationUsers.Id = inserted.Id;
    END
END;
GO

-- Create ApplicationUserRoles table
CREATE TABLE ApplicationUserRoles (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId NVARCHAR(450) NOT NULL,
    Name NVARCHAR(256) NOT NULL,
    CONSTRAINT FK_UserRoles_User FOREIGN KEY (UserId) REFERENCES ApplicationUsers(Id) ON DELETE CASCADE
);
GO

-- Create ValidMemberRoles table
CREATE TABLE ValidMemberRoles (
    Role NVARCHAR(50) PRIMARY KEY
);
GO

-- Insert valid roles
INSERT INTO ValidMemberRoles (Role)
VALUES ('member'), ('admin'), ('moderator');
GO

-- Create Room table
CREATE TABLE Room (
    RoomId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    RoomName VARCHAR(100) NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
);
GO

-- Create Member table
CREATE TABLE Member (
    MemberId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(), 
    UserId NVARCHAR(450) NOT NULL,  
    RoomId UNIQUEIDENTIFIER NOT NULL,  
    Role NVARCHAR(50) NOT NULL DEFAULT 'member', 
    JoinedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Member_UserId FOREIGN KEY (UserId) REFERENCES ApplicationUsers(Id),
    CONSTRAINT FK_Member_RoomId FOREIGN KEY (RoomId) REFERENCES Room(RoomId),
    CONSTRAINT FK_Member_ValidRole FOREIGN KEY (Role) REFERENCES ValidMemberRoles(Role)
);
GO

-- Create Message table
CREATE TABLE Message (
    MessageId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(), 
    RoomId UNIQUEIDENTIFIER NOT NULL,  
    MemberId UNIQUEIDENTIFIER NOT NULL,  
    MessageContent NVARCHAR(500) NOT NULL, 
    SentAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Message_RoomId FOREIGN KEY (RoomId) REFERENCES Room(RoomId),
    CONSTRAINT FK_Message_MemberId FOREIGN KEY (MemberId) REFERENCES Member(MemberId)
);
GO
