CREATE TABLE [SC].[IncomeSynchronizeLog]
(
	[LogID] NVARCHAR(36) NOT NULL PRIMARY KEY, 
	[SourceName] nvarchar(128) NOT NULL,
    [StartTime] DATETIME NOT NULL, 
    [EndTime] DATETIME NULL, 
	CreateTime DATETIME NOT NULL DEFAULT getdate(),
    [OperatorID] NVARCHAR(36) NOT NULL, 
    [OperatorName] NVARCHAR(128) NOT NULL, 
    [NumberOfModifiedItems] INT NOT NULL, 
    [NumberOfExceptions] INT NOT NULL, 
    [Status] INT NOT NULL
)

GO
