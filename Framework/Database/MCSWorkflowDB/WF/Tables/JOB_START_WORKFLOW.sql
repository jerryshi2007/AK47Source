CREATE TABLE [WF].[JOB_START_WORKFLOW] (
    [JOB_ID]        NVARCHAR(64) NOT NULL,
    [PROCESS_KEY]   NVARCHAR(64) NULL,
    [OPERATOR_ID]   NVARCHAR(36) NULL,
    [OPERATOR_NAME] NVARCHAR(64) NULL,
	[TENANT_CODE] NVARCHAR(36) NULL DEFAULT 'D5561180-7617-4B67-B68B-1F0EA604B509'
    CONSTRAINT [PK_JOB_START_WORKFLOW] PRIMARY KEY CLUSTERED ([JOB_ID] ASC)
);

GO

CREATE INDEX [IX_JOB_START_WORKFLOW_TENANT_CODE] ON [WF].[JOB_START_WORKFLOW] ([TENANT_CODE])

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'计划任务-定时启动流程表', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'JOB_START_WORKFLOW';


GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'作业的ID',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'JOB_START_WORKFLOW',
    @level2type = N'COLUMN',
    @level2name = N'JOB_ID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'流程定义的KEY',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'JOB_START_WORKFLOW',
    @level2type = N'COLUMN',
    @level2name = N'PROCESS_KEY'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'流程启动的操作人ID',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'JOB_START_WORKFLOW',
    @level2type = N'COLUMN',
    @level2name = N'OPERATOR_ID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'流程启动的操作人名称',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'JOB_START_WORKFLOW',
    @level2type = N'COLUMN',
    @level2name = N'OPERATOR_NAME'