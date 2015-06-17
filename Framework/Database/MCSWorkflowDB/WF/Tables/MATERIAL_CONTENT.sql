CREATE TABLE [WF].[MATERIAL_CONTENT] (
    [CONTENT_ID]   NVARCHAR (36)   NOT NULL,
    [RELATIVE_ID]  NVARCHAR (36)   NULL,
    [CLASS]        NVARCHAR (255)   NULL,
    [FILE_NAME]    NVARCHAR (255)  NULL,
    [FILE_SIZE]    BIGINT          CONSTRAINT [DF_MATERIAL_CONTENT_FILE_SIZE] DEFAULT ((0)) NULL,
    [CREATOR_ID]   NVARCHAR (36)   NULL,
    [CREATOR_NAME] NVARCHAR (64)   NULL,
    [CREATE_TIME]  DATETIME        CONSTRAINT [DF_MATERIAL_CONTENT_CREATE_TIME] DEFAULT (getdate()) NULL,
    [UPDATE_TIME]  DATETIME        CONSTRAINT [DF_MATERIAL_CONTENT_UPDATE_TIME] DEFAULT (getdate()) NULL,
    [CONTENT_DATA] VARBINARY (MAX) NULL,
	[TENANT_CODE] NVARCHAR(36) NULL DEFAULT 'D5561180-7617-4B67-B68B-1F0EA604B509'
    CONSTRAINT [PK_MATERIAL_CONTENT] PRIMARY KEY CLUSTERED ([CONTENT_ID] ASC)
);

GO

CREATE INDEX [IX_MATERIAL_CONTENT_TENANT_CODE] ON [WF].[MATERIAL_CONTENT] ([TENANT_CODE])

GO
CREATE NONCLUSTERED INDEX [IX_MATERIAL_CONTENT_RELATIVE_ID]
    ON [WF].[MATERIAL_CONTENT]([RELATIVE_ID] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'附件内容表', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'MATERIAL_CONTENT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'附件内容的ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'MATERIAL_CONTENT', @level2type = N'COLUMN', @level2name = N'CONTENT_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'内容相关的ID，例如表单的RESOURCE_ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'MATERIAL_CONTENT', @level2type = N'COLUMN', @level2name = N'RELATIVE_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'类别', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'MATERIAL_CONTENT', @level2type = N'COLUMN', @level2name = N'CLASS';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'原始文件名', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'MATERIAL_CONTENT', @level2type = N'COLUMN', @level2name = N'FILE_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'原始文件尺寸', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'MATERIAL_CONTENT', @level2type = N'COLUMN', @level2name = N'FILE_SIZE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'创建人的ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'MATERIAL_CONTENT', @level2type = N'COLUMN', @level2name = N'CREATOR_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'创建人的名称', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'MATERIAL_CONTENT', @level2type = N'COLUMN', @level2name = N'CREATOR_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'创建时间', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'MATERIAL_CONTENT', @level2type = N'COLUMN', @level2name = N'CREATE_TIME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改时间', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'MATERIAL_CONTENT', @level2type = N'COLUMN', @level2name = N'UPDATE_TIME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'内容', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'MATERIAL_CONTENT', @level2type = N'COLUMN', @level2name = N'CONTENT_DATA';

