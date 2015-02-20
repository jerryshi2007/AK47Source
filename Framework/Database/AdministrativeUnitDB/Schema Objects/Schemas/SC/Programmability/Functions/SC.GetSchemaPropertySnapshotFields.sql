/* 获取Schema属性快照的字段 */

CREATE FUNCTION [SC].[GetSchemaPropertySnapshotFields]
(
	@schemaName NVARCHAR(256),
	@snapshotFilter INT,
	@splitChar NVARCHAR(64)
)
RETURNS NVARCHAR(MAX)
AS
BEGIN
	DECLARE @fields NVARCHAR(MAX)
	DECLARE @propertyName NVARCHAR(255)
	DECLARE @snapshotFieldName NVARCHAR(255)

	SET @fields = ''

	DECLARE PropertySearchCursor CURSOR FOR 
		SELECT PD.Name, PD.SnapshotFieldName
		FROM SC.SchemaDefine SD INNER JOIN SC.SchemaPropertyDefine PD ON SD.Name = PD.SchemaName
		WHERE SD.Name = @schemaName AND (SnapshotMode & @snapshotFilter) <> 0

	OPEN PropertySearchCursor
  
		FETCH NEXT FROM PropertySearchCursor INTO @propertyName, @snapshotFieldName

		WHILE @@FETCH_STATUS = 0
		BEGIN
			IF @snapshotFieldName IS NULL OR @snapshotFieldName = ''
				SET @snapshotFieldName = @propertyName

			IF @fields <> ''
				SET @fields = @fields + @splitChar

			SET @fields = @fields + '[' + @snapshotFieldName + ']'

			FETCH NEXT FROM PropertySearchCursor INTO @propertyName, @snapshotFieldName
		END

	CLOSE PropertySearchCursor;
	DEALLOCATE PropertySearchCursor;

	return @fields
END
