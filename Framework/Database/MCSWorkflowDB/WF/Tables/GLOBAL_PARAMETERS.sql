CREATE TABLE [WF].[GLOBAL_PARAMETERS] (
    [KEY]        NVARCHAR (64) NOT NULL,
    [PROPERTIES] XML           NULL,
	[TENANT_CODE] NVARCHAR(36) NOT NULL DEFAULT 'D5561180-7617-4B67-B68B-1F0EA604B509'
    CONSTRAINT [PK_GLOBAL_PARAMETERS] PRIMARY KEY CLUSTERED ([KEY] ASC, [TENANT_CODE] ASC)
);

GO

CREATE INDEX [IX_GLOBAL_PARAMETERS_TENANT_CODE] ON [WF].[GLOBAL_PARAMETERS] ([TENANT_CODE])

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'全局参数', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'GLOBAL_PARAMETERS';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'参数集的Key', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'GLOBAL_PARAMETERS', @level2type = N'COLUMN', @level2name = N'KEY';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'参数集的属性', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'GLOBAL_PARAMETERS', @level2type = N'COLUMN', @level2name = N'PROPERTIES';

