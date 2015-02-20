CREATE TABLE [SC].[AUAdminScopeItemSnapshot]
(
	[ID]               NVARCHAR (36) NOT NULL,
    [VersionStartTime] DATETIME      NOT NULL,
    [VersionEndTime]   DATETIME      CONSTRAINT [DF_AUAdminScopeItemSnapshot_VersionEndTime] DEFAULT ('99990909 00:00:00') NULL,
    [Status]           INT           CONSTRAINT [DF_AUAdminScopeItemSnapshot_Status] DEFAULT ((1)) NULL,
    [SchemaType]       NVARCHAR (64) NULL,
    [CreateDate] DATETIME NULL DEFAULT GETDATE(), 
    [CreatorID] NVARCHAR(36) NULL, 
    [CreatorName] NVARCHAR(255) NULL, 
	[SearchContent]    NVARCHAR (MAX) NOT NULL,
	[RowUniqueID]      NVARCHAR (36)  CONSTRAINT [DF_AUAdminScopeItem_RowUniqueID] DEFAULT (CONVERT([nvarchar](36),newid())) NOT NULL,
	[AUScopeItemName]  NVARCHAR(255)
    CONSTRAINT [PK_AUAdminScopeItemSnapshot] PRIMARY KEY CLUSTERED ([ID] ASC, [VersionStartTime] DESC) NOT NULL
)

GO

EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'状态',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'AUAdminScopeItemSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'Status'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'创建日期',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'AUAdminScopeItemSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'CreateDate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'创建者ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'AUAdminScopeItemSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'CreatorID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'创建者名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'AUAdminScopeItemSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'CreatorName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'AUAdminScopeItemSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'ID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'版本开始时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'AUAdminScopeItemSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'VersionStartTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'版本结束时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'AUAdminScopeItemSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'VersionEndTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Schema类型',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'AUAdminScopeItemSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'SchemaType'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'全文检索的内容',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'AUAdminScopeItemSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'SearchContent'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'供全文检索的行唯一ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'AUAdminScopeItemSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'RowUniqueID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'可见的名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'AUAdminScopeItemSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'AUScopeItemName'
GO

CREATE INDEX [IX_AUAdminScopeItemSnapshot_AUScopeItemName] ON [SC].[AUAdminScopeItemSnapshot] ([AUScopeItemName])

GO

CREATE FULLTEXT INDEX ON [SC].[AUAdminScopeItemSnapshot] ([SearchContent]) KEY INDEX [IX_AUAdminScopeItemSnapshot_RowUniqueID] ON [SCFullTextIndex] WITH CHANGE_TRACKING AUTO

GO

CREATE UNIQUE INDEX [IX_AUAdminScopeItemSnapshot_RowUniqueID] ON [SC].[AUAdminScopeItemSnapshot] ([RowUniqueID])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'管理范围对象',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'AUAdminScopeItemSnapshot',
    @level2type = NULL,
    @level2name = NULL