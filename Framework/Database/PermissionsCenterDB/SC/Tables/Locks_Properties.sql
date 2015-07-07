EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'锁ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'Locks',
    @level2type = N'COLUMN',
    @level2name = N'LockID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'相关的资源ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'Locks',
    @level2type = N'COLUMN',
    @level2name = N'ResourceID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'上锁人的ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'Locks',
    @level2type = N'COLUMN',
    @level2name = N'LockPersonID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'上锁人的名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'Locks',
    @level2type = N'COLUMN',
    @level2name = 'LockPersonName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'上锁的时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'Locks',
    @level2type = N'COLUMN',
    @level2name = N'LockTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'锁的有效期，以秒为单位',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'Locks',
    @level2type = N'COLUMN',
    @level2name = N'EffectiveTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'锁的类型',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'Locks',
    @level2type = N'COLUMN',
    @level2name = N'LockType'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'描述信息',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'Locks',
    @level2type = N'COLUMN',
    @level2name = N'Description'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'操作锁表',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'Locks',
    @level2type = NULL,
    @level2name = NULL
GO