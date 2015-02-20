CREATE TABLE [SC].[SchemaDefine]
(
	[Name] NVARCHAR(64) NOT NULL PRIMARY KEY,
	[TableName] NVARCHAR(255) NULL, 
    [SnapshotTable] NVARCHAR(255) NULL, 
    [IsUsersContainer] INT NULL DEFAULT 0, 
    [Category] NVARCHAR(64) NULL,
	[CodeNameKey] NVARCHAR(64) NULL,
	[CodeNameValidationMethod] INT NULL DEFAULT 0,
	[FullPathValidationMethod] INT NULL DEFAULT 0,
    [SortOrder] INT NULL DEFAULT 0, 
    [IsUsersContainerMember] INT NULL DEFAULT 0, 
    [ToSchemaObjectSnapshot] INT NULL DEFAULT 1, 
    [LogoImage] NVARCHAR(MAX) NULL, 
    [IsRelation] INT NULL DEFAULT 0
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Schema的定义信息',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaDefine',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Schema的名字',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaDefine',
    @level2type = N'COLUMN',
    @level2name = N'Name'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'快照表的名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaDefine',
    @level2type = N'COLUMN',
    @level2name = N'SnapshotTable'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'是不是群组或角色之类的人员的容器',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaDefine',
    @level2type = N'COLUMN',
    @level2name = 'IsUsersContainer'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'类别的名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaDefine',
    @level2type = N'COLUMN',
    @level2name = N'Category'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'排序号',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaDefine',
    @level2type = N'COLUMN',
    @level2name = N'SortOrder'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'是不是群组或角色之类的人员的容器的成员。通常人员会设置为true',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaDefine',
    @level2type = N'COLUMN',
    @level2name = N'IsUsersContainerMember'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'是否导出到通用的Snapshot表',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaDefine',
    @level2type = N'COLUMN',
    @level2name = N'ToSchemaObjectSnapshot'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Logo的图片地址',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaDefine',
    @level2type = N'COLUMN',
    @level2name = N'LogoImage'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'是否是关系类的Schema',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaDefine',
    @level2type = N'COLUMN',
    @level2name = N'IsRelation'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'基本对象表的名称。默认为SC.SchemaObject',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaDefine',
    @level2type = N'COLUMN',
    @level2name = N'TableName'
GO


EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'代码名称的键（数据库不用）',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaDefine',
    @level2type = N'COLUMN',
    @level2name = N'CodeNameKey'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'代码名称校验方法（数据库不用）',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaDefine',
    @level2type = N'COLUMN',
    @level2name = N'CodeNameValidationMethod'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'全路径校验方法（数据库不用）',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaDefine',
    @level2type = N'COLUMN',
    @level2name = N'FullPathValidationMethod'