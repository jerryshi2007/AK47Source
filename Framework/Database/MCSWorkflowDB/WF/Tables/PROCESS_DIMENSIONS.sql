CREATE TABLE [WF].[PROCESS_DIMENSIONS]
(
	[PROCESS_ID] NVARCHAR(36) NOT NULL PRIMARY KEY CLUSTERED, 
    [DATA] XML NULL, 
    [UPDATE_TIME] DATETIME NULL DEFAULT GETDATE(), 
    [TENANT_CODE] NVARCHAR(36) NULL DEFAULT 'D5561180-7617-4B67-B68B-1F0EA604B509'
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'流程ID',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'PROCESS_DIMENSIONS',
    @level2type = N'COLUMN',
    @level2name = N'PROCESS_ID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'流程的描述信息',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'PROCESS_DIMENSIONS',
    @level2type = N'COLUMN',
    @level2name = N'DATA'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'最后更新时间',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'PROCESS_DIMENSIONS',
    @level2type = N'COLUMN',
    @level2name = N'UPDATE_TIME'
GO

/****** Object:  Index [PrimaryXmlIndex-Data]    Script Date: 2013/8/20 18:06:29 ******/
CREATE PRIMARY XML INDEX [PrimaryXmlIndex-Data] ON [WF].[PROCESS_DIMENSIONS]
(
	[DATA]
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO

/****** Object:  Index [SecondaryXmlIndex-Data-Path]    Script Date: 2013/8/20 18:07:09 ******/
CREATE XML INDEX [SecondaryXmlIndex-Data-Path] ON [WF].[PROCESS_DIMENSIONS]
(
	[DATA]
)
USING XML INDEX [PrimaryXmlIndex-Data] FOR PATH WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO

/****** Object:  Index [SecondaryXmlIndex-Data-Property]    Script Date: 2013/8/20 18:07:34 ******/
CREATE XML INDEX [SecondaryXmlIndex-Data-Property] ON [WF].[PROCESS_DIMENSIONS]
(
	[DATA]
)
USING XML INDEX [PrimaryXmlIndex-Data] FOR PROPERTY WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO

/****** Object:  Index [SecondaryXmlIndex-Data-Value]    Script Date: 2013/8/20 18:07:45 ******/
CREATE XML INDEX [SecondaryXmlIndex-Data-Value] ON [WF].[PROCESS_DIMENSIONS]
(
	[DATA]
)
USING XML INDEX [PrimaryXmlIndex-Data] FOR VALUE WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'流程实例提取出的维度表',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'PROCESS_DIMENSIONS',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'租户代码',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'PROCESS_DIMENSIONS',
    @level2type = N'COLUMN',
    @level2name = N'TENANT_CODE'
GO

CREATE INDEX [IX_PROCESS_DIMENSIONS_TENANT_CODE] ON [WF].[PROCESS_DIMENSIONS] ([TENANT_CODE])

GO

CREATE INDEX [IX_PROCESS_DIMENSIONS_UPDATE_TIME] ON [WF].[PROCESS_DIMENSIONS] ([UPDATE_TIME], [TENANT_CODE])
