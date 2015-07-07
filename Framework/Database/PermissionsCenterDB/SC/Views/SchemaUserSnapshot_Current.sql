/*SchemaUserSnapshot 的当前视图，和索引*/

CREATE VIEW [SC].[SchemaUserSnapshot_Current]
WITH SCHEMABINDING 
AS
SELECT [ID], [VersionStartTime], [VersionEndTime], [Status], [CreateDate], [SearchContent], [RowUniqueID], [SchemaType], [CreatorID], [CreatorName],
	[Name], [CodeName], [DisplayName], [FirstName], [LastName], [Mail], [Sip], [MP], [WP], [Address], [OwnerID], [OwnerName],
	[AccountDisabled], [PasswordNotRequired], [DontExpirePassword], [AccountExpires], [AccountInspires], [Comment]
FROM [SC].[SchemaUserSnapshot]
WHERE [VersionEndTime] = CONVERT(DATETIME, '99990909 00:00:00', 112) AND [Status] = 1

GO

/****** Object:  Index [SchemaUserSnapshot_Current_ClusteredIndex]    Script Date: 2013/5/17 16:16:13 ******/
CREATE UNIQUE CLUSTERED INDEX [SchemaUserSnapshot_Current_ClusteredIndex] ON [SC].[SchemaUserSnapshot_Current]
(
	[ID] ASC,
	[VersionStartTime] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

CREATE UNIQUE INDEX [IX_SchemaUserSnapshot_Current_RowID] ON [SC].[SchemaUserSnapshot_Current] ([RowUniqueID])

GO

CREATE INDEX [IX_SchemaUserSnapshot_Current_CodeName] ON [SC].[SchemaUserSnapshot_Current] ([CodeName])

GO

CREATE INDEX [IX_SchemaUserSnapshot_Current_OwnerID] ON [SC].[SchemaUserSnapshot_Current] ([OwnerID])

GO


