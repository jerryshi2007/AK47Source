EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'日志ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'IncomeSynchronizeLog',
    @level2type = N'COLUMN',
    @level2name = N'LogID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'同步开始时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'IncomeSynchronizeLog',
    @level2type = N'COLUMN',
    @level2name = N'StartTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'同步结束时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'IncomeSynchronizeLog',
    @level2type = N'COLUMN',
    @level2name = N'EndTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'操作人ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'IncomeSynchronizeLog',
    @level2type = N'COLUMN',
    @level2name = N'OperatorID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'操作人名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'IncomeSynchronizeLog',
    @level2type = N'COLUMN',
    @level2name = N'OperatorName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'已修改的群组个数',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'IncomeSynchronizeLog',
    @level2type = N'COLUMN',
    @level2name = 'NumberOfModifiedItems'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'发生的错误个数',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'IncomeSynchronizeLog',
    @level2type = N'COLUMN',
    @level2name = N'NumberOfExceptions'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'同步状态',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'IncomeSynchronizeLog',
    @level2type = N'COLUMN',
    @level2name = N'Status'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'创建时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'IncomeSynchronizeLog',
    @level2type = N'COLUMN',
    @level2name = N'CreateTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'同步来源',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'IncomeSynchronizeLog',
    @level2type = N'COLUMN',
    @level2name = N'SourceName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'其他系统同步至权限中心的执行日志',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'IncomeSynchronizeLog',
    @level2type = NULL,
    @level2name = NULL
GO