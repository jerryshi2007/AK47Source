--关系对象的当前视图
CREATE VIEW [SC].[SchemaRelationObjects_Current]
	AS SELECT * FROM [SC].[SchemaRelationObjects]
WHERE [VersionEndTime] = CONVERT(DATETIME, '99990909 00:00:00', 112) AND [Status] = 1
