CREATE TABLE [WF].[TASK_ASSIGNEES] (
    [ID]            NVARCHAR (36)  NOT NULL,
    [RESOURCE_ID]   NVARCHAR (36)  NULL,
    [INNER_ID]      INT            NULL,
    [TYPE]          NVARCHAR (36)  NULL,
    [ASSIGNEE_ID]   NVARCHAR (36)  NOT NULL,
    [ASSIGNEE_NAME] NVARCHAR (64)  NOT NULL,
    [URL]           NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_TASK_ASSIGNEES_1] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IDX_TASK_ASSIGNEES_RESOURCE_ID]
    ON [WF].[TASK_ASSIGNEES]([RESOURCE_ID] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'任务的分派人', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'TASK_ASSIGNEES';

