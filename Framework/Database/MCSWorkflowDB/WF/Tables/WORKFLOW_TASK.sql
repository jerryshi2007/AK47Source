CREATE TABLE [WF].[WORKFLOW_TASK] (
    [TASK_ID]                NVARCHAR (36)  NOT NULL,
    [TASK_NAME]              NVARCHAR (128) NOT NULL,
    [TASK_DESCRIPTION]       NVARCHAR (128) NULL,
    [TASK_RETURNVALUE]       NVARCHAR (128) NULL,
    [PARENT_ID]              NVARCHAR (36)  NULL,
    [SORT_ID]                INT            CONSTRAINT [DF_WORKFLOW_TASK_SORT_ID] DEFAULT ((0)) NOT NULL,
    [START_TIME]             DATETIME       NULL,
    [END_TIME]               DATETIME       NULL,
    [TIMESPANTYPE]           NCHAR (1)      NULL,
    [TIMESPAN]               FLOAT (53)     NULL,
    [PROCESS_ID]             NVARCHAR (36)  NOT NULL,
    [RESOURCE_ID]            NVARCHAR (36)  NULL,
    [TEMPLATE_KEY]           NVARCHAR (64)  NULL,
    [TEMPLATE_NAME]          NVARCHAR (128) NULL,
    [PRESENCE_STATUS]        NCHAR (1)      NULL,
    [RELATED_INDICATORS_KEY] NVARCHAR (64)  NULL,
    [ISKEY_POINT]            BIT            NULL,
    [KEY_POINT_LEVEL]        INT            NULL,
    [POSITIONREQ_ID]         NVARCHAR (36)  NULL,
    CONSTRAINT [PK_WORKFLOW_TASK] PRIMARY KEY CLUSTERED ([TASK_ID] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'计划安排表', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'WORKFLOW_TASK';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'计划ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'WORKFLOW_TASK', @level2type = N'COLUMN', @level2name = N'TASK_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'计划名称', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'WORKFLOW_TASK', @level2type = N'COLUMN', @level2name = N'TASK_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'包含当前计划的计划ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'WORKFLOW_TASK', @level2type = N'COLUMN', @level2name = N'PARENT_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'计划编制任务排序值', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'WORKFLOW_TASK', @level2type = N'COLUMN', @level2name = N'SORT_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'计划开始时间', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'WORKFLOW_TASK', @level2type = N'COLUMN', @level2name = N'START_TIME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'计划结束时间', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'WORKFLOW_TASK', @level2type = N'COLUMN', @level2name = N'END_TIME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'任务开始时间和结束时间的跨度', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'WORKFLOW_TASK', @level2type = N'COLUMN', @level2name = N'TIMESPAN';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'流程ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'WORKFLOW_TASK', @level2type = N'COLUMN', @level2name = N'PROCESS_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'资源ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'WORKFLOW_TASK', @level2type = N'COLUMN', @level2name = N'RESOURCE_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'分支流程模板的Key', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'WORKFLOW_TASK', @level2type = N'COLUMN', @level2name = N'TEMPLATE_KEY';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否下达', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'WORKFLOW_TASK', @level2type = N'COLUMN', @level2name = N'PRESENCE_STATUS';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'关联指标KEY', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'WORKFLOW_TASK', @level2type = N'COLUMN', @level2name = N'RELATED_INDICATORS_KEY';

