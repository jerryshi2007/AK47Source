CREATE TABLE [WF].[ANALYSIS_DATES]
(
	[DateKey]              INT           NOT NULL ,
    [FullDateAlternateKey] DATE          NOT NULL ,
    [DayNumberOfWeek]      TINYINT       NOT NULL ,
    [DayNameOfWeek] NCHAR(3) NOT NULL ,
    [DayNumberOfMonth]     TINYINT       NOT NULL ,
    [DayNumberOfYear]      SMALLINT      NOT NULL ,
    [WeekNumberOfYear]     TINYINT       NOT NULL ,
    [MonthName]     NVARCHAR (10) NOT NULL,
    [MonthNumber]    TINYINT       NOT NULL,
    [Quarter]      TINYINT       NOT NULL,
	[HalfYear]	TINYINT NOT NULL,
    [Year]         SMALLINT      NOT NULL,
    CONSTRAINT [PK_DimDate_DateKey] PRIMARY KEY CLUSTERED ([DateKey] ASC),
    CONSTRAINT [AK_DimDate_FullDateAlternateKey] UNIQUE NONCLUSTERED ([FullDateAlternateKey] ASC)
);


GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'日期的键',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'ANALYSIS_DATES',
    @level2type = N'COLUMN',
    @level2name = N'DateKey'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'日期的候选键',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'ANALYSIS_DATES',
    @level2type = N'COLUMN',
    @level2name = N'FullDateAlternateKey'
GO

GO

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'周日，注意星期天是1',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'ANALYSIS_DATES',
    @level2type = N'COLUMN',
    @level2name = N'DayNumberOfWeek'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'日期的中文名',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'ANALYSIS_DATES',
    @level2type = N'COLUMN',
    @level2name = N'DayNameOfWeek'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'日期在月份中是第几天',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'ANALYSIS_DATES',
    @level2type = N'COLUMN',
    @level2name = N'DayNumberOfMonth'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'日期在当年是第几天',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'ANALYSIS_DATES',
    @level2type = N'COLUMN',
    @level2name = N'DayNumberOfYear'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'日期在一年中是第几天',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'ANALYSIS_DATES',
    @level2type = N'COLUMN',
    @level2name = N'WeekNumberOfYear'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'日期月份的名称',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'ANALYSIS_DATES',
    @level2type = N'COLUMN',
    @level2name = N'MonthName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'日期月份在当年是第几月',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'ANALYSIS_DATES',
    @level2type = N'COLUMN',
    @level2name = N'MonthNumber'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'日期是第几个季度',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'ANALYSIS_DATES',
    @level2type = N'COLUMN',
    @level2name = N'Quarter'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'年份',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'ANALYSIS_DATES',
    @level2type = N'COLUMN',
    @level2name = N'Year'
GO

GO

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'是否是下半年',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'ANALYSIS_DATES',
    @level2type = N'COLUMN',
    @level2name = N'HalfYear'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'用于统计分析生成的日期相关的维度',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'ANALYSIS_DATES',
    @level2type = NULL,
    @level2name = NULL