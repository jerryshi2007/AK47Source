CREATE TABLE [SC].[ConditionCalculateResult]
(
	[OwnerID] NVARCHAR(36) NOT NULL , 
    [UserID] NVARCHAR(36) NOT NULL, 
    [CreateTime] DATETIME NULL DEFAULT GETDATE(), 
    CONSTRAINT [PK_ConditionCalculateResult] PRIMARY KEY ([OwnerID], [UserID])
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'条件计算的结果',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ConditionCalculateResult',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Owner的ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ConditionCalculateResult',
    @level2type = N'COLUMN',
    @level2name = N'OwnerID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'计算出的用户ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ConditionCalculateResult',
    @level2type = N'COLUMN',
    @level2name = N'UserID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'创建时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ConditionCalculateResult',
    @level2type = N'COLUMN',
    @level2name = N'CreateTime'
GO

CREATE INDEX [IX_ConditionCalculateResult_UserID] ON [SC].[ConditionCalculateResult] ([UserID])
