CREATE TABLE [SC].[AUAdminScopeSnapshot]
(
	[ID]               NVARCHAR (36) NOT NULL,
    [VersionStartTime] DATETIME      NOT NULL,
    [VersionEndTime]   DATETIME      CONSTRAINT [DF_AUAdminScope_VersionEndTime] DEFAULT ('99990909 00:00:00') NULL,
    [Status]           INT           CONSTRAINT [DF_AUAdminScope_Status] DEFAULT ((1)) NULL,
    [SchemaType]       NVARCHAR (64) NULL,
    [CreateDate] DATETIME NULL DEFAULT GETDATE(), 
    [CreatorID] NVARCHAR(36) NULL, 
    [CreatorName] NVARCHAR(255) NULL, 
	[SearchContent]    NVARCHAR (MAX) NULL,
	[RowUniqueID]      NVARCHAR (36)  CONSTRAINT [DF_AUAdminScope_RowUniqueID] DEFAULT (CONVERT([nvarchar](36),newid())) NOT NULL,
    [ScopeSchemaType] NVARCHAR(64) NULL, 
    CONSTRAINT [PK_AUAdminScope] PRIMARY KEY CLUSTERED ([ID] ASC, [VersionStartTime] DESC)
)

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_AUAdminScopeSnapshot_RowID] ON [SC].[AUAdminScopeSnapshot] ([RowUniqueID])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'管理单元管理范围的快照',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'AUAdminScopeSnapshot',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'AUAdminScopeSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'ID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'版本开始时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'AUAdminScopeSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'VersionStartTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'版本结束时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'AUAdminScopeSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'VersionEndTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'状态',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'AUAdminScopeSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'Status'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Schema类型',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'AUAdminScopeSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'SchemaType'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'创建日期',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'AUAdminScopeSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'CreateDate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'创建者ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'AUAdminScopeSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'CreatorID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'创建者名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'AUAdminScopeSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'CreatorName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'供全文检索的字段',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'AUAdminScopeSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'SearchContent'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'行唯一索引',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'AUAdminScopeSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'RowUniqueID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'管理范围的Schema类型名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'AUAdminScopeSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'ScopeSchemaType'
GO

CREATE INDEX [IX_AUAdminScopeSnapshot_ScopeSchemaType] ON [SC].[AUAdminScopeSnapshot] ([ScopeSchemaType])

GO

CREATE FULLTEXT INDEX ON [SC].[AUAdminScopeSnapshot] ([SearchContent]) KEY INDEX [IX_AUAdminScopeSnapshot_RowID] ON [SCFullTextIndex] WITH CHANGE_TRACKING AUTO
