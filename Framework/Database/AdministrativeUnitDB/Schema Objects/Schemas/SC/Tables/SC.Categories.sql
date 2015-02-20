CREATE TABLE [SC].[Categories]
(
	[ID] NVARCHAR(36) NOT NULL , 
    [Name] NVARCHAR(64) NOT NULL, 
    [ParentID] NVARCHAR(36) NULL, 
    [Status] INT NOT NULL DEFAULT 1, 
    [VersionStartTime] DATETIME NOT NULL DEFAULT GETDATE(), 
    [VersionEndTime] DATETIME NULL DEFAULT ('99990909 00:00:00'), 
	[Level] int NOT NULL DEFAULT 0,
	Rank int not null default 0,
    [FullPath] NVARCHAR(414) NOT NULL, 
    CONSTRAINT [PK_Categories] PRIMARY KEY ([ID],[VersionStartTime]) 
)

GO


CREATE INDEX [IX_L2Categories_Column_L1ID] ON [SC].[Categories] ([ParentID])
INCLUDE([Name])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'分类的ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'Categories',
    @level2type = N'COLUMN',
    @level2name = 'ID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'分类名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'Categories',
    @level2type = N'COLUMN',
    @level2name = N'Name'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'引用上级分类的Id(一级分类为NULL)',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'Categories',
    @level2type = N'COLUMN',
    @level2name = 'ParentID'
GO

GO

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'状态，为1表示启用',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'Categories',
    @level2type = N'COLUMN',
    @level2name = N'Status'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'版本开始时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'Categories',
    @level2type = N'COLUMN',
    @level2name = 'VersionStartTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'表示二级分类',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'Categories',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'版本结束时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'Categories',
    @level2type = N'COLUMN',
    @level2name = N'VersionEndTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'从一级分类开始的完整路径',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'Categories',
    @level2type = N'COLUMN',
    @level2name = N'FullPath'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'层级，0表示顶层',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'Categories',
    @level2type = N'COLUMN',
    @level2name = N'Level'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'优先级，数字越大，越优先',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'Categories',
    @level2type = N'COLUMN',
    @level2name = N'Rank'