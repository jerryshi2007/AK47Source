CREATE TABLE [WF].[JOB_SCHEDULES] (
    [JOB_ID]      NVARCHAR (64) NOT NULL,
    [SCHEDULE_ID] NVARCHAR (64) NOT NULL,
	[TENANT_CODE] NVARCHAR(36) NULL DEFAULT 'D5561180-7617-4B67-B68B-1F0EA604B509'
    CONSTRAINT [PK_JOB_SCHEDULES] PRIMARY KEY CLUSTERED ([JOB_ID] ASC, [SCHEDULE_ID] ASC)
);

GO

CREATE INDEX [IX_JOB_SCHEDULES_TENANT_CODE] ON [WF].[JOB_SCHEDULES] ([TENANT_CODE])

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'任务与计划对应关系表', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'JOB_SCHEDULES';


GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'作业的ID',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'JOB_SCHEDULES',
    @level2type = N'COLUMN',
    @level2name = N'JOB_ID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'计划的ID',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'JOB_SCHEDULES',
    @level2type = N'COLUMN',
    @level2name = N'SCHEDULE_ID'