CREATE TABLE [WF].[WORKFLOW_TASK_VERSION] (
    [TASK_ID]        NVARCHAR (36) NOT NULL,
    [OLD_START_TIME] DATETIME      NULL,
    [OLD_END_TIME]   DATETIME      NULL,
    [OLD_TIMESPAN]   FLOAT (53)    NULL,
    [PLAN_ID]        NVARCHAR (36) NOT NULL,
    CONSTRAINT [PK_WORKFLOW_TASK_VERSION_1] PRIMARY KEY CLUSTERED ([TASK_ID] ASC, [PLAN_ID] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'计划安排表', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'WORKFLOW_TASK_VERSION';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'计划ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'WORKFLOW_TASK_VERSION', @level2type = N'COLUMN', @level2name = N'TASK_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'计划开始时间', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'WORKFLOW_TASK_VERSION', @level2type = N'COLUMN', @level2name = N'OLD_START_TIME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'计划结束时间', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'WORKFLOW_TASK_VERSION', @level2type = N'COLUMN', @level2name = N'OLD_END_TIME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'任务开始时间和结束时间的跨度', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'WORKFLOW_TASK_VERSION', @level2type = N'COLUMN', @level2name = N'OLD_TIMESPAN';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'流程ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'WORKFLOW_TASK_VERSION', @level2type = N'COLUMN', @level2name = N'PLAN_ID';

