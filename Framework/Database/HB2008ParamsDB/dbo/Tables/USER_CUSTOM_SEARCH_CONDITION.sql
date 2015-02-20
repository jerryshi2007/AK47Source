CREATE TABLE [dbo].[USER_CUSTOM_SEARCH_CONDITION]
(
    [ID] [nvarchar](36) NOT NULL,
	[USER_ID] [nvarchar](36) NOT NULL,
	[CONDITION_NAME] [nvarchar](50) NOT NULL,
	[CONDITION_CONTENT] [nvarchar](max) NOT NULL,
	[CONDITION_TYPE] [nvarchar](50) NOT NULL,
	[CREATE_TIME] [datetime] NOT NULL,
	[RESOURCE_ID] [nvarchar](36) NOT NULL,
	CONSTRAINT [PK_USER_CUSTOM_SEARCH_CONDITION] PRIMARY KEY CLUSTERED ([ID] ASC)
);
GO


CREATE INDEX [IX_USER_CUSTOM_SEARCH_CONDITION_USER_ID] ON [dbo].[USER_CUSTOM_SEARCH_CONDITION] ([USER_ID], [RESOURCE_ID])

GO

CREATE INDEX [IX_USER_CUSTOM_SEARCH_CONDITION_RESOURCE_ID] ON [dbo].[USER_CUSTOM_SEARCH_CONDITION] ([RESOURCE_ID])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'编码',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'USER_CUSTOM_SEARCH_CONDITION',
    @level2type = N'COLUMN',
    @level2name = N'ID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'用户ID',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'USER_CUSTOM_SEARCH_CONDITION',
    @level2type = N'COLUMN',
    @level2name = N'USER_ID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'条件名称',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'USER_CUSTOM_SEARCH_CONDITION',
    @level2type = N'COLUMN',
    @level2name = N'CONDITION_NAME'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'条件内容',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'USER_CUSTOM_SEARCH_CONDITION',
    @level2type = N'COLUMN',
    @level2name = N'CONDITION_CONTENT'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'条件类型',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'USER_CUSTOM_SEARCH_CONDITION',
    @level2type = N'COLUMN',
    @level2name = N'CONDITION_TYPE'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'创建时间',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'USER_CUSTOM_SEARCH_CONDITION',
    @level2type = N'COLUMN',
    @level2name = N'CREATE_TIME'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'资源ID',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'USER_CUSTOM_SEARCH_CONDITION',
    @level2type = N'COLUMN',
    @level2name = N'RESOURCE_ID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'用户保存的搜索条件的历史',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'USER_CUSTOM_SEARCH_CONDITION',
    @level2type = NULL,
    @level2name = NULL