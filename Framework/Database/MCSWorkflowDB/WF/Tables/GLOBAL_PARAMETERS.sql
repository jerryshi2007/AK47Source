CREATE TABLE [WF].[GLOBAL_PARAMETERS] (
    [KEY]        NVARCHAR (64) NOT NULL,
    [PROPERTIES] XML           NULL,
    CONSTRAINT [PK_GLOBAL_PARAMETERS] PRIMARY KEY CLUSTERED ([KEY] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'全局参数', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'GLOBAL_PARAMETERS';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'参数集的Key', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'GLOBAL_PARAMETERS', @level2type = N'COLUMN', @level2name = N'KEY';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'参数集的属性', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'GLOBAL_PARAMETERS', @level2type = N'COLUMN', @level2name = N'PROPERTIES';

