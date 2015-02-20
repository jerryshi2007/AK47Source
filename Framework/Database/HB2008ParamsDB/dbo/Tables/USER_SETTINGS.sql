CREATE TABLE [dbo].[USER_SETTINGS] (
    [USER_ID]  NVARCHAR (36) NOT NULL,
    [SETTINGS] XML           NULL,
    CONSTRAINT [PK_USER_SETTING_1] PRIMARY KEY CLUSTERED ([USER_ID] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'用户ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'USER_SETTINGS', @level2type = N'COLUMN', @level2name = N'USER_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'用户的设置信息', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'USER_SETTINGS', @level2type = N'COLUMN', @level2name = N'SETTINGS';


GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'用户的个人设置信息',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'USER_SETTINGS',
    @level2type = NULL,
    @level2name = NULL