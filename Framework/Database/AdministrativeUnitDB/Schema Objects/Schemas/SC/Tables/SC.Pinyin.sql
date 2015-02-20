CREATE TABLE [SC].[Pinyin]
(
	[Pinyin] NVARCHAR(255) NOT NULL , 
    [HZ] NVARCHAR(128) NOT NULL, 
    [Weight] INT NULL DEFAULT 0, 
    CONSTRAINT [PK_Pinyin] PRIMARY KEY ([Pinyin], [HZ])
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'拼音',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'Pinyin',
    @level2type = N'COLUMN',
    @level2name = N'Pinyin'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'汉字',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'Pinyin',
    @level2type = N'COLUMN',
    @level2name = N'HZ'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'权重',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'Pinyin',
    @level2type = N'COLUMN',
    @level2name = N'Weight'
GO

CREATE INDEX [IX_Pinyin_HZ] ON [SC].[Pinyin] ([HZ])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'汉字拼音表',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'Pinyin',
    @level2type = NULL,
    @level2name = NULL