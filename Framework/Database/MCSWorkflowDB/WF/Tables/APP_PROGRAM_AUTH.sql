CREATE TABLE [WF].[APP_PROGRAM_AUTH]
(
	[APPLICATION_NAME] NVARCHAR(64) NOT NULL , 
    [PROGRAM_NAME] NVARCHAR(64) NOT NULL, 
    [AUTH_TYPE] NVARCHAR(64) NOT NULL, 
    [ROLE_ID] NVARCHAR(36) NULL, 
    [ROLE_DESCRIPTION] NVARCHAR(MAX) NULL, 
    [CREATE_TIME] DATETIME NULL DEFAULT GETDATE(), 
	[TENANT_CODE] NVARCHAR(36) NOT NULL DEFAULT 'D5561180-7617-4B67-B68B-1F0EA604B509'
    PRIMARY KEY ([APPLICATION_NAME], [PROGRAM_NAME], [AUTH_TYPE], [TENANT_CODE])
)

GO

CREATE INDEX [IX_APP_PROGRAM_AUTH_TENANT_CODE] ON [WF].[APP_PROGRAM_AUTH] ([TENANT_CODE])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'应用名称',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'APP_PROGRAM_AUTH',
    @level2type = N'COLUMN',
    @level2name = N'APPLICATION_NAME'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'应用模块名称',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'APP_PROGRAM_AUTH',
    @level2type = N'COLUMN',
    @level2name = N'PROGRAM_NAME'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'授权类型',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'APP_PROGRAM_AUTH',
    @level2type = N'COLUMN',
    @level2name = N'AUTH_TYPE'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'角色ID',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'APP_PROGRAM_AUTH',
    @level2type = N'COLUMN',
    @level2name = N'ROLE_ID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'角色描述',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'APP_PROGRAM_AUTH',
    @level2type = N'COLUMN',
    @level2name = N'ROLE_DESCRIPTION'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'创建时间',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'APP_PROGRAM_AUTH',
    @level2type = N'COLUMN',
    @level2name = N'CREATE_TIME'
GO

CREATE INDEX [IX_APP_PROGRAM_AUTH_ROLE_ID] ON [WF].[APP_PROGRAM_AUTH] ([ROLE_ID])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'流程实例按照应用类别授权管理的配置表',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'APP_PROGRAM_AUTH',
    @level2type = NULL,
    @level2name = NULL