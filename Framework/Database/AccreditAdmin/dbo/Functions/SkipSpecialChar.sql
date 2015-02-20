-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[SkipSpecialChar]
(
	@exp NVARCHAR(max),
	@startIndex INT
)
RETURNS int
AS
BEGIN
	DECLARE @count INT
	SET @count = @startIndex
	
	DECLARE @result INT
	SET @result = 0

	DECLARE @currentChar NVARCHAR(1)
	SET @currentChar = ''

	WHILE (@count <= LEN(@exp))
	BEGIN
		SET @currentChar = SUBSTRING(@exp, @count, 1)
		
		IF (@currentChar <> ',' AND @currentChar <> ' ' AND @currentChar <> '"' AND @currentChar <> '(' AND @currentChar <> ')')
		BEGIN
			SET @result = @count;
			BREAK;
		END
		
		SET @count = @count + 1
	END
	
	RETURN @result
END
