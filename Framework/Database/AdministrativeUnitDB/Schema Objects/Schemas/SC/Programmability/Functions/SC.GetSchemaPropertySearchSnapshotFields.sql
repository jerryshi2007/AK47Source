--根据schema名和过滤规则，获取Schema属性全文检索的字段

CREATE FUNCTION [SC].[GetSchemaPropertySearchSnapshotFields]
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

	SET @fields = ''

	DECLARE PropertyCursor CURSOR FOR 
		SELECT PD.Name
		FROM SC.SchemaDefine SD INNER JOIN SC.SchemaPropertyDefine PD ON SD.Name = PD.SchemaName
		WHERE SD.Name = @schemaName AND (SnapshotMode & @snapshotFilter) <> 0

	OPEN PropertyCursor
  
	FETCH NEXT FROM PropertyCursor INTO @propertyName

	WHILE @@FETCH_STATUS = 0
	BEGIN
		IF @fields <> ''
			SET @fields = @fields + ' + ' + '''' + @splitChar + ''' + '

		SET @fields = @fields + 'ISNULL(Data.value(''Object[1]/@' + @propertyName + ''', ''NVARCHAR(MAX)''),'''') '

		FETCH NEXT FROM PropertyCursor INTO @propertyName
	END

	CLOSE PropertyCursor;
	DEALLOCATE PropertyCursor;

	return @fields
END
