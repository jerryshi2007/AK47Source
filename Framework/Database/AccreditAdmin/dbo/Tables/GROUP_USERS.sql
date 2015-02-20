CREATE TABLE [dbo].[GROUP_USERS] (
    [GROUP_GUID]       NVARCHAR (36) NOT NULL,
    [USER_GUID]        NVARCHAR (36) NOT NULL,
    [USER_PARENT_GUID] NVARCHAR (36) NOT NULL,
    [INNER_SORT]       NVARCHAR (6)  CONSTRAINT [DF_GROUP_USER_INNER_SORT] DEFAULT ('000000') NOT NULL,
    [CREATE_TIME]      DATETIME      CONSTRAINT [DF_GROUP_USER_CREATE_TIME] DEFAULT (getdate()) NOT NULL,
    [MODIFY_TIME]      DATETIME      CONSTRAINT [DF_GROUP_USER_MODIFY_TIME] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_GROUP_USERS] PRIMARY KEY CLUSTERED ([GROUP_GUID] ASC, [USER_GUID] ASC, [USER_PARENT_GUID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_GROUP_USERS_INNER_SORT]
    ON [dbo].[GROUP_USERS]([INNER_SORT] ASC);


GO
CREATE TRIGGER DELETE_GROUP_USERS
ON dbo.GROUP_USERS 
FOR DELETE 
AS
BEGIN
	UPDATE GROUPS SET GROUPS.MODIFY_TIME = GETDATE() FROM DELETED WHERE GROUPS.GUID = DELETED.GROUP_GUID
END

GO
CREATE TRIGGER INSERT_GROUP_USERS
ON dbo.GROUP_USERS 
FOR INSERT
AS
BEGIN
	UPDATE GROUPS SET GROUPS.MODIFY_TIME = GETDATE() FROM INSERTED WHERE GROUPS.GUID = INSERTED.GROUP_GUID
END

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'人员组的标志ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'GROUP_USERS', @level2type = N'COLUMN', @level2name = N'GROUP_GUID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'用户的标志ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'GROUP_USERS', @level2type = N'COLUMN', @level2name = N'USER_GUID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'人员所在机构的标识（主要用于兼职问题）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'GROUP_USERS', @level2type = N'COLUMN', @level2name = N'USER_PARENT_GUID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'用户在组中的排序', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'GROUP_USERS', @level2type = N'COLUMN', @level2name = N'INNER_SORT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'关系创建时间', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'GROUP_USERS', @level2type = N'COLUMN', @level2name = N'CREATE_TIME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'最近修改时间', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'GROUP_USERS', @level2type = N'COLUMN', @level2name = N'MODIFY_TIME';

