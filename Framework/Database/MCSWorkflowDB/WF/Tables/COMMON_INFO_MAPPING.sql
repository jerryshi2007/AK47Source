CREATE TABLE [WF].[COMMON_INFO_MAPPING](
	[COMMON_INFO_ID] [nvarchar](36) NOT NULL,
	[RESOURCE_ID] [nvarchar](36) NOT NULL,
	[PROCESS_ID] [nvarchar](36) NOT NULL,
	[TENANT_CODE] NVARCHAR(36) NULL DEFAULT 'D5561180-7617-4B67-B68B-1F0EA604B509'
	CONSTRAINT [PK_COMMON_INFO_MAPPING] PRIMARY KEY CLUSTERED 
	(
		[RESOURCE_ID] ASC,
		[PROCESS_ID] ASC
	)
);

GO

CREATE INDEX [IX_COMMON_INFO_MAPPING_TENANT_CODE] ON [WF].[COMMON_INFO_MAPPING] ([TENANT_CODE])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'全文检索信息（APPLICATIONS_COMMON_INFO）中的ID和ResourceID和ProcessID的对应表',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'COMMON_INFO_MAPPING',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'全文检索表的ID',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'COMMON_INFO_MAPPING',
    @level2type = N'COLUMN',
    @level2name = N'COMMON_INFO_ID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'表单ID',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'COMMON_INFO_MAPPING',
    @level2type = N'COLUMN',
    @level2name = N'RESOURCE_ID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'流程ID',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'COMMON_INFO_MAPPING',
    @level2type = N'COLUMN',
    @level2name = N'PROCESS_ID'