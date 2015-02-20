CREATE TABLE [WF].[JOB_SCHEDULE_DEF] (
    [SCHEDULE_ID]    NVARCHAR (64) NOT NULL,
    [SCHEDULE_NAME]  NVARCHAR (64) NULL,
	[SCHEDULE_TYPE]   INT           NULL DEFAULT 0,
    [START_TIME]     DATETIME      NULL,
    [END_TIME]       DATETIME      NULL,
    [ENABLED]        NCHAR (1)     NULL,
    [FREQUENCY_DATA] XML           NULL,
    CONSTRAINT [PK_SCHEDULE] PRIMARY KEY CLUSTERED ([SCHEDULE_ID] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'计划定义表', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'JOB_SCHEDULE_DEF';


GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'计划类型，默认是0，普通类型',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'JOB_SCHEDULE_DEF',
    @level2type = N'COLUMN',
    @level2name = 'SCHEDULE_TYPE'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'计划的ID',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'JOB_SCHEDULE_DEF',
    @level2type = N'COLUMN',
    @level2name = N'SCHEDULE_ID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'计划的名称',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'JOB_SCHEDULE_DEF',
    @level2type = N'COLUMN',
    @level2name = N'SCHEDULE_NAME'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'计划的开始时间',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'JOB_SCHEDULE_DEF',
    @level2type = N'COLUMN',
    @level2name = N'START_TIME'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'计划的结束时间',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'JOB_SCHEDULE_DEF',
    @level2type = N'COLUMN',
    @level2name = N'END_TIME'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'是否启用',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'JOB_SCHEDULE_DEF',
    @level2type = N'COLUMN',
    @level2name = N'ENABLED'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'频度定义的数据',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'JOB_SCHEDULE_DEF',
    @level2type = N'COLUMN',
    @level2name = N'FREQUENCY_DATA'