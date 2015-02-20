CREATE TABLE [WeChat].[AppMessages]
(
	[AppMessageID] INT NOT NULL PRIMARY KEY, 
    [MessageType] NVARCHAR(36) NULL, 
    [Title] NVARCHAR(128) NOT NULL, 
    [Content] NVARCHAR(MAX) NOT NULL, 
    [Digest] NVARCHAR(256) NULL, 
    [Author] NVARCHAR(50) NULL, 
    [SourceUrl] NVARCHAR(256) NULL, 
    [FileID] INT NULL, 
    [ShowCover] INT NULL, 
    [CreateTime] DATETIME NULL, 
    [ContentUrl] NVARCHAR(256) NULL, 
    [ImageUrl] NVARCHAR(256) NULL, 
    [Count] INT NULL
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'多少图文',
    @level0type = N'SCHEMA',
    @level0name = N'WeChat',
    @level1type = N'TABLE',
    @level1name = N'AppMessages',
    @level2type = N'COLUMN',
    @level2name = N'Count'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'标题',
    @level0type = N'SCHEMA',
    @level0name = N'WeChat',
    @level1type = N'TABLE',
    @level1name = N'AppMessages',
    @level2type = N'COLUMN',
    @level2name = N'Title'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'内容',
    @level0type = N'SCHEMA',
    @level0name = N'WeChat',
    @level1type = N'TABLE',
    @level1name = N'AppMessages',
    @level2type = N'COLUMN',
    @level2name = N'Content'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'摘要',
    @level0type = N'SCHEMA',
    @level0name = N'WeChat',
    @level1type = N'TABLE',
    @level1name = N'AppMessages',
    @level2type = N'COLUMN',
    @level2name = N'Digest'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'作者',
    @level0type = N'SCHEMA',
    @level0name = N'WeChat',
    @level1type = N'TABLE',
    @level1name = N'AppMessages',
    @level2type = N'COLUMN',
    @level2name = N'Author'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'相关链接',
    @level0type = N'SCHEMA',
    @level0name = N'WeChat',
    @level1type = N'TABLE',
    @level1name = N'AppMessages',
    @level2type = N'COLUMN',
    @level2name = N'SourceUrl'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'文件ID',
    @level0type = N'SCHEMA',
    @level0name = N'WeChat',
    @level1type = N'TABLE',
    @level1name = N'AppMessages',
    @level2type = N'COLUMN',
    @level2name = N'FileID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'封面是否显示在内容中',
    @level0type = N'SCHEMA',
    @level0name = N'WeChat',
    @level1type = N'TABLE',
    @level1name = N'AppMessages',
    @level2type = N'COLUMN',
    @level2name = N'ShowCover'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'封面的URL',
    @level0type = N'SCHEMA',
    @level0name = N'WeChat',
    @level1type = N'TABLE',
    @level1name = N'AppMessages',
    @level2type = N'COLUMN',
    @level2name = N'ImageUrl'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'内容的URL',
    @level0type = N'SCHEMA',
    @level0name = N'WeChat',
    @level1type = N'TABLE',
    @level1name = N'AppMessages',
    @level2type = N'COLUMN',
    @level2name = N'ContentUrl'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'创建时间',
    @level0type = N'SCHEMA',
    @level0name = N'WeChat',
    @level1type = N'TABLE',
    @level1name = N'AppMessages',
    @level2type = N'COLUMN',
    @level2name = N'CreateTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'消息类型',
    @level0type = N'SCHEMA',
    @level0name = N'WeChat',
    @level1type = N'TABLE',
    @level1name = N'AppMessages',
    @level2type = N'COLUMN',
    @level2name = N'MessageType'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'消息ID',
    @level0type = N'SCHEMA',
    @level0name = N'WeChat',
    @level1type = N'TABLE',
    @level1name = N'AppMessages',
    @level2type = N'COLUMN',
    @level2name = N'AppMessageID'