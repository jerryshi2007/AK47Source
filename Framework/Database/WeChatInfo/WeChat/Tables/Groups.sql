CREATE TABLE [WeChat].[Groups]
(
	[AccountID] NVARCHAR(36) NOT NULL , 
    [GroupID] INT NOT NULL, 
    [Name] NVARCHAR(64) NULL, 
    [Count] INT NULL DEFAULT 0,
	[CreateTime] DATETIME NULL DEFAULT GETDATE(), 
    [UpdateTime] DATETIME NULL DEFAULT GETDATE(),
    PRIMARY KEY ([AccountID], [GroupID])
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'微信公众号下的分组表',
    @level0type = N'SCHEMA',
    @level0name = N'WeChat',
    @level1type = N'TABLE',
    @level1name = N'Groups',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'微信公众号的ID',
    @level0type = N'SCHEMA',
    @level0name = N'WeChat',
    @level1type = N'TABLE',
    @level1name = N'Groups',
    @level2type = N'COLUMN',
    @level2name = N'AccountID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'公众号下的组ID',
    @level0type = N'SCHEMA',
    @level0name = N'WeChat',
    @level1type = N'TABLE',
    @level1name = N'Groups',
    @level2type = N'COLUMN',
    @level2name = N'GroupID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'组的名称',
    @level0type = N'SCHEMA',
    @level0name = N'WeChat',
    @level1type = N'TABLE',
    @level1name = N'Groups',
    @level2type = N'COLUMN',
    @level2name = N'Name'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'组内粉丝的数量',
    @level0type = N'SCHEMA',
    @level0name = N'WeChat',
    @level1type = N'TABLE',
    @level1name = N'Groups',
    @level2type = N'COLUMN',
    @level2name = N'Count'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'记录创建时间',
    @level0type = N'SCHEMA',
    @level0name = N'WeChat',
    @level1type = N'TABLE',
    @level1name = N'Groups',
    @level2type = N'COLUMN',
    @level2name = N'CreateTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'记录更新时间',
    @level0type = N'SCHEMA',
    @level0name = N'WeChat',
    @level1type = N'TABLE',
    @level1name = N'Groups',
    @level2type = N'COLUMN',
    @level2name = N'UpdateTime'