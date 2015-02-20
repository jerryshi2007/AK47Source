﻿CREATE TABLE [SC].[SchemaUserSnapshot] (
    [ID]               NVARCHAR (36)  NOT NULL,
    [VersionStartTime] DATETIME       NOT NULL,
    [VersionEndTime]   DATETIME       CONSTRAINT [DF_SchemaUser_VersionEndTime] DEFAULT ('99990909 00:00:00') NULL,
    [Status]           INT            CONSTRAINT [DF_SchemaUser_Status] DEFAULT ((1)) NULL,
	[CreateDate]	   DATETIME NULL DEFAULT GETDATE(), 
    [Name]             NVARCHAR (255) NULL,
    [DisplayName]      NVARCHAR (255) NULL,
	[CodeName]		   NVARCHAR (64) NULL,
    [FirstName]        NVARCHAR (64)  NULL,
    [LastName]         NVARCHAR (64)  NULL,
    [SearchContent]    NVARCHAR (MAX) NULL,
    [RowUniqueID]      NVARCHAR (36)  CONSTRAINT [DF_SchemaUser_RowUniqueID] DEFAULT (CONVERT([nvarchar](36),newid())) NOT NULL,
    [SchemaType] NVARCHAR(36) NULL, 
    [Mail] NVARCHAR(255) NULL, 
    [Sip] NVARCHAR(255) NULL, 
    [MP] NVARCHAR(36) NULL, 
    [WP] NVARCHAR(36) NULL, 
    [Address] NVARCHAR(MAX) NULL, 
	[CreatorID] NVARCHAR(36) NULL, 
    [CreatorName] NVARCHAR(255) NULL,
	[OwnerID] NVARCHAR(36) NULL, 
    [OwnerName] NVARCHAR(255) NULL,
    [AccountDisabled] INT NULL DEFAULT 0, 
    [PasswordNotRequired] INT NULL DEFAULT 0, 
    [DontExpirePassword] INT NULL DEFAULT 0, 
    [AccountExpires] DATETIME NULL, 
    [AccountInspires] DATETIME NULL, 
    [Comment] NVARCHAR(255) NULL, 
    CONSTRAINT [PK_SchemaUser] PRIMARY KEY CLUSTERED ([ID] ASC, [VersionStartTime] DESC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_SchemaUser_RowID]
    ON [SC].[SchemaUserSnapshot]([RowUniqueID] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'全文检索字段', @level0type = N'SCHEMA', @level0name = N'SC', @level1type = N'TABLE', @level1name = N'SchemaUserSnapshot', @level2type = N'COLUMN', @level2name = N'SearchContent';


GO

CREATE INDEX [IX_SchemaUserSnapshot_CodeName] ON [SC].[SchemaUserSnapshot] ([CodeName])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'用户的快照表',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaUserSnapshot',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'对象的ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaUserSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'ID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'版本开始时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaUserSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'VersionStartTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'版本结束时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaUserSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'VersionEndTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'对象的状态',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaUserSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'Status'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'对象的创建时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaUserSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'CreateDate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'用户的名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaUserSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'Name'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'显示名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaUserSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'DisplayName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'用户的代码（登录名）',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaUserSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'CodeName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'用户的名',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaUserSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'FirstName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'用户的姓',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaUserSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'LastName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'行唯一标识，用于全文检索',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaUserSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'RowUniqueID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'对象的Schema名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaUserSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'SchemaType'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'用户的邮件地址',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaUserSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'Mail'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'用户的即时消息地址',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaUserSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'Sip'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'用户的手机号',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaUserSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'MP'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'用户的工作电话',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaUserSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'WP'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'用户的地址',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaUserSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'Address'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'创建人的ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaUserSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'CreatorID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'创建人的名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaUserSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'CreatorName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'拥有者（组织）的ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaUserSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'OwnerID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'拥有者（组织）的名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaUserSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'OwnerName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'账户是否禁用',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaUserSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'AccountDisabled'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'是否不需要密码',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaUserSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'PasswordNotRequired'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'密码永不过期',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaUserSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'DontExpirePassword'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'账户过期时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaUserSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'AccountExpires'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'账户生效时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaUserSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'AccountInspires'
GO

CREATE INDEX [IX_SchemaUserSnapshot_OwnerID] ON [SC].[SchemaUserSnapshot] ([OwnerID])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'注释',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaUserSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'Comment'