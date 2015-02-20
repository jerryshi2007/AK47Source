CREATE TABLE [SC].[SchemaPropertyExtensions]
(
    [TargetSchemaType] NVARCHAR(36) NOT NULL, 
    [SourceID] NVARCHAR(36) NOT NULL, 
    [Definition] XML NOT NULL, 
    [Description] NVARCHAR(255) NULL, 
    CONSTRAINT [PK_SchemaPropertyExtensions] PRIMARY KEY ([TargetSchemaType], [SourceID]) 
)

GO

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'目标Schema类型',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaPropertyExtensions',
    @level2type = N'COLUMN',
    @level2name = N'TargetSchemaType'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'扩展属性的识别ID。对于管理单元来说，应该是管理架构的ID。',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaPropertyExtensions',
    @level2type = N'COLUMN',
    @level2name = N'SourceID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'属性定义的xml',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaPropertyExtensions',
    @level2type = N'COLUMN',
    @level2name = N'Definition'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'扩展属性定义',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaPropertyExtensions',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'描述',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaPropertyExtensions',
    @level2type = N'COLUMN',
    @level2name = N'Description'
GO
