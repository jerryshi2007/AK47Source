CREATE TABLE [dbo].[PAGE_VIEW_STATE] (
    [ID]          BIGINT         IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [DATA]        NVARCHAR (MAX) NULL,
    [CREATE_DATE] DATETIME       CONSTRAINT [DF_PAGE_VIEW_STATE_CREATE_DATE] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_PAGE_VIEW_STATE] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'页面的ViewState', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PAGE_VIEW_STATE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PAGE_VIEW_STATE', @level2type = N'COLUMN', @level2name = N'ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'ViewState的内容', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PAGE_VIEW_STATE', @level2type = N'COLUMN', @level2name = N'DATA';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'创建时间', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PAGE_VIEW_STATE', @level2type = N'COLUMN', @level2name = N'CREATE_DATE';

