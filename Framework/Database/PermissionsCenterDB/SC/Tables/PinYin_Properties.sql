EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'拼音',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'PinYin',
    @level2type = N'COLUMN',
    @level2name = N'PinYin'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'汉字',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'PinYin',
    @level2type = N'COLUMN',
    @level2name = N'HZ'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'权重',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'PinYin',
    @level2type = N'COLUMN',
    @level2name = N'Weight'
GO

EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'汉字拼音表',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'PinYin',
    @level2type = NULL,
    @level2name = NULL
GO