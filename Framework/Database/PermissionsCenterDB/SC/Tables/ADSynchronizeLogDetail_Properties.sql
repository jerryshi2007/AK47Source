EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'日志明细ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ADSynchronizeLogDetail',
    @level2type = N'COLUMN',
    @level2name = N'LogDetailID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'同步ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ADSynchronizeLogDetail',
    @level2type = N'COLUMN',
    @level2name = N'SynchronizeID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'权限中心对象ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ADSynchronizeLogDetail',
    @level2type = N'COLUMN',
    @level2name = N'SCObjectID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'权限中心对象名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ADSynchronizeLogDetail',
    @level2type = N'COLUMN',
    @level2name = N'SCObjectName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'AD对象ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ADSynchronizeLogDetail',
    @level2type = N'COLUMN',
    @level2name = N'ADObjectID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'AD对象名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ADSynchronizeLogDetail',
    @level2type = N'COLUMN',
    @level2name = N'ADObjectName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'动作名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ADSynchronizeLogDetail',
    @level2type = N'COLUMN',
    @level2name = N'ActionName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'明细',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ADSynchronizeLogDetail',
    @level2type = N'COLUMN',
    @level2name = N'Detail'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'创建时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ADSynchronizeLogDetail',
    @level2type = N'COLUMN',
    @level2name = N'CreateTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'权限中心同步到AD的执行日志错误和删除记录明细',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ADSynchronizeLogDetail',
    @level2type = NULL,
    @level2name = NULL
GO