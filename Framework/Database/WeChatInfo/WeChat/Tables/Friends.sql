CREATE TABLE [WeChat].[Friends]
(
	[FakeID] NVARCHAR(36) NOT NULL , 
    [AccountID] NVARCHAR(36) NOT NULL, 
    [OpenID] NVARCHAR(36) NULL, 
    [NickName] NVARCHAR(64) NULL, 
    [GroupID] INT NULL, 
    [RemarkName] NVARCHAR(64) NULL, 
    [CreateTime] DATETIME NULL DEFAULT GETDATE(), 
    [UpdateTime] DATETIME NULL DEFAULT GETDATE(), 
    PRIMARY KEY ([AccountID], [FakeID])
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'所属的微信公众号的ID',
    @level0type = N'SCHEMA',
    @level0name = N'WeChat',
    @level1type = N'TABLE',
    @level1name = N'Friends',
    @level2type = N'COLUMN',
    @level2name = N'AccountID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'该粉丝在某个公众号下的OpenID',
    @level0type = N'SCHEMA',
    @level0name = N'WeChat',
    @level1type = N'TABLE',
    @level1name = N'Friends',
    @level2type = N'COLUMN',
    @level2name = N'OpenID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'粉丝的昵称',
    @level0type = N'SCHEMA',
    @level0name = N'WeChat',
    @level1type = N'TABLE',
    @level1name = N'Friends',
    @level2type = N'COLUMN',
    @level2name = N'NickName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'该粉丝在某个公众号下所属的组',
    @level0type = N'SCHEMA',
    @level0name = N'WeChat',
    @level1type = N'TABLE',
    @level1name = N'Friends',
    @level2type = N'COLUMN',
    @level2name = N'GroupID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'该粉丝在某个公众号下备注昵称',
    @level0type = N'SCHEMA',
    @level0name = N'WeChat',
    @level1type = N'TABLE',
    @level1name = N'Friends',
    @level2type = N'COLUMN',
    @level2name = N'RemarkName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'记录创建时间',
    @level0type = N'SCHEMA',
    @level0name = N'WeChat',
    @level1type = N'TABLE',
    @level1name = N'Friends',
    @level2type = N'COLUMN',
    @level2name = N'CreateTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'记录更新时间',
    @level0type = N'SCHEMA',
    @level0name = N'WeChat',
    @level1type = N'TABLE',
    @level1name = N'Friends',
    @level2type = N'COLUMN',
    @level2name = N'UpdateTime'
GO

CREATE INDEX [IX_Friends_OpenID] ON [WeChat].[Friends] ([OpenID])

GO

CREATE INDEX [IX_Friends_AccountID_OpenID] ON [WeChat].[Friends] ([AccountID], [OpenID])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'微信的粉丝表',
    @level0type = N'SCHEMA',
    @level0name = N'WeChat',
    @level1type = N'TABLE',
    @level1name = N'Friends',
    @level2type = NULL,
    @level2name = NULL
GO

CREATE INDEX [IX_Friends_FakeID] ON [WeChat].[Friends] ([FakeID])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'粉丝在微信中模拟ID',
    @level0type = N'SCHEMA',
    @level0name = N'WeChat',
    @level1type = N'TABLE',
    @level1name = N'Friends',
    @level2type = N'COLUMN',
    @level2name = N'FakeID'