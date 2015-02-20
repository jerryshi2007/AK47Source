CREATE TABLE [WF].[COUNTRY_CODE] (
    [Code]         NVARCHAR (36) NOT NULL,
    [CnName]       NVARCHAR (50) NOT NULL,
    [EnName]       NVARCHAR (50) NOT NULL,
    [Abbreviation] NVARCHAR (36) NULL,
    [ValidStatus]  BIT           DEFAULT ((1)) NULL,
    [SortNo]       INT           NULL,
    PRIMARY KEY CLUSTERED ([Code] ASC)
);


GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'国家编码，用于电话号码控件',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'COUNTRY_CODE',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'唯一ID',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'COUNTRY_CODE',
    @level2type = N'COLUMN',
    @level2name = N'Code'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'中文名',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'COUNTRY_CODE',
    @level2type = N'COLUMN',
    @level2name = N'CnName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'英文名',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'COUNTRY_CODE',
    @level2type = N'COLUMN',
    @level2name = N'EnName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'缩写，例如CN，US',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'COUNTRY_CODE',
    @level2type = N'COLUMN',
    @level2name = N'Abbreviation'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'排序号',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'COUNTRY_CODE',
    @level2type = N'COLUMN',
    @level2name = N'SortNo'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'是否启用',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'COUNTRY_CODE',
    @level2type = N'COLUMN',
    @level2name = N'ValidStatus'