CREATE TABLE ValidMemberRoles (
    Role NVARCHAR(50) PRIMARY KEY
);

-- Inserir os pap�is v�lidos
INSERT INTO ValidMemberRoles (Role)
VALUES ('member'), ('admin'), ('moderator');

