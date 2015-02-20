CREATE TABLE [dbo].[APP_OPERATION_TYPE] (
    [GUID]        NVARCHAR (36)  NOT NULL,
    [CODE_NAME]   NVARCHAR (255) NOT NULL,
    [DISPLAYNAME] NVARCHAR (64)  NOT NULL,
    [APP_GUID]    NVARCHAR (36)  NOT NULL,
    [VISIBLE]     NVARCHAR (1)   NOT NULL,
    [DISCRIPTION] NVARCHAR (255) NULL,
    [CLASS]       NVARCHAR (255) NULL
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'应用操作类型GUID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'APP_OPERATION_TYPE', @level2type = N'COLUMN', @level2name = N'GUID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'应用操作类型名称（英文）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'APP_OPERATION_TYPE', @level2type = N'COLUMN', @level2name = N'CODE_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'操作类型描述', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'APP_OPERATION_TYPE', @level2type = N'COLUMN', @level2name = N'DISCRIPTION';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'操作类型的级别编号', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'APP_OPERATION_TYPE', @level2type = N'COLUMN', @level2name = N'CLASS';

