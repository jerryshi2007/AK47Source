CREATE TABLE [dbo].[APPLICATIONS_DELETE] (
    [ID]              NVARCHAR (36)  NOT NULL,
    [NAME]            NVARCHAR (32)  NOT NULL,
    [CODE_NAME]       NVARCHAR (32)  NOT NULL,
    [DESCRIPTION]     NVARCHAR (128) NULL,
    [SORT_ID]         INT            NOT NULL,
    [RESOURCE_LEVEL]  NVARCHAR (32)  NOT NULL,
    [CHILDREN_COUNT]  INT            NOT NULL,
    [ADD_SUBAPP]      NCHAR (1)      NULL,
    [USE_SCOPE]       NCHAR (1)      NULL,
    [INHERITED_STATE] INT            NULL,
    [MODIFY_TIME]     DATETIME       NOT NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_APPLICATIONS_DELETE]
    ON [dbo].[APPLICATIONS_DELETE]([ID] ASC) WITH (FILLFACTOR = 50, PAD_INDEX = ON);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'应用GUID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'APPLICATIONS_DELETE', @level2type = N'COLUMN', @level2name = N'ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'应用名称', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'APPLICATIONS_DELETE', @level2type = N'COLUMN', @level2name = N'NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'英文标识', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'APPLICATIONS_DELETE', @level2type = N'COLUMN', @level2name = N'CODE_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'应用描述', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'APPLICATIONS_DELETE', @level2type = N'COLUMN', @level2name = N'DESCRIPTION';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'序号 ', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'APPLICATIONS_DELETE', @level2type = N'COLUMN', @level2name = N'SORT_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'层次级别', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'APPLICATIONS_DELETE', @level2type = N'COLUMN', @level2name = N'RESOURCE_LEVEL';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'子应用计数器', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'APPLICATIONS_DELETE', @level2type = N'COLUMN', @level2name = N'CHILDREN_COUNT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否可添加子应用（n：不添加，y：添加）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'APPLICATIONS_DELETE', @level2type = N'COLUMN', @level2name = N'ADD_SUBAPP';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否使用服务范围（n：不使用，y：使用）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'APPLICATIONS_DELETE', @level2type = N'COLUMN', @level2name = N'USE_SCOPE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'掩码：1服务范围、2角色、4功能、8角色功能关系、16被授权对象的继承', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'APPLICATIONS_DELETE', @level2type = N'COLUMN', @level2name = N'INHERITED_STATE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'最后修改时间', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'APPLICATIONS_DELETE', @level2type = N'COLUMN', @level2name = N'MODIFY_TIME';

