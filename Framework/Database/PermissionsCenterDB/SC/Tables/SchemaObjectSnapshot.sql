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

CREATE INDEX [IX_SchemaObjectSnapshot_StartTime] ON [SC].[SchemaObjectSnapshot] ([VersionStartTime])

GO


CREATE INDEX [IX_SchemaObjectSnapshot_Name] ON [SC].[SchemaObjectSnapshot] ([Name])

GO

CREATE INDEX [IX_SchemaObjectSnapshot_CodeName] ON [SC].[SchemaObjectSnapshot] ([CodeName])

GO

CREATE INDEX [IX_SchemaObjectSnapshot_SchemaType] ON [SC].[SchemaObjectSnapshot] ([SchemaType])

GO
