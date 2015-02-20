CREATE TABLE [dbo].[APPLICATIONS] (
    [ID]              NVARCHAR (36)  NOT NULL,
    [NAME]            NVARCHAR (32)  NOT NULL,
    [CODE_NAME]       NVARCHAR (32)  NOT NULL,
    [DESCRIPTION]     NVARCHAR (128) NULL,
    [SORT_ID]         INT            NOT NULL,
    [RESOURCE_LEVEL]  NVARCHAR (32)  NOT NULL,
    [CHILDREN_COUNT]  INT            CONSTRAINT [DF_APPLICATION_CHILDREN_COUNT] DEFAULT (0) NOT NULL,
    [ADD_SUBAPP]      NCHAR (1)      CONSTRAINT [DF_APPLICATION_ADD_SUBAPP] DEFAULT (N'y') NULL,
    [USE_SCOPE]       NCHAR (1)      CONSTRAINT [DF_APPLICATION_USE_SCOPE] DEFAULT (N'n') NULL,
    [INHERITED_STATE] INT            CONSTRAINT [DF_APPLICATION_INHERITED_STATE] DEFAULT (0) NULL,
    [MODIFY_TIME]     DATETIME       CONSTRAINT [DF_APPLICATION_MODIFY_TIME] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_APPLICATION] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [CODE_NAME]
    ON [dbo].[APPLICATIONS]([CODE_NAME] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_RESOURCE_LEVEL]
    ON [dbo].[APPLICATIONS]([RESOURCE_LEVEL] ASC) WITH (FILLFACTOR = 50, PAD_INDEX = ON);


GO
CREATE TRIGGER [TRIG_APPLICATION_DELETE] ON [dbo].[APPLICATIONS] 
FOR DELETE 
AS
BEGIN
	UPDATE APPLICATIONS SET APPLICATIONS.CHILDREN_COUNT = APPLICATIONS.CHILDREN_COUNT - 1
	    FROM deleted WHERE APPLICATIONS.RESOURCE_LEVEL = LEFT(deleted.RESOURCE_LEVEL, LEN(deleted.RESOURCE_LEVEL) - 3)
	DELETE [SCOPES] FROM deleted WHERE APP_ID = deleted.ID
	DELETE ROLES FROM deleted WHERE APP_ID = deleted.ID
	DELETE [FUNCTIONS] FROM deleted WHERE APP_ID = deleted.ID
	DELETE [FUNCTION_SETS] FROM deleted WHERE APP_ID = deleted.ID
	INSERT INTO APPLICATIONS_DELETE SELECT * FROM deleted
END

GO
CREATE TRIGGER [TRIG_APPLICATION_INSERT] ON [dbo].[APPLICATIONS] 
FOR INSERT  NOT FOR REPLICATION
AS
BEGIN
	UPDATE APPLICATIONS SET APPLICATIONS.CHILDREN_COUNT = APPLICATIONS.CHILDREN_COUNT + 1
	    FROM inserted WHERE APPLICATIONS.RESOURCE_LEVEL = LEFT(inserted.RESOURCE_LEVEL, LEN(inserted.RESOURCE_LEVEL) - 3)
END

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'应用GUID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'APPLICATIONS', @level2type = N'COLUMN', @level2name = N'ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'应用名称', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'APPLICATIONS', @level2type = N'COLUMN', @level2name = N'NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'英文标识', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'APPLICATIONS', @level2type = N'COLUMN', @level2name = N'CODE_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'应用描述', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'APPLICATIONS', @level2type = N'COLUMN', @level2name = N'DESCRIPTION';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'序号 ', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'APPLICATIONS', @level2type = N'COLUMN', @level2name = N'SORT_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'层次级别', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'APPLICATIONS', @level2type = N'COLUMN', @level2name = N'RESOURCE_LEVEL';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'子应用计数器', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'APPLICATIONS', @level2type = N'COLUMN', @level2name = N'CHILDREN_COUNT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否可添加子应用（n：不添加，y：添加）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'APPLICATIONS', @level2type = N'COLUMN', @level2name = N'ADD_SUBAPP';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否使用服务范围（n：不使用，y：使用）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'APPLICATIONS', @level2type = N'COLUMN', @level2name = N'USE_SCOPE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'掩码：1服务范围、2角色、4功能、8角色功能关系、16被授权对象的继承', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'APPLICATIONS', @level2type = N'COLUMN', @level2name = N'INHERITED_STATE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'最后修改时间', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'APPLICATIONS', @level2type = N'COLUMN', @level2name = N'MODIFY_TIME';

