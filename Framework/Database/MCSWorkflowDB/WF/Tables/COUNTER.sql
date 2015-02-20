CREATE TABLE [WF].[COUNTER] (
    [COUNTER_ID]  NVARCHAR (255) NOT NULL,
    [COUNT_VALUE] INT            CONSTRAINT [DF_COUNTER_COUNT_VALUE] DEFAULT ((0)) NULL,
    CONSTRAINT [PK_COUNTER] PRIMARY KEY CLUSTERED ([COUNTER_ID] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'计数器ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'COUNTER', @level2type = N'COLUMN', @level2name = N'COUNTER_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'用于计数值', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'COUNTER', @level2type = N'COLUMN', @level2name = N'COUNT_VALUE';

