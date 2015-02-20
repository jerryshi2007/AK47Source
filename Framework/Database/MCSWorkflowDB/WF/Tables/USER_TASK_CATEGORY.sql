CREATE TABLE [WF].[USER_TASK_CATEGORY] (
    [CATEGORY_GUID] NVARCHAR (36) NOT NULL,
    [CATEGORY_NAME] NVARCHAR (64) NULL,
    [USER_ID]       NVARCHAR (36) NULL,
    [INNER_SORT_ID] INT           NULL,
    CONSTRAINT [PK_USER_TASK_CATEGORY] PRIMARY KEY CLUSTERED ([CATEGORY_GUID] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'待办箱处理夹', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'USER_TASK_CATEGORY';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'类别ID，唯一标识', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'USER_TASK_CATEGORY', @level2type = N'COLUMN', @level2name = N'CATEGORY_GUID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'类别名称', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'USER_TASK_CATEGORY', @level2type = N'COLUMN', @level2name = N'CATEGORY_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'用户ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'USER_TASK_CATEGORY', @level2type = N'COLUMN', @level2name = N'USER_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'类别排序', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'USER_TASK_CATEGORY', @level2type = N'COLUMN', @level2name = N'INNER_SORT_ID';

