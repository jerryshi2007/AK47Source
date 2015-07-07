CREATE TABLE [SC].[Locks]
(
	[LockID] NVARCHAR(36) NOT NULL PRIMARY KEY, 
    [ResourceID] NVARCHAR(36) NULL, 
    [LockPersonID] NVARCHAR(36) NULL, 
    [LockPersonName] NVARCHAR(255) NULL, 
    [LockTime] DATETIME NULL DEFAULT GETDATE(), 
    [EffectiveTime] INT NULL, 
    [LockType] INT NULL, 
    [Description] NVARCHAR(255) NULL
)

GO
