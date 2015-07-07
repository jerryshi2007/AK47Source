CREATE TABLE [SC].[ConditionCalculateResult]
(
	[OwnerID] NVARCHAR(36) NOT NULL , 
    [UserID] NVARCHAR(36) NOT NULL, 
    [CreateTime] DATETIME NULL DEFAULT GETDATE(), 
    CONSTRAINT [PK_ConditionCalculateResult] PRIMARY KEY ([OwnerID], [UserID])
)

GO
CREATE INDEX [IX_ConditionCalculateResult_UserID] ON [SC].[ConditionCalculateResult] ([UserID])
