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

