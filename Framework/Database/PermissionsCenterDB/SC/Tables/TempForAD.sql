CREATE TABLE [SC].[TempForAD]
(
	[CodeName] NVARCHAR(255) NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(255) NOT NULL, 
    [Mobile] NVARCHAR(255) NULL, 
    [OtherMobile] NVARCHAR(255) NULL, 
    [Company] NVARCHAR(255) NULL, 
    [Department] NVARCHAR(255) NULL
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'代码名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'TempForAD',
    @level2type = N'COLUMN',
    @level2name = 'CodeName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'对象名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'TempForAD',
    @level2type = N'COLUMN',
    @level2name = 'Name'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'手机号',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'TempForAD',
    @level2type = N'COLUMN',
    @level2name = 'Mobile'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'其他数据库，逗号分隔',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'TempForAD',
    @level2type = N'COLUMN',
    @level2name = N'OtherMobile'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'部门',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'TempForAD',
    @level2type = N'COLUMN',
    @level2name = N'Department'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'公司',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'TempForAD',
    @level2type = N'COLUMN',
    @level2name = N'Company'
GO

CREATE INDEX [IX_TempForAD_Column] ON [SC].[TempForAD] ([CodeName])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'临时从AD导出数据来给权限中心用的',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'TempForAD',
    @level2type = NULL,
    @level2name = NULL