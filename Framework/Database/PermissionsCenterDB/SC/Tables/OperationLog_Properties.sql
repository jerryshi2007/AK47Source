EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'操作日志',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'OperationLog',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'日志创建时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'OperationLog',
    @level2type = N'COLUMN',
    @level2name = N'CreateTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'环境上下文ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'OperationLog',
    @level2type = N'COLUMN',
    @level2name = N'CorrelationID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'相关对象的ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'OperationLog',
    @level2type = N'COLUMN',
    @level2name = N'ResourceID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'类别',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'OperationLog',
    @level2type = N'COLUMN',
    @level2name = N'Category'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'操作人ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'OperationLog',
    @level2type = N'COLUMN',
    @level2name = N'OperatorID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'操作人名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'OperationLog',
    @level2type = N'COLUMN',
    @level2name = N'OperatorName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'实际操作人ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'OperationLog',
    @level2type = N'COLUMN',
    @level2name = N'RealOperatorID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'实际操作人名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'OperationLog',
    @level2type = N'COLUMN',
    @level2name = N'RealOperatorName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'请求的上下文',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'OperationLog',
    @level2type = N'COLUMN',
    @level2name = N'RequestContext'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'主题',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'OperationLog',
    @level2type = N'COLUMN',
    @level2name = N'Subject'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'操作对象的类型',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'OperationLog',
    @level2type = N'COLUMN',
    @level2name = N'SchemaType'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'操作类型',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'OperationLog',
    @level2type = N'COLUMN',
    @level2name = N'OperationType'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'供全文检索的字段',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'OperationLog',
    @level2type = N'COLUMN',
    @level2name = N'SearchContent'
GO