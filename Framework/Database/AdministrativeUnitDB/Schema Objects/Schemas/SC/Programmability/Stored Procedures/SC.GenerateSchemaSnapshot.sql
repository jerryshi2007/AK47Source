/* 生成一个Schema的快照  */

CREATE PROCEDURE [SC].[GenerateSchemaSnapshot]
	@schemaName NVARCHAR(128)
AS
BEGIN
	DECLARE @snapshotTableName NVARCHAR(255)
	DECLARE @tableName NVARCHAR(255)
	DECLARE @toSchemaObjectSnapshot INT

	SELECT TOP 1 @snapshotTableName = SD.SnapshotTable, @tableName = SD.TableName, @toSchemaObjectSnapshot = ToSchemaObjectSnapshot
	FROM SC.SchemaDefine SD
	WHERE SD.Name = @schemaName

	IF (@snapshotTableName IS NOT NULL)
		EXECUTE SC.GenerateSchemaTableSnapshot @schemaName, 1, @tableName, @snapshotTableName

	IF (@toSchemaObjectSnapshot = 1)
		EXECUTE SC.GenerateSchemaTableSnapshot @schemaName, 16, 'SC.SchemaObject', 'SC.SchemaObjectSnapshot'
END
