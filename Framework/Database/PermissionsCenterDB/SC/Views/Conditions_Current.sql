/*Conditons 的当前视图，和索引*/

CREATE VIEW [SC].[Conditions_Current] WITH SCHEMABINDING 
AS
SELECT [OwnerID],[Type],[SortID],[Description],[Condition],[VersionStartTime],[VersionEndTime],[Status]
FROM [SC].[Conditions]
WHERE [VersionEndTime] = CONVERT(DATETIME, '99990909 00:00:00', 112) AND [Status] = 1

GO

CREATE UNIQUE CLUSTERED INDEX [Conditions_Current_ClusteredIndex] ON [SC].[Conditions_Current]
(
	[OwnerID] ASC,
	[Type] ASC,
	[SortID] ASC,
	[VersionStartTime] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)

GO