CREATE TABLE [SC].[ADSynchronizeLog]
(
	[LogID] NVARCHAR(36) NOT NULL PRIMARY KEY, 
    [SynchronizeID] NVARCHAR(36) NOT NULL, 
    [StartTime] DATETIME NOT NULL, 
    [EndTime] DATETIME NULL, 
    [OperatorID] NVARCHAR(36) NULL, 
    [OperatorName] NVARCHAR(128) NULL, 
    [SynchronizeResult] INT NOT NULL, 
    [ExceptionCount] INT NOT NULL, 
    [CreateTime] DATETIME NOT NULL DEFAULT getdate(), 
    [AddingItemCount] INT NOT NULL, 
    [DeletingItemCount] INT NOT NULL, 
    [ModifyingItemCount] INT NOT NULL, 
    [AddedItemCount] INT NOT NULL, 
    [DeletedItemCount] INT NOT NULL, 
    [ModifiedItemCount] INT NOT NULL
)

GO
