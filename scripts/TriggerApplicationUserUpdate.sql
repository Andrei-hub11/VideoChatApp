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