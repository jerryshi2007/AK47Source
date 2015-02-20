CREATE TABLE [WeChat].[RecentMessages]
(
	[AccountID] NVARCHAR(36) NOT NULL,
	[MessageID] BIGINT NOT NULL,
	[MessageType] NVARCHAR(64) NULL ,
	[FakeID] NVARCHAR(36),
	[NickName] NVARCHAR(64) NULL,
	[ToFakeID] NVARCHAR(36) NULL,
	[SentTime] DATETIME NULL,
	[Content] NVARCHAR(MAX) NULL,
	[Source] NVARCHAR(64) NULL,
	[MessageStatus] INT NULL DEFAULT 0,
	[HasReply] INT NULL DEFAULT 0,
	[RefuseReason] NVARCHAR(255) NULL, 
    CONSTRAINT [PK_RecentMessages] PRIMARY KEY ([AccountID], [MessageID])
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'微信公众平台中某公众号的最近消息',
    @level0type = N'SCHEMA',
    @level0name = N'WeChat',
    @level1type = N'TABLE',
    @level1name = N'RecentMessages',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'微信公众号的ID',
    @level0type = N'SCHEMA',
    @level0name = N'WeChat',
    @level1type = N'TABLE',
    @level1name = N'RecentMessages',
    @level2type = N'COLUMN',
    @level2name = N'AccountID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'消息ID',
    @level0type = N'SCHEMA',
    @level0name = N'WeChat',
    @level1type = N'TABLE',
    @level1name = N'RecentMessages',
    @level2type = N'COLUMN',
    @level2name = N'MessageID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'消息类型',
    @level0type = N'SCHEMA',
    @level0name = N'WeChat',
    @level1type = N'TABLE',
    @level1name = N'RecentMessages',
    @level2type = N'COLUMN',
    @level2name = N'MessageType'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'发消息者的FakeID',
    @level0type = N'SCHEMA',
    @level0name = N'WeChat',
    @level1type = N'TABLE',
    @level1name = N'RecentMessages',
    @level2type = N'COLUMN',
    @level2name = N'FakeID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'发消息者的昵称',
    @level0type = N'SCHEMA',
    @level0name = N'WeChat',
    @level1type = N'TABLE',
    @level1name = N'RecentMessages',
    @level2type = N'COLUMN',
    @level2name = N'NickName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'发送目标的FakeID',
    @level0type = N'SCHEMA',
    @level0name = N'WeChat',
    @level1type = N'TABLE',
    @level1name = N'RecentMessages',
    @level2type = N'COLUMN',
    @level2name = N'ToFakeID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'发送时间',
    @level0type = N'SCHEMA',
    @level0name = N'WeChat',
    @level1type = N'TABLE',
    @level1name = N'RecentMessages',
    @level2type = N'COLUMN',
    @level2name = N'SentTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'发送内容',
    @level0type = N'SCHEMA',
    @level0name = N'WeChat',
    @level1type = N'TABLE',
    @level1name = N'RecentMessages',
    @level2type = N'COLUMN',
    @level2name = N'Content'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'发送源',
    @level0type = N'SCHEMA',
    @level0name = N'WeChat',
    @level1type = N'TABLE',
    @level1name = N'RecentMessages',
    @level2type = N'COLUMN',
    @level2name = N'Source'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'消息状态',
    @level0type = N'SCHEMA',
    @level0name = N'WeChat',
    @level1type = N'TABLE',
    @level1name = N'RecentMessages',
    @level2type = N'COLUMN',
    @level2name = N'MessageStatus'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'是否有回复',
    @level0type = N'SCHEMA',
    @level0name = N'WeChat',
    @level1type = N'TABLE',
    @level1name = N'RecentMessages',
    @level2type = N'COLUMN',
    @level2name = N'HasReply'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'拒绝原因',
    @level0type = N'SCHEMA',
    @level0name = N'WeChat',
    @level1type = N'TABLE',
    @level1name = N'RecentMessages',
    @level2type = N'COLUMN',
    @level2name = N'RefuseReason'
GO

CREATE INDEX [IX_RecentMessages_MessageID] ON [WeChat].[RecentMessages] ([MessageID])

GO

CREATE INDEX [IX_RecentMessages_SentTime] ON [WeChat].[RecentMessages] ([SentTime])
