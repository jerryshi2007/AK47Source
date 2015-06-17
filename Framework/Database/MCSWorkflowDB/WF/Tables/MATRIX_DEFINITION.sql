CREATE TABLE [WF].[MATRIX_DEFINITION] (
    [DEF_KEY]     NVARCHAR (64)  NOT NULL,
    [NAME]        NVARCHAR (64)  NULL,
    [DESCRIPTION] NVARCHAR (MAX) NULL,
    [ENABLED]     NCHAR (1)      NULL,
	[TENANT_CODE] NVARCHAR(36) NOT NULL DEFAULT 'D5561180-7617-4B67-B68B-1F0EA604B509'
    CONSTRAINT [PK_MATRIX_DEFINITION] PRIMARY KEY CLUSTERED ([DEF_KEY] ASC, [TENANT_CODE] ASC)
);

GO

CREATE INDEX [IX_MATRIX_DEFINITION_TENANT_CODE] ON [WF].[MATRIX_DEFINITION] ([TENANT_CODE])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'矩阵的KEY',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'MATRIX_DEFINITION',
    @level2type = N'COLUMN',
    @level2name = N'DEF_KEY'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'矩阵的名称',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'MATRIX_DEFINITION',
    @level2type = N'COLUMN',
    @level2name = N'NAME'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'矩阵的描述',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'MATRIX_DEFINITION',
    @level2type = N'COLUMN',
    @level2name = N'DESCRIPTION'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'是否启用',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'MATRIX_DEFINITION',
    @level2type = N'COLUMN',
    @level2name = N'ENABLED'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'矩阵的定义',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'MATRIX_DEFINITION',
    @level2type = NULL,
    @level2name = NULL