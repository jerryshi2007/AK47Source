/* ItemAndContainerSnapshot 的当前视图，和索引*/

CREATE VIEW [SC].[ItemAndContainerSnapshot_Current]
WITH SCHEMABINDING
	AS 
	SELECT ItemID, ContainerID, VersionStartTime, VersionEndTime, Status, ItemSchemaType, ContainerSchemaType
	FROM [SC].[ItemAndContainerSnapshot]
WHERE [VersionEndTime] = CONVERT(DATETIME, '99990909 00:00:00', 112) AND [Status] = 1
GO

/****** Object:  Index [ItemAndContainerSnapshot_Current_ClusteredIndex]    Script Date: 2013/5/17 16:16:13 ******/
CREATE UNIQUE CLUSTERED INDEX [ItemAndContainerSnapshot_Current_ClusteredIndex] ON [SC].[ItemAndContainerSnapshot_Current]
(
	[ItemID] ASC,
	[ContainerID] ASC,
	[VersionStartTime] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE INDEX [IX_ItemAndContainerSnapshot_Current_ContainerID] ON [SC].[ItemAndContainerSnapshot_Current] ([ContainerID])

GO