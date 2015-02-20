CREATE TABLE [WF].[GENERIC_FORM_DATA] (
    [RESOURCE_ID]    NVARCHAR (36)  NOT NULL,
    [SUBJECT]        NVARCHAR (255)  NULL,
    [CREATOR_ID]     NVARCHAR (36)  NOT NULL,
    [CREATOR_NAME]   NVARCHAR (64)  NOT NULL,
    [CREATE_TIME]    DATETIME       CONSTRAINT [DF_FORM_DATA_CREATE_TIME] DEFAULT (getdate()) NOT NULL,
    [XML_CONTENT]    XML            NOT NULL,
    [SEARCH_CONTENT] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_GENERIC_FORM_DATA] PRIMARY KEY CLUSTERED ([RESOURCE_ID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IDX_GENERIC_FORM_DATA_CREATE_TIME]
    ON [WF].[GENERIC_FORM_DATA]([CREATE_TIME] ASC);


GO
CREATE NONCLUSTERED INDEX [IDX_GENERIC_FORM_DATA_CREATOR]
    ON [WF].[GENERIC_FORM_DATA]([CREATOR_ID] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'通用表单数据', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'GENERIC_FORM_DATA';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'表单ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'GENERIC_FORM_DATA', @level2type = N'COLUMN', @level2name = N'RESOURCE_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'拟制人ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'GENERIC_FORM_DATA', @level2type = N'COLUMN', @level2name = N'CREATOR_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'创建人姓名', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'GENERIC_FORM_DATA', @level2type = N'COLUMN', @level2name = N'CREATOR_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'创建时间', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'GENERIC_FORM_DATA', @level2type = N'COLUMN', @level2name = N'CREATE_TIME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Xml内容', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'GENERIC_FORM_DATA', @level2type = N'COLUMN', @level2name = N'XML_CONTENT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'全文检索字段', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'GENERIC_FORM_DATA', @level2type = N'COLUMN', @level2name = N'SEARCH_CONTENT';


GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'表单标题',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'GENERIC_FORM_DATA',
    @level2type = N'COLUMN',
    @level2name = N'SUBJECT'