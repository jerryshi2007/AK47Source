CREATE TABLE [SC].[ADSynchronizeLogDetail]
(
	[LogDetailID] NVARCHAR(36) NOT NULL PRIMARY KEY, 
    [SynchronizeID] NVARCHAR(36) NOT NULL, 
    [SCObjectID] NVARCHAR(36) NULL, 
    [SCObjectName] NVARCHAR(256) NULL, 
    [ADObjectID] NVARCHAR(36) NULL, 
    [ADObjectName] NVARCHAR(256) NULL, 
    [ActionName] NVARCHAR(32) NOT NULL, 
    [Detail] NVARCHAR(MAX) NOT NULL, 
    [CreateTime] DATETIME NOT NULL DEFAULT getdate()
)

GO
