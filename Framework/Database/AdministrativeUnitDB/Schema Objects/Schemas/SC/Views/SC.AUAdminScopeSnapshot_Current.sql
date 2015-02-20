-- AUAdminScopeSnapshot 的当前视图和索引
CREATE VIEW [SC].[AUAdminScopeSnapshot_Current]
WITH SCHEMABINDING
	AS 
	SELECT ID, VersionStartTime, VersionEndTime, Status, SchemaType, CreateDate, CreatorID, CreatorName, SearchContent, RowUniqueID, ScopeSchemaType 
	FROM [SC].[AUAdminScopeSnapshot]
WHERE [VersionEndTime] = CONVERT(DATETIME, '99990909 00:00:00', 112) AND [Status] = 1
GO
/****** Object:  Index [AUAdminScopeSnapshot_Current_ClusteredIndex]    Script Date: 2013/5/17 16:16:13 ******/
CREATE UNIQUE CLUSTERED INDEX [AUAdminScopeSnapshot_Current_ClusteredIndex] ON [SC].[AUAdminScopeSnapshot_Current]
(
	[ID] ASC,
	[VersionStartTime] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE INDEX [IX_AUAdminScopeSnapshot_Current_CodeName] ON [SC].[AUAdminScopeSnapshot_Current] ([ScopeSchemaType])

GO