CREATE TABLE [SC].[AUAdminScopeTypes]
(
	[SchemaType] NVARCHAR(64) NOT NULL, 
    [SchemaName] NVARCHAR(64) NOT NULL, 
    CONSTRAINT [PK_AUAdminScopeTypes] PRIMARY KEY ([SchemaType])
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'管理范围的类型',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'AUAdminScopeTypes',
    @level2type = N'COLUMN',
    @level2name = 'SchemaType'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'管理范围的名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'AUAdminScopeTypes',
    @level2type = N'COLUMN',
    @level2name = N'SchemaName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'存储管理范围的类型',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'AUAdminScopeTypes',
    @level2type = NULL,
    @level2name = NULL