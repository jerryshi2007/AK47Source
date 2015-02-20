CREATE TABLE [SC].[SchemaObject]
(
	[ID]               NVARCHAR (36) NOT NULL,
    [VersionStartTime] DATETIME      NOT NULL,
    [VersionEndTime]   DATETIME      CONSTRAINT [DF_SchemaObject_VersionEndTime] DEFAULT ('99990909 00:00:00') NULL,
    [Status]           INT           CONSTRAINT [DF_SchemaObject_Status] DEFAULT ((1)) NULL,
    [Data]             XML           NULL,
    [SchemaType]       NVARCHAR (64) NULL,
    [CreateDate] DATETIME NULL DEFAULT GETDATE(), 
    [CreatorID] NVARCHAR(36) NULL, 
    [CreatorName] NVARCHAR(255) NULL, 
    CONSTRAINT [PK_SchemaObject] PRIMARY KEY CLUSTERED ([ID] ASC, [VersionStartTime] DESC)
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'对象ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaObject',
    @level2type = N'COLUMN',
    @level2name = N'ID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'版本开始时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaObject',
    @level2type = N'COLUMN',
    @level2name = N'VersionStartTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'版本结束时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaObject',
    @level2type = N'COLUMN',
    @level2name = N'VersionEndTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'对象的状态',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaObject',
    @level2type = N'COLUMN',
    @level2name = N'Status'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'对象的属性集合',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaObject',
    @level2type = N'COLUMN',
    @level2name = N'Data'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'对象的Schema名称',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaObject',
    @level2type = N'COLUMN',
    @level2name = N'SchemaType'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'对象的创建时间',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaObject',
    @level2type = N'COLUMN',
    @level2name = N'CreateDate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'创建者的ID',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaObject',
    @level2type = N'COLUMN',
    @level2name = N'CreatorID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'创建者的名字',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaObject',
    @level2type = N'COLUMN',
    @level2name = N'CreatorName'
GO


CREATE INDEX [IX_SchemaObject_SchemaType] ON [SC].[SchemaObject] ([SchemaType])

GO

CREATE PRIMARY XML INDEX [XML_IX_SchemaObject_Data] ON [SC].[SchemaObject] ([Data])

GO

CREATE XML INDEX [SecondaryXmlIndex-SchemaObject-Path] ON [SC].[SchemaObject]
(
	[Data]
)
USING XML INDEX [XML_IX_SchemaObject_Data] FOR PATH WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO

CREATE XML INDEX [SecondaryXmlIndex-SchemaObject-Property] ON [SC].[SchemaObject]
(
	[Data]
)
USING XML INDEX [XML_IX_SchemaObject_Data] FOR PROPERTY WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO

CREATE XML INDEX [SecondaryXmlIndex-SchemaObject-Value] ON [SC].[SchemaObject]
(
	[Data]
)
USING XML INDEX [XML_IX_SchemaObject_Data] FOR VALUE WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Schema对象表，保存基本对象',
    @level0type = N'SCHEMA',
    @level0name = N'SC',
    @level1type = N'TABLE',
    @level1name = N'SchemaObject',
    @level2type = NULL,
    @level2name = NULL