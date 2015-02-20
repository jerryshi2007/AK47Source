/* 生成一个Schema表的快照数据 */
CREATE PROCEDURE [SC].[GenerateSchemaTableSnapshot]
	@schemaName NVARCHAR(128),
	@snapshotFieldMask INT,
	@tableName NVARCHAR(255),
	@snapshotTableName NVARCHAR(255)
AS
BEGIN
	IF (@snapshotTableName IS NOT NULL)
	BEGIN
		IF @tableName IS NULL OR @tableName = ''
			SET @tableName = 'SC.SchemaObject'

		--DECLARE @clearSql NVARCHAR(MAX)

		--SET @clearSql = 'TRUNCATE TABLE ' + @snapshotTableName

		--EXECUTE dbo.sp_executesql @clearSql

		DECLARE @standardFields NVARCHAR(256)

		SET @standardFields = 'VersionStartTime, VersionEndTime, SchemaType, Status, CreateDate, CreatorID, CreatorName'

		DECLARE @valueFields NVARCHAR(MAX)
		DECLARE @searchValueFields NVARCHAR(MAX)

		SET @searchValueFields = SC.GetSchemaPropertySearchSnapshotFields(@schemaName, 8, ' ')
		SET @valueFields = SC.GetSchemaPropertyValueSnapshotFields(@schemaName, @snapshotFieldMask, ', ')

		DECLARE @selectFields NVARCHAR(MAX)
		DECLARE @allValueFields NVARCHAR(MAX)

		IF (@valueFields <> '')
		BEGIN
			SET @allValueFields = @standardFields + ', ' + @valueFields
			SET @selectFields = @standardFields + ', ' + SC.GetSchemaPropertySnapshotFields(@schemaName, @snapshotFieldMask, ', ')
		END
		ELSE
		BEGIN
			SET @allValueFields = @standardFields
			SET @selectFields = @standardFields
		END
	
		IF (@searchValueFields <> '')
		BEGIN
			SET @allValueFields = @allValueFields + ', ' + @searchValueFields
			SET @selectFields = @selectFields + ', SearchContent'
		 END

		DECLARE @sql NVARCHAR(MAX)

		SET @sql = 'INSERT INTO ' + @snapshotTableName + '(' + @selectFields + ') SELECT ' + @allValueFields + ' FROM ' + @tableName + ' WHERE SchemaType = ''' + @schemaName + ''''

		PRINT @sql

		EXECUTE dbo.sp_executesql @sql
	END
END
