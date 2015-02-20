CREATE TABLE [dbo].[USERS_INFO_EXTEND] (
    [ID]              NVARCHAR (36)  NOT NULL,
    [GENDER]          NVARCHAR (1)   NOT NULL,
    [NATION]          NVARCHAR (16)  NOT NULL,
    [BIRTHDAY]        DATETIME       NULL,
    [START_WORK_TIME] DATETIME       NULL,
    [MOBILE]          NVARCHAR (16)  NULL,
    [MOBILE2]         NVARCHAR (16)  NULL,
    [OFFICE_TEL]      NVARCHAR (64)  NULL,
    [INTRANET_EMAIL]  NVARCHAR (128) NULL,
    [INTERNET_EMAIL]  NVARCHAR (128) NULL,
    [IM_ADDRESS]      NVARCHAR (128) NULL,
    [MEMO]            NVARCHAR (255) NULL,
    [SIGN_IMAGE_PATH] NVARCHAR (255) NULL,
    CONSTRAINT [PK_USERS_INFO_EXTEND] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'性别', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'USERS_INFO_EXTEND', @level2type = N'COLUMN', @level2name = N'GENDER';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'民族', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'USERS_INFO_EXTEND', @level2type = N'COLUMN', @level2name = N'NATION';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'出生日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'USERS_INFO_EXTEND', @level2type = N'COLUMN', @level2name = N'BIRTHDAY';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'入职日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'USERS_INFO_EXTEND', @level2type = N'COLUMN', @level2name = N'START_WORK_TIME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'手机', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'USERS_INFO_EXTEND', @level2type = N'COLUMN', @level2name = N'MOBILE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'手机', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'USERS_INFO_EXTEND', @level2type = N'COLUMN', @level2name = N'MOBILE2';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'办公室电话', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'USERS_INFO_EXTEND', @level2type = N'COLUMN', @level2name = N'OFFICE_TEL';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'内网Email', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'USERS_INFO_EXTEND', @level2type = N'COLUMN', @level2name = N'INTRANET_EMAIL';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'外网Email', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'USERS_INFO_EXTEND', @level2type = N'COLUMN', @level2name = N'INTERNET_EMAIL';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'即时通讯地址', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'USERS_INFO_EXTEND', @level2type = N'COLUMN', @level2name = N'IM_ADDRESS';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'个人签名图片路径', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'USERS_INFO_EXTEND', @level2type = N'COLUMN', @level2name = N'SIGN_IMAGE_PATH';


GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'用户的扩展信息（基本废弃）',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'USERS_INFO_EXTEND',
    @level2type = NULL,
    @level2name = NULL