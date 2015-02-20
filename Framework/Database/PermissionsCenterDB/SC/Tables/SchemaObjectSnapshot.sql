CREATE TABLE [SC].[SchemaObjectSnapshot]
(
	[ID]               NVARCHAR (36)  NOT NULL,
    [VersionStartTime] DATETIME       NOT NULL,
    [VersionEndTime]   DATETIME       CONSTRAINT [DF_SchemaObjectSnapshot_VersionEndTime] DEFAULT ('99990909 00:00:00') NULL,
    [Status]           INT            CONSTRAINT [DF_SchemaObjectSnapshot_Status] DEFAULT ((1)) NULL,
	[CreateDate]       DATETIME NULL DEFAULT GETDATE(), 
    [Name]             NVARCHAR (255) NULL,
    [DisplayName]      NVARCHAR (255) NULL,
	[CodeName]         NVARCHAR (64) NULL,
	[AccountDisabled]  INT NULL DEFAULT 0,
    [SearchContent]    NVARCHAR (MAX) NULL,
    [RowUniqueID]      NVARCHAR (36)  CONSTRAINT [DF_SchemaObjectSnapshot_RowUniqueID] DEFAULT (CONVERT([nvarchar](36),newid())) NOT NULL,
    [SchemaType] NVARCHAR(36) NULL, 
	[CreatorID] NVARCHAR(36) NULL, 
    [CreatorName] NVARCHAR(255) NULL,
    [Comment] NVARCHAR(255) NULL, 
    CONSTRAINT [PK_SchemaObjectSnapshot] PRIMARY KEY CLUSTERED ([ID] ASC, [VersionStartTime] DESC)
)
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_SchemaObjectSnapshot_RowID] ON [SC].[SchemaObjectSnapshot] ([RowUniqueID])

GO

CREATE FULLTEXT INDEX ON [SC].[SchemaObjectSnapshot]
    ([SearchContent] LANGUAGE 2052)
    KEY INDEX [IX_SchemaObjectSnapshot_RowID]
    ON [SCFullTextIndex] WITH CHANGE_TRACKING AUTO

GO

CREATE INDEX [IX_SchemaObjectSnapshot_StartTime] ON [SC].[SchemaObjectSnapshot] ([VersionStartTime])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'一般对象快照',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaObjectSnapshot',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'对象的ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaObjectSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'ID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'版本开始时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaObjectSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'VersionStartTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'版本结束时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaObjectSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'VersionEndTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'对象的状态',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaObjectSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'Status'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'对象创建时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaObjectSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'CreateDate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'对象的名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaObjectSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'Name'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'对象的显示名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaObjectSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'DisplayName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'对象的代码名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaObjectSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'CodeName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'全文检索字段',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaObjectSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'SearchContent'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'行唯一索引，用于全文检索',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaObjectSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'RowUniqueID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'对象的Schema名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaObjectSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'SchemaType'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'创建人的ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaObjectSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'CreatorID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'创建人的名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaObjectSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'CreatorName'
GO

CREATE INDEX [IX_SchemaObjectSnapshot_Name] ON [SC].[SchemaObjectSnapshot] ([Name])

GO

CREATE INDEX [IX_SchemaObjectSnapshot_CodeName] ON [SC].[SchemaObjectSnapshot] ([CodeName])

GO

CREATE INDEX [IX_SchemaObjectSnapshot_SchemaType] ON [SC].[SchemaObjectSnapshot] ([SchemaType])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'账户是否禁用',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaObjectSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'AccountDisabled'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'注释',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaObjectSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'Comment'