CREATE TABLE [dbo].[PINYIN] (
    [PINYIN] NVARCHAR (255) NOT NULL,
    [HZ]     NVARCHAR (128) NOT NULL,
    [WEIGHT] INT            CONSTRAINT [DF_PINYIN_WEIGHT] DEFAULT ((0)) NULL,
    CONSTRAINT [PK_PINYIN2] PRIMARY KEY CLUSTERED ([PINYIN] ASC, [HZ] ASC) WITH (FILLFACTOR = 90)
);


GO
CREATE NONCLUSTERED INDEX [IX_HZ]
    ON [dbo].[PINYIN]([HZ] ASC) WITH (FILLFACTOR = 90);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'对应拼音', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PINYIN', @level2type = N'COLUMN', @level2name = N'PINYIN';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'对应汉字', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PINYIN', @level2type = N'COLUMN', @level2name = N'HZ';

