CREATE TABLE [SC].[IncomeSynchronizeLogDetail]
(
	[LogDetailID] NVARCHAR(36) NOT NULL PRIMARY KEY , 
    [LogID] NVARCHAR(36) NOT NULL, 
	[CreateTime] datetime NOT NULL DEFAULT getdate(),
    [SCObjectID] NVARCHAR(36) NOT NULL, 
	[ActionType] int NOT NULL,
	[Summary] NVARCHAR(256) NOT NULL DEFAULT '',
    [Detail] NVARCHAR(MAX) NOT NULL
)
GO

EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'日志详情ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'IncomeSynchronizeLogDetail',
    @level2type = N'COLUMN',
    @level2name = N'LogDetailID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'日志ID，为日志表ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'IncomeSynchronizeLogDetail',
    @level2type = N'COLUMN',
    @level2name = N'LogID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'IncomeSynchronizeLogDetail',
    @level2type = N'COLUMN',
    @level2name = N'CreateTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'权限中心对象ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'IncomeSynchronizeLogDetail',
    @level2type = N'COLUMN',
    @level2name = N'SCObjectID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'操作类型：暂定0未知，1新增，2修改，3删除',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'IncomeSynchronizeLogDetail',
    @level2type = N'COLUMN',
    @level2name = N'ActionType'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'摘要',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'IncomeSynchronizeLogDetail',
    @level2type = N'COLUMN',
    @level2name = N'Summary'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'异常消息',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'IncomeSynchronizeLogDetail',
    @level2type = N'COLUMN',
    @level2name = N'Detail'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'其他系统同步至权限中心的执行日志错误详情',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'IncomeSynchronizeLogDetail',
    @level2type = NULL,
    @level2name = NULL