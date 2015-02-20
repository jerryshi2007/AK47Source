CREATE TABLE [dbo].[AUTHENTICATION_CODE]
(
	[AUTHENTICATION_ID] NVARCHAR(36) NOT NULL PRIMARY KEY, 
    [AUTHENTICATION_TYPE] NVARCHAR(64) NULL, 
    [AUTHENTICATION_CODE] NVARCHAR(32) NULL, 
    [CREATE_TIME] DATETIME NULL DEFAULT GETDATE(), 
    [EXPIRE_TIME] DATETIME NULL
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'认证码ID',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'AUTHENTICATION_CODE',
    @level2type = N'COLUMN',
    @level2name = N'AUTHENTICATION_ID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'认证码的类型，例如SMS',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'AUTHENTICATION_CODE',
    @level2type = N'COLUMN',
    @level2name = N'AUTHENTICATION_TYPE'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'认证码',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'AUTHENTICATION_CODE',
    @level2type = N'COLUMN',
    @level2name = N'AUTHENTICATION_CODE'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'创建时间',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'AUTHENTICATION_CODE',
    @level2type = N'COLUMN',
    @level2name = N'CREATE_TIME'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'过期时间',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'AUTHENTICATION_CODE',
    @level2type = N'COLUMN',
    @level2name = N'EXPIRE_TIME'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'颁发的认证码表',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'AUTHENTICATION_CODE',
    @level2type = NULL,
    @level2name = NULL