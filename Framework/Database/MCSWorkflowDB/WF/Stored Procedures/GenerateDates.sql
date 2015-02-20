/*
生成 分析用的日期表
*/
CREATE PROCEDURE [WF].[GenerateDates]
--ALTER PROCEDURE [WF].[GenerateDates]
	@beginTime datetime,
	@endTime datetime
AS
BEGIN
	DECLARE @curtime datetime
	DECLARE @yy int
	DECLARE @mm tinyint
	DECLARE @dd tinyint
	DECLARE @hh tinyint
	
	SET NOCOUNT ON
	TRUNCATE TABLE WF.ANALYSIS_DATES
	SET @curtime = @beginTime

	WHILE( @curtime <= @endTime)
	BEGIN
		SELECT 
			@yy = DATEPART(YEAR,@curtime),
			@mm = DATEPART(MONTH,@curtime),
			@dd = DATEPART(DAY,@curtime)
		
		SET @yy = (((@yy * 100 + @mm)*100+ @dd))

		EXEC WF.AppendAnalysisDate @yy,@curtime

		SELECT @curtime = DATEADD(DAY,1,@curtime) 	
	END

	RETURN 0
END
