CREATE TABLE [dbo].[LOG_SYS_INFO] (
    [SORT]        INT            IDENTITY (1, 1) NOT NULL,
    [USERNAME]    NVARCHAR (64)  NULL,
    [USERID]      NVARCHAR (64)  NULL,
    [USERDN]      NVARCHAR (255) NULL,
    [HOSTADDRESS] NVARCHAR (64)  NULL,
    [HOSTNAME]    NVARCHAR (64)  NULL,
    [IEVER]       NVARCHAR (64)  NULL,
    [WINVER]      NVARCHAR (64)  NULL,
    [KILLINFO]    NVARCHAR (255) NULL,
    [ENVIRONMENT] NVARCHAR (255) NULL,
    [LOG_DATE]    DATETIME       NULL
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'应用系统登录Log记录', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'LOG_SYS_INFO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'LOG_SYS_INFO', @level2type = N'COLUMN', @level2name = N'SORT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'用户的ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'LOG_SYS_INFO', @level2type = N'COLUMN', @level2name = N'USERNAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'用户的ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'LOG_SYS_INFO', @level2type = N'COLUMN', @level2name = N'USERID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'用户DN', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'LOG_SYS_INFO', @level2type = N'COLUMN', @level2name = N'USERDN';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'客户机IP', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'LOG_SYS_INFO', @level2type = N'COLUMN', @level2name = N'HOSTADDRESS';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'客户机名称', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'LOG_SYS_INFO', @level2type = N'COLUMN', @level2name = N'HOSTNAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'ie版本信息', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'LOG_SYS_INFO', @level2type = N'COLUMN', @level2name = N'IEVER';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'windows版本信息', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'LOG_SYS_INFO', @level2type = N'COLUMN', @level2name = N'WINVER';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'杀毒软件信息', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'LOG_SYS_INFO', @level2type = N'COLUMN', @level2name = N'KILLINFO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'环境信息', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'LOG_SYS_INFO', @level2type = N'COLUMN', @level2name = N'ENVIRONMENT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'日志记录时间', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'LOG_SYS_INFO', @level2type = N'COLUMN', @level2name = N'LOG_DATE';

