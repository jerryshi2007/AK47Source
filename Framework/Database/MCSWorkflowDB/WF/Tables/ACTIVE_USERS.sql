CREATE TABLE [WF].[ACTIVE_USERS] (
    [ACTIVE_USER_ID] NVARCHAR (36) NOT NULL,
    [DEPARTMENT_ID]  NVARCHAR (36) NOT NULL,
    [DISPLAY_NAME]   NVARCHAR (64) NULL,
    CONSTRAINT [PK_ACTIVE_USERS_1] PRIMARY KEY CLUSTERED ([ACTIVE_USER_ID] ASC, [DEPARTMENT_ID] ASC)
);


GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'活跃的用户，没有被删除的用户',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'ACTIVE_USERS',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'活跃的用户ID',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'ACTIVE_USERS',
    @level2type = N'COLUMN',
    @level2name = N'ACTIVE_USER_ID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'用户的组织',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'ACTIVE_USERS',
    @level2type = N'COLUMN',
    @level2name = N'DEPARTMENT_ID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'用户的显示名称',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'ACTIVE_USERS',
    @level2type = N'COLUMN',
    @level2name = N'DISPLAY_NAME'