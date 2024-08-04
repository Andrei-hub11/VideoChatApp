CREATE TRIGGER trigger_InsteadOfInsertUser
ON ApplicationUsers
INSTEAD OF INSERT
AS
BEGIN
    IF EXISTS (SELECT 1 FROM inserted WHERE Email IN (SELECT Email FROM ApplicationUsers))
    BEGIN
        RAISERROR ('The email address provided is already in use. Please choose a different email address.', 16, 1);
        RETURN;
    END

    INSERT INTO ApplicationUsers (Id, UserName, Email, PasswordHash, ProfileImage, ProfileImagePath)
    SELECT Id, UserName, Email, PasswordHash, ProfileImage, ProfileImagePath
    FROM inserted;
END
