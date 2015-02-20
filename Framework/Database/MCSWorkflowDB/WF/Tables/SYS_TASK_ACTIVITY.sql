CREATE TABLE [WF].[SYS_TASK_ACTIVITY]
(
	[ID] NVARCHAR(36) NOT NULL PRIMARY KEY, 
    [PROCESS_ID] NVARCHAR(36) NOT NULL, 
    [NAME] NVARCHAR(64) NULL, 
    [SEQUENCE] INT NULL,
	[STATUS] NVARCHAR(64) NULL, 
    [START_TIME] DATETIME NULL, 
    [END_TIME] DATETIME NULL, 
    [BLOCKING_TYPE] NVARCHAR(64) NULL, 
    [TASK_DATA] NVARCHAR(MAX) NULL, 
    [STATUS_TEXT] NVARCHAR(MAX) NULL
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'任务流程活动表',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'SYS_TASK_ACTIVITY',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'任务流程活动ID',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'SYS_TASK_ACTIVITY',
    @level2type = N'COLUMN',
    @level2name = N'ID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'任务流程的流程ID',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'SYS_TASK_ACTIVITY',
    @level2type = N'COLUMN',
    @level2name = N'PROCESS_ID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'活动名称',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'SYS_TASK_ACTIVITY',
    @level2type = N'COLUMN',
    @level2name = N'NAME'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'活动的状态',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'SYS_TASK_ACTIVITY',
    @level2type = N'COLUMN',
    @level2name = N'STATUS'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'活动开始时间',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'SYS_TASK_ACTIVITY',
    @level2type = N'COLUMN',
    @level2name = N'START_TIME'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'活动完成时间',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'SYS_TASK_ACTIVITY',
    @level2type = N'COLUMN',
    @level2name = N'END_TIME'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'分支活动的阻塞模式',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'SYS_TASK_ACTIVITY',
    @level2type = N'COLUMN',
    @level2name = N'BLOCKING_TYPE'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'序列化后的任务数据',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'SYS_TASK_ACTIVITY',
    @level2type = N'COLUMN',
    @level2name = N'TASK_DATA'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'活动的序号',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'SYS_TASK_ACTIVITY',
    @level2type = N'COLUMN',
    @level2name = N'SEQUENCE'
GO

CREATE INDEX [IX_SYS_TASK_ACTIVITY_PROCESS_ID] ON [WF].[SYS_TASK_ACTIVITY] ([PROCESS_ID])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'状态描述',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'SYS_TASK_ACTIVITY',
    @level2type = N'COLUMN',
    @level2name = N'STATUS_TEXT'