CREATE TABLE [WF].[WORKITEM_RELATED_PERSON] (
    [SORT_ID]                  INT            NOT NULL,
    [PERSON_ID]                NVARCHAR (36)  NOT NULL,
    [PERSON_NAME]              NVARCHAR (64)  NOT NULL,
    [PERSON_TYPE]              INT            NOT NULL,
    [BELONG_ORGANIZATION_ID]   NVARCHAR (36)  NULL,
    [BELONG_ORGANIZATION_NAME] NVARCHAR (128) NULL,
    [WORKITEM_ID]              NVARCHAR (36)  NOT NULL,
    CONSTRAINT [PK_WORKITEM_RELATED_PERSON] PRIMARY KEY CLUSTERED ([PERSON_ID] ASC, [PERSON_TYPE] ASC, [WORKITEM_ID] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'计划安排处理人,负责人人员表', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'WORKITEM_RELATED_PERSON';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'计划编制中人员的排序值', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'WORKITEM_RELATED_PERSON', @level2type = N'COLUMN', @level2name = N'SORT_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'人员ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'WORKITEM_RELATED_PERSON', @level2type = N'COLUMN', @level2name = N'PERSON_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'人员名称', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'WORKITEM_RELATED_PERSON', @level2type = N'COLUMN', @level2name = N'PERSON_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'人员类型(0:处理人 1:负责人 2:接收人)', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'WORKITEM_RELATED_PERSON', @level2type = N'COLUMN', @level2name = N'PERSON_TYPE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'处理人所属组织的ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'WORKITEM_RELATED_PERSON', @level2type = N'COLUMN', @level2name = N'BELONG_ORGANIZATION_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'处理人所属组织( 取其默认一项)', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'WORKITEM_RELATED_PERSON', @level2type = N'COLUMN', @level2name = N'BELONG_ORGANIZATION_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'计划安排表外键', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'WORKITEM_RELATED_PERSON', @level2type = N'COLUMN', @level2name = N'WORKITEM_ID';

