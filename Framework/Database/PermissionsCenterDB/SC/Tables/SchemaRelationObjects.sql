CREATE TABLE [SC].[SchemaRelationObjects] (
    [ParentID]         NVARCHAR (36)  NOT NULL,
    [ObjectID]         NVARCHAR (36)  NOT NULL,
    [VersionStartTime] DATETIME       NOT NULL,
    [VersionEndTime]   DATETIME       CONSTRAINT [DF_SchemaObjectRelations_VersionEndTime] DEFAULT ('99990909 00:00:00') NULL,
    [Status]           INT            CONSTRAINT [DF_SchemaObjectRelations_Status] DEFAULT ((1)) NULL,
	[IsDefault]	       INT            CONSTRAINT [DF_SchemaObjectRelations_Default] DEFAULT ((1)) NULL,
    [InnerSort]        INT            CONSTRAINT [DF_SchemaRelationObjects_InnerSort] DEFAULT ((0)) NULL,
	[FullPath]         NVARCHAR(414)  NULL,
	[GlobalSort]       NVARCHAR(414)  NULL,
    [Data]             XML            NULL,
    [SchemaType]       NVARCHAR (64)  NULL,
    [ParentSchemaType] NVARCHAR(64) NULL, 
    [ChildSchemaType] NVARCHAR(64) NULL, 
    [CreateDate] DATETIME NULL DEFAULT GETDATE(), 
	[CreatorID] NVARCHAR(36) NULL, 
    [CreatorName] NVARCHAR(255) NULL,
    CONSTRAINT [PK_SchemaObjectRelations] PRIMARY KEY CLUSTERED ([ParentID] ASC, [ObjectID] ASC, [VersionStartTime] DESC)
);

GO

CREATE INDEX [IX_SchemaRelationObjects_ObjectID] ON [SC].[SchemaRelationObjects] ([ObjectID])

GO

CREATE INDEX [IX_SchemaRelationObjects_FullPath] ON [SC].[SchemaRelationObjects] ([FullPath], [VersionStartTime])

GO

CREATE INDEX [IX_SchemaRelationObjects_GlobalSort] ON [SC].[SchemaRelationObjects] ([GlobalSort], [VersionStartTime])
