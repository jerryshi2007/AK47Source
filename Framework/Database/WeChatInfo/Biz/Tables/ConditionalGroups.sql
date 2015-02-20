CREATE TABLE [Biz].[ConditionalGroups]
(
	[GroupID] NVARCHAR(36) NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(64) NULL, 
    [Description] NVARCHAR(255) NULL, 
    [Condition] NVARCHAR(MAX) NULL, 
    [CalculateTime] DATETIME NULL
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'条件组',
    @level0type = N'SCHEMA',
    @level0name = N'Biz',
    @level1type = N'TABLE',
    @level1name = N'ConditionalGroups',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'组ID',
    @level0type = N'SCHEMA',
    @level0name = N'Biz',
    @level1type = N'TABLE',
    @level1name = N'ConditionalGroups',
    @level2type = N'COLUMN',
    @level2name = N'GroupID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'组名称',
    @level0type = N'SCHEMA',
    @level0name = N'Biz',
    @level1type = N'TABLE',
    @level1name = N'ConditionalGroups',
    @level2type = N'COLUMN',
    @level2name = N'Name'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'组描述',
    @level0type = N'SCHEMA',
    @level0name = N'Biz',
    @level1type = N'TABLE',
    @level1name = N'ConditionalGroups',
    @level2type = N'COLUMN',
    @level2name = N'Description'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'组的筛选条件表达式',
    @level0type = N'SCHEMA',
    @level0name = N'Biz',
    @level1type = N'TABLE',
    @level1name = N'ConditionalGroups',
    @level2type = N'COLUMN',
    @level2name = N'Condition'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'最后计算条件的时间',
    @level0type = N'SCHEMA',
    @level0name = N'Biz',
    @level1type = N'TABLE',
    @level1name = N'ConditionalGroups',
    @level2type = N'COLUMN',
    @level2name = N'CalculateTime'