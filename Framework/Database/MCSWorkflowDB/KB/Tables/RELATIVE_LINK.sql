CREATE TABLE [KB].[RELATIVE_LINK] (
    [RELATIVE_LINK_ID]              NVARCHAR (36)  NOT NULL,
    [RELATIVE_LINK_GROUP_ID]        NVARCHAR (36)  NULL,
    [CODE_NAME]                     NVARCHAR (200) NOT NULL,
    [RELATIVE_LINK_GROUP_CODE_NAME] NVARCHAR (500) NULL,
    [LINK]                          NVARCHAR (MAX) NULL,
    [ENABLE]                        NCHAR (1)      DEFAULT ('0') NULL,
    [CREATE_TIME]                   DATETIME       DEFAULT (getdate()) NULL,
	[TENANT_CODE] NVARCHAR(36) NULL DEFAULT 'D5561180-7617-4B67-B68B-1F0EA604B509',
    CONSTRAINT [PK_RELATIVE_LINK] PRIMARY KEY NONCLUSTERED ([RELATIVE_LINK_ID] ASC)
);

GO
CREATE INDEX [IX_RELATIVE_LINK_TENANT_CODE] ON [KB].[RELATIVE_LINK] ([TENANT_CODE])

GO
CREATE UNIQUE CLUSTERED INDEX [IX_RELATIVE_LINK]
    ON [KB].[RELATIVE_LINK]([CODE_NAME] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = '相关链接', @level0type = N'SCHEMA', @level0name = N'KB', @level1type = N'TABLE', @level1name = N'RELATIVE_LINK';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = '相关链接ID', @level0type = N'SCHEMA', @level0name = N'KB', @level1type = N'TABLE', @level1name = N'RELATIVE_LINK', @level2type = N'COLUMN', @level2name = N'RELATIVE_LINK_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = '相关链接组ID', @level0type = N'SCHEMA', @level0name = N'KB', @level1type = N'TABLE', @level1name = N'RELATIVE_LINK', @level2type = N'COLUMN', @level2name = N'RELATIVE_LINK_GROUP_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = '相关链接的名称', @level0type = N'SCHEMA', @level0name = N'KB', @level1type = N'TABLE', @level1name = N'RELATIVE_LINK', @level2type = N'COLUMN', @level2name = N'CODE_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'RELATIVE_LINK_GROUP_CODE_NAME', @level0type = N'SCHEMA', @level0name = N'KB', @level1type = N'TABLE', @level1name = N'RELATIVE_LINK', @level2type = N'COLUMN', @level2name = N'RELATIVE_LINK_GROUP_CODE_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = '链接', @level0type = N'SCHEMA', @level0name = N'KB', @level1type = N'TABLE', @level1name = N'RELATIVE_LINK', @level2type = N'COLUMN', @level2name = N'LINK';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = '是否可用', @level0type = N'SCHEMA', @level0name = N'KB', @level1type = N'TABLE', @level1name = N'RELATIVE_LINK', @level2type = N'COLUMN', @level2name = N'ENABLE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = '创建时间', @level0type = N'SCHEMA', @level0name = N'KB', @level1type = N'TABLE', @level1name = N'RELATIVE_LINK', @level2type = N'COLUMN', @level2name = N'CREATE_TIME';

