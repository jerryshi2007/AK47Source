/*ACL的当前视图,索引*/

CREATE VIEW [SC].[Acl_Current]
WITH SCHEMABINDING 
AS
SELECT [ContainerID], [ContainerPermission],  [MemberID], [VersionStartTime], [VersionEndTime], [Status], [SortID], [Data], [ContainerSchemaType], [MemberSchemaType]
FROM [SC].[Acl]
WHERE [VersionEndTime] = CONVERT(DATETIME, '99990909 00:00:00', 112) AND [Status] = 1

GO

/****** Object:  Index [Conditions_Current_ClusteredIndex]    Script Date: 2013/5/17 16:16:13 ******/
CREATE UNIQUE CLUSTERED INDEX [Acl_Current_ClusteredIndex] ON [SC].[Acl_Current]
(
	[ContainerID] ASC,
	[ContainerPermission] ASC,
	[MemberID] ASC,
	[VersionStartTime] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

CREATE INDEX [IX_Acl_Current_MemberID] ON [SC].[Acl_Current] ([MemberID])