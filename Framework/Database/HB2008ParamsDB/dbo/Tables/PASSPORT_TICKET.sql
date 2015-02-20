CREATE TABLE [dbo].[PASSPORT_TICKET] (
    [SIGNIN_ID]          NVARCHAR (36) NOT NULL,
    [APP_SIGNIN_ID]      NVARCHAR (36) NOT NULL,
    [APP_ID]             NVARCHAR (64) NULL,
    [APP_SIGNIN_TIME]    DATETIME      CONSTRAINT [DF_APP_SIGNIN_INFO_APP_SIGNIN_TIME] DEFAULT (getdate()) NOT NULL,
    [APP_SIGNIN_TIMEOUT] DATETIME      NULL,
    [APP_SIGNIN_URL]     NTEXT         NULL,
    [APP_SIGNIN_IP]      NVARCHAR (32) NULL,
    [APP_LOGOFF_URL]     NTEXT         NULL,
    [DEL_FLAG]           NVARCHAR (1)  CONSTRAINT [DF_APP_SIGNIN_INFO_DEL_FLAG] DEFAULT (N'n') NULL,
    CONSTRAINT [PK_APP_SIGNIN_INFO] PRIMARY KEY CLUSTERED ([SIGNIN_ID] ASC, [APP_SIGNIN_ID] ASC) WITH (FILLFACTOR = 90)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'认证的ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PASSPORT_TICKET', @level2type = N'COLUMN', @level2name = N'SIGNIN_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'应用认证的ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PASSPORT_TICKET', @level2type = N'COLUMN', @level2name = N'APP_SIGNIN_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'应用的ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PASSPORT_TICKET', @level2type = N'COLUMN', @level2name = N'APP_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'应用登录的时间', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PASSPORT_TICKET', @level2type = N'COLUMN', @level2name = N'APP_SIGNIN_TIME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'应用登录的超时时间', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PASSPORT_TICKET', @level2type = N'COLUMN', @level2name = N'APP_SIGNIN_TIMEOUT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'应用登录时的URL', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PASSPORT_TICKET', @level2type = N'COLUMN', @level2name = N'APP_SIGNIN_URL';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'应用登录时的密码', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PASSPORT_TICKET', @level2type = N'COLUMN', @level2name = N'APP_SIGNIN_IP';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'应用注销时的URL', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PASSPORT_TICKET', @level2type = N'COLUMN', @level2name = N'APP_LOGOFF_URL';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'删除标志', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PASSPORT_TICKET', @level2type = N'COLUMN', @level2name = N'DEL_FLAG';

