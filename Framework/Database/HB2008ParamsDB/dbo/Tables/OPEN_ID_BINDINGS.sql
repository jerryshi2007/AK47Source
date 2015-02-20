CREATE TABLE [dbo].[OPEN_ID_BINDINGS]
(
	[OPEN_ID] NVARCHAR(36) NOT NULL , 
    [USER_ID] NVARCHAR(36) NOT NULL, 
    [OPEN_ID_TYPE] NVARCHAR(64) NULL, 
    [CREATE_TIME] DATETIME NULL DEFAULT GETDATE(), 
    CONSTRAINT [PK_OPEN_ID_BINDINGS] PRIMARY KEY ([OPEN_ID]) 
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'用户ID的绑定列表，一个OpenID只能绑定一个用户',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'OPEN_ID_BINDINGS',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'外部用户的OPEN_ID',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'OPEN_ID_BINDINGS',
    @level2type = N'COLUMN',
    @level2name = N'OPEN_ID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'内部的用户',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'OPEN_ID_BINDINGS',
    @level2type = N'COLUMN',
    @level2name = N'USER_ID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'外部OpenID的来源类型，例如QQ、WeChat等',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'OPEN_ID_BINDINGS',
    @level2type = N'COLUMN',
    @level2name = N'OPEN_ID_TYPE'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'记录创建时间',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'OPEN_ID_BINDINGS',
    @level2type = N'COLUMN',
    @level2name = N'CREATE_TIME'
GO

CREATE INDEX [IX_OPEN_ID_BINDINGS_USER_ID] ON [dbo].[OPEN_ID_BINDINGS] ([USER_ID])
