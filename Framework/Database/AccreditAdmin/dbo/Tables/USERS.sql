CREATE TABLE [dbo].[USERS] (
    [GUID]          NVARCHAR (36)  NOT NULL,
    [FIRST_NAME]    NVARCHAR (32)  NOT NULL,
    [LAST_NAME]     NVARCHAR (32)  NOT NULL,
    [LOGON_NAME]    NVARCHAR (64)  NOT NULL,
    [IC_CARD]       NVARCHAR (16)  NULL,
    [PWD_TYPE_GUID] NVARCHAR (36)  NULL,
    [USER_PWD]      NVARCHAR (255) NULL,
    [RANK_CODE]     NVARCHAR (32)  NOT NULL,
    [E_MAIL]        NVARCHAR (64)  NULL,
    [POSTURAL]      INT            CONSTRAINT [DF_USERS_STATUS] DEFAULT ((8)) NOT NULL,
    [CREATE_TIME]   DATETIME       CONSTRAINT [DF_USER_CREATE_TIME] DEFAULT (getdate()) NOT NULL,
    [MODIFY_TIME]   DATETIME       CONSTRAINT [DF_USER_MODIFY_TIME] DEFAULT (getdate()) NOT NULL,
    [AD_COUNT]      INT            CONSTRAINT [DF_USERS_AD_COUNT] DEFAULT ((0)) NOT NULL,
    [PERSON_ID]     NVARCHAR (7)   NULL,
    [SYSDISTINCT1]  NVARCHAR (16)  NULL,
    [SYSDISTINCT2]  NVARCHAR (32)  NULL,
    [SYSCONTENT1]   NVARCHAR (32)  NULL,
    [SYSCONTENT2]   NVARCHAR (64)  NULL,
    [SYSCONTENT3]   NVARCHAR (128) NULL,
    [PINYIN]        NVARCHAR (64)  NULL,
    CONSTRAINT [PK_USER] PRIMARY KEY CLUSTERED ([GUID] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_USERS_LOGON_NAME]
    ON [dbo].[USERS]([LOGON_NAME] ASC);


GO

/*1------------USERS[Insert,Delete]*/
CREATE TRIGGER [dbo].[DELETE_USERS]
ON [dbo].[USERS] 
FOR DELETE 
AS
BEGIN
	DELETE OU_USERS FROM DELETED WHERE OU_USERS.USER_GUID = DELETED.GUID
	DELETE SECRETARIES FROM DELETED WHERE SECRETARIES.LEADER_GUID = DELETED.GUID OR SECRETARIES.SECRETARY_GUID = DELETED.GUID
	DELETE GROUP_USERS FROM DELETED WHERE GROUP_USERS.USER_GUID = DELETED.GUID
END


GO

CREATE TRIGGER [dbo].[INSERT_UPDATE_USERS]
ON [dbo].[USERS]
FOR INSERT, UPDATE
AS
BEGIN
	DECLARE @@personID AS NVARCHAR(255)
	DECLARE @@icCard AS NVARCHAR(255)
	DECLARE @@SysDis1 AS NVARCHAR(255)
	DECLARE @@SysDis2 AS NVARCHAR(255)
	DECLARE @@RowNum AS INT
	--SELECT @@personID = PERSON_ID, @@SysDis1 = SYSDISTINCT1, @@SysDis2 = SYSDISTINCT2, @@icCard=IC_CARD FROM INSERTED
	/*
	SELECT @@RowNum=COUNT(*) FROM USERS WHERE SYSDISTINCT1 IS NOT NULL AND SYSDISTINCT1 = @@SysDis1
	IF @@RowNum > 1
	BEGIN
		RAISERROR('对不起，系统中已经存在了一个“备用字段1”为“%s”的数据项!', 16, 1, @@SysDis1)
	END
	SELECT @@RowNum=COUNT(*) FROM USERS WHERE SYSDISTINCT2 IS NOT NULL AND SYSDISTINCT2 = @@SysDis2
	IF @@RowNum > 1
	BEGIN
		RAISERROR('对不起，系统中已经存在了一个“备用字段2”为“%s”的数据项!', 16, 1, @@SysDis2)
	END
	
	SELECT @@RowNum=COUNT(*) FROM USERS WHERE PERSON_ID IS NOT NULL AND PERSON_ID = @@personID
	IF @@RowNum > 1
	BEGIN
		RAISERROR('对不起，系统中已经存在了一个“用户编号”为“%s”的数据项!', 16, 1, @@personID)
	END
	
	SELECT @@RowNum=COUNT(*) FROM USERS WHERE IC_CARD IS NOT NULL AND PERSON_ID = @@icCard
	IF @@RowNum > 1
	BEGIN
		RAISERROR('对不起，系统中已经存在了一个“IC卡号”为“%s”的数据项!', 16, 1, @@icCard)
	END
	*/
END


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'用户身份标志ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'USERS', @level2type = N'COLUMN', @level2name = N'GUID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'用户的名', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'USERS', @level2type = N'COLUMN', @level2name = N'FIRST_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'用户的姓', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'USERS', @level2type = N'COLUMN', @level2name = N'LAST_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'用户的登录名称', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'USERS', @level2type = N'COLUMN', @level2name = N'LOGON_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'用户的IC卡号', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'USERS', @level2type = N'COLUMN', @level2name = N'IC_CARD';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'使用密码的加密算法', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'USERS', @level2type = N'COLUMN', @level2name = N'PWD_TYPE_GUID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'用户所使用的密码（加密存储）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'USERS', @level2type = N'COLUMN', @level2name = N'USER_PWD';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'用户本身的级别信息数据', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'USERS', @level2type = N'COLUMN', @level2name = N'RANK_CODE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'用户默认使用的EMAIL', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'USERS', @level2type = N'COLUMN', @level2name = N'E_MAIL';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'用户的在系统中的状态（1、禁用状态；2、要求下次登录修改密码；4、正常使用；）掩码方式实现', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'USERS', @level2type = N'COLUMN', @level2name = N'POSTURAL';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'创建时间', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'USERS', @level2type = N'COLUMN', @level2name = N'CREATE_TIME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'最近修改时间', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'USERS', @level2type = N'COLUMN', @level2name = N'MODIFY_TIME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否在AD中建立对应的账号', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'USERS', @level2type = N'COLUMN', @level2name = N'AD_COUNT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'海关人员编码', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'USERS', @level2type = N'COLUMN', @level2name = N'PERSON_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'备用字段1(16位,不允许重复)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'USERS', @level2type = N'COLUMN', @level2name = N'SYSDISTINCT1';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'备用字段2(32位,不允许重复)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'USERS', @level2type = N'COLUMN', @level2name = N'SYSDISTINCT2';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'备用字段3(32位,允许重复)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'USERS', @level2type = N'COLUMN', @level2name = N'SYSCONTENT1';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'(传真)备用字段4(64位,允许重复)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'USERS', @level2type = N'COLUMN', @level2name = N'SYSCONTENT2';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'备用字段5(128位,允许重复)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'USERS', @level2type = N'COLUMN', @level2name = N'SYSCONTENT3';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'用户的拼音名', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'USERS', @level2type = N'COLUMN', @level2name = N'PINYIN';

