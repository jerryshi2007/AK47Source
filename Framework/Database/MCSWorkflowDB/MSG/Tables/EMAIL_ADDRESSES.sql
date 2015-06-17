CREATE TABLE [MSG].[EMAIL_ADDRESSES] (
    [MESSAGE_ID]   NVARCHAR (36)  NOT NULL,
    [CLASS]        NVARCHAR (32)  NOT NULL,
    [SORT_ID]      INT            NOT NULL,
    [ADDRESS]      NVARCHAR (255) NULL,
    [DISPLAY_NAME] NVARCHAR (255) NULL,
	[TENANT_CODE] NVARCHAR(36) NULL DEFAULT 'D5561180-7617-4B67-B68B-1F0EA604B509',
    CONSTRAINT [PK_EMAIL_ADDRESSES] PRIMARY KEY CLUSTERED ([MESSAGE_ID] ASC, [CLASS] ASC, [SORT_ID] ASC)
);

GO

CREATE INDEX [IX_EMAIL_ADDRESSES_TENANT_CODE] ON [MSG].[EMAIL_ADDRESSES] ([TENANT_CODE])
GO

EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'邮件的收件人地址表。是EMAIL_MESSAGES或SENT_EMAIL_MESSAGES的子表。', @level0type = N'SCHEMA', @level0name = N'MSG', @level1type = N'TABLE', @level1name = N'EMAIL_ADDRESSES';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'邮件的ID，对应EMAIL_MESSAGES表', @level0type = N'SCHEMA', @level0name = N'MSG', @level1type = N'TABLE', @level1name = N'EMAIL_ADDRESSES', @level2type = N'COLUMN', @level2name = N'MESSAGE_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'地址的类别', @level0type = N'SCHEMA', @level0name = N'MSG', @level1type = N'TABLE', @level1name = N'EMAIL_ADDRESSES', @level2type = N'COLUMN', @level2name = N'CLASS';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'排序号', @level0type = N'SCHEMA', @level0name = N'MSG', @level1type = N'TABLE', @level1name = N'EMAIL_ADDRESSES', @level2type = N'COLUMN', @level2name = N'SORT_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'地址', @level0type = N'SCHEMA', @level0name = N'MSG', @level1type = N'TABLE', @level1name = N'EMAIL_ADDRESSES', @level2type = N'COLUMN', @level2name = N'ADDRESS';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'显示名称', @level0type = N'SCHEMA', @level0name = N'MSG', @level1type = N'TABLE', @level1name = N'EMAIL_ADDRESSES', @level2type = N'COLUMN', @level2name = N'DISPLAY_NAME';

