--AUSchemaRoleSnapshot 的当前视图和索引
CREATE VIEW [SC].[AUSchemaRoleSnapshot_Current]
WITH SCHEMABINDING
	AS 
	SELECT ID, VersionStartTime, VersionEndTime, Status, SchemaType, CreateDate, CreatorID, CreatorName, SearchContent, RowUniqueID, CodeName, Name, DisplayName, Comment
	FROM [SC].[AUSchemaRoleSnapshot]
WHERE [VersionEndTime] = CONVERT(DATETIME, '99990909 00:00:00', 112) AND [Status] = 1

GO

/****** Object:  Index [AUSchemaRoleSnapshot_Current_ClusteredIndex]    Script Date: 2013/5/17 16:16:13 ******/
CREATE UNIQUE CLUSTERED INDEX [AUSchemaRoleSnapshot_Current_ClusteredIndex] ON [SC].[AUSchemaRoleSnapshot_Current]
(
	[ID] ASC,
	[VersionStartTime] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE UNIQUE INDEX [IX_AUSchemaRoleSnapshot_Current_RowID] ON [SC].[AUSchemaRoleSnapshot_Current] ([RowUniqueID])

GO

CREATE INDEX [IX_AUSchemaRoleSnapshot_Current_CodeName] ON [SC].[AUSchemaRoleSnapshot_Current] ([CodeName])

GO

CREATE FULLTEXT INDEX ON [SC].[AUSchemaRoleSnapshot_Current]
    ([SearchContent] LANGUAGE 2052)
    KEY INDEX [IX_AUSchemaRoleSnapshot_Current_RowID]
    ON [SCFullTextIndex] WITH CHANGE_TRACKING AUTO