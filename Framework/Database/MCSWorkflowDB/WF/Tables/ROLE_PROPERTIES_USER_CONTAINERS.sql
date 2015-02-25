CREATE TABLE [WF].[ROLE_PROPERTIES_USER_CONTAINERS] (
    [ROLE_ID]       NVARCHAR (36)  NOT NULL,
    [ROW_NUMBER]    INT            NOT NULL,
    [OPERATOR_TYPE] INT            NOT NULL,
    [OPERATOR_ID]   NVARCHAR (36)  NOT NULL,
    [OPERATOR_NAME] NVARCHAR (255) NULL,
	[TENANT_CODE]   NVARCHAR (36)  NULL DEFAULT 'D5561180-7617-4B67-B68B-1F0EA604B509',
    PRIMARY KEY CLUSTERED ([ROLE_ID] ASC, [ROW_NUMBER] ASC, [OPERATOR_TYPE] ASC, [OPERATOR_ID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_ROLE_PROPERTIES_USER_CONTAINERS_OPERATOR_ID]
    ON [WF].[ROLE_PROPERTIES_USER_CONTAINERS]([OPERATOR_ID], [TENANT_CODE]);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'角色ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'ROLE_PROPERTIES_USER_CONTAINERS', @level2type = N'COLUMN', @level2name = N'ROLE_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'行数', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'ROLE_PROPERTIES_USER_CONTAINERS', @level2type = N'COLUMN', @level2name = N'ROW_NUMBER';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'运算类型', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'ROLE_PROPERTIES_USER_CONTAINERS', @level2type = N'COLUMN', @level2name = N'OPERATOR_TYPE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'矩阵ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'ROLE_PROPERTIES_USER_CONTAINERS', @level2type = N'COLUMN', @level2name = N'OPERATOR_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'矩阵名称', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'ROLE_PROPERTIES_USER_CONTAINERS', @level2type = N'COLUMN', @level2name = N'OPERATOR_NAME';


GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'租户编码',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'ROLE_PROPERTIES_USER_CONTAINERS',
    @level2type = N'COLUMN',
    @level2name = N'TENANT_CODE'
GO

CREATE INDEX [IX_ROLE_PROPERTIES_USER_CONTAINERS_TENANT_CODE] ON [WF].[ROLE_PROPERTIES_USER_CONTAINERS] ([TENANT_CODE])
