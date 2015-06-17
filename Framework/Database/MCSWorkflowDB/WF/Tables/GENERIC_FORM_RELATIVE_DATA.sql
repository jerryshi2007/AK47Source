CREATE TABLE [WF].[GENERIC_FORM_RELATIVE_DATA] (
    [RESOURCE_ID]    NVARCHAR (36)  NOT NULL,
    [CLASS]          NVARCHAR (64)  NOT NULL,
    [SORT_ID]        INT            NOT NULL,
    [XML_CONTENT]    XML            NULL,
    [SEARCH_CONTENT] NVARCHAR (MAX) NULL,
    [ROW_ID]         NVARCHAR (36)  CONSTRAINT [DF_GENERIC_FORM_RELATIVE_DATA_ROW_ID] DEFAULT (newid()) NOT NULL,
	[TENANT_CODE] NVARCHAR(36) NULL DEFAULT 'D5561180-7617-4B67-B68B-1F0EA604B509'
    CONSTRAINT [PK_GENERIC_FORM_RELATIVE_DATA] PRIMARY KEY CLUSTERED ([RESOURCE_ID] ASC, [CLASS] ASC, [SORT_ID] ASC)
);

GO

CREATE INDEX [IX_GENERIC_FORM_RELATIVE_DATA_TENANT_CODE] ON [WF].[GENERIC_FORM_RELATIVE_DATA] ([TENANT_CODE])

GO
CREATE UNIQUE NONCLUSTERED INDEX [GENERIC_FORM_RELATIVE_DATA_ROW_ID]
    ON [WF].[GENERIC_FORM_RELATIVE_DATA]([ROW_ID] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'通用表单数据的相关数据', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'GENERIC_FORM_RELATIVE_DATA';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'对应的通用表单的ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'GENERIC_FORM_RELATIVE_DATA', @level2type = N'COLUMN', @level2name = N'RESOURCE_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'类别', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'GENERIC_FORM_RELATIVE_DATA', @level2type = N'COLUMN', @level2name = N'CLASS';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'类别下的小序号', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'GENERIC_FORM_RELATIVE_DATA', @level2type = N'COLUMN', @level2name = N'SORT_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'数据项的内容', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'GENERIC_FORM_RELATIVE_DATA', @level2type = N'COLUMN', @level2name = N'XML_CONTENT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'全文检索字段', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'GENERIC_FORM_RELATIVE_DATA', @level2type = N'COLUMN', @level2name = N'SEARCH_CONTENT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'唯一ID，专门为了创建全文检索', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'GENERIC_FORM_RELATIVE_DATA', @level2type = N'COLUMN', @level2name = N'ROW_ID';

