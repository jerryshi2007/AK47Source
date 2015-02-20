CREATE TABLE [WF].[ACTIVITY_TEMPLATE] (
    [ID]           NVARCHAR (64)  NOT NULL,
    [NAME]         NCHAR (64)     NULL,
    [CATEGORY]     NVARCHAR (64)  NULL,
    [CONTENT]      NVARCHAR (MAX) NULL,
    [CREATOR_ID]   NVARCHAR (36)  NULL,
    [CREATOR_NAME] NVARCHAR (64)  NULL,
    [CREATE_TIME]  DATETIME       CONSTRAINT [DF_ACTIVITY_TEMPLATE_CREATE_TIME] DEFAULT (getdate()) NULL,
    [AVAILABLE]    NCHAR (1)      CONSTRAINT [DF_ACTIVITY_TEMPLATE_AVAILABLE] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_ACTIVITY_TEMPLATE] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'活动点模板表', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'ACTIVITY_TEMPLATE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'模板ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'ACTIVITY_TEMPLATE', @level2type = N'COLUMN', @level2name = N'ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'模板名称', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'ACTIVITY_TEMPLATE', @level2type = N'COLUMN', @level2name = N'NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'模板分类', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'ACTIVITY_TEMPLATE', @level2type = N'COLUMN', @level2name = N'CATEGORY';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'模板内容', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'ACTIVITY_TEMPLATE', @level2type = N'COLUMN', @level2name = N'CONTENT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'模板创建人ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'ACTIVITY_TEMPLATE', @level2type = N'COLUMN', @level2name = N'CREATOR_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'模板创建人', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'ACTIVITY_TEMPLATE', @level2type = N'COLUMN', @level2name = N'CREATOR_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'模板创建时间', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'ACTIVITY_TEMPLATE', @level2type = N'COLUMN', @level2name = N'CREATE_TIME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'模板是否可用', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'ACTIVITY_TEMPLATE', @level2type = N'COLUMN', @level2name = N'AVAILABLE';

