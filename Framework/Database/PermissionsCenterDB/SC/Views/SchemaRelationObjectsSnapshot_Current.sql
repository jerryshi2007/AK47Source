﻿/* SchemaRelationObjects 的当前视图，和索引*/

CREATE VIEW [SC].[SchemaRelationObjectsSnapshot_Current]
WITH SCHEMABINDING 
AS
SELECT [ParentID], [ObjectID], [VersionStartTime], [VersionEndTime], [Status], [IsDefault], [InnerSort], [FullPath], [GlobalSort], [CreateDate], [ParentSchemaType], [ChildSchemaType], [SearchContent], [RowUniqueID], [SchemaType], [CreatorID], [CreatorName]
FROM [SC].[SchemaRelationObjectsSnapshot]
WHERE [VersionEndTime] = CONVERT(DATETIME, '99990909 00:00:00', 112) AND [Status] = 1

GO

/****** Object:  Index [SchemaRelationObjectsSnapshot_Current_ClusteredIndex]    Script Date: 2013/5/17 16:16:13 ******/
CREATE UNIQUE CLUSTERED INDEX [SchemaRelationObjectsSnapshot_Current_ClusteredIndex] ON [SC].[SchemaRelationObjectsSnapshot_Current]
(
	[ParentID] ASC,
	[ObjectID] ASC,
	[VersionStartTime] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

CREATE UNIQUE INDEX [IX_SchemaRelationObjectsSnapshot_Current_RowID] ON [SC].[SchemaRelationObjectsSnapshot_Current] ([RowUniqueID])

GO

CREATE INDEX [IX_SchemaRelationObjectsSnapshot_Current_ObjectID] ON [SC].[SchemaRelationObjectsSnapshot_Current] ([ObjectID])

GO

CREATE INDEX [IX_SchemaRelationObjectsSnapshot_Current_FullPath] ON [SC].[SchemaRelationObjectsSnapshot_Current] ([FullPath], [VersionStartTime])

GO

CREATE INDEX [IX_SchemaRelationObjectsSnapshot_Current__V_GlobalSort] ON [SC].[SchemaRelationObjectsSnapshot_Current] ([GlobalSort], [VersionStartTime])

GO

