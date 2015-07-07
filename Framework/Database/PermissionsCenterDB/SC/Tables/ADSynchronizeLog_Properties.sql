EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'日志ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ADSynchronizeLog',
    @level2type = N'COLUMN',
    @level2name = N'LogID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'同步ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ADSynchronizeLog',
    @level2type = N'COLUMN',
    @level2name = N'SynchronizeID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'同步开始时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ADSynchronizeLog',
    @level2type = N'COLUMN',
    @level2name = N'StartTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'同步结束时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ADSynchronizeLog',
    @level2type = N'COLUMN',
    @level2name = N'EndTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'操作人ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ADSynchronizeLog',
    @level2type = N'COLUMN',
    @level2name = N'OperatorID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'操作人名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ADSynchronizeLog',
    @level2type = N'COLUMN',
    @level2name = N'OperatorName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'同步结果',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ADSynchronizeLog',
    @level2type = N'COLUMN',
    @level2name = N'SynchronizeResult'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'异常数量',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ADSynchronizeLog',
    @level2type = N'COLUMN',
    @level2name = N'ExceptionCount'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'创建时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ADSynchronizeLog',
    @level2type = N'COLUMN',
    @level2name = N'CreateTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'权限中心同步到AD的执行日志',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ADSynchronizeLog',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'需要增加的对象个数',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ADSynchronizeLog',
    @level2type = N'COLUMN',
    @level2name = N'AddingItemCount'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'需要删除的对象个数',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ADSynchronizeLog',
    @level2type = N'COLUMN',
    @level2name = N'DeletingItemCount'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'需要修改的对象个数',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ADSynchronizeLog',
    @level2type = N'COLUMN',
    @level2name = N'ModifyingItemCount'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'增加的对象个数',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ADSynchronizeLog',
    @level2type = N'COLUMN',
    @level2name = N'AddedItemCount'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'删除的对象个数',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ADSynchronizeLog',
    @level2type = N'COLUMN',
    @level2name = N'DeletedItemCount'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'修改的对象个数',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ADSynchronizeLog',
    @level2type = N'COLUMN',
    @level2name = N'ModifiedItemCount'
GO