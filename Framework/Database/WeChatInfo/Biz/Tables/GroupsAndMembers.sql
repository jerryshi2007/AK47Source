CREATE TABLE [Biz].[GroupsAndMembers]
(
	[GroupID] NVARCHAR(36) NOT NULL , 
    [MemberID] NVARCHAR(36) NOT NULL, 
    CONSTRAINT [PK_GroupsAndMembers] PRIMARY KEY ([GroupID], [MemberID])
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'组ID',
    @level0type = N'SCHEMA',
    @level0name = N'Biz',
    @level1type = N'TABLE',
    @level1name = N'GroupsAndMembers',
    @level2type = N'COLUMN',
    @level2name = N'GroupID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'成员ID',
    @level0type = N'SCHEMA',
    @level0name = N'Biz',
    @level1type = N'TABLE',
    @level1name = N'GroupsAndMembers',
    @level2type = N'COLUMN',
    @level2name = N'MemberID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'组和成员的映射表',
    @level0type = N'SCHEMA',
    @level0name = N'Biz',
    @level1type = N'TABLE',
    @level1name = N'GroupsAndMembers',
    @level2type = NULL,
    @level2name = NULL
GO

CREATE INDEX [IX_GroupsAndMembers_MemberID] ON [Biz].[GroupsAndMembers] ([MemberID])
