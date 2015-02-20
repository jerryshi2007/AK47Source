CREATE TABLE [dbo].[USER_RECENT_DATA] (
    [USER_ID]     NVARCHAR (36) NOT NULL,
    [CATEGORY]    NVARCHAR (64) NOT NULL,
    [DATA]        XML           NULL,
    [UPDATE_TIME] DATETIME      CONSTRAINT [DF_USER_RECENT_DATA_UPDATE_TIME] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_USER_RECENT_DATA] PRIMARY KEY CLUSTERED ([USER_ID] ASC, [CATEGORY] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'用户ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'USER_RECENT_DATA', @level2type = N'COLUMN', @level2name = N'USER_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'类别', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'USER_RECENT_DATA', @level2type = N'COLUMN', @level2name = N'CATEGORY';


GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'用户最近所访问的数据',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'USER_RECENT_DATA',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'用户最近所访问的数据',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'USER_RECENT_DATA',
    @level2type = N'COLUMN',
    @level2name = N'DATA'