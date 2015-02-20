CREATE TABLE [SC].[SchemaMembersSnapshot]
(
	[ContainerID] NVARCHAR(36) NOT NULL , 
    [MemberID] NVARCHAR(36) NOT NULL, 
    [VersionStartTime] DATETIME NOT NULL, 
    [VersionEndTime] DATETIME NULL DEFAULT ('99990909 00:00:00'), 
    [Status] INT NULL DEFAULT 1, 
    [InnerSort] INT NULL DEFAULT 0, 
    [SchemaType] NVARCHAR(64) NULL, 
    [SearchContent] NVARCHAR(MAX) NULL, 
    [RowUniqueID] NVARCHAR(36) NOT NULL DEFAULT (CONVERT([nvarchar](36),newid())), 
    [ContainerSchemaType] NVARCHAR(64) NULL, 
    [MemberSchemaType] NVARCHAR(64) NULL, 
	[CreateDate] DATETIME NULL DEFAULT GETDATE(),
	[CreatorID] NVARCHAR(36) NULL, 
    [CreatorName] NVARCHAR(255) NULL,
    CONSTRAINT [PK_SchemaMembersSnapshot] PRIMARY KEY ([ContainerID], [MemberID], [VersionStartTime] DESC)
)

GO

CREATE INDEX [IX_SchemaMembersSnapshot_ObjectID] ON [SC].[SchemaMembersSnapshot] ([MemberID])

GO

CREATE UNIQUE INDEX [IX_SchemaMembersSnapshot_RowUniqueID] ON [SC].[SchemaMembersSnapshot] ([RowUniqueID])

GO

CREATE FULLTEXT INDEX ON [SC].[SchemaMembersSnapshot] ([SearchContent] LANGUAGE 2052) KEY INDEX [IX_SchemaMembersSnapshot_RowUniqueID] ON [SCFullTextIndex] WITH CHANGE_TRACKING AUTO

GO

EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'成员关系对象快照表',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaMembersSnapshot',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'容器的ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaMembersSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'ContainerID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'成员的ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaMembersSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'MemberID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'版本开始时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaMembersSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'VersionStartTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'版本结束时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaMembersSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'VersionEndTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'对象的状态',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaMembersSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'Status'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'内部排序号',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaMembersSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'InnerSort'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'对象的Schema名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaMembersSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'SchemaType'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'容器的Schema名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaMembersSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'SearchContent'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'成员的Schema名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaMembersSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'RowUniqueID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'容器的Schema名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaMembersSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'ContainerSchemaType'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'成员的Schema名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaMembersSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'MemberSchemaType'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'创建时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaMembersSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'CreateDate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'创建人的ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaMembersSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'CreatorID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'创建人的名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaMembersSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'CreatorName'