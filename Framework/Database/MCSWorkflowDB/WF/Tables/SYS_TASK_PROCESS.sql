CREATE TABLE [WF].[SYS_TASK_PROCESS]
(
	[ID] NVARCHAR(36) NOT NULL PRIMARY KEY, 
    [NAME] NVARCHAR(64) NULL, 
	[RESOURCE_ID] NVARCHAR(36) NULL,
    [OWNER_ACTIVITY_ID] NVARCHAR(36) NULL, 
    [CURRENT_ACTIVITY_INDEX] INT NULL, 
    [SEQUENCE] INT NULL, 
    [STATUS] NVARCHAR(64) NULL,
	[START_TIME] DATETIME NULL, 
    [END_TIME] DATETIME NULL, 
    [UPDATE_TAG] INT NULL DEFAULT 1, 
    [STATUS_TEXT] NVARCHAR(36) NULL,
	[TENANT_CODE] NVARCHAR(36) NULL DEFAULT 'D5561180-7617-4B67-B68B-1F0EA604B509'
)

GO

CREATE INDEX [IX_SYS_TASK_PROCESS_TENANT_CODE] ON [WF].[SYS_TASK_PROCESS] ([TENANT_CODE])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'任务流程ID',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'SYS_TASK_PROCESS',
    @level2type = N'COLUMN',
    @level2name = N'ID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'任务流程表',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'SYS_TASK_PROCESS',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'任务流程名称',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'SYS_TASK_PROCESS',
    @level2type = N'COLUMN',
    @level2name = N'NAME'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'父流程活动的ID',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'SYS_TASK_PROCESS',
    @level2type = N'COLUMN',
    @level2name = N'OWNER_ACTIVITY_ID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'当前活动的序号',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'SYS_TASK_PROCESS',
    @level2type = N'COLUMN',
    @level2name = N'CURRENT_ACTIVITY_INDEX'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'任务流程的序号',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'SYS_TASK_PROCESS',
    @level2type = N'COLUMN',
    @level2name = N'SEQUENCE'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'流程开始时间',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'SYS_TASK_PROCESS',
    @level2type = N'COLUMN',
    @level2name = N'START_TIME'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'流程结束时间',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'SYS_TASK_PROCESS',
    @level2type = N'COLUMN',
    @level2name = N'END_TIME'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'流程保存标记，防并发处理',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'SYS_TASK_PROCESS',
    @level2type = N'COLUMN',
    @level2name = N'UPDATE_TAG'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'流程的状态',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'SYS_TASK_PROCESS',
    @level2type = N'COLUMN',
    @level2name = N'STATUS'
GO

CREATE INDEX [IX_SYS_TASK_PROCESS_RESOURCE_ID] ON [WF].[SYS_TASK_PROCESS] ([RESOURCE_ID])

GO

CREATE INDEX [IX_SYS_TASK_PROCESS_OWNER_ACTIVITY_ID] ON [WF].[SYS_TASK_PROCESS] ([OWNER_ACTIVITY_ID])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'状态描述',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'SYS_TASK_PROCESS',
    @level2type = N'COLUMN',
    @level2name = N'STATUS_TEXT'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'流程相关的资源ID',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'SYS_TASK_PROCESS',
    @level2type = N'COLUMN',
    @level2name = N'RESOURCE_ID'