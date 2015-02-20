CREATE TABLE [Biz].[Members]
(
	[MemberID] NVARCHAR(36) NOT NULL PRIMARY KEY, 
    [MemberName] NVARCHAR(50) NULL, 
    [Age] INT NULL, 
    [Gender] NVARCHAR(50) NULL, 
    [AnnualHouseholdIncome] NVARCHAR(50) NULL, 
    [NativePlace] NVARCHAR(128) NULL, 
    [RegisteredPermanentResidence] NVARCHAR(128) NULL, 
    [HousePaymentPrice] NVARCHAR(50) NULL, 
    [FamilyComposition] NVARCHAR(50) NULL
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'会员表',
    @level0type = N'SCHEMA',
    @level0name = N'Biz',
    @level1type = N'TABLE',
    @level1name = N'Members',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'会员ID',
    @level0type = N'SCHEMA',
    @level0name = N'Biz',
    @level1type = N'TABLE',
    @level1name = N'Members',
    @level2type = N'COLUMN',
    @level2name = N'MemberID'
GO

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'会员名称',
    @level0type = N'SCHEMA',
    @level0name = N'Biz',
    @level1type = N'TABLE',
    @level1name = N'Members',
    @level2type = N'COLUMN',
    @level2name = 'MemberName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'年龄',
    @level0type = N'SCHEMA',
    @level0name = N'Biz',
    @level1type = N'TABLE',
    @level1name = N'Members',
    @level2type = N'COLUMN',
    @level2name = 'Age'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'性别',
    @level0type = N'SCHEMA',
    @level0name = N'Biz',
    @level1type = N'TABLE',
    @level1name = N'Members',
    @level2type = N'COLUMN',
    @level2name = N'Gender'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'家庭年收入',
    @level0type = N'SCHEMA',
    @level0name = N'Biz',
    @level1type = N'TABLE',
    @level1name = N'Members',
    @level2type = N'COLUMN',
    @level2name = 'AnnualHouseholdIncome'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'籍贯',
    @level0type = N'SCHEMA',
    @level0name = N'Biz',
    @level1type = N'TABLE',
    @level1name = N'Members',
    @level2type = N'COLUMN',
    @level2name = N'NativePlace'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'户口所在地',
    @level0type = N'SCHEMA',
    @level0name = N'Biz',
    @level1type = N'TABLE',
    @level1name = N'Members',
    @level2type = N'COLUMN',
    @level2name = N'RegisteredPermanentResidence'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'购房价格',
    @level0type = N'SCHEMA',
    @level0name = N'Biz',
    @level1type = N'TABLE',
    @level1name = N'Members',
    @level2type = N'COLUMN',
    @level2name = N'HousePaymentPrice'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'家庭结构',
    @level0type = N'SCHEMA',
    @level0name = N'Biz',
    @level1type = N'TABLE',
    @level1name = N'Members',
    @level2type = N'COLUMN',
    @level2name = 'FamilyComposition'