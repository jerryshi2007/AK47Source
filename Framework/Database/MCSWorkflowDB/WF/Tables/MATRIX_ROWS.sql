CREATE TABLE [WF].[MATRIX_ROWS] (
    [MATRIX_ID]     NVARCHAR (64) NOT NULL,
    [MATRIX_ROW_ID] INT           NOT NULL,
    [OPERATOR_TYPE] INT           NULL,
    [OPERATOR]      NVARCHAR (64) NULL,
	[TENANT_CODE] NVARCHAR(36) NOT NULL DEFAULT 'D5561180-7617-4B67-B68B-1F0EA604B509'
    CONSTRAINT [PK_MATRIX_ROWS] PRIMARY KEY CLUSTERED ([MATRIX_ID] ASC, [MATRIX_ROW_ID] ASC, [TENANT_CODE] ASC)
);

GO

CREATE INDEX [IX_MATRIX_ROWS_TENANT_CODE] ON [WF].[MATRIX_ROWS] ([TENANT_CODE])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'矩阵的行信息',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'MATRIX_ROWS',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'矩阵的实例ID',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'MATRIX_ROWS',
    @level2type = N'COLUMN',
    @level2name = N'MATRIX_ID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'矩阵的行ID',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'MATRIX_ROWS',
    @level2type = N'COLUMN',
    @level2name = N'MATRIX_ROW_ID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'操作人类型(USERS或ROLES)',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'MATRIX_ROWS',
    @level2type = N'COLUMN',
    @level2name = N'OPERATOR_TYPE'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'操作人的ID',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'MATRIX_ROWS',
    @level2type = N'COLUMN',
    @level2name = N'OPERATOR'