CREATE TABLE [MSG].[SENT_EMAIL_MESSAGES] (
    [MESSAGE_ID]       NVARCHAR (36)  NOT NULL,
    [SORT_ID]          INT            NOT NULL,
    [BODY]             NVARCHAR (MAX) NULL,
    [BODY_ENCODING]    NVARCHAR (64)  NULL,
    [HEADERS_ENCODING] NVARCHAR (64)  NULL,
    [IS_BODY_HTML]     NCHAR (1)      NULL,
    [PRIORITY]         INT            CONSTRAINT [DF_SENT_EMAIL_MESSAGES_PRIORITY] DEFAULT ((0)) NULL,
    [SUBJECT]          NVARCHAR (MAX) NULL,
    [SUBJECT_ENCODING] NVARCHAR (64)  NULL,
    [STATUS]           INT            CONSTRAINT [DF_SENT_EMAIL_MESSAGES_STATUS] DEFAULT ((0)) NULL,
    [STATUS_TEXT]      NVARCHAR (MAX) NULL,
    [SENT_TIME]        DATETIME       NULL,
	[TENANT_CODE] NVARCHAR(36) NULL DEFAULT 'D5561180-7617-4B67-B68B-1F0EA604B509',
    CONSTRAINT [PK_SENT_EMAIL_MESSAGES] PRIMARY KEY NONCLUSTERED ([MESSAGE_ID] ASC)
);

GO
CREATE INDEX [IX_SENT_EMAIL_MESSAGES_TENANT_CODE] ON [MSG].[SENT_EMAIL_MESSAGES] ([TENANT_CODE])

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'待发送邮件列表', @level0type = N'SCHEMA', @level0name = N'MSG', @level1type = N'TABLE', @level1name = N'SENT_EMAIL_MESSAGES';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'排序号', @level0type = N'SCHEMA', @level0name = N'MSG', @level1type = N'TABLE', @level1name = N'SENT_EMAIL_MESSAGES', @level2type = N'COLUMN', @level2name = N'SORT_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'邮件体', @level0type = N'SCHEMA', @level0name = N'MSG', @level1type = N'TABLE', @level1name = N'SENT_EMAIL_MESSAGES', @level2type = N'COLUMN', @level2name = N'BODY';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'邮件体是否是HTML', @level0type = N'SCHEMA', @level0name = N'MSG', @level1type = N'TABLE', @level1name = N'SENT_EMAIL_MESSAGES', @level2type = N'COLUMN', @level2name = N'IS_BODY_HTML';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'邮件优先级', @level0type = N'SCHEMA', @level0name = N'MSG', @level1type = N'TABLE', @level1name = N'SENT_EMAIL_MESSAGES', @level2type = N'COLUMN', @level2name = N'PRIORITY';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'邮件标题', @level0type = N'SCHEMA', @level0name = N'MSG', @level1type = N'TABLE', @level1name = N'SENT_EMAIL_MESSAGES', @level2type = N'COLUMN', @level2name = N'SUBJECT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'邮件标题编码', @level0type = N'SCHEMA', @level0name = N'MSG', @level1type = N'TABLE', @level1name = N'SENT_EMAIL_MESSAGES', @level2type = N'COLUMN', @level2name = N'SUBJECT_ENCODING';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'发送状态', @level0type = N'SCHEMA', @level0name = N'MSG', @level1type = N'TABLE', @level1name = N'SENT_EMAIL_MESSAGES', @level2type = N'COLUMN', @level2name = N'STATUS';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'发送返回结果', @level0type = N'SCHEMA', @level0name = N'MSG', @level1type = N'TABLE', @level1name = N'SENT_EMAIL_MESSAGES', @level2type = N'COLUMN', @level2name = N'STATUS_TEXT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'发送时间', @level0type = N'SCHEMA', @level0name = N'MSG', @level1type = N'TABLE', @level1name = N'SENT_EMAIL_MESSAGES', @level2type = N'COLUMN', @level2name = N'SENT_TIME';


GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'邮件的ID，对应EMAIL_MESSAGES表',
    @level0type = N'SCHEMA',
    @level0name = N'MSG',
    @level1type = N'TABLE',
    @level1name = N'SENT_EMAIL_MESSAGES',
    @level2type = N'COLUMN',
    @level2name = N'MESSAGE_ID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'邮件体的编码方式',
    @level0type = N'SCHEMA',
    @level0name = N'MSG',
    @level1type = N'TABLE',
    @level1name = N'SENT_EMAIL_MESSAGES',
    @level2type = N'COLUMN',
    @level2name = N'BODY_ENCODING'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'邮件头的编码方式',
    @level0type = N'SCHEMA',
    @level0name = N'MSG',
    @level1type = N'TABLE',
    @level1name = N'SENT_EMAIL_MESSAGES',
    @level2type = N'COLUMN',
    @level2name = N'HEADERS_ENCODING'