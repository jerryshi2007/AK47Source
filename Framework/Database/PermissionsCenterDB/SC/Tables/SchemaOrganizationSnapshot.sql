CREATE TABLE [SC].[SchemaOrganizationSnapshot]
(
	[ID]               NVARCHAR (36)  NOT NULL,
    [VersionStartTime] DATETIME       NOT NULL,
    [VersionEndTime]   DATETIME       CONSTRAINT [DF_SchemaOrganization_VersionEndTime] DEFAULT ('99990909 00:00:00') NULL,
    [Status]           INT            CONSTRAINT [DF_SchemaOrganization_Status] DEFAULT ((1)) NULL,
	[CreateDate]       DATETIME NULL DEFAULT GETDATE(), 
    [Name]             NVARCHAR (255) NULL,
    [DisplayName]      NVARCHAR (255) NULL,
	[CodeName]         NVARCHAR (64) NULL,
	[AllowAclInheritance] INT DEFAULT 1,
    [SearchContent]    NVARCHAR (MAX) NULL,
    [RowUniqueID]      NVARCHAR (36)  CONSTRAINT [DF_SchemaOrganization_RowUniqueID] DEFAULT (CONVERT([nvarchar](36),newid())) NOT NULL,
    [SchemaType] NVARCHAR(36) NULL, 
	[CreatorID] NVARCHAR(36) NULL, 
    [CreatorName] NVARCHAR(255) NULL,
    [Comment] NVARCHAR(255) NULL, 
    CONSTRAINT [PK_SchemaOrganization] PRIMARY KEY CLUSTERED ([ID] ASC, [VersionStartTime] DESC)
)

GO

GO


CREATE UNIQUE NONCLUSTERED INDEX [IX_SchemaOrganizationSnapshot_RowID] ON [SC].[SchemaOrganizationSnapshot] ([RowUniqueID])

GO

CREATE FULLTEXT INDEX ON [SC].[SchemaOrganizationSnapshot]
    ([SearchContent] LANGUAGE 2052)
    KEY INDEX [IX_SchemaOrganizationSnapshot_RowID]
    ON [SCFullTextIndex] WITH CHANGE_TRACKING AUTO

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'组织的快照',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaOrganizationSnapshot',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'对象的ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaOrganizationSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'ID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'版本开始时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaOrganizationSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'VersionStartTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'版本结束时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaOrganizationSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'VersionEndTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'对象的状态',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaOrganizationSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'Status'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'创建时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaOrganizationSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'CreateDate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'组织的名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaOrganizationSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'Name'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'组织的显示名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaOrganizationSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'DisplayName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'组织的代码',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaOrganizationSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'CodeName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'全文检索字段',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaOrganizationSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'SearchContent'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'行唯一标识，用于全文检索',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaOrganizationSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'RowUniqueID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'对象的Schema名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaOrganizationSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'SchemaType'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'创建者的ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaOrganizationSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'CreatorID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'创建者的名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaOrganizationSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'CreatorName'
GO

CREATE INDEX [IX_SchemaOrganizationSnapshot_CodeName] ON [SC].[SchemaOrganizationSnapshot] ([CodeName])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'是否允许继承容器的权限',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaOrganizationSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'AllowAclInheritance'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'注释',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaOrganizationSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'Comment'