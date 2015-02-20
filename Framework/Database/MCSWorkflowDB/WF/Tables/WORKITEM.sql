CREATE TABLE [WF].[WORKITEM] (
    [WORKITEM_ID]            NVARCHAR (36)  NOT NULL,
    [PARENT_ID]              NVARCHAR (36)  NULL,
    [SORT_ID]                INT            CONSTRAINT [DF_WORKITEM_SORT_ID] DEFAULT ((0)) NOT NULL,
    [POSITIONREQ_ID]         NVARCHAR (36)  NULL,
    [GROUP_ID]               NVARCHAR (36)  NULL,
    [ENABLED_EDIT]           NCHAR (1)      NOT NULL,
    [WORKITEM_NAME]          NVARCHAR (128) NOT NULL,
    [START_TIME]             DATETIME       NULL,
    [END_TIME]               DATETIME       NULL,
    [TIMESPAN]               FLOAT (53)     NULL,
    [PROCESS_ID]             NVARCHAR (36)  NOT NULL,
    [RESOURCE_ID]            NVARCHAR (36)  NOT NULL,
    [PROCESS_KEY]            NVARCHAR (64)  NOT NULL,
    [ACTIVITY_KEY]           NVARCHAR (64)  NOT NULL,
    [TEMPLATE_KEY]           NVARCHAR (64)  NULL,
    [TEMPLATE_NAME]          NVARCHAR (128) NULL,
    [PROJECT_NAME]           NVARCHAR (64)  NULL,
    [URL]                    NVARCHAR (128) NULL,
    [STATUS]                 NVARCHAR (12)  NULL,
    [ISWORKITEM]             NCHAR (1)      NULL,
    [ACTUALEND_TIME]         DATETIME       NULL,
    [PRESENCE_STATUS]        NCHAR (1)      NULL,
    [SUBNODE_RUNTYPE]        INT            NULL,
    [RELATED_INDICATORS_KEY] NVARCHAR (64)  NULL,
    CONSTRAINT [PK_WORKITEM] PRIMARY KEY CLUSTERED ([WORKITEM_ID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IDX_WORK_ITEM_PROCESS_ID]
    ON [WF].[WORKITEM]([PROCESS_ID] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'计划安排表', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'WORKITEM';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'计划ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'WORKITEM', @level2type = N'COLUMN', @level2name = N'WORKITEM_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'包含当前计划的计划ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'WORKITEM', @level2type = N'COLUMN', @level2name = N'PARENT_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'计划编制任务排序值', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'WORKITEM', @level2type = N'COLUMN', @level2name = N'SORT_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'岗位需求ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'WORKITEM', @level2type = N'COLUMN', @level2name = N'POSITIONREQ_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'所属群组ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'WORKITEM', @level2type = N'COLUMN', @level2name = N'GROUP_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否可编辑(0:否，1：是)', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'WORKITEM', @level2type = N'COLUMN', @level2name = N'ENABLED_EDIT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'计划名称', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'WORKITEM', @level2type = N'COLUMN', @level2name = N'WORKITEM_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'计划开始时间', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'WORKITEM', @level2type = N'COLUMN', @level2name = N'START_TIME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'计划结束时间', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'WORKITEM', @level2type = N'COLUMN', @level2name = N'END_TIME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'任务开始时间和结束时间的跨度', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'WORKITEM', @level2type = N'COLUMN', @level2name = N'TIMESPAN';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'流程ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'WORKITEM', @level2type = N'COLUMN', @level2name = N'PROCESS_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'资源ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'WORKITEM', @level2type = N'COLUMN', @level2name = N'RESOURCE_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'流程描述KEY', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'WORKITEM', @level2type = N'COLUMN', @level2name = N'PROCESS_KEY';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'活动描述KEY', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'WORKITEM', @level2type = N'COLUMN', @level2name = N'ACTIVITY_KEY';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'分支流程模板的Key', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'WORKITEM', @level2type = N'COLUMN', @level2name = N'TEMPLATE_KEY';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'项目名称', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'WORKITEM', @level2type = N'COLUMN', @level2name = N'PROJECT_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'表单地址', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'WORKITEM', @level2type = N'COLUMN', @level2name = N'URL';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'流程状态', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'WORKITEM', @level2type = N'COLUMN', @level2name = N'STATUS';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否工作项', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'WORKITEM', @level2type = N'COLUMN', @level2name = N'ISWORKITEM';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'实际完成时间', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'WORKITEM', @level2type = N'COLUMN', @level2name = N'ACTUALEND_TIME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否下达', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'WORKITEM', @level2type = N'COLUMN', @level2name = N'PRESENCE_STATUS';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'子节点运行类型（1：串行；2：并行）', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'WORKITEM', @level2type = N'COLUMN', @level2name = N'SUBNODE_RUNTYPE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'关联指标KEY', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'WORKITEM', @level2type = N'COLUMN', @level2name = N'RELATED_INDICATORS_KEY';

