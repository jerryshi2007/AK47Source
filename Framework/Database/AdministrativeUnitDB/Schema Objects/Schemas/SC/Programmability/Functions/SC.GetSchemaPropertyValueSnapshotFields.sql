/* 根据schema名称和快照过滤类型，分隔符，获取schema属性值的快照字段 */

CREATE FUNCTION [SC].[GetSchemaPropertyValueSnapshotFields]
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
	DECLARE @dataType NVARCHAR(64)

	SET @fields = ''

	DECLARE PropertyCursor CURSOR FOR 
		SELECT PD.Name, PD.DataType, PD.SnapshotFieldName
		FROM SC.SchemaDefine SD INNER JOIN SC.SchemaPropertyDefine PD ON SD.Name = PD.SchemaName
		WHERE SD.Name = @schemaName AND (SnapshotMode & @snapshotFilter) <> 0

	OPEN PropertyCursor
  
		FETCH NEXT FROM PropertyCursor INTO @propertyName, @dataType, @snapshotFieldName

		WHILE @@FETCH_STATUS = 0
		BEGIN
			IF @snapshotFieldName IS NULL OR @snapshotFieldName = ''
				SET @snapshotFieldName = @propertyName

			IF @fields <> ''
				SET @fields = @fields + @splitChar

			SET @fields = @fields + 'SC.ConvertPropertyValue(Data.value(''Object[1]/@' + @propertyName + ''', ''NVARCHAR(MAX)''), ''' + @dataType + ''') AS ' + '[' + @snapshotFieldName + ']'

			FETCH NEXT FROM PropertyCursor INTO @propertyName, @dataType, @snapshotFieldName
		END

	CLOSE PropertyCursor;
	DEALLOCATE PropertyCursor;

	return @fields
END
