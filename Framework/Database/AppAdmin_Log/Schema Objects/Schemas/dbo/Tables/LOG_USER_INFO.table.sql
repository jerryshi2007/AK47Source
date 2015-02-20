CREATE TABLE [dbo].[LOG_USER_INFO] (
    [SORT]             INT             IDENTITY (1, 1) NOT NULL,
    [USERNAME]         NVARCHAR (64)   NULL,
    [USERID]           NVARCHAR (64)   NULL,
    [USERDN]           NVARCHAR (255)  NULL,
    [HOSTADDRESS]      NVARCHAR (64)   NULL,
    [APPLICATION_NAME] NVARCHAR (64)   NULL,
    [PROGRAM_NAME]     NVARCHAR (64)   NULL,
    [URL]              NVARCHAR (2048) NULL,
    [RESOURCEID]       NVARCHAR (36)   NULL,
    [OPERATION_TYPE]   NVARCHAR (255)  NULL,
    [STATE_BEFORE]     NTEXT           NULL,
    [STATE_AFTER]      NTEXT           NULL,
    [LOG_DATE]         DATETIME        NULL,
    [APP_DATA]         NTEXT           NULL
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'用户操作记录', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'LOG_USER_INFO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'id', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'LOG_USER_INFO', @level2type = N'COLUMN', @level2name = N'SORT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'用户显示名称', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'LOG_USER_INFO', @level2type = N'COLUMN', @level2name = N'USERNAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'用户登录名称', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'LOG_USER_INFO', @level2type = N'COLUMN', @level2name = N'USERID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'用户DN', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'LOG_USER_INFO', @level2type = N'COLUMN', @level2name = N'USERDN';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'主机地址', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'LOG_USER_INFO', @level2type = N'COLUMN', @level2name = N'HOSTADDRESS';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'应用名', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'LOG_USER_INFO', @level2type = N'COLUMN', @level2name = N'APPLICATION_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'程序名称', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'LOG_USER_INFO', @level2type = N'COLUMN', @level2name = N'PROGRAM_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'访问页面URL', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'LOG_USER_INFO', @level2type = N'COLUMN', @level2name = N'URL';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'访问的资源名称', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'LOG_USER_INFO', @level2type = N'COLUMN', @level2name = N'RESOURCEID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'操作类型', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'LOG_USER_INFO', @level2type = N'COLUMN', @level2name = N'OPERATION_TYPE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'操作前的状态', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'LOG_USER_INFO', @level2type = N'COLUMN', @level2name = N'STATE_BEFORE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'操作后的状态', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'LOG_USER_INFO', @level2type = N'COLUMN', @level2name = N'STATE_AFTER';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'日志的记录时间', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'LOG_USER_INFO', @level2type = N'COLUMN', @level2name = N'LOG_DATE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'对状态数据的描述', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'LOG_USER_INFO', @level2type = N'COLUMN', @level2name = N'APP_DATA';

