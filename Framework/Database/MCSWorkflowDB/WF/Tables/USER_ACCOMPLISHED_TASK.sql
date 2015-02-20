CREATE TABLE [WF].[USER_ACCOMPLISHED_TASK] (
    [TASK_GUID]             NVARCHAR (36)   NOT NULL,
    [APPLICATION_NAME]      NVARCHAR (64)   NULL,
    [PROGRAM_NAME]          NVARCHAR (64)   NULL,
    [TASK_LEVEL]            INT             NULL,
    [TASK_TITLE]            NVARCHAR (1024) NULL,
    [RESOURCE_ID]           NVARCHAR (36)   NULL,
    [PROCESS_ID]            NVARCHAR (36)   NULL,
    [ACTIVITY_ID]           NVARCHAR (36)   NULL,
    [URL]                   NVARCHAR (2048) NULL,
    [DATA]                  NVARCHAR (2048) NULL,
    [EMERGENCY]             INT             NULL,
    [PURPOSE]               NVARCHAR (64)   NULL,
    [STATUS]                NCHAR (1)       NULL,
    [TASK_START_TIME]       DATETIME        CONSTRAINT [DF_USER_ACCOMPLISHED_TASK_TASK_START_TIME] DEFAULT (getdate()) NULL,
    [EXPIRE_TIME]           DATETIME        NULL,
    [SOURCE_ID]             NVARCHAR (36)   NULL,
    [SOURCE_NAME]           NVARCHAR (64)   NULL,
    [SEND_TO_USER]          NVARCHAR (36)   NULL,
    [SEND_TO_USER_NAME]     NVARCHAR (64)   NULL,
    [READ_TIME]             DATETIME        NULL,
    [COMPLETED_TIME]        DATETIME        CONSTRAINT [DF_USER_ACCOMPLISHED_TASK_COMPLETED_TIME] DEFAULT (getdate()) NULL,
    [DRAFT_DEPARTMENT_NAME] NVARCHAR (512)  NULL,
    [DELIVER_TIME]          DATETIME        CONSTRAINT [DF_USER_ACCOMPLISHED_TASK_DELIVER_TIME] DEFAULT (getdate()) NULL,
    [DRAFT_USER_ID]         NVARCHAR (36)   NULL,
    [DRAFT_USER_NAME]       NVARCHAR (64)   NULL,
	[TENANT_CODE] NVARCHAR(36) NULL DEFAULT 'D5561180-7617-4B67-B68B-1F0EA604B509', 
    CONSTRAINT [PK_USER_ACCOMPLISHED_TASK] PRIMARY KEY CLUSTERED ([TASK_GUID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_USER_ACCOMPLISHED_TASK_RESOURCE_ID]
    ON [WF].[USER_ACCOMPLISHED_TASK]([RESOURCE_ID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_USER_ACCOMPLISHED_TASK_SEND_TO_USER]
    ON [WF].[USER_ACCOMPLISHED_TASK]([SEND_TO_USER] ASC, [TENANT_CODE] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'已办箱实体类', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'USER_ACCOMPLISHED_TASK';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'消息ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'USER_ACCOMPLISHED_TASK', @level2type = N'COLUMN', @level2name = N'TASK_GUID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'应用名称', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'USER_ACCOMPLISHED_TASK', @level2type = N'COLUMN', @level2name = N'APPLICATION_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'模块名称', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'USER_ACCOMPLISHED_TASK', @level2type = N'COLUMN', @level2name = N'PROGRAM_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'任务级别，即消息类型 则0为None;1为VeryLow;2为提醒消息;3为阅知消息;4为办理消息;5为VeryHigh', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'USER_ACCOMPLISHED_TASK', @level2type = N'COLUMN', @level2name = N'TASK_LEVEL';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'题目', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'USER_ACCOMPLISHED_TASK', @level2type = N'COLUMN', @level2name = N'TASK_TITLE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'资源ID，即基于应用上划分的某件事情为资源的ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'USER_ACCOMPLISHED_TASK', @level2type = N'COLUMN', @level2name = N'RESOURCE_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'与任务关联的流程ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'USER_ACCOMPLISHED_TASK', @level2type = N'COLUMN', @level2name = N'PROCESS_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'流程当前活动节点的ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'USER_ACCOMPLISHED_TASK', @level2type = N'COLUMN', @level2name = N'ACTIVITY_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'已办事项的链接', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'USER_ACCOMPLISHED_TASK', @level2type = N'COLUMN', @level2name = N'URL';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'将来发消息时的附加消息，非结构化', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'USER_ACCOMPLISHED_TASK', @level2type = N'COLUMN', @level2name = N'DATA';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'紧急程度  则0为None;1为急件；2为平急；3为加急；4为特急；5为特提；', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'USER_ACCOMPLISHED_TASK', @level2type = N'COLUMN', @level2name = N'EMERGENCY';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'发待办时目的描述，从表单上过来的.即非标题性质的描述', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'USER_ACCOMPLISHED_TASK', @level2type = N'COLUMN', @level2name = N'PURPOSE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'待办事项状态,即1为待办事项；2为待阅事项', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'USER_ACCOMPLISHED_TASK', @level2type = N'COLUMN', @level2name = N'STATUS';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'任务开始时间', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'USER_ACCOMPLISHED_TASK', @level2type = N'COLUMN', @level2name = N'TASK_START_TIME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'过期时间', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'USER_ACCOMPLISHED_TASK', @level2type = N'COLUMN', @level2name = N'EXPIRE_TIME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'发送人ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'USER_ACCOMPLISHED_TASK', @level2type = N'COLUMN', @level2name = N'SOURCE_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'发送人的名称', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'USER_ACCOMPLISHED_TASK', @level2type = N'COLUMN', @level2name = N'SOURCE_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'发送到某人，即接收人的ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'USER_ACCOMPLISHED_TASK', @level2type = N'COLUMN', @level2name = N'SEND_TO_USER';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'接收人的名称', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'USER_ACCOMPLISHED_TASK', @level2type = N'COLUMN', @level2name = N'SEND_TO_USER_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'阅读的时间', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'USER_ACCOMPLISHED_TASK', @level2type = N'COLUMN', @level2name = N'READ_TIME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'完成的时间', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'USER_ACCOMPLISHED_TASK', @level2type = N'COLUMN', @level2name = N'COMPLETED_TIME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'起草的部门名称', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'USER_ACCOMPLISHED_TASK', @level2type = N'COLUMN', @level2name = N'DRAFT_DEPARTMENT_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'起草的时间', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'USER_ACCOMPLISHED_TASK', @level2type = N'COLUMN', @level2name = N'DELIVER_TIME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'起草人的ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'USER_ACCOMPLISHED_TASK', @level2type = N'COLUMN', @level2name = N'DRAFT_USER_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'起草人的名称', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'USER_ACCOMPLISHED_TASK', @level2type = N'COLUMN', @level2name = N'DRAFT_USER_NAME';


GO

CREATE INDEX [IX_USER_ACCOMPLISHED_TASK_ACTIVITY_ID] ON [WF].[USER_ACCOMPLISHED_TASK] ([ACTIVITY_ID])

GO

CREATE INDEX [IX_USER_ACCOMPLISHED_TASK_PROCESS_ID] ON [WF].[USER_ACCOMPLISHED_TASK] ([PROCESS_ID], [SEND_TO_USER])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'租户代码',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'USER_ACCOMPLISHED_TASK',
    @level2type = N'COLUMN',
    @level2name = N'TENANT_CODE'
GO

CREATE INDEX [IX_USER_ACCOMPLISHED_TASK_TENANT_CODE] ON [WF].[USER_ACCOMPLISHED_TASK] ([TENANT_CODE])
