CREATE TABLE [WF].[APPLICATIONS_ALIAS]
(
	[APP_ALIAS_CODE_NAME] NVARCHAR(64) NOT NULL , 
    [APP_CODE_NAME] NVARCHAR(64) NOT NULL, 
    PRIMARY KEY ([APP_ALIAS_CODE_NAME], [APP_CODE_NAME])
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'应用别名表，用于多个名称对应到同一个CODE_NAME',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'APPLICATIONS_ALIAS',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'别名',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'APPLICATIONS_ALIAS',
    @level2type = N'COLUMN',
    @level2name = N'APP_ALIAS_CODE_NAME'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'对应的应用的CODE_NAME',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'APPLICATIONS_ALIAS',
    @level2type = N'COLUMN',
    @level2name = 'APP_CODE_NAME'
GO

CREATE INDEX [IX_APPLICATIONS_ALIAS_APP_CODE_NAME] ON [WF].[APPLICATIONS_ALIAS] ([APP_CODE_NAME])
