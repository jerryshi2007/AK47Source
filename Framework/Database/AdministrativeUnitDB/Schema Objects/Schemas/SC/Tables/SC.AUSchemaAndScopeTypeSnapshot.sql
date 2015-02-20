/*
暂时不要
CREATE TABLE [SC].[AUSchemaAndScopeTypeSnapshot]
(
	[AUSchemaID] NVARCHAR(64) NOT NULL, 
    [AdminScopeSchemaType] NVARCHAR(64) NOT NULL, 
    [VersionStartTime] DATETIME      NOT NULL,
    [VersionEndTime]   DATETIME      CONSTRAINT [DF_SchemaAndScopeType_VersionEndTime] DEFAULT ('99990909 00:00:00') NULL,
    [Status]           INT           CONSTRAINT [DF_SchemaAndScopeType_Status] DEFAULT ((1)) NULL,
)

GO

EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'管理单元架构的ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'AUSchemaAndScopeTypeSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'AUSchemaID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'管理单元管理范围的SchemaType',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'AUSchemaAndScopeTypeSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'AdminScopeSchemaType'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'版本开始时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'AUSchemaAndScopeTypeSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'VersionStartTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'版本结束时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'AUSchemaAndScopeTypeSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'VersionEndTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'状态',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'AUSchemaAndScopeTypeSnapshot',
    @level2type = N'COLUMN',
    @level2name = N'Status'
	*/