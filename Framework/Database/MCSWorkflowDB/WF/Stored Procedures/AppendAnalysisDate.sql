CREATE PROCEDURE [WF].[AppendAnalysisDate]
--ALTER PROCEDURE [WF].[AppendAnalysisDate]
	@key INT = 0,
	@date DATETIME
AS
INSERT INTO WF.ANALYSIS_DATES
(
	DateKey, FullDateAlternateKey, DayNumberOfWeek, DayNameOfWeek, DayNumberOfMonth, DayNumberOfYear, WeekNumberOfYear, MonthName, MonthNumber, Quarter, HalfYear, Year	
)
 VALUES(
	@key, -- datekey
	@date,--FullDateAlternateKey
	DATEPART(WEEKDAY, @date), --DayNumberOfWeek
	CHOOSE(DATEPART(WEEKDAY, @date),'星期日','星期一','星期二','星期三','星期四','星期五','星期六'),
	DATEPART(DAY, @date), --DayNumberOfMonth
	DATEPART(DAYOFYEAR, @date), -- DayNumberOfYear
	DATEPART(WEEK, @date), -- WeekNumberOfYear
	CONVERT(nvarchar(2), DATEPART(MONTH, @date)) + '月',--MonthName,
	DATEPART(MONTH,@date), --MonthNumber,
	DATEPART(QUARTER, @date), --Quarter,
	CHOOSE(DATEPART(MONTH, @date),1,1,1,1,1,1, 2,2,2,2,2,2), --HalfYear,
	DATEPART(YEAR ,@date) --Year
)

RETURN 0

