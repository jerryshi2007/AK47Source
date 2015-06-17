CREATE TABLE [KB].[RELATIVE_LINK_GROUP] (
    [RELATIVE_LINK_GROUP_ID] NVARCHAR (36)  NOT NULL,
    [CODE_NAME]              NVARCHAR (200) NOT NULL,
    [DESCRIPTION]            NVARCHAR (MAX) NULL,
    [ENABLE]                 NCHAR (1)      DEFAULT ('0') NULL,
    [CREATE_TIME]            DATETIME       DEFAULT (getdate()) NULL,
	[TENANT_CODE] NVARCHAR(36) NULL DEFAULT 'D5561180-7617-4B67-B68B-1F0EA604B509',
    CONSTRAINT [PK_RELATIVE_LINK_GROUP] PRIMARY KEY NONCLUSTERED ([RELATIVE_LINK_GROUP_ID] ASC)
);

GO
CREATE INDEX [IX_RELATIVE_LINK_GROUP_TENANT_CODE] ON [KB].[RELATIVE_LINK_GROUP] ([TENANT_CODE])

GO
CREATE UNIQUE CLUSTERED INDEX [IX_RELATIVE_LINK_GROUP]
    ON [KB].[RELATIVE_LINK_GROUP]([CODE_NAME] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'RELATIVE_LINK_GROUP', @level0type = N'SCHEMA', @level0name = N'KB', @level1type = N'TABLE', @level1name = N'RELATIVE_LINK_GROUP';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = '相关链接组ID', @level0type = N'SCHEMA', @level0name = N'KB', @level1type = N'TABLE', @level1name = N'RELATIVE_LINK_GROUP', @level2type = N'COLUMN', @level2name = N'RELATIVE_LINK_GROUP_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = '相关链接分类', @level0type = N'SCHEMA', @level0name = N'KB', @level1type = N'TABLE', @level1name = N'RELATIVE_LINK_GROUP', @level2type = N'COLUMN', @level2name = N'CODE_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = '描述', @level0type = N'SCHEMA', @level0name = N'KB', @level1type = N'TABLE', @level1name = N'RELATIVE_LINK_GROUP', @level2type = N'COLUMN', @level2name = N'DESCRIPTION';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = '是否可用', @level0type = N'SCHEMA', @level0name = N'KB', @level1type = N'TABLE', @level1name = N'RELATIVE_LINK_GROUP', @level2type = N'COLUMN', @level2name = N'ENABLE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = '创建时间', @level0type = N'SCHEMA', @level0name = N'KB', @level1type = N'TABLE', @level1name = N'RELATIVE_LINK_GROUP', @level2type = N'COLUMN', @level2name = N'CREATE_TIME';

