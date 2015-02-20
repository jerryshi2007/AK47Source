CREATE TABLE [WF].[PROCESS_DESCRIPTOR_CATEGORY] (
    [ID]   NVARCHAR (64)  CONSTRAINT [DF_PROCESS_DESCRIPTOR_CATEGORY_ID] DEFAULT (newid()) NOT NULL,
    [NAME] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_PROCESS_DESCRIPTOR_CATEGORY] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'流程定义的类别',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'PROCESS_DESCRIPTOR_CATEGORY',
    @level2type = NULL,
    @level2name = NULL