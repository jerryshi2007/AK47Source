CREATE TABLE [WF].[PROCESS_RELATIVE_PARAMS] (
    [PROCESS_ID]  NVARCHAR (36)  NOT NULL,
    [PARAM_KEY]   NVARCHAR (64)  NOT NULL,
    [PARAM_VALUE] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_PROCESS_RELATIVE_PARAMS] PRIMARY KEY CLUSTERED ([PROCESS_ID] ASC, [PARAM_KEY] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'流程中与用户待办相关的参数', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'PROCESS_RELATIVE_PARAMS';


GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'流程实例的ID',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'PROCESS_RELATIVE_PARAMS',
    @level2type = N'COLUMN',
    @level2name = N'PROCESS_ID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'参数的KEY',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'PROCESS_RELATIVE_PARAMS',
    @level2type = N'COLUMN',
    @level2name = N'PARAM_KEY'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'参数的值',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'PROCESS_RELATIVE_PARAMS',
    @level2type = N'COLUMN',
    @level2name = N'PARAM_VALUE'