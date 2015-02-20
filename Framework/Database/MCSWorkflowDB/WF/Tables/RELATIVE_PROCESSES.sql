CREATE TABLE [WF].[RELATIVE_PROCESSES] (
    [RELATIVE_ID]  NVARCHAR (36)   NOT NULL,
    [PROCESS_ID]   NVARCHAR (36)   NOT NULL,
    [DESCRIPTION]  NVARCHAR (64)   NULL,
    [RELATIVE_URL] NVARCHAR (2048) NULL,
    CONSTRAINT [PK_RELATIVE_PROCESS] PRIMARY KEY CLUSTERED ([RELATIVE_ID] ASC, [PROCESS_ID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_RELATIVE_PROCESS_PROCESS_ID]
    ON [WF].[RELATIVE_PROCESSES]([PROCESS_ID] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'流程的相关信息表', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'RELATIVE_PROCESSES';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'相关信息的ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'RELATIVE_PROCESSES', @level2type = N'COLUMN', @level2name = N'RELATIVE_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'流程的ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'RELATIVE_PROCESSES', @level2type = N'COLUMN', @level2name = N'PROCESS_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'描述', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'RELATIVE_PROCESSES', @level2type = N'COLUMN', @level2name = N'DESCRIPTION';

