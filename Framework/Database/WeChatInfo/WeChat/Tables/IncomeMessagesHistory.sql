CREATE TABLE [WeChat].[IncomeMessagesHistory]
(
	[MessageID] BIGINT NOT NULL PRIMARY KEY, 
    [ToOpenID] NVARCHAR(36) NULL, 
    [FromOpenID] NVARCHAR(36) NULL, 
    [SentTime] DATETIME NULL, 
    [MessageType] NVARCHAR(64) NULL, 
    [Content] NVARCHAR(MAX) NULL, 
    [RawXml] XML NULL
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'消息ID',
    @level0type = N'SCHEMA',
    @level0name = N'WeChat',
    @level1type = N'TABLE',
    @level1name = N'IncomeMessagesHistory',
    @level2type = N'COLUMN',
    @level2name = N'MessageID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'微信公众号（机器人）接收到的原始历史消息',
    @level0type = N'SCHEMA',
    @level0name = N'WeChat',
    @level1type = N'TABLE',
    @level1name = N'IncomeMessagesHistory',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'发送到的OpenID',
    @level0type = N'SCHEMA',
    @level0name = N'WeChat',
    @level1type = N'TABLE',
    @level1name = N'IncomeMessagesHistory',
    @level2type = N'COLUMN',
    @level2name = N'ToOpenID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'从哪个OpenID发送',
    @level0type = N'SCHEMA',
    @level0name = N'WeChat',
    @level1type = N'TABLE',
    @level1name = N'IncomeMessagesHistory',
    @level2type = N'COLUMN',
    @level2name = N'FromOpenID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'发送时间',
    @level0type = N'SCHEMA',
    @level0name = N'WeChat',
    @level1type = N'TABLE',
    @level1name = N'IncomeMessagesHistory',
    @level2type = N'COLUMN',
    @level2name = 'SentTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'消息类型',
    @level0type = N'SCHEMA',
    @level0name = N'WeChat',
    @level1type = N'TABLE',
    @level1name = N'IncomeMessagesHistory',
    @level2type = N'COLUMN',
    @level2name = N'MessageType'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'消息内容',
    @level0type = N'SCHEMA',
    @level0name = N'WeChat',
    @level1type = N'TABLE',
    @level1name = N'IncomeMessagesHistory',
    @level2type = N'COLUMN',
    @level2name = N'Content'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'原始的xml',
    @level0type = N'SCHEMA',
    @level0name = N'WeChat',
    @level1type = N'TABLE',
    @level1name = N'IncomeMessagesHistory',
    @level2type = N'COLUMN',
    @level2name = N'RawXml'
GO

CREATE INDEX [IX_IncomeMessagesHistory_SentTime] ON [WeChat].[IncomeMessagesHistory] ([SentTime])
