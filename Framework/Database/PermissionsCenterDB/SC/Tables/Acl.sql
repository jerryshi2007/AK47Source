CREATE TABLE [SC].[Acl]
(
	[ContainerID]			NVARCHAR (36)  NOT NULL,
	[ContainerPermission]	NVARCHAR(64) NOT NULL,
    [MemberID]				NVARCHAR (36)  NOT NULL,
    [VersionStartTime]		DATETIME       NOT NULL,
    [VersionEndTime]		DATETIME       CONSTRAINT [DF_Acl_VersionEndTime] DEFAULT ('99990909 00:00:00') NULL,
    [Status]				INT            CONSTRAINT [DF_Acl_Status] DEFAULT ((1)) NULL,
    [SortID]				INT            CONSTRAINT [DF_Acl_InnerSort] DEFAULT ((0)) NULL,
    [Data]					XML            NULL,
    [ContainerSchemaType]	NVARCHAR(64) NULL, 
    [MemberSchemaType]		NVARCHAR(64) NULL, 
    CONSTRAINT [PK_Acl] PRIMARY KEY CLUSTERED ([ContainerID] ASC, [ContainerPermission] ASC, [MemberID] ASC, [VersionStartTime] DESC)
)

GO

CREATE INDEX [IX_Acl_MemberID] ON [SC].[Acl] ([MemberID])
