CREATE TABLE [SC].[IncomeSynchronizeLogDetail]
(
	[LogDetailID] NVARCHAR(36) NOT NULL PRIMARY KEY , 
    [LogID] NVARCHAR(36) NOT NULL, 
	[CreateTime] datetime NOT NULL DEFAULT getdate(),
    [SCObjectID] NVARCHAR(36) NOT NULL, 
	[ActionType] int NOT NULL,
	[Summary] NVARCHAR(256) NOT NULL DEFAULT '',
    [Detail] NVARCHAR(MAX) NOT NULL
)
GO