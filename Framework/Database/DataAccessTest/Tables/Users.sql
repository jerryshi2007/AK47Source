CREATE TABLE [dbo].[Users]
(
	[UserID] NVARCHAR(36) NOT NULL PRIMARY KEY, 
    [UserName] NVARCHAR(64) NULL, 
    [Gender] NVARCHAR(16) NULL
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'用户ID',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Users',
    @level2type = N'COLUMN',
    @level2name = N'UserID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'用户名称',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Users',
    @level2type = N'COLUMN',
    @level2name = N'UserName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'性别',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Users',
    @level2type = N'COLUMN',
    @level2name = N'Gender'