CREATE TABLE [WF].[SYS_TASK]
(
	[TASK_GUID]		NVARCHAR(36) NOT NULL,
    [SORT_ID]		INT NOT NULL IDENTITY,
	[CATEGORY]		NVARCHAR (64)   NULL,
    [TASK_TYPE]		NVARCHAR (64)   NULL,
    [TASK_TITLE]	NVARCHAR (1024) NULL,
    [RESOURCE_ID]	NVARCHAR (36)   NULL,
	[STATUS]		NVARCHAR(64)       NULL, 
    [CREATE_TIME]	DATETIME NULL DEFAULT GETDATE(), 
    [START_TIME]	DATETIME NULL, 
    [END_TIME]		DATETIME NULL,
	[SOURCE_ID]		NVARCHAR (36)   NULL,
    [SOURCE_NAME]	NVARCHAR (64)   NULL,
	[URL]			NVARCHAR (2048) NULL,
    [DATA]			XML  NULL, 
    [STATUS_TEXT]	NVARCHAR(MAX) NULL,
	CONSTRAINT [PK_SysTask] PRIMARY KEY NONCLUSTERED ([TASK_GUID] ASC)
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'任务的PK，不是聚集索引',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'SYS_TASK',
    @level2type = N'COLUMN',
    @level2name = N'TASK_GUID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'任务的序号，自增且聚集索引',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'SYS_TASK',
    @level2type = N'COLUMN',
    @level2name = N'SORT_ID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'关联ID，例如JOB_ID',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'SYS_TASK',
    @level2type = N'COLUMN',
    @level2name = N'RESOURCE_ID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'任务的分类',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'SYS_TASK',
    @level2type = N'COLUMN',
    @level2name = 'CATEGORY'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'任务的类别，会对应到不同的程序进行处理',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'SYS_TASK',
    @level2type = N'COLUMN',
    @level2name = N'TASK_TYPE'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'任务的标题',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'SYS_TASK',
    @level2type = N'COLUMN',
    @level2name = N'TASK_TITLE'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'任务状态',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'SYS_TASK',
    @level2type = N'COLUMN',
    @level2name = N'STATUS'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'任务的创建时间，不是执行时间',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'SYS_TASK',
    @level2type = N'COLUMN',
    @level2name = N'CREATE_TIME'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'任务的执行时间',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'SYS_TASK',
    @level2type = N'COLUMN',
    @level2name = N'START_TIME'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'任务的完成时间',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'SYS_TASK',
    @level2type = N'COLUMN',
    @level2name = N'END_TIME'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'创建者的ID',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'SYS_TASK',
    @level2type = N'COLUMN',
    @level2name = N'SOURCE_ID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'创建者的名称',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'SYS_TASK',
    @level2type = N'COLUMN',
    @level2name = N'SOURCE_NAME'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'任务相关的Url',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'SYS_TASK',
    @level2type = N'COLUMN',
    @level2name = N'URL'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'任务相关的数据',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'SYS_TASK',
    @level2type = N'COLUMN',
    @level2name = N'DATA'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'任务的状态文本，通常是错误信息',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'SYS_TASK',
    @level2type = N'COLUMN',
    @level2name = N'STATUS_TEXT'
GO

CREATE CLUSTERED INDEX [IX_SYS_TASK_SortID] ON [WF].[SYS_TASK] ([SORT_ID])

GO

CREATE INDEX [IX_SYS_TASK_ResourceID] ON [WF].[SYS_TASK] ([RESOURCE_ID])

GO

CREATE INDEX [IX_SYS_TASK_SourceID] ON [WF].[SYS_TASK] ([SOURCE_ID])

GO


CREATE FULLTEXT INDEX ON [WF].[SYS_TASK] ([TASK_TITLE] LANGUAGE 2052)
KEY INDEX [PK_SysTask] ON [MCS_WORKFLOW] WITH CHANGE_TRACKING AUTO

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'存放待执行和正在执行的系统任务',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'SYS_TASK',
    @level2type = NULL,
    @level2name = NULL