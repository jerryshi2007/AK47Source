CREATE TABLE [SC].[Conditions]
(
	[OwnerID] NVARCHAR(36) NOT NULL , 
	[Type] NVARCHAR(64) NOT NULL,
    [SortID] INT NOT NULL, 
    [Description] NVARCHAR(255) NULL, 
    [Condition] NVARCHAR(MAX) NULL, 
    [VersionStartTime] DATETIME NOT NULL, 
    [VersionEndTime] DATETIME NULL DEFAULT ('99990909 00:00:00'), 
    [Status] INT NULL DEFAULT 1, 
    CONSTRAINT [PK_Conditions] PRIMARY KEY ([OwnerID], [Type], [SortID], [VersionStartTime]) 
	)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'条件表达式拥有者的ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'Conditions',
    @level2type = N'COLUMN',
    @level2name = N'OwnerID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'表达式的类别(应为AdminScope)',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'Conditions',
    @level2type = N'COLUMN',
    @level2name = N'Type'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'内部排序号(在此情况下，必须为1)',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'Conditions',
    @level2type = N'COLUMN',
    @level2name = N'SortID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'描述',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'Conditions',
    @level2type = N'COLUMN',
    @level2name = N'Description'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'条件表达式',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'Conditions',
    @level2type = N'COLUMN',
    @level2name = N'Condition'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'版本开始时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'Conditions',
    @level2type = N'COLUMN',
    @level2name = N'VersionStartTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'版本结束时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'Conditions',
    @level2type = N'COLUMN',
    @level2name = N'VersionEndTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'状态',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'Conditions',
    @level2type = N'COLUMN',
    @level2name = N'Status'
GO

EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'条件表达式',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'Conditions',
    @level2type = NULL,
    @level2name = NULL