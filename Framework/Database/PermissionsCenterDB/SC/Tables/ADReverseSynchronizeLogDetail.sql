CREATE TABLE [SC].[ADReverseSynchronizeLogDetail]
(
	[LogDetailID] NVARCHAR(36) NOT NULL PRIMARY KEY , 
    [LogID] NVARCHAR(36) NOT NULL, 
	CreateTime datetime NOT NULL DEFAULT getdate(),
    [SCObjectID] NVARCHAR(36) NOT NULL, 
    [ADObjectID] NVARCHAR(36) NOT NULL, 
    [ADObjectName] NCHAR(36) NOT NULL, 
	[Summary] NVARCHAR(256) NOT NULL DEFAULT '',
    [Detail] NVARCHAR(MAX) NOT NULL
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'日志详细信息ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ADReverseSynchronizeLogDetail',
    @level2type = N'COLUMN',
    @level2name = N'LogDetailID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'日志ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ADReverseSynchronizeLogDetail',
    @level2type = N'COLUMN',
    @level2name = N'LogID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'权限中心对象ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ADReverseSynchronizeLogDetail',
    @level2type = N'COLUMN',
    @level2name = N'SCObjectID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'AD对象ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ADReverseSynchronizeLogDetail',
    @level2type = N'COLUMN',
    @level2name = N'ADObjectID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'AD对象名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ADReverseSynchronizeLogDetail',
    @level2type = N'COLUMN',
    @level2name = N'ADObjectName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'详细信息',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ADReverseSynchronizeLogDetail',
    @level2type = N'COLUMN',
    @level2name = N'Detail'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'同步时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ADReverseSynchronizeLogDetail',
    @level2type = N'COLUMN',
    @level2name = N'CreateTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'摘要',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ADReverseSynchronizeLogDetail',
    @level2type = N'COLUMN',
    @level2name = N'Summary'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'AD中的信息同步到权限中心的日志错误明细',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ADReverseSynchronizeLogDetail',
    @level2type = NULL,
    @level2name = NULL