CREATE TABLE [MSG].[EMAIL_ATTACHMENTS] (
    [MESSAGE_ID]   NVARCHAR (36)   NOT NULL,
    [SORT_ID]      INT             NOT NULL,
    [FILE_NAME]    NVARCHAR (255)  NULL,
    [CONTENT_DATA] VARBINARY (MAX) NULL,
    CONSTRAINT [PK_EMAIL_ATTACHMENTS] PRIMARY KEY CLUSTERED ([MESSAGE_ID] ASC, [SORT_ID] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'邮件的附件。是EMAIL_MESSAGES或SENT_EMAIL_MESSAGES的子表。', @level0type = N'SCHEMA', @level0name = N'MSG', @level1type = N'TABLE', @level1name = N'EMAIL_ATTACHMENTS';


GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'邮件的ID，对应EMAIL_MESSAGES表',
    @level0type = N'SCHEMA',
    @level0name = N'MSG',
    @level1type = N'TABLE',
    @level1name = N'EMAIL_ATTACHMENTS',
    @level2type = N'COLUMN',
    @level2name = N'MESSAGE_ID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'排序号',
    @level0type = N'SCHEMA',
    @level0name = N'MSG',
    @level1type = N'TABLE',
    @level1name = N'EMAIL_ATTACHMENTS',
    @level2type = N'COLUMN',
    @level2name = N'SORT_ID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'附件的文件名',
    @level0type = N'SCHEMA',
    @level0name = N'MSG',
    @level1type = N'TABLE',
    @level1name = N'EMAIL_ATTACHMENTS',
    @level2type = N'COLUMN',
    @level2name = N'FILE_NAME'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'附件内容',
    @level0type = N'SCHEMA',
    @level0name = N'MSG',
    @level1type = N'TABLE',
    @level1name = N'EMAIL_ATTACHMENTS',
    @level2type = N'COLUMN',
    @level2name = N'CONTENT_DATA'