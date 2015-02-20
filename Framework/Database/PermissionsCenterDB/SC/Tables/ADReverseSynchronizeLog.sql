CREATE TABLE [SC].[ADReverseSynchronizeLog]
(
	[LogID] NVARCHAR(36) NOT NULL PRIMARY KEY, 
    [StartTime] DATETIME NOT NULL, 
    [EndTime] DATETIME NULL, 
	CreateTime DATETIME NOT NULL DEFAULT getdate(),
    [OperatorID] NVARCHAR(36) NOT NULL, 
    [OperatorName] NVARCHAR(128) NOT NULL, 
    [NumberOfModifiedItems] INT NOT NULL, 
    [NumberOfExceptions] INT NOT NULL, 
    [Status] INT NOT NULL
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'日志ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ADReverseSynchronizeLog',
    @level2type = N'COLUMN',
    @level2name = N'LogID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'同步开始时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ADReverseSynchronizeLog',
    @level2type = N'COLUMN',
    @level2name = N'StartTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'同步结束时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ADReverseSynchronizeLog',
    @level2type = N'COLUMN',
    @level2name = N'EndTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'操作人ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ADReverseSynchronizeLog',
    @level2type = N'COLUMN',
    @level2name = N'OperatorID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'操作人名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ADReverseSynchronizeLog',
    @level2type = N'COLUMN',
    @level2name = N'OperatorName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'已修改的群组个数',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ADReverseSynchronizeLog',
    @level2type = N'COLUMN',
    @level2name = 'NumberOfModifiedItems'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'发生的错误个数',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ADReverseSynchronizeLog',
    @level2type = N'COLUMN',
    @level2name = N'NumberOfExceptions'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'同步状态',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ADReverseSynchronizeLog',
    @level2type = N'COLUMN',
    @level2name = N'Status'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'创建时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ADReverseSynchronizeLog',
    @level2type = N'COLUMN',
    @level2name = N'CreateTime'
GO

GO

EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'AD的信息反向到权限中心的执行日志',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ADReverseSynchronizeLog',
    @level2type = NULL,
    @level2name = NULL