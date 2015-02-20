CREATE TABLE [SC].[SchemaPermissionSnapshot]
(
	[ID]               NVARCHAR (36)  NOT NULL,
    [VersionStartTime] DATETIME       NOT NULL,
    [VersionEndTime]   DATETIME       CONSTRAINT [DF_SchemaPermissionSnapshot_VersionEndTime] DEFAULT ('99990909 00:00:00') NULL,
    [Status]           INT            CONSTRAINT [DF_SchemaPermissionSnapshot_Status] DEFAULT ((1)) NULL,
	[CreateDate]       DATETIME NULL DEFAULT GETDATE(), 
    [Name]             NVARCHAR (255) NULL,
    [DisplayName]      NVARCHAR (255) NULL,
	[CodeName]         NVARCHAR (64) NULL,
    [SearchContent]    NVARCHAR (MAX) NULL,
    [RowUniqueID]      NVARCHAR (36)  CONSTRAINT [DF_SchemaPermissionSnapshot_RowUniqueID] DEFAULT (CONVERT([nvarchar](36),newid())) NOT NULL,
    [SchemaType] NVARCHAR(36) NULL, 
	[CreatorID] NVARCHAR(36) NULL, 
    [CreatorName] NVARCHAR(255) NULL,
    [Comment] NVARCHAR(255) NULL, 
    CONSTRAINT [PK_SchemaPermissionSnapshot] PRIMARY KEY CLUSTERED ([ID] ASC, [VersionStartTime] DESC)
)

GO

GO


CREATE UNIQUE NONCLUSTERED INDEX [IX_SchemaPermissionSnapshot_RowID] ON [SC].[SchemaPermissionSnapshot] ([RowUniqueID])

GO

CREATE FULLTEXT INDEX ON [SC].[SchemaPermissionSnapshot]
    ([SearchContent] LANGUAGE 2052)
    KEY INDEX [IX_SchemaPermissionSnapshot_RowID]
    ON [SCFullTextIndex] WITH CHANGE_TRACKING AUTO

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'权限对象的ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaPermissionSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'ID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'版本开始时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaPermissionSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'VersionStartTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'版本结束时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaPermissionSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'VersionEndTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'对象的状态',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaPermissionSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'Status'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'创建时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaPermissionSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'CreateDate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'权限的名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaPermissionSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'Name'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'显示名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaPermissionSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'DisplayName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'权限的代码',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaPermissionSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'CodeName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'全文检索字段',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaPermissionSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'SearchContent'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'行标识',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaPermissionSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'RowUniqueID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'对象的Schema标识',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaPermissionSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'SchemaType'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'创建人的ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaPermissionSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'CreatorID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'创建人的名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaPermissionSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'CreatorName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'权限对象快照表',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaPermissionSnapshot',
    @level2type = NULL,
    @level2name = NULL
GO

CREATE INDEX [IX_SchemaPermissionSnapshot_CodeName] ON [SC].[SchemaPermissionSnapshot] ([CodeName])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'注释',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaPermissionSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'Comment'