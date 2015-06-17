CREATE TABLE [WF].[MATRIX_ROWS_USERS]
(
	[MATRIX_ID] NVARCHAR(36) NOT NULL, 
    [MATRIX_ROW_ID] NVARCHAR(64) NOT NULL, 
    [USER_ID] NVARCHAR(36) NOT NULL, 
    [USER_NAME] NVARCHAR(64) NULL, 
	[TENANT_CODE] NVARCHAR(36) NOT NULL DEFAULT 'D5561180-7617-4B67-B68B-1F0EA604B509'
    CONSTRAINT [PK_MATRIX_ROWS_USERS] PRIMARY KEY ([MATRIX_ID], [MATRIX_ROW_ID], [USER_ID], [TENANT_CODE])
)

GO

CREATE INDEX [IX_MATRIX_ROWS_USERS_TENANT_CODE] ON [WF].[MATRIX_ROWS_USERS] ([TENANT_CODE])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'矩阵ID',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'MATRIX_ROWS_USERS',
    @level2type = N'COLUMN',
    @level2name = N'MATRIX_ID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'矩阵的行ID',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'MATRIX_ROWS_USERS',
    @level2type = N'COLUMN',
    @level2name = N'MATRIX_ROW_ID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'人员ID',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'MATRIX_ROWS_USERS',
    @level2type = N'COLUMN',
    @level2name = N'USER_ID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'人员名称',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'MATRIX_ROWS_USERS',
    @level2type = N'COLUMN',
    @level2name = N'USER_NAME'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'矩阵行中的人员表',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'MATRIX_ROWS_USERS',
    @level2type = NULL,
    @level2name = NULL
GO

CREATE INDEX [IX_MATRIX_ROWS_USERS_USER_ID] ON [WF].[MATRIX_ROWS_USERS] ([USER_ID])
