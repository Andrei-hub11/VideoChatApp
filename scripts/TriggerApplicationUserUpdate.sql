CREATE TRIGGER TriggerApplicationUserUpdate
ON ApplicationUser
AFTER UPDATE
AS
BEGIN
    IF UPDATE(UserName) OR UPDATE(Email) OR UPDATE(PasswordHash) OR UPDATE(ProfileImage) OR UPDATE(ProfileImageUrl)
    BEGIN
        UPDATE ApplicationUser
        SET UpdatedAt = GETDATE()
        FROM ApplicationUser
        INNER JOIN inserted ON ApplicationUser.Id = inserted.Id;
    END
END;