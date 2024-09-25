CREATE TABLE ValidMemberRoles (
    Role NVARCHAR(50) PRIMARY KEY
);

-- Inserir os papéis válidos
INSERT INTO ValidMemberRoles (Role)
VALUES ('member'), ('admin'), ('moderator');

