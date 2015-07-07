CREATE TABLE [SC].[ADReverseSynchronizeLog]
(
	[LogID] NVARCHAR(36) NOT NULL PRIMARY KEY, 
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
