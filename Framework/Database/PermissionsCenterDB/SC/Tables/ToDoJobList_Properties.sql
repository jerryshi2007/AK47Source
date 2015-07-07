EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'作业ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ToDoJobList',
    @level2type = N'COLUMN',
    @level2name = N'ID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'作业源的ID，一般是某个对象ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ToDoJobList',
    @level2type = N'COLUMN',
    @level2name = 'SourceID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'后台作业列表',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ToDoJobList',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'创建时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ToDoJobList',
    @level2type = N'COLUMN',
    @level2name = N'CreateTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'执行时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ToDoJobList',
    @level2type = N'COLUMN',
    @level2name = N'ExecuteTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'作业类型',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ToDoJobList',
    @level2type = N'COLUMN',
    @level2name = N'Type'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'相关数据',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ToDoJobList',
    @level2type = N'COLUMN',
    @level2name = N'Data'
GO

CREATE INDEX [IX_ToDoJobList_SourceID] ON [SC].[ToDoJobList] ([SourceID])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'作业的描述信息',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ToDoJobList',
    @level2type = N'COLUMN',
    @level2name = N'Description'