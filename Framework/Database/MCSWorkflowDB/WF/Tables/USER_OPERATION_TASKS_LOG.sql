CREATE TABLE [WF].[USER_OPERATION_TASKS_LOG] (
    [LOG_ID]            BIGINT           NOT NULL,
    [TASK_ID]           NVARCHAR (36) NOT NULL,
    [SEND_TO_USER_ID]   NVARCHAR (36) NOT NULL,
    [SEND_TO_USER_NAME] NVARCHAR (64) NULL,
    CONSTRAINT [PK_UserPassword] PRIMARY KEY CLUSTERED ([LOG_ID] ASC, [TASK_ID] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'日志的ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'USER_OPERATION_TASKS_LOG', @level2type = N'COLUMN', @level2name = N'LOG_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'对应的待办的ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'USER_OPERATION_TASKS_LOG', @level2type = N'COLUMN', @level2name = N'TASK_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'待办收件人的ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'USER_OPERATION_TASKS_LOG', @level2type = N'COLUMN', @level2name = N'SEND_TO_USER_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'待办收件人的名称', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'USER_OPERATION_TASKS_LOG', @level2type = N'COLUMN', @level2name = N'SEND_TO_USER_NAME';

