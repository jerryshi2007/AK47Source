CREATE TABLE [SC].[ItemAndContainerSnapshot]
(
	[ItemID] NVARCHAR(36) NOT NULL , 
    [ContainerID] NVARCHAR(36) NOT NULL,
	[VersionStartTime] DATETIME NOT NULL,
	[VersionEndTime]   DATETIME      CONSTRAINT [DF_ItemAndContainerSnapshot_VersionEndTime] DEFAULT ('99990909 00:00:00') NULL,
    [Status]           INT           CONSTRAINT [DF_ItemAndContainerSnapshot_Status] DEFAULT ((1)) NULL,
    [ItemSchemaType] NVARCHAR(64) NULL, 
    [ContainerSchemaType] NVARCHAR(64) NULL, 
    CONSTRAINT [PK_ItemAndContainerSnapshot] PRIMARY KEY ([ItemID], [ContainerID], [VersionStartTime])
)

GO

CREATE INDEX [IX_ItemAndContainerSnapshot_ItemSchemaType] ON [SC].[ItemAndContainerSnapshot] ([ItemSchemaType])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'管理范围项目ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ItemAndContainerSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'ItemID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'包含者的ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ItemAndContainerSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'ContainerID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'版本开始时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ItemAndContainerSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'VersionStartTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'版本结束时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ItemAndContainerSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'VersionEndTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'状态',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ItemAndContainerSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'Status'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'管理范围项目的类型',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ItemAndContainerSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'ItemSchemaType'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'包含者的SchemaType',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ItemAndContainerSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'ContainerSchemaType'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'管理范围对象和容器的快照',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'ItemAndContainerSnapshot',
    @level2type = NULL,
    @level2name = NULL