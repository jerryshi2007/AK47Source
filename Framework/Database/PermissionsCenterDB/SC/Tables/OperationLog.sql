CREATE TABLE [SC].[OperationLog]
(
	[ID] INT NOT NULL IDENTITY, 
    [ResourceID] NVARCHAR(36) NULL, 
    [CorrelationID] NVARCHAR(36) NULL, 
    [Category] NVARCHAR(64) NULL, 
    [OperatorID] NVARCHAR(36) NULL, 
    [OperatorName] NVARCHAR(128) NULL, 
    [RealOperatorID] NVARCHAR(36) NULL, 
    [RealOperatorName] NVARCHAR(128) NULL, 
    [RequestContext] NVARCHAR(MAX) NULL, 
    [Subject] NVARCHAR(MAX) NULL, 
    [SchemaType] NVARCHAR(64) NULL, 
    [OperationType] NVARCHAR(64) NULL, 
    [SearchContent] NVARCHAR(MAX) NULL, 
    [CreateTime] DATETIME NULL DEFAULT getdate(), 
	CONSTRAINT [PK_OperationLog] PRIMARY KEY CLUSTERED ([ID] DESC)
)

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

CREATE INDEX [IX_OperationLog_ResourceID] ON [SC].[OperationLog] ([ResourceID])

GO

CREATE FULLTEXT INDEX ON [SC].[OperationLog] ([SearchContent] LANGUAGE 2052, [Subject] LANGUAGE 2052) KEY INDEX [PK_OperationLog] ON [SCFullTextIndex] WITH CHANGE_TRACKING AUTO

GO
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