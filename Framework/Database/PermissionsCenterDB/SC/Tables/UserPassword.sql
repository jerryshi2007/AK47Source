CREATE TABLE [SC].[UserPassword]
(
	[UserID] NVARCHAR(36) NOT NULL , 
    [PasswordType] NVARCHAR(36) NOT NULL, 
    [AlgorithmType] NVARCHAR(36) NULL, 
    [Password] NVARCHAR(64) NULL, 
    CONSTRAINT [PK_UserPassword] PRIMARY KEY ([UserID], [PasswordType])
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'用户ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'UserPassword',
    @level2type = N'COLUMN',
    @level2name = N'UserID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'密码类型',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'UserPassword',
    @level2type = N'COLUMN',
    @level2name = N'PasswordType'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'算法类型',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'UserPassword',
    @level2type = N'COLUMN',
    @level2name = N'AlgorithmType'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'密码Hash后的结果',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'UserPassword',
    @level2type = N'COLUMN',
    @level2name = N'Password'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'保存用户密码',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'UserPassword',
    @level2type = NULL,
    @level2name = NULL