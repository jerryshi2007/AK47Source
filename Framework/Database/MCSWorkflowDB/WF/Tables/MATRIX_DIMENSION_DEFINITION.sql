CREATE TABLE [WF].[MATRIX_DIMENSION_DEFINITION] (
    [MATRIX_DEF_KEY] NVARCHAR (64)  NOT NULL,
    [DIMENSION_KEY]  NVARCHAR (64)  NOT NULL,
    [SORT_ORDER]     INT            NULL,
    [NAME]           NVARCHAR (64)  NULL,
    [DESCRIPTION]    NVARCHAR (MAX) NULL,
    [DATA_TYPE]      INT            NULL,
    CONSTRAINT [PK_MATRIX_DIMENSION_DEFINITION] PRIMARY KEY CLUSTERED ([MATRIX_DEF_KEY] ASC, [DIMENSION_KEY] ASC)
);


GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'矩阵的维度（列）定义',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'MATRIX_DIMENSION_DEFINITION',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'矩阵的KEY',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'MATRIX_DIMENSION_DEFINITION',
    @level2type = N'COLUMN',
    @level2name = N'MATRIX_DEF_KEY'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'维度的KEY',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'MATRIX_DIMENSION_DEFINITION',
    @level2type = N'COLUMN',
    @level2name = N'DIMENSION_KEY'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'排序号',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'MATRIX_DIMENSION_DEFINITION',
    @level2type = N'COLUMN',
    @level2name = N'SORT_ORDER'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'维度的名称',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'MATRIX_DIMENSION_DEFINITION',
    @level2type = N'COLUMN',
    @level2name = N'NAME'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'维度的描述',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'MATRIX_DIMENSION_DEFINITION',
    @level2type = N'COLUMN',
    @level2name = N'DESCRIPTION'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'维度的数据类型',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'MATRIX_DIMENSION_DEFINITION',
    @level2type = N'COLUMN',
    @level2name = N'DATA_TYPE'