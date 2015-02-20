/*生成插入表的SQL*/
CREATE PROCEDURE [SC].[GenerateInsertSql]
	@tablename varchar(256)
AS
BEGIN
	DECLARE @sql VARCHAR(MAX)
	DECLARE @sqlValues VARCHAR(MAX)

	SET @sql =' ('
	SET @sqlValues = 'VALUES (''+'
		SELECT @sqlValues = @sqlValues + cols + ' + '','' + ' , @sql = @sql + '[' + name + '],'
		FROM
		(SELECT CASE
				WHEN xtype IN (48, 52, 56, 59, 60, 62, 104, 106, 108, 122, 127) 
					THEN 'CASE WHEN ' + name + ' IS NULL THEN ''NULL'' ELSE ' + 'CAST(' + name + ' AS VARCHAR)'+' END'
				WHEN xtype IN (58, 61)
					THEN 'CASE WHEN ' + name + ' IS NULL THEN ''NULL'' ELSE ' + ''''''''' + ' + 'CONVERT(NVARCHAR, ' + name + N', 25)' + ' + ''''''''' + ' END'
				WHEN xtype IN (241)
					THEN 'CASE WHEN ' + name + ' IS NULL THEN ''NULL'' ELSE ' + ''''''''' + ' + 'CONVERT(NVARCHAR(MAX), ' + name + N')' + ' + ''''''''' + ' END'
				WHEN xtype IN (167)
					THEN 'CASE WHEN ' + name + ' IS NULL THEN ''NULL'' ELSE ' + ''''''''' + ' + 'REPLACE(' + name +', '''''''', '''''''''''')' + ' + ''''''''' + ' END'
				WHEN xtype IN (231)
					THEN 'CASE WHEN ' + name + ' IS NULL THEN ''NULL'' ELSE ' + '''N'''''' + ' + 'REPLACE(' + name +', '''''''', '''''''''''')' + ' + ''''''''' + ' END'
				WHEN xtype IN (175)
					THEN 'CASE WHEN ' + name + ' IS NULL THEN ''NULL'' ELSE ' + ''''''''' + ' + 'CAST(REPLACE(' + name +', '''''''', '''''''''''') AS CHAR(' + CAST(LENGTH AS VARCHAR) + ')) + ''''''''' + ' END'
				WHEN xtype IN (239)
					THEN 'CASE WHEN ' + name + ' IS NULL THEN ''NULL'' ELSE ' + '''N'''''' + ' + 'CAST(REPLACE(' + name +','''''''','''''''''''') AS CHAR(' + CAST(LENGTH AS VARCHAR) + ')) + ''''''''' + ' END'
				ELSE '''NULL'''
			END AS Cols, name
		FROM syscolumns 
		WHERE id = object_id(@tablename)
		) T
	
	SET @sql ='SELECT ''INSERT INTO ' + @tablename + LEFT(@sql, LEN(@sql) - 1) + ') ' + LEFT(@sqlValues, LEN(@sqlValues) - 4) + ')'' FROM ' + @tablename
	
	PRINT @sql
	EXEC (@sql)
END
