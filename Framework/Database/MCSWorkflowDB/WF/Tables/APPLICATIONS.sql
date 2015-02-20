CREATE TABLE [WF].[APPLICATIONS]
(
	[CODE_NAME] NVARCHAR(64) NOT NULL PRIMARY KEY, 
    [NAME] NVARCHAR(64) NULL, 
    [SORT] INT NULL DEFAULT 0
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'应用的代码名称',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'APPLICATIONS',
    @level2type = N'COLUMN',
    @level2name = N'CODE_NAME'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'应用的名称',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'APPLICATIONS',
    @level2type = N'COLUMN',
    @level2name = N'NAME'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'应用的定义列表',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'APPLICATIONS',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'排序',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'APPLICATIONS',
    @level2type = N'COLUMN',
    @level2name = N'SORT'