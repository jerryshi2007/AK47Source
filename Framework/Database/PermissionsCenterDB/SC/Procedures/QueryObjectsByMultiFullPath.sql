/*根据多个用分号分隔的全路径查询对象*/
CREATE PROCEDURE [SC].[QueryObjectsByMultiFullPath]
	@fullPaths NVARCHAR(MAX)
AS
BEGIN
	SET NOCOUNT ON
	DECLARE @i INT
	SET @i = 1

	DECLARE @tempName NVARCHAR(MAX)
	SET @tempName = ''

	DECLARE @result TABLE(ID NVARCHAR(36), Name NVARCHAR(255), DisplayName NVARCHAR(255), CodeName NVARCHAR(64), ParentID NVARCHAR(36), InnerSort INT, IsDefault INT, Status INT, SchemaType NVARCHAR(36))

	WHILE @i <= LEN(@fullPaths)
	BEGIN
		IF SUBSTRING(@fullPaths, @i, 1) = ';'
		BEGIN
			INSERT INTO @result
				EXEC SC.QueryObjectsByFullPath @tempName

			SET @tempName = ''
		END
		ELSE
			SET @tempName = @tempName + SUBSTRING(@fullPaths, @i, 1)

		SET @i = @i + 1
	END

	IF @tempName  <> ''
	BEGIN
		INSERT INTO @result
			EXEC SC.QueryObjectsByFullPath @tempName
	END

	SET NOCOUNT OFF
	SELECT * FROM @result
END