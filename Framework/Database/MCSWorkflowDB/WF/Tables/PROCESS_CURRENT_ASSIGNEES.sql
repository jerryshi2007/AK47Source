CREATE TABLE [WF].[PROCESS_CURRENT_ASSIGNEES] (
    [PROCESS_ID]    NVARCHAR (36)  NULL,
    [ACTIVITY_ID]   NVARCHAR (36)  NOT NULL,
    [USER_PATH]     NVARCHAR (412) NULL,
    [ASSIGNEE_TYPE] INT            CONSTRAINT [DF_PROCESS_CURRENT_ASSIGNEES_ASSIGNEE_TYPE] DEFAULT ((0)) NOT NULL,
    [USER_ID]       NVARCHAR (36)  NULL,
    [USER_NAME]     NVARCHAR (64)  NULL,
    [ID]            INT            IDENTITY (1, 1) NOT NULL,
    [URL]           NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_PROCESS_CURRENT_ASSIGNEES] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [INDEX_PROCESSASSIGNEES]
    ON [WF].[PROCESS_CURRENT_ASSIGNEES]([PROCESS_ID] ASC);


GO
CREATE NONCLUSTERED INDEX [INDEX_PROCESSASSIGNEES_USER_PATH]
    ON [WF].[PROCESS_CURRENT_ASSIGNEES]([ACTIVITY_ID] ASC, [USER_PATH] ASC, [ASSIGNEE_TYPE] ASC);


GO
CREATE NONCLUSTERED INDEX [INDEX_PROCESSASSIGNEES_USER_ID]
    ON [WF].[PROCESS_CURRENT_ASSIGNEES]([USER_ID] ASC)
    INCLUDE([PROCESS_ID], [ACTIVITY_ID]);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'流程当前活动点的执行人', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'PROCESS_CURRENT_ASSIGNEES';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'流程ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'PROCESS_CURRENT_ASSIGNEES', @level2type = N'COLUMN', @level2name = N'PROCESS_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'流程的当前活动点ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'PROCESS_CURRENT_ASSIGNEES', @level2type = N'COLUMN', @level2name = N'ACTIVITY_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'用户的PATH，相应于人员机构系统', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'PROCESS_CURRENT_ASSIGNEES', @level2type = N'COLUMN', @level2name = N'USER_PATH';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'受托类型  即Normal；Delegated', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'PROCESS_CURRENT_ASSIGNEES', @level2type = N'COLUMN', @level2name = N'ASSIGNEE_TYPE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'用户ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'PROCESS_CURRENT_ASSIGNEES', @level2type = N'COLUMN', @level2name = N'USER_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'用户名称  即当前活动点的执行人', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'PROCESS_CURRENT_ASSIGNEES', @level2type = N'COLUMN', @level2name = N'USER_NAME';


GO

CREATE INDEX [INDEX_PROCESSASSIGNEES_UserName] ON [WF].[PROCESS_CURRENT_ASSIGNEES] ([USER_NAME])
