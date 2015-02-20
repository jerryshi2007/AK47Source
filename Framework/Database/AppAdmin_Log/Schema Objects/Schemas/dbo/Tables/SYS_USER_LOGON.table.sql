CREATE TABLE [dbo].[SYS_USER_LOGON] (
    [ID]                INT            IDENTITY (1, 1) NOT NULL,
    [USER_GUID]         NVARCHAR (36)  NULL,
    [USER_DISPLAYNAME]  NVARCHAR (32)  NULL,
    [USER_LOGONNAME]    NVARCHAR (32)  NULL,
    [USER_DISTINCTNAME] NVARCHAR (255) NOT NULL,
    [HOST_NAME]         NVARCHAR (64)  NOT NULL,
    [HOST_IP]           NVARCHAR (16)  NOT NULL,
    [IE_VERSION]        NVARCHAR (255) NULL,
    [WINDOWS_VERSION]   NVARCHAR (255) NULL,
    [KILL_VIRUS]        NVARCHAR (64)  NULL,
    [LOG_DATE]          DATETIME       NOT NULL,
    [STATUS]            NVARCHAR (16)  NOT NULL,
    [DESCRIPTION]       NVARCHAR (255) NULL
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'标识（自增）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SYS_USER_LOGON', @level2type = N'COLUMN', @level2name = N'ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'登录用户', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SYS_USER_LOGON', @level2type = N'COLUMN', @level2name = N'USER_GUID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'登录用户的显示名称', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SYS_USER_LOGON', @level2type = N'COLUMN', @level2name = N'USER_DISPLAYNAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'登录名称', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SYS_USER_LOGON', @level2type = N'COLUMN', @level2name = N'USER_LOGONNAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'登录用户全地址', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SYS_USER_LOGON', @level2type = N'COLUMN', @level2name = N'USER_DISTINCTNAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'客户端电脑的名称', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SYS_USER_LOGON', @level2type = N'COLUMN', @level2name = N'HOST_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'客户端电脑的IP', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SYS_USER_LOGON', @level2type = N'COLUMN', @level2name = N'HOST_IP';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'客户端IE版本', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SYS_USER_LOGON', @level2type = N'COLUMN', @level2name = N'IE_VERSION';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'客户端Windows版本', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SYS_USER_LOGON', @level2type = N'COLUMN', @level2name = N'WINDOWS_VERSION';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'客户端的杀毒软件', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SYS_USER_LOGON', @level2type = N'COLUMN', @level2name = N'KILL_VIRUS';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'登录系统的时间', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SYS_USER_LOGON', @level2type = N'COLUMN', @level2name = N'LOG_DATE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'登录是否成功（Failed/Successful）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SYS_USER_LOGON', @level2type = N'COLUMN', @level2name = N'STATUS';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'描述信息数据', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SYS_USER_LOGON', @level2type = N'COLUMN', @level2name = N'DESCRIPTION';

