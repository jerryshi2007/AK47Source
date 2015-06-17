CREATE TABLE [WF].[COUNTER] (
    [COUNTER_ID]  NVARCHAR (255) NOT NULL,
    [COUNT_VALUE] INT            CONSTRAINT [DF_COUNTER_COUNT_VALUE] DEFAULT ((0)) NULL,
	[TENANT_CODE] NVARCHAR(36) NOT NULL DEFAULT 'D5561180-7617-4B67-B68B-1F0EA604B509'
    CONSTRAINT [PK_COUNTER] PRIMARY KEY CLUSTERED ([COUNTER_ID] ASC, [TENANT_CODE] ASC)
);

GO

CREATE INDEX [IX_COUNTER_TENANT_CODE] ON [WF].[COUNTER] ([TENANT_CODE])

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'计数器ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'COUNTER', @level2type = N'COLUMN', @level2name = N'COUNTER_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'用于计数值', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'COUNTER', @level2type = N'COLUMN', @level2name = N'COUNT_VALUE';

