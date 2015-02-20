CREATE TABLE [SC].[SchemaMembers] (
    [ContainerID]		NVARCHAR (36)  NOT NULL,
    [MemberID]			NVARCHAR (36)  NOT NULL,
    [VersionStartTime]	DATETIME       NOT NULL,
    [VersionEndTime]	DATETIME       CONSTRAINT [DF_SchemaMembers_VersionEndTime] DEFAULT ('99990909 00:00:00') NULL,
    [Status]			INT            CONSTRAINT [DF_SchemaMembers_Status] DEFAULT ((1)) NULL,
    [InnerSort]			INT            CONSTRAINT [DF_SchemaMembers_InnerSort] DEFAULT ((0)) NULL,
    [Data]				XML            NULL,
    [SchemaType]		NVARCHAR (64)  NULL,
    [ContainerSchemaType]	NVARCHAR(64) NULL, 
    [MemberSchemaType]	NVARCHAR(64) NULL, 
	[CreateDate] DATETIME NULL DEFAULT GETDATE(),
	[CreatorID] NVARCHAR(36) NULL, 
    [CreatorName] NVARCHAR(255) NULL,
    CONSTRAINT [PK_SchemaObjectMembers] PRIMARY KEY CLUSTERED ([ContainerID] ASC, [MemberID] ASC, [VersionStartTime] DESC)
);

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'成员关系对象表',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaMembers',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'容器的ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaMembers',
    @level2type = N'COLUMN',
    @level2name = N'ContainerID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'成员的ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaMembers',
    @level2type = N'COLUMN',
    @level2name = N'MemberID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'版本开始时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaMembers',
    @level2type = N'COLUMN',
    @level2name = N'VersionStartTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'版本结束时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaMembers',
    @level2type = N'COLUMN',
    @level2name = N'VersionEndTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'对象的状态',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaMembers',
    @level2type = N'COLUMN',
    @level2name = N'Status'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'内部排序号',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaMembers',
    @level2type = N'COLUMN',
    @level2name = N'InnerSort'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'对象的属性集合',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaMembers',
    @level2type = N'COLUMN',
    @level2name = N'Data'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'对象的Schema名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaMembers',
    @level2type = N'COLUMN',
    @level2name = N'SchemaType'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'容器的Schema名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaMembers',
    @level2type = N'COLUMN',
    @level2name = N'ContainerSchemaType'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'成员的Schema名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaMembers',
    @level2type = N'COLUMN',
    @level2name = N'MemberSchemaType'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'创建时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaMembers',
    @level2type = N'COLUMN',
    @level2name = N'CreateDate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'创建人的ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaMembers',
    @level2type = N'COLUMN',
    @level2name = N'CreatorID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'创建人的名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaMembers',
    @level2type = N'COLUMN',
    @level2name = N'CreatorName'