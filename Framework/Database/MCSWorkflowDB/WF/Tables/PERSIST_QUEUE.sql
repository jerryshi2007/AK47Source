CREATE TABLE [WF].[PERSIST_QUEUE]
(
	[SORT_ID] INT NOT NULL PRIMARY KEY CLUSTERED IDENTITY, 
    [PROCESS_ID] NVARCHAR(36) NOT NULL, 
    [UPDATE_TAG] INT NULL DEFAULT 0, 
    [CREATE_TIME] DATETIME NULL DEFAULT GETDATE(),
	[PROCESS_TIME] DATETIME NULL, 
    [STATUS_TEXT] NVARCHAR(MAX) NULL
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'流程ID',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'PERSIST_QUEUE',
    @level2type = N'COLUMN',
    @level2name = N'PROCESS_ID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'流程的更新标记',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'PERSIST_QUEUE',
    @level2type = N'COLUMN',
    @level2name = N'UPDATE_TAG'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'创建时间',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'PERSIST_QUEUE',
    @level2type = N'COLUMN',
    @level2name = N'CREATE_TIME'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'状态信息',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'PERSIST_QUEUE',
    @level2type = N'COLUMN',
    @level2name = N'STATUS_TEXT'
GO

CREATE INDEX [IX_PERSIST_QUEUE_PROCESS_ID] ON [WF].[PERSIST_QUEUE] ([PROCESS_ID])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'处理时间',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'PERSIST_QUEUE',
    @level2type = N'COLUMN',
    @level2name = N'PROCESS_TIME'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'保存流程附加数据的异步队列',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'PERSIST_QUEUE',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'排序号',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'PERSIST_QUEUE',
    @level2type = N'COLUMN',
    @level2name = N'SORT_ID'