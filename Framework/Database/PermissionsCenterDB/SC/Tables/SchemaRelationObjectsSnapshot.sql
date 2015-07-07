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

CREATE INDEX [IX_SchemaRelationObjectsSnapshot_FullPath] ON [SC].[SchemaRelationObjectsSnapshot] ([FullPath], [VersionStartTime])

GO

CREATE INDEX [IX_SchemaRelationObjectsSnapshot_GlobalSort] ON [SC].[SchemaRelationObjectsSnapshot] ([GlobalSort], [VersionStartTime])

GO
