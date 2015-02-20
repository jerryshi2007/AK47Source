/* SchemaObject 的当前视图，和索引*/

CREATE VIEW [SC].[SchemaObject_Current]
WITH SCHEMABINDING 
AS
SELECT [ID], [VersionStartTime], [VersionEndTime], [Status], [Data], [SchemaType], [CreateDate], [CreatorID], [CreatorName]
FROM [SC].[SchemaObject]
WHERE [VersionEndTime] = CONVERT(DATETIME, '99990909 00:00:00', 112) AND [Status] = 1

GO

/****** Object:  Index [SchemaObject_Current_ClusteredIndex]    Script Date: 2014/1/24 16:37:14 ******/
CREATE UNIQUE CLUSTERED INDEX [SchemaObject_Current_ClusteredIndex] ON [SC].[SchemaObject_Current]
(
	[ID] ASC,
	[VersionStartTime] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO