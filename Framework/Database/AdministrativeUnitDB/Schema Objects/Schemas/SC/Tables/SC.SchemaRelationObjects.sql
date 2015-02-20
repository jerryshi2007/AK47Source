CREATE TABLE [SC].[SchemaRelationObjects]
(
	[ParentID]         NVARCHAR (36)  NOT NULL,
    [ObjectID]         NVARCHAR (36)  NOT NULL,
    [VersionStartTime] DATETIME       NOT NULL,
    [VersionEndTime]   DATETIME       CONSTRAINT [DF_SchemaObjectRelations_VersionEndTime] DEFAULT ('99990909 00:00:00') NULL,
    [Status]           INT            CONSTRAINT [DF_SchemaObjectRelations_Status] DEFAULT ((1)) NULL,
	[IsDefault]	       INT            CONSTRAINT [DF_SchemaObjectRelations_Default] DEFAULT ((1)) NULL,
    [InnerSort]        INT            CONSTRAINT [DF_SchemaRelationObjects_InnerSort] DEFAULT ((0)) NULL,
	[FullPath]         NVARCHAR(414)  NULL,
	[GlobalSort]       NVARCHAR(414)  NULL,
    [Data]             XML            NULL,
    [SchemaType]       NVARCHAR (64)  NULL,
    [ParentSchemaType] NVARCHAR(64) NULL, 
    [ChildSchemaType] NVARCHAR(64) NULL, 
    [CreateDate] DATETIME NULL DEFAULT GETDATE(), 
	[CreatorID] NVARCHAR(36) NULL, 
    [CreatorName] NVARCHAR(255) NULL,
    CONSTRAINT [PK_SchemaObjectRelations] PRIMARY KEY CLUSTERED ([ParentID] ASC, [ObjectID] ASC, [VersionStartTime] DESC)
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'父对象ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaRelationObjects',
    @level2type = N'COLUMN',
    @level2name = N'ParentID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'子对象ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaRelationObjects',
    @level2type = N'COLUMN',
    @level2name = N'ObjectID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'版本开始时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaRelationObjects',
    @level2type = N'COLUMN',
    @level2name = N'VersionStartTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'版本结束时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaRelationObjects',
    @level2type = N'COLUMN',
    @level2name = N'VersionEndTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'对象的状态',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaRelationObjects',
    @level2type = N'COLUMN',
    @level2name = N'Status'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'是否是默认关系',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaRelationObjects',
    @level2type = N'COLUMN',
    @level2name = N'IsDefault'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'内部排序号',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaRelationObjects',
    @level2type = N'COLUMN',
    @level2name = N'InnerSort'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'全路径',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaRelationObjects',
    @level2type = N'COLUMN',
    @level2name = N'FullPath'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'全局排序号',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaRelationObjects',
    @level2type = N'COLUMN',
    @level2name = N'GlobalSort'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'对象的属性集合',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaRelationObjects',
    @level2type = N'COLUMN',
    @level2name = N'Data'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'对象的Schema名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaRelationObjects',
    @level2type = N'COLUMN',
    @level2name = N'SchemaType'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'父对象的Schema名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaRelationObjects',
    @level2type = N'COLUMN',
    @level2name = N'ParentSchemaType'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'子对象的Schema名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaRelationObjects',
    @level2type = N'COLUMN',
    @level2name = N'ChildSchemaType'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'对象创建时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaRelationObjects',
    @level2type = N'COLUMN',
    @level2name = N'CreateDate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'对象创建人的ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaRelationObjects',
    @level2type = N'COLUMN',
    @level2name = N'CreatorID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'对象创建人的名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaRelationObjects',
    @level2type = N'COLUMN',
    @level2name = N'CreatorName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Schema关系对象',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaRelationObjects',
    @level2type = NULL,
    @level2name = NULL
GO


