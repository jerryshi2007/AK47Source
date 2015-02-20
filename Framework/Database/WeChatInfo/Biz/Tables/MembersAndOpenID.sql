CREATE TABLE [Biz].[MembersAndOpenID]
(
	[MemberID] NVARCHAR(36) NOT NULL , 
    [OpenID] NVARCHAR(36) NOT NULL, 
    CONSTRAINT [PK_MembersAndOpenID] PRIMARY KEY ([MemberID], [OpenID]) 
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'会员ID',
    @level0type = N'SCHEMA',
    @level0name = N'Biz',
    @level1type = N'TABLE',
    @level1name = N'MembersAndOpenID',
    @level2type = N'COLUMN',
    @level2name = N'MemberID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'该粉丝在某个公众号下的OpenID',
    @level0type = N'SCHEMA',
    @level0name = N'Biz',
    @level1type = N'TABLE',
    @level1name = N'MembersAndOpenID',
    @level2type = N'COLUMN',
    @level2name = N'OpenID'