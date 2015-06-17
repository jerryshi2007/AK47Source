CREATE TABLE [KB].[TIP] (
    [TIP_ID]      NVARCHAR (36)  NOT NULL,
    [CODE_NAME]   NVARCHAR (200) NOT NULL,
    [CULTURE]     NVARCHAR (50)  NOT NULL,
    [CONTENT]     NVARCHAR (MAX) NULL,
    [ENABLE]      NCHAR (1)      DEFAULT ('0') NULL,
    [CREATE_TIME] DATETIME       DEFAULT (getdate()) NULL,
	[TENANT_CODE] NVARCHAR(36) NULL DEFAULT 'D5561180-7617-4B67-B68B-1F0EA604B509',
    CONSTRAINT [PK_TIP] PRIMARY KEY NONCLUSTERED ([TIP_ID] ASC)
);

GO
CREATE INDEX [IX_TIP_TENANT_CODE] ON [KB].[TIP] ([TENANT_CODE])

GO
CREATE UNIQUE CLUSTERED INDEX [IX_TIP]
    ON [KB].[TIP]([CODE_NAME] ASC, [CULTURE] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = '提示表', @level0type = N'SCHEMA', @level0name = N'KB', @level1type = N'TABLE', @level1name = N'TIP';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = '提示ID', @level0type = N'SCHEMA', @level0name = N'KB', @level1type = N'TABLE', @level1name = N'TIP', @level2type = N'COLUMN', @level2name = N'TIP_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = '提示的名称', @level0type = N'SCHEMA', @level0name = N'KB', @level1type = N'TABLE', @level1name = N'TIP', @level2type = N'COLUMN', @level2name = N'CODE_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = '区域', @level0type = N'SCHEMA', @level0name = N'KB', @level1type = N'TABLE', @level1name = N'TIP', @level2type = N'COLUMN', @level2name = N'CULTURE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = 'TIP的内容', @level0type = N'SCHEMA', @level0name = N'KB', @level1type = N'TABLE', @level1name = N'TIP', @level2type = N'COLUMN', @level2name = N'CONTENT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = '是否可用', @level0type = N'SCHEMA', @level0name = N'KB', @level1type = N'TABLE', @level1name = N'TIP', @level2type = N'COLUMN', @level2name = N'ENABLE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = '创建时间', @level0type = N'SCHEMA', @level0name = N'KB', @level1type = N'TABLE', @level1name = N'TIP', @level2type = N'COLUMN', @level2name = N'CREATE_TIME';

