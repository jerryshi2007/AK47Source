CREATE TABLE [WF].[MATRIX_MAIN] (
    [MATRIX_ID]        NVARCHAR (64) NOT NULL,
    [DEF_KEY]          NVARCHAR (64) NULL,
    [PROCESS_KEY]      NVARCHAR (64) NULL,
    [ACTIVITY_KEY]     NVARCHAR (36) NULL,
    [CREATOR_ID]       NVARCHAR (36) NULL,
    [CREATOR_NAME]     NVARCHAR (64) NULL,
    [LAST_MODIFY_TIME] DATETIME      NULL,
    CONSTRAINT [PK_MATRIX_MAIN] PRIMARY KEY CLUSTERED ([MATRIX_ID] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IDX_MATRIX_MAIN_PROCESS_KEY]
    ON [WF].[MATRIX_MAIN]([PROCESS_KEY] ASC);


GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'矩阵实例表',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'MATRIX_MAIN',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'矩阵使用的唯一标识',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'MATRIX_MAIN',
    @level2type = N'COLUMN',
    @level2name = N'MATRIX_ID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'矩阵定义的KEY',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'MATRIX_MAIN',
    @level2type = N'COLUMN',
    @level2name = N'DEF_KEY'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'流程的KEY',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'MATRIX_MAIN',
    @level2type = N'COLUMN',
    @level2name = N'PROCESS_KEY'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'活动的KEY',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'MATRIX_MAIN',
    @level2type = N'COLUMN',
    @level2name = N'ACTIVITY_KEY'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'创建者的ID',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'MATRIX_MAIN',
    @level2type = N'COLUMN',
    @level2name = N'CREATOR_ID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'创建者的名称',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'MATRIX_MAIN',
    @level2type = N'COLUMN',
    @level2name = N'CREATOR_NAME'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'最后更新时间',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'MATRIX_MAIN',
    @level2type = N'COLUMN',
    @level2name = N'LAST_MODIFY_TIME'