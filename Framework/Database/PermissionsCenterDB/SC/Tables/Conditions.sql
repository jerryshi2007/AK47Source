CREATE TABLE [SC].[Conditions]
(
	[OwnerID] NVARCHAR(36) NOT NULL , 
	[Type] NVARCHAR(64) NOT NULL,
    [SortID] INT NOT NULL, 
    [Description] NVARCHAR(255) NULL, 
    [Condition] NVARCHAR(MAX) NULL, 
    [VersionStartTime] DATETIME NOT NULL, 
    [VersionEndTime] DATETIME NULL DEFAULT ('99990909 00:00:00'), 
    [Status] INT NULL DEFAULT 1, 
    CONSTRAINT [PK_Conditions] PRIMARY KEY ([OwnerID], [Type], [SortID], [VersionStartTime]) 
)

GO
