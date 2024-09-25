CREATE TABLE Message (
    MessageId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(), 
    RoomId UNIQUEIDENTIFIER NOT NULL,  
    MemberId UNIQUEIDENTIFIER NOT NULL,  
    MessageContent NVARCHAR(500) NOT NULL, 
    SentAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Message_RoomId FOREIGN KEY (RoomId) REFERENCES Room(RoomId),
    CONSTRAINT FK_Message_MemberId FOREIGN KEY (MemberId) REFERENCES Member(MemberId)
);