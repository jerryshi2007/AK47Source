CREATE TABLE [dbo].[RANK_DEFINE] (
    [CODE_NAME]  NVARCHAR (32) NOT NULL,
    [SORT_ID]    INT           CONSTRAINT [DF_RANK_DEFINE_SORT] DEFAULT (0) NOT NULL,
    [NAME]       NVARCHAR (32) NOT NULL,
    [VISIBLE]    INT           CONSTRAINT [DF_RANK_DEFINE_DISPLAY] DEFAULT (1) NOT NULL,
    [RANK_CLASS] INT           CONSTRAINT [DF_RANK_DEFINE_RANK_CLASS] DEFAULT (1) NOT NULL,
    CONSTRAINT [PK_RANK_DEFINE] PRIMARY KEY CLUSTERED ([CODE_NAME] ASC),
    CONSTRAINT [IX_RANK_DEFINE_CODE_NAME] UNIQUE NONCLUSTERED ([CODE_NAME] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'行政级别的标志ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'RANK_DEFINE', @level2type = N'COLUMN', @level2name = N'CODE_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'内部排序', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'RANK_DEFINE', @level2type = N'COLUMN', @level2name = N'SORT_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'行政级别的显示名称', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'RANK_DEFINE', @level2type = N'COLUMN', @level2name = N'NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'显示与否（0、不显示；1、显示）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'RANK_DEFINE', @level2type = N'COLUMN', @level2name = N'VISIBLE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'行政级别使用对象（1、机构；2、人员）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'RANK_DEFINE', @level2type = N'COLUMN', @level2name = N'RANK_CLASS';

