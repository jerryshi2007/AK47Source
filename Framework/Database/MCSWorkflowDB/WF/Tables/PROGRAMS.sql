CREATE TABLE [WF].[PROGRAMS]
(
    [APPLICATION_CODE_NAME] NVARCHAR(64) NOT NULL, 
    [CODE_NAME] NVARCHAR(64) NOT NULL, 
    [NAME] NVARCHAR(64) NULL, 
    [SORT] INT NULL DEFAULT 0, 
    CONSTRAINT [PK_PROGRAMS] PRIMARY KEY ([APPLICATION_CODE_NAME], [CODE_NAME])
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'代码名称',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'PROGRAMS',
    @level2type = N'COLUMN',
    @level2name = N'CODE_NAME'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'名称',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'PROGRAMS',
    @level2type = N'COLUMN',
    @level2name = N'NAME'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'应用的名称',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'PROGRAMS',
    @level2type = N'COLUMN',
    @level2name = N'APPLICATION_CODE_NAME'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'排序号',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'PROGRAMS',
    @level2type = N'COLUMN',
    @level2name = N'SORT'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'应用程序分类表',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'PROGRAMS',
    @level2type = NULL,
    @level2name = NULL
GO