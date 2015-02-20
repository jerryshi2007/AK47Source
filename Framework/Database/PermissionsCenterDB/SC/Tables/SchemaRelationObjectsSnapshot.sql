CREATE TABLE [SC].[SchemaRelationObjectsSnapshot]
(
	[ParentID] NVARCHAR(36) NOT NULL , 
    [ObjectID] NVARCHAR(36) NOT NULL, 
    [VersionStartTime] DATETIME NOT NULL, 
    [VersionEndTime] DATETIME NULL DEFAULT ('99990909 00:00:00'), 
    [Status] INT NULL DEFAULT 1, 
	[IsDefault] INT NULL DEFAULT 1,
    [InnerSort] INT NULL DEFAULT 0, 
	[FullPath]  NVARCHAR(414)  NULL,
	[GlobalSort] NVARCHAR(414)  NULL,
    [SchemaType] NVARCHAR(64) NULL, 
    [SearchContent] NVARCHAR(MAX) NULL, 
    [RowUniqueID] NVARCHAR(36) NOT NULL DEFAULT (CONVERT([nvarchar](36),newid())), 
    [ParentSchemaType] NVARCHAR(64) NULL, 
    [ChildSchemaType] NVARCHAR(64) NULL, 
	[CreateDate] DATETIME NULL DEFAULT GETDATE(), 
	[CreatorID] NVARCHAR(36) NULL, 
    [CreatorName] NVARCHAR(255) NULL,
    CONSTRAINT [PK_SchemaRelationObjectsSnapshot] PRIMARY KEY ([ParentID], [ObjectID], [VersionStartTime] DESC)
)

GO

CREATE INDEX [IX_SchemaRelationObjectsSnapshot_ObjectID] ON [SC].[SchemaRelationObjectsSnapshot] ([ObjectID])

GO

CREATE UNIQUE INDEX [IX_SchemaRelationObjectsSnapshot_RowUniqueID] ON [SC].[SchemaRelationObjectsSnapshot] ([RowUniqueID])

GO

CREATE FULLTEXT INDEX ON [SC].[SchemaRelationObjectsSnapshot] ([SearchContent] LANGUAGE 2052) KEY INDEX [IX_SchemaRelationObjectsSnapshot_RowUniqueID] ON [SCFullTextIndex] WITH CHANGE_TRACKING AUTO

GO

EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'对象关系表的快照',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaRelationObjectsSnapshot',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'父对象ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaRelationObjectsSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'ParentID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'子对象ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaRelationObjectsSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'ObjectID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'版本开始时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaRelationObjectsSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'VersionStartTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'版本结束时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaRelationObjectsSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'VersionEndTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'对象的状态',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaRelationObjectsSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'Status'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'是否是默认关系',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaRelationObjectsSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'IsDefault'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'内部排序号',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaRelationObjectsSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'InnerSort'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'对象的Schema名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaRelationObjectsSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'SchemaType'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'全文检索字段',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaRelationObjectsSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'SearchContent'
GO

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'子对象的Schema名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaRelationObjectsSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'ChildSchemaType'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'父对象的Schema名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaRelationObjectsSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'ParentSchemaType'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'行标识，用于全文检索',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaRelationObjectsSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'RowUniqueID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'对象创建时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaRelationObjectsSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'CreateDate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'创建人的ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaRelationObjectsSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'CreatorID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'创建人的名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaRelationObjectsSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'CreatorName'
GO

CREATE INDEX [IX_SchemaRelationObjectsSnapshot_FullPath] ON [SC].[SchemaRelationObjectsSnapshot] ([FullPath], [VersionStartTime])

GO

CREATE INDEX [IX_SchemaRelationObjectsSnapshot_GlobalSort] ON [SC].[SchemaRelationObjectsSnapshot] ([GlobalSort], [VersionStartTime])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'全路径',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaRelationObjectsSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'FullPath'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'全局排序号',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaRelationObjectsSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'GlobalSort'