CREATE TABLE [dbo].[ORGANIZATIONS] (
    [GUID]             NVARCHAR (36)  NOT NULL,
    [DISPLAY_NAME]     NVARCHAR (64)  NOT NULL,
    [OBJ_NAME]         NVARCHAR (64)  NOT NULL,
    [PARENT_GUID]      NVARCHAR (36)  NULL,
    [RANK_CODE]        NVARCHAR (32)  NOT NULL,
    [INNER_SORT]       NVARCHAR (6)   CONSTRAINT [DF_ORGANIZATION_INNER_SORT] DEFAULT ((0)) NOT NULL,
    [ORIGINAL_SORT]    NVARCHAR (255) CONSTRAINT [DF_ORGANIZATION_ORIGINAL_SORT] DEFAULT ((0)) NOT NULL,
    [GLOBAL_SORT]      NVARCHAR (255) CONSTRAINT [DF_ORGANIZATION_GLOBAL_SORT] DEFAULT ((0)) NOT NULL,
    [ALL_PATH_NAME]    NVARCHAR (255) NOT NULL,
    [ORG_CLASS]        INT            CONSTRAINT [DF_ORGANIZATIONS_ORG_CLASS] DEFAULT ((0)) NOT NULL,
    [ORG_TYPE]         INT            CONSTRAINT [DF_ORGANIZATIONS_ORG_TYPE] DEFAULT ((2)) NOT NULL,
    [CHILDREN_COUNTER] INT            CONSTRAINT [DF_ORGANIZATIONS_CHILDREN_COUNTER] DEFAULT ((0)) NOT NULL,
    [STATUS]           INT            CONSTRAINT [DF_ORGANIZATIONS_STATUS] DEFAULT ((1)) NOT NULL,
    [CUSTOMS_CODE]     NVARCHAR (4)   NULL,
    [DESCRIPTION]      NVARCHAR (255) NULL,
    [CREATE_TIME]      DATETIME       CONSTRAINT [DF_ORGANIZATIONS_CREATE_TIME] DEFAULT (getdate()) NOT NULL,
    [MODIFY_TIME]      DATETIME       CONSTRAINT [DF_ORGANIZATIONS_MODIFY_TIME] DEFAULT (getdate()) NOT NULL,
    [SYSDISTINCT1]     NVARCHAR (16)  NULL,
    [SYSDISTINCT2]     NVARCHAR (32)  NULL,
    [SYSCONTENT1]      NVARCHAR (32)  NULL,
    [SYSCONTENT2]      NVARCHAR (64)  NULL,
    [SYSCONTENT3]      NVARCHAR (128) NULL,
    [SEARCH_NAME]      NVARCHAR (255) NULL,
    CONSTRAINT [PK_ORGANIZATION] PRIMARY KEY CLUSTERED ([GUID] ASC),
    CONSTRAINT [IX_ORGANIZATIONS_ALL_PATH_NAME] UNIQUE NONCLUSTERED ([ALL_PATH_NAME] ASC),
    CONSTRAINT [IX_ORGANIZATIONS_ORIGINAL_SORT] UNIQUE NONCLUSTERED ([ORIGINAL_SORT] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_ORGANIZATIONS_PARENT_GUID]
    ON [dbo].[ORGANIZATIONS]([PARENT_GUID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ORGANIZATIONS_OBJ_NAME]
    ON [dbo].[ORGANIZATIONS]([OBJ_NAME] ASC);


GO

/*4------------ORGANIZATIONS[Insert,Delete]*/
CREATE TRIGGER [dbo].[DELETE_ORGANIZATIONS]
ON [dbo].[ORGANIZATIONS] 
FOR DELETE 
AS
BEGIN
	DELETE ORGANIZATIONS FROM DELETED WHERE ORGANIZATIONS.ORIGINAL_SORT LIKE DELETED.ORIGINAL_SORT + '%'
	DELETE OU_USERS FROM DELETED WHERE OU_USERS.PARENT_GUID = DELETED.GUID
	DELETE GROUPS FROM DELETED WHERE GROUPS.PARENT_GUID = DELETED.GUID
	UPDATE ORGANIZATIONS SET MODIFY_TIME = GETDATE() FROM DELETED WHERE ORGANIZATIONS.GUID = DELETED.PARENT_GUID
END


GO

CREATE TRIGGER [dbo].[INSERT_UPDATE_ORGANIZATIONS]
ON [dbo].[ORGANIZATIONS]
FOR INSERT, UPDATE
AS
BEGIN
	DECLARE @@ObjID AS NVARCHAR(36)
	DECLARE @@ObjName AS NVARCHAR(255)
	DECLARE @@AllPathName AS NVARCHAR(512)
	DECLARE @@SysDis1 AS NVARCHAR(255)
	DECLARE @@SysDis2 AS NVARCHAR(255)
	DECLARE @@RowNum AS INT
	
	SELECT @@ObjID = [GUID], @@objName = OBJ_NAME, @@SysDis1 = SYSDISTINCT1, @@SysDis2 = SYSDISTINCT2, @@AllPathName=ALL_PATH_NAME FROM INSERTED
	
	/*
	SELECT @@RowNum=COUNT(*) FROM ORGANIZATIONS WHERE SYSDISTINCT1 IS NOT NULL AND SYSDISTINCT1 = @@SysDis1
	IF @@RowNum > 1
	BEGIN
		RAISERROR('对不起，系统中已经存在了一个“备用字段1”为“%s”的数据项!', 16, 1, @@SysDis1)
	END
	SELECT @@RowNum=COUNT(*) FROM ORGANIZATIONS WHERE SYSDISTINCT2 IS NOT NULL AND SYSDISTINCT2 = @@SysDis2
	IF @@RowNum > 1
	BEGIN
		RAISERROR('对不起，系统中已经存在了一个“备用字段2”为“%s”的数据项!', 16, 1, @@SysDis2)
	END
	*/
	
	IF @@ObjName <> RIGHT(@@AllPathName, LEN(@@ObjName))
	BEGIN
		RAISERROR('对不起，新增加“机构”的“系统位置（%s）”与“对象名称（%s）”不符！', 16, 1, @@AllPathName, @@ObjName)
	END
	
	SELECT @@RowNum=COUNT(*) 
	FROM 	(	SELECT GUID,  ALL_PATH_NAME FROM GROUPS WHERE ALL_PATH_NAME = @@AllPathName
			UNION
			SELECT GUID, ALL_PATH_NAME FROM ORGANIZATIONS WHERE ALL_PATH_NAME = @@AllPathName AND @@ObjID <> [GUID]
			UNION
			SELECT USER_GUID,  ALL_PATH_NAME FROM OU_USERS WHERE ALL_PATH_NAME = @@AllPathName
		) T
	IF @@RowNum > 1
	BEGIN
		RAISERROR('对不起，系统中已经存在"系统位置"--“%s”，请换一个新的对象名称！', 16, 1, @@AllPathName)
	END
END


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'部门的标志ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ORGANIZATIONS', @level2type = N'COLUMN', @level2name = N'GUID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'显示名称', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ORGANIZATIONS', @level2type = N'COLUMN', @level2name = N'DISPLAY_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'对象名称（部门内唯一）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ORGANIZATIONS', @level2type = N'COLUMN', @level2name = N'OBJ_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'父部门的标志ID（注：树结构中第一个节点没有值）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ORGANIZATIONS', @level2type = N'COLUMN', @level2name = N'PARENT_GUID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'机构的行政级别信息数据', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ORGANIZATIONS', @level2type = N'COLUMN', @level2name = N'RANK_CODE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'部门内部排序号', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ORGANIZATIONS', @level2type = N'COLUMN', @level2name = N'INNER_SORT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'在系统中的全地址（不用于排序，仅仅标志所在部门的路径关系）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ORGANIZATIONS', @level2type = N'COLUMN', @level2name = N'ORIGINAL_SORT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'用户在部门中的全地址（用于全国大排序）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ORGANIZATIONS', @level2type = N'COLUMN', @level2name = N'GLOBAL_SORT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'用户在系统中的全程文字表述（例如：全国海关\海关总署\信息中心\应用开发二处）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ORGANIZATIONS', @level2type = N'COLUMN', @level2name = N'ALL_PATH_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'部门的一些特殊属性（1总署、2分署、4特派办、8直属、16院校、32隶属海关、64派驻机构）采用掩码实现', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ORGANIZATIONS', @level2type = N'COLUMN', @level2name = N'ORG_CLASS';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'部门的一些特殊属性（1虚拟机构、2一般部门、4办公室（厅）、8综合处）采用掩码实现', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ORGANIZATIONS', @level2type = N'COLUMN', @level2name = N'ORG_TYPE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'记录部门内部使用的最大号值（记录值为下一个可使用值，从0开始）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ORGANIZATIONS', @level2type = N'COLUMN', @level2name = N'CHILDREN_COUNTER';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'状态（1、正常使用；2、直接逻辑删除；4、机构级联逻辑删除；8、人员级联逻辑删除；）掩码方式实现', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ORGANIZATIONS', @level2type = N'COLUMN', @level2name = N'STATUS';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'关区代码', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ORGANIZATIONS', @level2type = N'COLUMN', @level2name = N'CUSTOMS_CODE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'附加说明信息', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ORGANIZATIONS', @level2type = N'COLUMN', @level2name = N'DESCRIPTION';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'创建时间', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ORGANIZATIONS', @level2type = N'COLUMN', @level2name = N'CREATE_TIME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'最近修改时间', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ORGANIZATIONS', @level2type = N'COLUMN', @level2name = N'MODIFY_TIME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'备用字段1(16位,不允许重复)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ORGANIZATIONS', @level2type = N'COLUMN', @level2name = N'SYSDISTINCT1';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'备用字段2(32位,不允许重复)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ORGANIZATIONS', @level2type = N'COLUMN', @level2name = N'SYSDISTINCT2';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'备用字段3(32位,允许重复)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ORGANIZATIONS', @level2type = N'COLUMN', @level2name = N'SYSCONTENT1';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'备用字段4(64位,允许重复)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ORGANIZATIONS', @level2type = N'COLUMN', @level2name = N'SYSCONTENT2';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'备用字段5(128位,允许重复)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ORGANIZATIONS', @level2type = N'COLUMN', @level2name = N'SYSCONTENT3';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'检索名称', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ORGANIZATIONS', @level2type = N'COLUMN', @level2name = N'SEARCH_NAME';

