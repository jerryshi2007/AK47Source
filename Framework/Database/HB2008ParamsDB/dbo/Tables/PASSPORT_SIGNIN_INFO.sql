CREATE TABLE [dbo].[PASSPORT_SIGNIN_INFO] (
    [SIGNIN_ID]           NVARCHAR (36)  NOT NULL,
    [SORT_ID]             INT            IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [SIGNIN_TIME]         DATETIME       CONSTRAINT [DF_SIGNIN_INFO_SIGNIN_TIME] DEFAULT (getdate()) NOT NULL,
    [SIGNIN_TIMEOUT]      DATETIME       NULL,
    [USER_ID]             NVARCHAR (64)  NOT NULL,
    [DOMAIN]              NVARCHAR (128) NULL,
    [AUTHENTICATE_SERVER] NVARCHAR (128) NULL,
    CONSTRAINT [PK_SIGNIN_INFO] PRIMARY KEY NONCLUSTERED ([SIGNIN_ID] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [IX_SIGNIN_INFO_SORT_ID] UNIQUE CLUSTERED ([SORT_ID] DESC) WITH (FILLFACTOR = 90)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'用户每一次真正认证的记录', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PASSPORT_SIGNIN_INFO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'登录认证的ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PASSPORT_SIGNIN_INFO', @level2type = N'COLUMN', @level2name = N'SIGNIN_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'记录的序号(降序索引)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PASSPORT_SIGNIN_INFO', @level2type = N'COLUMN', @level2name = N'SORT_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'登录时间', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PASSPORT_SIGNIN_INFO', @level2type = N'COLUMN', @level2name = N'SIGNIN_TIME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'认证后，认证信息的超时时间', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PASSPORT_SIGNIN_INFO', @level2type = N'COLUMN', @level2name = N'SIGNIN_TIMEOUT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'用户的登录名', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PASSPORT_SIGNIN_INFO', @level2type = N'COLUMN', @level2name = N'USER_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'用户认证时的域名', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PASSPORT_SIGNIN_INFO', @level2type = N'COLUMN', @level2name = N'DOMAIN';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'认证服务器名称', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PASSPORT_SIGNIN_INFO', @level2type = N'COLUMN', @level2name = N'AUTHENTICATE_SERVER';

