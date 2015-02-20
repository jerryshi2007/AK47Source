﻿CREATE TABLE [SC].[CompletedJobList]
(
	[ID] INT NOT NULL , 
    [SourceID] NVARCHAR(36) NULL, 
    [CreateTime] DATETIME NULL DEFAULT GETDATE(), 
    [ExecuteTime] DATETIME NULL DEFAULT GETDATE(), 
    [Type] NVARCHAR(64) NULL, 
	[Description] NVARCHAR(255) NULL,
    [Data] NVARCHAR(MAX) NULL, 
    PRIMARY KEY ([ID] DESC)
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'作业ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'CompletedJobList',
    @level2type = N'COLUMN',
    @level2name = N'ID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'作业源的ID，一般是某个对象ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'CompletedJobList',
    @level2type = N'COLUMN',
    @level2name = N'SourceID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'创建时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'CompletedJobList',
    @level2type = N'COLUMN',
    @level2name = N'CreateTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'执行时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'CompletedJobList',
    @level2type = N'COLUMN',
    @level2name = N'ExecuteTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'作业类型',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'CompletedJobList',
    @level2type = N'COLUMN',
    @level2name = N'Type'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'相关数据',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'CompletedJobList',
    @level2type = N'COLUMN',
    @level2name = N'Data'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'作业的描述信息',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'CompletedJobList',
    @level2type = N'COLUMN',
    @level2name = N'Description'
GO

CREATE INDEX [IX_CompletedJobList_ID] ON [SC].[CompletedJobList] ([ID])
