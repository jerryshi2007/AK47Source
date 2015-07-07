/*UserAndContainerSnapshot 的当前视图，和索引*/

CREATE VIEW [SC].[UserAndContainerSnapshot_Current]
WITH SCHEMABINDING 
AS
SELECT [UserID], [ContainerID], [VersionStartTime], [VersionEndTime], [Status], [UserSchemaType], [ContainerSchemaType]
	
FROM [SC].[UserAndContainerSnapshot]
WHERE [VersionEndTime] = CONVERT(DATETIME, '99990909 00:00:00', 112) AND [Status] = 1

GO

/****** Object:  Index [UserAndContainerSnapshot_Current_ClusteredIndex]    Script Date: 2013/5/17 16:16:13 ******/
CREATE UNIQUE CLUSTERED INDEX [UserAndContainerSnapshot_Current_ClusteredIndex] ON [SC].[UserAndContainerSnapshot_Current]
(
	[UserID] ASC,
	[ContainerID] ASC,
	[VersionStartTime] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

CREATE INDEX [IX_UserAndContainerSnapshot_Current_ContainerID] ON [SC].[UserAndContainerSnapshot_Current] ([ContainerID])

GO