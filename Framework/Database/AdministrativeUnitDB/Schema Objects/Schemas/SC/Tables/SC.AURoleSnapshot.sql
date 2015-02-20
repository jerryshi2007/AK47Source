CREATE TABLE [SC].[AURoleSnapshot]
(
	[ID]               NVARCHAR (36) NOT NULL,
    [VersionStartTime] DATETIME      NOT NULL,
    [VersionEndTime]   DATETIME      CONSTRAINT [DF_AURole_VersionEndTime] DEFAULT ('99990909 00:00:00') NULL,
    [Status]           INT           CONSTRAINT [DF_AURole_Status] DEFAULT ((1)) NULL,
    [SchemaType]       NVARCHAR (64) NULL,
    [CreateDate] DATETIME NULL DEFAULT GETDATE(), 
    [CreatorID] NVARCHAR(36) NULL, 
    [CreatorName] NVARCHAR(255) NULL, 
	[SearchContent]    NVARCHAR (MAX) NULL,
	[RowUniqueID]      NVARCHAR (36)  CONSTRAINT [DF_AURole_RowUniqueID] DEFAULT (CONVERT([nvarchar](36),newid())) NOT NULL,
    [SchemaRoleID] NVARCHAR(36) NULL, 
    CONSTRAINT [PK_AURole] PRIMARY KEY CLUSTERED ([ID] ASC, [VersionStartTime] DESC)
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'角色的ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'AURoleSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'ID'
GO


EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'此角色引用的管理架构角色',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'AURoleSnapshot',
    @level2type = N'COLUMN',
    @level2name = 'SchemaRoleID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'版本开始时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'AURoleSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'VersionStartTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'版本结束时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'AURoleSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'VersionEndTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'状态',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'AURoleSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'Status'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'创建日期',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'AURoleSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'CreateDate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'创建者ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'AURoleSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'CreatorID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'创建者名',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'AURoleSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'CreatorName'
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_AURoleSnapshot_RowID] ON [SC].[AURoleSnapshot] ([RowUniqueID])

GO

CREATE FULLTEXT INDEX ON [SC].[AURoleSnapshot] ([SearchContent] LANGUAGE 2052) KEY INDEX [IX_AURoleSnapshot_RowID] ON [SCFullTextIndex] WITH CHANGE_TRACKING AUTO
