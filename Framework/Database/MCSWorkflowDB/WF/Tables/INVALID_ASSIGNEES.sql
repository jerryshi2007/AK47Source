CREATE TABLE [WF].[INVALID_ASSIGNEES] (
    [PROCESS_ID]          NVARCHAR (36)  NOT NULL,
    [ACTIVITY_ID]         NVARCHAR (36)  NOT NULL,
    [USER_ID]             NVARCHAR (36)  NOT NULL,
    [USER_NAME]           NVARCHAR (64)  NULL,
    [USER_PATH]           NVARCHAR (255) NULL,
    [ACTIVITY_START_TIME] DATETIME       NULL,
    [ACTIVITY_STATUS]     NVARCHAR (32)  NULL,
	[TENANT_CODE] NVARCHAR(36) NULL DEFAULT 'D5561180-7617-4B67-B68B-1F0EA604B509'
    CONSTRAINT [PK_WF.INVALID_ASSIGNEES] PRIMARY KEY CLUSTERED ([PROCESS_ID] ASC, [ACTIVITY_ID] ASC, [USER_ID] ASC)
);

GO

CREATE INDEX [IX_INVALID_ASSIGNEES_TENANT_CODE] ON [WF].[INVALID_ASSIGNEES] ([TENANT_CODE])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'流程中非法的用户（已删除）',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'INVALID_ASSIGNEES',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'流程的ID',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'INVALID_ASSIGNEES',
    @level2type = N'COLUMN',
    @level2name = N'PROCESS_ID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'活动的ID',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'INVALID_ASSIGNEES',
    @level2type = N'COLUMN',
    @level2name = N'ACTIVITY_ID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'用户ID',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'INVALID_ASSIGNEES',
    @level2type = N'COLUMN',
    @level2name = N'USER_ID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'用户名称',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'INVALID_ASSIGNEES',
    @level2type = N'COLUMN',
    @level2name = N'USER_NAME'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'活动开始时间',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'INVALID_ASSIGNEES',
    @level2type = N'COLUMN',
    @level2name = N'ACTIVITY_START_TIME'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'活动状态',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'INVALID_ASSIGNEES',
    @level2type = N'COLUMN',
    @level2name = N'ACTIVITY_STATUS'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'用户的全路径',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'INVALID_ASSIGNEES',
    @level2type = N'COLUMN',
    @level2name = N'USER_PATH'