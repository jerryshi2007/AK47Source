CREATE TABLE [SC].[Acl]
(
	[ContainerID]			NVARCHAR (36)  NOT NULL,
	[ContainerPermission]	NVARCHAR(64) NOT NULL,
    [MemberID]				NVARCHAR (36)  NOT NULL,
    [VersionStartTime]		DATETIME       NOT NULL,
    [VersionEndTime]		DATETIME       CONSTRAINT [DF_Acl_VersionEndTime] DEFAULT ('99990909 00:00:00') NULL,
    [Status]				INT            CONSTRAINT [DF_Acl_Status] DEFAULT ((1)) NULL,
    [SortID]				INT            CONSTRAINT [DF_Acl_InnerSort] DEFAULT ((0)) NULL,
    [Data]					XML            NULL,
    [ContainerSchemaType]	NVARCHAR(64) NULL, 
    [MemberSchemaType]		NVARCHAR(64) NULL, 
    CONSTRAINT [PK_Acl] PRIMARY KEY CLUSTERED ([ContainerID] ASC, [ContainerPermission] ASC, [MemberID] ASC, [VersionStartTime] DESC)
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'权限中心的访问控制表',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'Acl',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'容器的ID，例如组织或群组',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'Acl',
    @level2type = N'COLUMN',
    @level2name = N'ContainerID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'容器所对应的操作类型',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'Acl',
    @level2type = N'COLUMN',
    @level2name = N'ContainerPermission'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'容器下的成员ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'Acl',
    @level2type = N'COLUMN',
    @level2name = N'MemberID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'版本开始时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'Acl',
    @level2type = N'COLUMN',
    @level2name = N'VersionStartTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'版本完成时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'Acl',
    @level2type = N'COLUMN',
    @level2name = N'VersionEndTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'对象的状态',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'Acl',
    @level2type = N'COLUMN',
    @level2name = N'Status'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'对象的序号',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'Acl',
    @level2type = N'COLUMN',
    @level2name = 'SortID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'扩展数据，暂时没有用',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'Acl',
    @level2type = N'COLUMN',
    @level2name = N'Data'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'容器的类型',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'Acl',
    @level2type = N'COLUMN',
    @level2name = N'ContainerSchemaType'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'成员的类型',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'Acl',
    @level2type = N'COLUMN',
    @level2name = N'MemberSchemaType'
GO

GO

GO

GO

CREATE INDEX [IX_Acl_MemberID] ON [SC].[Acl] ([MemberID])
