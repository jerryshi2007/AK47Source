CREATE TABLE [SC].[UserPassword]
(
	[UserID] NVARCHAR(36) NOT NULL , 
    [PasswordType] NVARCHAR(36) NOT NULL, 
    [AlgorithmType] NVARCHAR(36) NULL, 
    [Password] NVARCHAR(64) NULL, 
    CONSTRAINT [PK_UserPassword] PRIMARY KEY ([UserID], [PasswordType])
)

GO
