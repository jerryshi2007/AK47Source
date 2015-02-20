CREATE TABLE [WF].[PROCESS_CURRENT_ACTIVITIES] (
    [PROCESS_ID]        NVARCHAR (36)  NOT NULL,
    [ACTIVITY_ID]       NVARCHAR (36)  NOT NULL,
    [ACTIVITY_DESC_KEY] NVARCHAR (64)  NULL,
    [ACTIVITY_TYPE]     INT            CONSTRAINT [DF_PROCESS_CURRENT_ACTIVITIES_ACTIVITY_TYPE] DEFAULT ((0)) NULL,
    [ACTIVITY_NAME]     NVARCHAR (64)  NULL,
    [STATUS]            NVARCHAR (32)  CONSTRAINT [DF_PROCESS_CURRENT_ACTIVITIES_STATUS] DEFAULT (N'NotRunning') NULL,
    [OPERATOR_ID]       NVARCHAR (36)  NULL,
    [OPERATOR_NAME]     NVARCHAR (64)  NULL,
    [OPERATOR_PATH]     NVARCHAR (512) NULL,
    [START_TIME]        DATETIME       NULL,
    [END_TIME]          DATETIME       NULL,
    CONSTRAINT [PK_PROCESS_CURRENT_ACTIVITIES] PRIMARY KEY CLUSTERED ([PROCESS_ID] ASC, [ACTIVITY_ID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_PROCESS_CURRENT_ACTIVITIES_ACTIVITY_ID]
    ON [WF].[PROCESS_CURRENT_ACTIVITIES]([ACTIVITY_ID] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'流程上已运行的活动实体类', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'PROCESS_CURRENT_ACTIVITIES';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'流程ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'PROCESS_CURRENT_ACTIVITIES', @level2type = N'COLUMN', @level2name = N'PROCESS_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'活动节点ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'PROCESS_CURRENT_ACTIVITIES', @level2type = N'COLUMN', @level2name = N'ACTIVITY_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'活动描述的 KEY', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'PROCESS_CURRENT_ACTIVITIES', @level2type = N'COLUMN', @level2name = N'ACTIVITY_DESC_KEY';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'活动节点类型 即0为NormalActivity；1为InitialActivity；4为CompletedActivity', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'PROCESS_CURRENT_ACTIVITIES', @level2type = N'COLUMN', @level2name = N'ACTIVITY_TYPE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'节点名称', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'PROCESS_CURRENT_ACTIVITIES', @level2type = N'COLUMN', @level2name = N'ACTIVITY_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'节点状态 即未运行；运行中；等待中；已完成；被终止', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'PROCESS_CURRENT_ACTIVITIES', @level2type = N'COLUMN', @level2name = N'STATUS';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'操作人ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'PROCESS_CURRENT_ACTIVITIES', @level2type = N'COLUMN', @level2name = N'OPERATOR_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'操作人名称', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'PROCESS_CURRENT_ACTIVITIES', @level2type = N'COLUMN', @level2name = N'OPERATOR_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'操作人PATH', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'PROCESS_CURRENT_ACTIVITIES', @level2type = N'COLUMN', @level2name = N'OPERATOR_PATH';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'开始时间', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'PROCESS_CURRENT_ACTIVITIES', @level2type = N'COLUMN', @level2name = N'START_TIME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'结束时间', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'PROCESS_CURRENT_ACTIVITIES', @level2type = N'COLUMN', @level2name = N'END_TIME';

