EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'操作类型',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SCOperationSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'OperationType'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'操作时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SCOperationSnapshot',
    @level2type = N'COLUMN',
    @level2name = 'OperateTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'操作人ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SCOperationSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'OperatorID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'操作人名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SCOperationSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'OperatorName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'最后操作的快照时间表，用于判断操作的最后时候时间，可以进行后续的Cache或同步处理',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SCOperationSnapshot',
    @level2type = NULL,
    @level2name = NULL
GO