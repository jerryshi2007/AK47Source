



CREATE  PROCEDURE [dbo].[CommonSplitPageQuery]
	@Fields nvarchar(1024),
	@Tables nvarchar(1024),
	@Where varchar(1024) = '',
	@OrderBy nvarchar(255) = '',
	@Key nvarchar(255),
	@PageNo INT = 1,
	@PageCount INT = 20,
	@RetRowCount bit = 1
AS
SET NOCOUNT ON

DECLARE @Top varchar(32)

SET @Top = ''

IF @PageCount >= 0
	SET @Top = 'TOP ' + CAST(@PageNo * @PageCount AS varchar(32))

DECLARE @CurFields varchar(1024)

SET @CurFields = 'CAST(' + @Key + ' AS varchar(255)) AS [KEY]'

DECLARE @Select varchar(1024)
DECLARE @OtherPart varchar(7680)

SET @Select = 'SELECT ' + @Top + ' ' + @CurFields

SET @OtherPart = ' FROM ' + @Tables

IF @Where <> ''
 	SET @OtherPart = @OtherPart + ' WHERE ' + @Where

DECLARE @CountSQL AS varchar(7680)

SET @CountSQL = 'SELECT COUNT(*) AS ROW_COUNT' + @OtherPart

IF @OrderBy <> ''
	SET @OtherPart = @OtherPart + ' ORDER BY ' + @OrderBy 

CREATE TABLE #KeyTable (ROW_NUMBER INT IDENTITY(1, 1) PRIMARY KEY, [ROW_KEY] varchar(255))

PRINT @Select + @OtherPart

INSERT INTO #KeyTable
EXEC (@Select + @OtherPart)

DECLARE @ROWS_SCANED INT
SET @ROWS_SCANED = @@ROWCOUNT

SET @CurFields = '#KeyTable.ROW_NUMBER, ' + @Fields

SET @Select = 'SELECT ' + @CurFields
SET @OtherPart = ' FROM ' + @Tables + ', #KeyTable'

IF @Where <> ''
 	SET @OtherPart = @OtherPart + ' WHERE #KeyTable.ROW_NUMBER > ' + CAST((@PageNo - 1) * @PageCount AS varchar(32))
			+ ' AND #KeyTable.ROW_NUMBER <= ' + CAST((@PageNo) * @PageCount AS varchar(32)) + 
			+ ' AND #KeyTable.ROW_KEY = ' + @Key
			+ ' AND ' + @Where
ELSE
	SET @OtherPart = @OtherPart + ' WHERE #KeyTable.ROW_NUMBER > ' + CAST((@PageNo - 1) * @PageCount AS varchar(32))
			+ ' AND #KeyTable.ROW_NUMBER <= ' + CAST((@PageNo) * @PageCount AS varchar(32))
			+ ' AND #KeyTable.ROW_KEY = ' + @Key

IF @OrderBy <> ''
	SET @OtherPart = @OtherPart + ' ORDER BY ' + @OrderBy

EXEC (@Select + @OtherPart)

DECLARE @ROWS_FETCHED INT
SET @ROWS_FETCHED = @@ROWCOUNT

PRINT @Select + @OtherPart

IF @RetRowCount = 1
	IF (@ROWS_FETCHED = 0)
		SELECT @ROWS_SCANED AS ROW_COUNT
	ELSE
	IF (@PageNo = 1)
		EXEC (@CountSQL)

PRINT @CountSQL

SET NOCOUNT OFF
