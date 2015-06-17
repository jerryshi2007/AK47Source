CREATE TABLE [WF].[MATRIX_CELLS] (
    [MATRIX_ID]     NVARCHAR (64) NOT NULL,
    [MATRIX_ROW_ID] NVARCHAR (64) NOT NULL,
    [DIMENSION_KEY] NVARCHAR (64) NOT NULL,
    [STRING_VALUE]  NVARCHAR (64) NULL,
	[TENANT_CODE] NVARCHAR(36) NOT NULL DEFAULT 'D5561180-7617-4B67-B68B-1F0EA604B509'
    CONSTRAINT [PK_MATRIX_CELLS] PRIMARY KEY CLUSTERED ([MATRIX_ID] ASC, [MATRIX_ROW_ID] ASC, [DIMENSION_KEY] ASC, [TENANT_CODE] ASC)
);

GO

CREATE INDEX [IX_MATRIX_CELLS_TENANT_CODE] ON [WF].[MATRIX_CELLS] ([TENANT_CODE])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'流程矩阵的单元格',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'MATRIX_CELLS',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'矩阵ID',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'MATRIX_CELLS',
    @level2type = N'COLUMN',
    @level2name = N'MATRIX_ID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'矩阵的行ID',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'MATRIX_CELLS',
    @level2type = N'COLUMN',
    @level2name = N'MATRIX_ROW_ID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'维度（列）的KEY',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'MATRIX_CELLS',
    @level2type = N'COLUMN',
    @level2name = N'DIMENSION_KEY'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'维度的值',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'MATRIX_CELLS',
    @level2type = N'COLUMN',
    @level2name = N'STRING_VALUE'