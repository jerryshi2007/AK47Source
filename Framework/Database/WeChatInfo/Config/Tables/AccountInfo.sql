CREATE TABLE [Config].[AccountInfo]
(
	[AccountID] NVARCHAR(36) NOT NULL PRIMARY KEY, 
    [UserID] NVARCHAR(64) NULL, 
    [Password] NVARCHAR(64) NULL
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'微信公众号的账户信息',
    @level0type = N'SCHEMA',
    @level0name = N'Config',
    @level1type = N'TABLE',
    @level1name = N'AccountInfo',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N' 微信公众号的ID',
    @level0type = N'SCHEMA',
    @level0name = N'Config',
    @level1type = N'TABLE',
    @level1name = N'AccountInfo',
    @level2type = N'COLUMN',
    @level2name = N'AccountID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'登录名',
    @level0type = N'SCHEMA',
    @level0name = N'Config',
    @level1type = N'TABLE',
    @level1name = N'AccountInfo',
    @level2type = N'COLUMN',
    @level2name = N'UserID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'登录密码',
    @level0type = N'SCHEMA',
    @level0name = N'Config',
    @level1type = N'TABLE',
    @level1name = N'AccountInfo',
    @level2type = N'COLUMN',
    @level2name = N'Password'