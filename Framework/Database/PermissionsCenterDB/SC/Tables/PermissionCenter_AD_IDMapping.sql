CREATE TABLE [SC].[PermissionCenter_AD_IDMapping]
(
	[SCObjectID] NVARCHAR(36) NOT NULL PRIMARY KEY, 
    [ADObjectGuid] NVARCHAR(36) NOT NULL, 
    [LastSynchronizedVersionTime] DATETIME NOT NULL
)

GO
