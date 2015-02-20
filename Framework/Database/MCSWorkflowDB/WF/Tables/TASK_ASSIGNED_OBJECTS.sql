CREATE TABLE [WF].[TASK_ASSIGNED_OBJECTS] (
    [ID]            NVARCHAR (36)  NOT NULL,
    [RESOURCE_ID]   NVARCHAR (36)  NULL,
    [INNER_ID]      INT            NULL,
    [TYPE]          NVARCHAR (36)  NULL,
    [ASSIGNEE_TYPE] INT            NOT NULL,
    [ASSIGNEE_ID]   NVARCHAR (36)  NOT NULL,
    [ASSIGNEE_NAME] NVARCHAR (64)  NOT NULL,
    [URL]           NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_TASK_ASSIGNED_OBJECTS_1] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IDX_TASK_ASSIGNED_OBJECTS_RESOURCE_ID]
    ON [WF].[TASK_ASSIGNED_OBJECTS]([RESOURCE_ID] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'任务分派的对象集合', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'TASK_ASSIGNED_OBJECTS';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'分派人的类型：1，组织；2，人员；3，组', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'TASK_ASSIGNED_OBJECTS', @level2type = N'COLUMN', @level2name = N'ASSIGNEE_TYPE';

