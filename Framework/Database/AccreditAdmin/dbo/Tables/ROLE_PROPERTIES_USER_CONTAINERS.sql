CREATE TABLE [dbo].[ROLE_PROPERTIES_USER_CONTAINERS] (
    [ROLE_ID]       NVARCHAR (36)  NOT NULL,
    [ROW_NUMBER]    INT            NOT NULL,
    [OPERATOR_TYPE] INT            NOT NULL,
    [OPERATOR_ID]   NVARCHAR (36)  NOT NULL,
    [OPERATOR_NAME] NVARCHAR (255) NULL,
    PRIMARY KEY CLUSTERED ([ROLE_ID] ASC, [ROW_NUMBER] ASC, [OPERATOR_TYPE] ASC, [OPERATOR_ID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_ROLE_PROPERTIES_USER_CONTAINERS_OPERATOR_ID]
    ON [dbo].[ROLE_PROPERTIES_USER_CONTAINERS]([OPERATOR_ID] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'角色ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ROLE_PROPERTIES_USER_CONTAINERS', @level2type = N'COLUMN', @level2name = N'ROLE_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'行数', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ROLE_PROPERTIES_USER_CONTAINERS', @level2type = N'COLUMN', @level2name = N'ROW_NUMBER';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'运算类型', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ROLE_PROPERTIES_USER_CONTAINERS', @level2type = N'COLUMN', @level2name = N'OPERATOR_TYPE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'矩阵ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ROLE_PROPERTIES_USER_CONTAINERS', @level2type = N'COLUMN', @level2name = N'OPERATOR_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'矩阵名称', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ROLE_PROPERTIES_USER_CONTAINERS', @level2type = N'COLUMN', @level2name = N'OPERATOR_NAME';

