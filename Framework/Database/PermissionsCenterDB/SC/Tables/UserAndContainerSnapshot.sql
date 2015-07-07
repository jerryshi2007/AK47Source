CREATE TABLE [SC].[UserAndContainerSnapshot]
(
	[UserID] NVARCHAR(36) NOT NULL , 
    [ContainerID] NVARCHAR(36) NOT NULL,
	[VersionStartTime] DATETIME NOT NULL,
	[VersionEndTime]   DATETIME      CONSTRAINT [DF_UserAndContainerSnapshot_VersionEndTime] DEFAULT ('99990909 00:00:00') NULL,
    [Status]           INT           CONSTRAINT [DF_UserAndContainerSnapshot_Status] DEFAULT ((1)) NULL,
    [UserSchemaType] NVARCHAR(64) NULL, 
    [ContainerSchemaType] NVARCHAR(64) NULL, 
    CONSTRAINT [PK_UserAndContainerSnapshot] PRIMARY KEY ([UserID], [ContainerID], [VersionStartTime])
)

GO

CREATE INDEX [IX_UserAndContainerSnapshot_Container] ON [SC].[UserAndContainerSnapshot] ([ContainerID])

GO
