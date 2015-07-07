CREATE TABLE [SC].[SCOperationSnapshot]
(
	[OperationType] NVARCHAR(64) NOT NULL PRIMARY KEY, 
    [OperateTime] DATETIME NULL DEFAULT GETDATE(), 
    [OperatorID] NVARCHAR(36) NULL, 
    [OperatorName] NVARCHAR(255) NULL
)

GO

CREATE INDEX [IX_SCOperationSnapshot_OperateTime] ON [SC].[SCOperationSnapshot] ([OperateTime] DESC)
