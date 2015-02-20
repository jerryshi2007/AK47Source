-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[GetParamValue]
(
	@exp NVARCHAR(max),
	@startIndex INT,
	@paramIndex INT
)
RETURNS NVARCHAR(max)
AS
BEGIN
	DECLARE @count INT
	SET @count = dbo.SkipSpecialChar(@exp, @startIndex)

	DECLARE @paramValue NVARCHAR(max)
	SET @paramValue = ''

	DECLARE @paramCount INT
	SET @paramCount = 1

	DECLARE @currentChar NVARCHAR(1)
	SET @currentChar = ''

	DECLARE @foundIndex INT
	SET @foundIndex = 0

	WHILE (@count > 0)
	BEGIN
		SET @foundIndex = dbo.FindSepecialChar(@exp, @count)
		
		IF (@foundIndex <> 0)
		BEGIN
			IF (@paramCount = @paramIndex)
			BEGIN
				SET @paramValue = SUBSTRING(@exp, @count, @foundIndex - @count)
				BREAK
			END
			ELSE
			BEGIN
				SET @count = dbo.SkipSpecialChar(@exp, @foundIndex)
				SET @paramCount = @paramCount + 1
			END
		END
	END

	RETURN @paramValue
END
