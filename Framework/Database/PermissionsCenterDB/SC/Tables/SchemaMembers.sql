CREATE TABLE [SC].[SchemaMembers] (
    [ContainerID]		NVARCHAR (36)  NOT NULL,
    [MemberID]			NVARCHAR (36)  NOT NULL,
    [VersionStartTime]	DATETIME       NOT NULL,
    [VersionEndTime]	DATETIME       CONSTRAINT [DF_SchemaMembers_VersionEndTime] DEFAULT ('99990909 00:00:00') NULL,
    [Status]			INT            CONSTRAINT [DF_SchemaMembers_Status] DEFAULT ((1)) NULL,
    [InnerSort]			INT            CONSTRAINT [DF_SchemaMembers_InnerSort] DEFAULT ((0)) NULL,
    [Data]				XML            NULL,
    [SchemaType]		NVARCHAR (64)  NULL,
    [ContainerSchemaType]	NVARCHAR(64) NULL, 
    [MemberSchemaType]	NVARCHAR(64) NULL, 
	[CreateDate] DATETIME NULL DEFAULT GETDATE(),
	[CreatorID] NVARCHAR(36) NULL, 
    [CreatorName] NVARCHAR(255) NULL,
    CONSTRAINT [PK_SchemaObjectMembers] PRIMARY KEY CLUSTERED ([ContainerID] ASC, [MemberID] ASC, [VersionStartTime] DESC)
);


GO

CREATE INDEX [IX_SchemaMembers_MemberID] ON [SC].[SchemaMembers] ([MemberID])

GO

