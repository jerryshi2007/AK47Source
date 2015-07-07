EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Schema中的属性定义',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaPropertyDefine',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Schema的名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaPropertyDefine',
    @level2type = N'COLUMN',
    @level2name = N'SchemaName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'属性的名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaPropertyDefine',
    @level2type = N'COLUMN',
    @level2name = 'Name'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'属性的显示名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaPropertyDefine',
    @level2type = N'COLUMN',
    @level2name = 'DisplayName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'描述信息',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaPropertyDefine',
    @level2type = N'COLUMN',
    @level2name = N'Description'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'数据类型',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaPropertyDefine',
    @level2type = N'COLUMN',
    @level2name = 'DataType'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'快照模式',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaPropertyDefine',
    @level2type = N'COLUMN',
    @level2name = N'SnapshotMode'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'类别',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaPropertyDefine',
    @level2type = N'COLUMN',
    @level2name = N'Category'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'所属页签',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaPropertyDefine',
    @level2type = N'COLUMN',
    @level2name = N'Tab'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'是否可见',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaPropertyDefine',
    @level2type = N'COLUMN',
    @level2name = N'Visible'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Editor的Key',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaPropertyDefine',
    @level2type = N'COLUMN',
    @level2name = N'EditorKey'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Editor参数',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaPropertyDefine',
    @level2type = N'COLUMN',
    @level2name = N'EditorParams'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'默认值',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaPropertyDefine',
    @level2type = N'COLUMN',
    @level2name = N'DefaultValue'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'是否只读',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaPropertyDefine',
    @level2type = N'COLUMN',
    @level2name = N'ReadOnly'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'排序号',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaPropertyDefine',
    @level2type = N'COLUMN',
    @level2name = N'SortOrder'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'数据持久化器的Key',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaPropertyDefine',
    @level2type = N'COLUMN',
    @level2name = N'PersisterKey'
GO

EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'是否必须',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaPropertyDefine',
    @level2type = N'COLUMN',
    @level2name = N'IsRequired'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'最大长度',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaPropertyDefine',
    @level2type = N'COLUMN',
    @level2name = N'MaxLength'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'快照对应的字段名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaPropertyDefine',
    @level2type = N'COLUMN',
    @level2name = N'SnapshotFieldName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'是否显示标题',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaPropertyDefine',
    @level2type = N'COLUMN',
    @level2name = N'ShowTitle'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'编辑器参数的设置工具的键',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaPropertyDefine',
    @level2type = N'COLUMN',
    @level2name = N'EditorParamsSettingsKey'