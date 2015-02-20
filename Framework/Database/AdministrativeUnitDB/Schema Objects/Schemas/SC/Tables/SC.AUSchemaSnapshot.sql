CREATE TABLE [SC].[AUSchemaSnapshot]
(
	[ID]               NVARCHAR (36) NOT NULL,
    [VersionStartTime] DATETIME      NOT NULL,
    [VersionEndTime]   DATETIME      CONSTRAINT [DF_AUSchema_VersionEndTime] DEFAULT ('99990909 00:00:00') NULL,
    [Status]           INT           CONSTRAINT [DF_AUSchema_Status] DEFAULT ((1)) NULL,
    [SchemaType]       NVARCHAR (64) NULL,
    [CreateDate] DATETIME NULL DEFAULT GETDATE(), 
    [CreatorID] NVARCHAR(36) NULL, 
    [CreatorName] NVARCHAR(255) NULL, 
	[SearchContent]    NVARCHAR (MAX) NULL,
	[RowUniqueID]      NVARCHAR (36)  CONSTRAINT [DF_AUSchema_RowUniqueID] DEFAULT (CONVERT([nvarchar](36),newid())) NOT NULL,

    [CodeName] NVARCHAR(64) NOT NULL, 
    [Name] NVARCHAR(255) NOT NULL, 
    [DisplayName] NVARCHAR(255) NOT NULL, 
    [CategoryID] NVARCHAR(64) NOT NULL, 
	[Scopes] NVARCHAR(MAX) NOT NULL, 
    
    [MasterRole] NVARCHAR(128) NULL, 
    [Comment] NVARCHAR(255) NULL, 
    CONSTRAINT [PK_AUSchema] PRIMARY KEY CLUSTERED ([ID] ASC, [VersionStartTime] DESC)
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'表示管理架构的快照',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'AUSchemaSnapshot',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'全文搜索内容',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'AUSchemaSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'SearchContent'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'引用的分类的键',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'AUSchemaSnapshot',
    @level2type = N'COLUMN',
    @level2name = 'CategoryID'
GO

EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'管理架构的ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'AUSchemaSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'ID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'版本开始时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'AUSchemaSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'VersionStartTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'版本结束时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'AUSchemaSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'VersionEndTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'状态',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'AUSchemaSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'Status'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'创建日期',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'AUSchemaSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'CreateDate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'AUSchemaSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'Name'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'显示名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'AUSchemaSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'DisplayName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'代码名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'AUSchemaSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'CodeName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'行的唯一ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'AUSchemaSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'RowUniqueID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Schema类型',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'AUSchemaSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'SchemaType'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'创建者ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'AUSchemaSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'CreatorID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'创建者名',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'AUSchemaSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'CreatorName'
GO

EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'范围的类别',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'AUSchemaSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'Scopes'
GO


CREATE UNIQUE NONCLUSTERED INDEX [IX_AUSchemaSnapshot_RowID] ON [SC].[AUSchemaSnapshot] ([RowUniqueID])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'此架构管理角色的代码名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'AUSchemaSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'MasterRole'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'注释',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'AUSchemaSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'Comment'
GO

CREATE FULLTEXT INDEX ON [SC].[AUSchemaSnapshot] ([SearchContent]) KEY INDEX [IX_AUSchemaSnapshot_RowID] ON [SCFullTextIndex] WITH CHANGE_TRACKING AUTO

GO

CREATE INDEX [IX_AUSchemaSnapshot_CodeName] ON [SC].[AUSchemaSnapshot] ([CodeName])

GO

CREATE INDEX [IX_AUSchemaSnapshot_CategoryID] ON [SC].[AUSchemaSnapshot] ([CategoryID])
