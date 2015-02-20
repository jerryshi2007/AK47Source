CREATE TABLE [dbo].[OU_USERS] (
    [PARENT_GUID]    NVARCHAR (36)  NOT NULL,
    [USER_GUID]      NVARCHAR (36)  NOT NULL,
    [DISPLAY_NAME]   NVARCHAR (64)  NOT NULL,
    [OBJ_NAME]       NVARCHAR (64)  NOT NULL,
    [INNER_SORT]     NVARCHAR (6)   CONSTRAINT [DF_OU_USER_INNER_SORT] DEFAULT ('0000') NOT NULL,
    [ORIGINAL_SORT]  NVARCHAR (255) CONSTRAINT [DF_OU_USER_ORIGINAL_SORT] DEFAULT ('0000') NOT NULL,
    [GLOBAL_SORT]    NVARCHAR (255) CONSTRAINT [DF_OU_USER_GLOBAL_SORT] DEFAULT ('0000') NOT NULL,
    [ALL_PATH_NAME]  NVARCHAR (255) CONSTRAINT [DF_OU_USER_ALL_PATH_NAME] DEFAULT (' ') NOT NULL,
    [STATUS]         INT            CONSTRAINT [DF_OU_USERS_STATUS] DEFAULT ((1)) NOT NULL,
    [SIDELINE]       INT            CONSTRAINT [DF_OU_USER_SIDELINE] DEFAULT ((0)) NOT NULL,
    [RANK_NAME]      NVARCHAR (32)  CONSTRAINT [DF_OU_USERS_RANK_NAME] DEFAULT (' ') NULL,
    [ATTRIBUTES]     INT            CONSTRAINT [DF_OU_USER_ATTRIBUTES] DEFAULT ((0)) NOT NULL,
    [DESCRIPTION]    NVARCHAR (255) CONSTRAINT [DF_OU_USERS_DESCRIPTION] DEFAULT (' ') NULL,
    [START_TIME]     DATETIME       CONSTRAINT [DF_OU_USERS_START_TIME] DEFAULT ('2000-01-01') NOT NULL,
    [END_TIME]       DATETIME       CONSTRAINT [DF_OU_USERS_END_TIME] DEFAULT ('9999-12-31') NOT NULL,
    [MODIFY_TIME]    DATETIME       CONSTRAINT [DF_OU_USER_MODIFY_TIME] DEFAULT (getdate()) NOT NULL,
    [CREATE_TIME]    DATETIME       CONSTRAINT [DF_OU_USER_CREATE_TIME] DEFAULT (getdate()) NOT NULL,
    [OUSYSDISTINCT1] NVARCHAR (16)  NULL,
    [OUSYSDISTINCT2] NVARCHAR (32)  NULL,
    [OUSYSCONTENT1]  NVARCHAR (32)  NULL,
    [OUSYSCONTENT2]  NVARCHAR (255) NULL,
    [OUSYSCONTENT3]  NVARCHAR (128) NULL,
    [SEARCH_NAME]    NVARCHAR (255) NULL,
    CONSTRAINT [PK_OU_USER] PRIMARY KEY CLUSTERED ([PARENT_GUID] ASC, [USER_GUID] ASC),
    CONSTRAINT [IX_OU_USERS_ALL_PATH_NAME] UNIQUE NONCLUSTERED ([ALL_PATH_NAME] ASC),
    CONSTRAINT [IX_OU_USERS_ORIGINAL_SORT] UNIQUE NONCLUSTERED ([ORIGINAL_SORT] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_OU_USERS_USER_OBJ_NAME]
    ON [dbo].[OU_USERS]([OBJ_NAME] ASC);


GO

/*3------------OU_USERS[Insert,Delete]*/
CREATE TRIGGER [dbo].[DELETE_OU_USERS]
ON [dbo].[OU_USERS] 
FOR DELETE 
AS
BEGIN
	UPDATE ORGANIZATIONS SET ORGANIZATIONS.MODIFY_TIME = GETDATE() FROM DELETED WHERE ORGANIZATIONS.GUID = DELETED.PARENT_GUID;
	DELETE GROUP_USERS FROM DELETED WHERE GROUP_USERS.USER_GUID = DELETED.USER_GUID AND GROUP_USERS.USER_PARENT_GUID = DELETED.PARENT_GUID
END


GO

CREATE TRIGGER [dbo].[INSERT_UPDATE_OU_USERS]
ON [dbo].[OU_USERS]
FOR INSERT, UPDATE
AS
BEGIN
	DECLARE @@ParentID AS NVARCHAR(36)
	DECLARE @@ObjName AS NVARCHAR(255)
	DECLARE @@AllPathName AS NVARCHAR(512)
	DECLARE @@SysDis1 AS NVARCHAR(255)
	DECLARE @@SysDis2 AS NVARCHAR(255)
	DECLARE @@RowNum AS INT
	SELECT @@ParentID = [PARENT_GUID], @@objName = OBJ_NAME, @@SysDis1 = OUSYSDISTINCT1, @@SysDis2 = OUSYSDISTINCT2, @@AllPathName=ALL_PATH_NAME FROM INSERTED
	
	/*
	SELECT @@RowNum=COUNT(*) FROM OU_USERS WHERE OUSYSDISTINCT1 IS NOT NULL AND OUSYSDISTINCT1 = @@SysDis1
	IF @@RowNum > 1
	BEGIN
		RAISERROR('对不起，系统中已经存在了一个“备用字段1”为“%s”的数据项!', 16, 1, @@SysDis1)
	END
	SELECT @@RowNum=COUNT(*) FROM OU_USERS WHERE OUSYSDISTINCT2 IS NOT NULL AND OUSYSDISTINCT2 = @@SysDis2
	IF @@RowNum > 1
	BEGIN
		RAISERROR('对不起，系统中已经存在了一个“备用字段2”为“%s”的数据项!', 16, 1, @@SysDis2)
	END
	*/

	IF @@ObjName <> RIGHT(@@AllPathName, LEN(@@ObjName))
	BEGIN
		RAISERROR('对不起，新增加“人员”的“系统位置（%s）”与“对象名称（%s）”不符！', 16, 1, @@AllPathName, @@ObjName)
	END

	SELECT @@RowNum=COUNT(*) 
	FROM 	(	SELECT GUID,  ALL_PATH_NAME FROM GROUPS WHERE ALL_PATH_NAME = @@AllPathName
			UNION
			SELECT GUID,  ALL_PATH_NAME FROM ORGANIZATIONS WHERE ALL_PATH_NAME = @@AllPathName AND [GUID] <> @@ParentID 
			UNION
			SELECT USER_GUID,  ALL_PATH_NAME FROM OU_USERS WHERE ALL_PATH_NAME = @@AllPathName
		) T
	IF @@RowNum > 1
	BEGIN
		RAISERROR('对不起，系统中已经存在"系统位置"--“%s”，请换一个新的对象名称！', 16, 1, @@AllPathName)
	END
END


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'所在部门的标志ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'OU_USERS', @level2type = N'COLUMN', @level2name = N'PARENT_GUID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'用户的标志ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'OU_USERS', @level2type = N'COLUMN', @level2name = N'USER_GUID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'用户的显示名称', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'OU_USERS', @level2type = N'COLUMN', @level2name = N'DISPLAY_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'用户的对象名称（解决兼职情况下不允许重名情况）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'OU_USERS', @level2type = N'COLUMN', @level2name = N'OBJ_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'用户在部门中的排序', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'OU_USERS', @level2type = N'COLUMN', @level2name = N'INNER_SORT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'用户在系统中的全地址（不用于排序，仅仅标志所在部门的路径关系）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'OU_USERS', @level2type = N'COLUMN', @level2name = N'ORIGINAL_SORT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'用户在部门中的全地址（用于全国大排序）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'OU_USERS', @level2type = N'COLUMN', @level2name = N'GLOBAL_SORT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'用户在系统中的全程文字表述（例如：全国海关\海关总署\信息中心\应用开发二处\朱佳炜）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'OU_USERS', @level2type = N'COLUMN', @level2name = N'ALL_PATH_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'状态（1、正常使用；2、直接逻辑删除；4、机构级联逻辑删除；8、人员级联逻辑删除；）掩码方式实现', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'OU_USERS', @level2type = N'COLUMN', @level2name = N'STATUS';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'改职位是否为兼职（0、主职；1、兼职）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'OU_USERS', @level2type = N'COLUMN', @level2name = N'SIDELINE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'用户在该部门中的职位', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'OU_USERS', @level2type = N'COLUMN', @level2name = N'RANK_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'用户的属性标志（普通成员0，党组成员1、署管干部2、交流干部4、借调干部8）掩码实现', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'OU_USERS', @level2type = N'COLUMN', @level2name = N'ATTRIBUTES';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'用户的附加描述信息', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'OU_USERS', @level2type = N'COLUMN', @level2name = N'DESCRIPTION';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'关系启用时间', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'OU_USERS', @level2type = N'COLUMN', @level2name = N'START_TIME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'关系结束时间', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'OU_USERS', @level2type = N'COLUMN', @level2name = N'END_TIME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'关系最近修改时间', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'OU_USERS', @level2type = N'COLUMN', @level2name = N'MODIFY_TIME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'关系建立的时间', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'OU_USERS', @level2type = N'COLUMN', @level2name = N'CREATE_TIME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'备用字段1(16位,不允许重复)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'OU_USERS', @level2type = N'COLUMN', @level2name = N'OUSYSDISTINCT1';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'备用字段2(32位,不允许重复)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'OU_USERS', @level2type = N'COLUMN', @level2name = N'OUSYSDISTINCT2';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'备用字段3(32位,允许重复)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'OU_USERS', @level2type = N'COLUMN', @level2name = N'OUSYSCONTENT1';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'备用字段4(64位,允许重复)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'OU_USERS', @level2type = N'COLUMN', @level2name = N'OUSYSCONTENT2';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'备用字段5(128位,允许重复)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'OU_USERS', @level2type = N'COLUMN', @level2name = N'OUSYSCONTENT3';

