CREATE TABLE [SC].[SchemaOrganizationSnapshot]
(
	[ID]               NVARCHAR (36)  NOT NULL,
    [VersionStartTime] DATETIME       NOT NULL,
    [VersionEndTime]   DATETIME       CONSTRAINT [DF_SchemaOrganization_VersionEndTime] DEFAULT ('99990909 00:00:00') NULL,
    [Status]           INT            CONSTRAINT [DF_SchemaOrganization_Status] DEFAULT ((1)) NULL,
	[CreateDate]       DATETIME NULL DEFAULT GETDATE(), 
    [Name]             NVARCHAR (255) NULL,
    [DisplayName]      NVARCHAR (255) NULL,
	[CodeName]         NVARCHAR (64) NULL,
	[AllowAclInheritance] INT DEFAULT 1,
    [SearchContent]    NVARCHAR (MAX) NULL,
    [RowUniqueID]      NVARCHAR (36)  CONSTRAINT [DF_SchemaOrganization_RowUniqueID] DEFAULT (CONVERT([nvarchar](36),newid())) NOT NULL,
    [SchemaType] NVARCHAR(36) NULL, 
	[CreatorID] NVARCHAR(36) NULL, 
    [CreatorName] NVARCHAR(255) NULL,
    [Comment] NVARCHAR(255) NULL, 
    CONSTRAINT [PK_SchemaOrganization] PRIMARY KEY CLUSTERED ([ID] ASC, [VersionStartTime] DESC)
)

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_SchemaOrganizationSnapshot_RowID] ON [SC].[SchemaOrganizationSnapshot] ([RowUniqueID])

GO

CREATE INDEX [IX_SchemaOrganizationSnapshot_CodeName] ON [SC].[SchemaOrganizationSnapshot] ([CodeName])

GO
