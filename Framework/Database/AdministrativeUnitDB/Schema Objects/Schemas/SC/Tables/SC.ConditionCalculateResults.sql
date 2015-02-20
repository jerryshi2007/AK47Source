CREATE TABLE [SC].[ConditionCalculateResults]
(
	[OwnerID] NVARCHAR(36) NOT NULL , 
    [ObjectID] NVARCHAR(36) NOT NULL, 
    [CreateTime] DATETIME NULL DEFAULT GETDATE(), 
    CONSTRAINT [PK_ConditionCalculateResults] PRIMARY KEY ([OwnerID],[ObjectID]) 
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Owner的ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ConditionCalculateResults',
    @level2type = N'COLUMN',
    @level2name = N'OwnerID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'表示条件计算的结果',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ConditionCalculateResults',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'计算出的对象ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ConditionCalculateResults',
    @level2type = N'COLUMN',
    @level2name = 'ObjectID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'创建时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ConditionCalculateResults',
    @level2type = N'COLUMN',
    @level2name = N'CreateTime'
GO
