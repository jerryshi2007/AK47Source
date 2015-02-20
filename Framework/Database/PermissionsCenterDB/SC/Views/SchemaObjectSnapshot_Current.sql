/*SchemaObjectSnapshot 的当前视图，和索引*/

CREATE VIEW [SC].[SchemaObjectSnapshot_Current]
WITH SCHEMABINDING 
AS
SELECT [ID], [VersionStartTime], [VersionEndTime], [Status], [CreateDate], [Name], [DisplayName], [CodeName], [AccountDisabled], [SearchContent], [RowUniqueID], [SchemaType], [CreatorID], [CreatorName], [Comment]
FROM [SC].[SchemaObjectSnapshot]
WHERE [VersionEndTime] = CONVERT(DATETIME, '99990909 00:00:00', 112) AND [Status] = 1

GO

/****** Object:  Index [SchemaObjectSnapshot_Current_ClusteredIndex]    Script Date: 2013/5/17 16:16:13 ******/
CREATE UNIQUE CLUSTERED INDEX [SchemaObjectSnapshot_Current_ClusteredIndex] ON [SC].[SchemaObjectSnapshot_Current]
(
	[ID] ASC,
	[VersionStartTime] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE UNIQUE INDEX [IX_SchemaObjectSnapshot_Current_RowID] ON [SC].[SchemaObjectSnapshot_Current] ([RowUniqueID])

GO

CREATE INDEX [IX_SchemaObjectSnapshot_Current_CodeName] ON [SC].[SchemaObjectSnapshot_Current] ([CodeName])

GO

CREATE FULLTEXT INDEX ON [SC].[SchemaObjectSnapshot_Current]
    ([SearchContent] LANGUAGE 2052)
    KEY INDEX [IX_SchemaObjectSnapshot_Current_RowID]
    ON [SCFullTextIndex] WITH CHANGE_TRACKING AUTO
