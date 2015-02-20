CREATE TABLE [dbo].[APP_LOG_TYPE] (
    [GUID]        NVARCHAR (36)  NOT NULL,
    [CODE_NAME]   NVARCHAR (255) NOT NULL,
    [DISPLAYNAME] NVARCHAR (64)  NOT NULL,
    [VISIBLE]     NVARCHAR (1)   NOT NULL,
    [DISCRIPTION] NVARCHAR (255) NULL,
    [CLASS]       NVARCHAR (255) NULL
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'应用类型GUID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'APP_LOG_TYPE', @level2type = N'COLUMN', @level2name = N'GUID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'应用标志名（英文）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'APP_LOG_TYPE', @level2type = N'COLUMN', @level2name = N'CODE_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'应用类型名称（中文）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'APP_LOG_TYPE', @level2type = N'COLUMN', @level2name = N'DISPLAYNAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否可见', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'APP_LOG_TYPE', @level2type = N'COLUMN', @level2name = N'VISIBLE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'应用描述', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'APP_LOG_TYPE', @level2type = N'COLUMN', @level2name = N'DISCRIPTION';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'优先级', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'APP_LOG_TYPE', @level2type = N'COLUMN', @level2name = N'CLASS';

