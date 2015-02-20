CREATE TABLE [dbo].[GROUPS] (
    [GUID]          NVARCHAR (36)  NOT NULL,
    [PARENT_GUID]   NVARCHAR (36)  NOT NULL,
    [DISPLAY_NAME]  NVARCHAR (32)  NOT NULL,
    [OBJ_NAME]      NVARCHAR (32)  NOT NULL,
    [INNER_SORT]    NVARCHAR (6)   NOT NULL,
    [ORIGINAL_SORT] NVARCHAR (255) NOT NULL,
    [GLOBAL_SORT]   NVARCHAR (255) NOT NULL,
    [ALL_PATH_NAME] NVARCHAR (255) NOT NULL,
    [STATUS]        INT            CONSTRAINT [DF_GROUPS_STATUS] DEFAULT (1) NOT NULL,
    [ATTRIBUTES]    INT            CONSTRAINT [DF_GROUP_ATTRIBUTES] DEFAULT (0) NULL,
    [DESCRIPTION]   NVARCHAR (255) NULL,
    [CREATE_TIME]   DATETIME       CONSTRAINT [DF_GROUP_CREATE_TIME] DEFAULT (getdate()) NOT NULL,
    [MODIFY_TIME]   DATETIME       CONSTRAINT [DF_GROUP_MODIFY_TIME] DEFAULT (getdate()) NOT NULL,
    [SYSDISTINCT1]  NVARCHAR (16)  NULL,
    [SYSDISTINCT2]  NVARCHAR (32)  NULL,
    [SYSCONTENT1]   NVARCHAR (32)  NULL,
    [SYSCONTENT2]   NVARCHAR (64)  CONSTRAINT [DF_GROUPS_SYSCONTENT2] DEFAULT (N'检索名称') NULL,
    [SYSCONTENT3]   NVARCHAR (128) NULL,
    [SEARCH_NAME]   NVARCHAR (255) NULL,
    CONSTRAINT [PK_GROUP] PRIMARY KEY CLUSTERED ([GUID] ASC),
    CONSTRAINT [IX_GROUPS_ALL_PATH_NAME] UNIQUE NONCLUSTERED ([ALL_PATH_NAME] ASC),
    CONSTRAINT [IX_GROUPS_ORIGINAL_SORT] UNIQUE NONCLUSTERED ([ORIGINAL_SORT] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_GROUPS_PARENT_GUID]
    ON [dbo].[GROUPS]([PARENT_GUID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_GROUPS_OBJ_NAME]
    ON [dbo].[GROUPS]([OBJ_NAME] ASC);


GO
/*2------------GROUPS[Insert,Delete]*/
CREATE TRIGGER [dbo].[DELETE_GROUPS]
ON [dbo].[GROUPS] 
FOR DELETE 
AS
BEGIN
	DELETE GROUP_USERS FROM DELETED WHERE GROUP_USERS.GROUP_GUID = DELETED.GUID
	UPDATE ORGANIZATIONS SET ORGANIZATIONS.MODIFY_TIME = GETDATE() FROM DELETED WHERE ORGANIZATIONS.GUID = DELETED.PARENT_GUID
END


GO

CREATE TRIGGER [dbo].[INSERT_UPDATE_GROUPS]
ON [dbo].[GROUPS] 
FOR INSERT, UPDATE
AS
BEGIN
	DECLARE @@ObjName AS NVARCHAR(255)
	DECLARE @@AllPathName AS NVARCHAR(512)
	DECLARE @@SysDis1 AS NVARCHAR(255)
	DECLARE @@SysDis2 AS NVARCHAR(255)
	DECLARE @@RowNum AS INT
	SELECT @@objName = OBJ_NAME, @@SysDis1 = SYSDISTINCT1, @@SysDis2 = SYSDISTINCT2, @@AllPathName=ALL_PATH_NAME FROM INSERTED
	
	/*
	SELECT @@RowNum=COUNT(*) FROM GROUPS WHERE SYSDISTINCT1 IS NOT NULL AND SYSDISTINCT1 = @@SysDis1
	IF @@RowNum > 1
	BEGIN
		RAISERROR('对不起，系统中已经存在了一个“备用字段1”为“%s”的数据项!', 16, 1, @@SysDis1)
	END
	SELECT @@RowNum=COUNT(*) FROM GROUPS WHERE SYSDISTINCT2 IS NOT NULL AND SYSDISTINCT2 = @@SysDis2
	IF @@RowNum > 1
	BEGIN
		RAISERROR('对不起，系统中已经存在了一个“备用字段2”为“%s”的数据项!', 16, 1, @@SysDis2)
	END
	*/
	
	IF @@ObjName <> RIGHT(@@AllPathName, LEN(@@ObjName))
	BEGIN
		RAISERROR('对不起，新增加“人员组”的“系统位置（%s）”与“对象名称（%s）”不符！', 16, 1, @@AllPathName, @@ObjName)
	END
	SELECT @@RowNum=COUNT(*) 
	FROM 	(	SELECT GUID,  ALL_PATH_NAME FROM GROUPS WHERE ALL_PATH_NAME = @@AllPathName
			UNION
			SELECT GUID,  ALL_PATH_NAME FROM ORGANIZATIONS WHERE ALL_PATH_NAME = @@AllPathName
			UNION
			SELECT USER_GUID,  ALL_PATH_NAME FROM OU_USERS WHERE ALL_PATH_NAME = @@AllPathName
		) T
	IF @@RowNum > 1
	BEGIN
		RAISERROR('对不起，系统中已经存在"系统位置"--“%s”，请换一个新的对象名称！', 16, 1, @@AllPathName)
	END
END


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'人员组的标志ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'GROUPS', @level2type = N'COLUMN', @level2name = N'GUID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'人员组所在部门的标志ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'GROUPS', @level2type = N'COLUMN', @level2name = N'PARENT_GUID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'显示名称', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'GROUPS', @level2type = N'COLUMN', @level2name = N'DISPLAY_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'对象名称', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'GROUPS', @level2type = N'COLUMN', @level2name = N'OBJ_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'组在部门中的排序', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'GROUPS', @level2type = N'COLUMN', @level2name = N'INNER_SORT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'组在系统中的全地址（不用于排序，仅仅标志所在部门的路径关系）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'GROUPS', @level2type = N'COLUMN', @level2name = N'ORIGINAL_SORT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'组在部门中的全地址（用于全国大排序）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'GROUPS', @level2type = N'COLUMN', @level2name = N'GLOBAL_SORT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'组在系统中的全程文字表述（例如：全国海关\海关总署\办公厅\办公厅所有成员组）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'GROUPS', @level2type = N'COLUMN', @level2name = N'ALL_PATH_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'状态（1、正常使用；2、直接逻辑删除；4、机构级联逻辑删除；8、人员级联逻辑删除；）掩码方式实现', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'GROUPS', @level2type = N'COLUMN', @level2name = N'STATUS';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'组的特殊属性（待定）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'GROUPS', @level2type = N'COLUMN', @level2name = N'ATTRIBUTES';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'附加信息', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'GROUPS', @level2type = N'COLUMN', @level2name = N'DESCRIPTION';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'创建时间', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'GROUPS', @level2type = N'COLUMN', @level2name = N'CREATE_TIME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改时间', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'GROUPS', @level2type = N'COLUMN', @level2name = N'MODIFY_TIME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'备用字段1(16位,不允许重复)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'GROUPS', @level2type = N'COLUMN', @level2name = N'SYSDISTINCT1';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'备用字段2(32位,不允许重复)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'GROUPS', @level2type = N'COLUMN', @level2name = N'SYSDISTINCT2';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'备用字段3(32位,允许重复)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'GROUPS', @level2type = N'COLUMN', @level2name = N'SYSCONTENT1';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'备用字段4(64位,允许重复)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'GROUPS', @level2type = N'COLUMN', @level2name = N'SYSCONTENT2';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'备用字段5(128位,允许重复)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'GROUPS', @level2type = N'COLUMN', @level2name = N'SYSCONTENT3';

