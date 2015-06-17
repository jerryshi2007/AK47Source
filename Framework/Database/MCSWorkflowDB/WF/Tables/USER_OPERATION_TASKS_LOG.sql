CREATE TABLE [WF].[USER_OPERATION_TASKS_LOG] (
    [LOG_ID]            BIGINT           NOT NULL,
    [TASK_ID]           NVARCHAR (36) NOT NULL,
    [SEND_TO_USER_ID]   NVARCHAR (36) NOT NULL,
    [SEND_TO_USER_NAME] NVARCHAR (64) NULL,
	[TENANT_CODE] NVARCHAR(36) NULL DEFAULT 'D5561180-7617-4B67-B68B-1F0EA604B509'
    CONSTRAINT [PK_UserPassword] PRIMARY KEY CLUSTERED ([LOG_ID] ASC, [TASK_ID] ASC)
);

GO

CREATE INDEX [IX_USER_OPERATION_TASKS_LOG_TENANT_CODE] ON [WF].[USER_OPERATION_TASKS_LOG] ([TENANT_CODE])

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'日志的ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'USER_OPERATION_TASKS_LOG', @level2type = N'COLUMN', @level2name = N'LOG_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'对应的待办的ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'USER_OPERATION_TASKS_LOG', @level2type = N'COLUMN', @level2name = N'TASK_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'待办收件人的ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'USER_OPERATION_TASKS_LOG', @level2type = N'COLUMN', @level2name = N'SEND_TO_USER_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'待办收件人的名称', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'USER_OPERATION_TASKS_LOG', @level2type = N'COLUMN', @level2name = N'SEND_TO_USER_NAME';

