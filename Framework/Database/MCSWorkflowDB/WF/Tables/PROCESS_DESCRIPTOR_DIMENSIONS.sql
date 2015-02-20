CREATE TABLE [WF].[PROCESS_DESCRIPTOR_DIMENSIONS]
(
	[UNIQUE_ID] BIGINT NOT NULL IDENTITY ,
	[PROCESS_KEY] NVARCHAR(64) NOT NULL , 
    [TENANT_CODE] NVARCHAR(36) NOT NULL DEFAULT 'D5561180-7617-4B67-B68B-1F0EA604B509',
	[DATA] XML NULL, 
    [UPDATE_TIME] DATETIME NULL DEFAULT GETDATE(), 
    CONSTRAINT [PK_PROCESS_DESCRIPTOR_DIMENSIONS] PRIMARY KEY ([UNIQUE_ID]) 
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'流程描述KEY',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'PROCESS_DESCRIPTOR_DIMENSIONS',
    @level2type = N'COLUMN',
    @level2name = N'PROCESS_KEY'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'流程的描述信息',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'PROCESS_DESCRIPTOR_DIMENSIONS',
    @level2type = N'COLUMN',
    @level2name = N'DATA'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'最后的更新时间',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'PROCESS_DESCRIPTOR_DIMENSIONS',
    @level2type = N'COLUMN',
    @level2name = N'UPDATE_TIME'

GO

/****** Object:  Index [PDD_PrimaryXmlIndex-Data]    Script Date: 2013/9/21 14:53:53 ******/
CREATE PRIMARY XML INDEX [PDD_PrimaryXmlIndex-Data] ON [WF].[PROCESS_DESCRIPTOR_DIMENSIONS]
(
	[DATA]
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO

/****** Object:  Index [PDD-SecondaryXmlIndex-Path]    Script Date: 2013/9/21 14:54:30 ******/
CREATE XML INDEX [PDD-SecondaryXmlIndex-Path] ON [WF].[PROCESS_DESCRIPTOR_DIMENSIONS]
(
	[DATA]
)
USING XML INDEX [PDD_PrimaryXmlIndex-Data] FOR PATH WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO

/****** Object:  Index [PDD-SecondaryXmlIndex-Property]    Script Date: 2013/9/21 14:54:47 ******/
CREATE XML INDEX [PDD-SecondaryXmlIndex-Property] ON [WF].[PROCESS_DESCRIPTOR_DIMENSIONS]
(
	[DATA]
)
USING XML INDEX [PDD_PrimaryXmlIndex-Data] FOR PROPERTY WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO

/****** Object:  Index [PDD-SecondaryXmlIndex-Value]    Script Date: 2013/9/21 14:55:08 ******/
CREATE XML INDEX [PDD-SecondaryXmlIndex-Value] ON [WF].[PROCESS_DESCRIPTOR_DIMENSIONS]
(
	[DATA]
)
USING XML INDEX [PDD_PrimaryXmlIndex-Data] FOR VALUE WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO


EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'流程定义中提取出的维度表',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'PROCESS_DESCRIPTOR_DIMENSIONS',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'租户代码',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'PROCESS_DESCRIPTOR_DIMENSIONS',
    @level2type = N'COLUMN',
    @level2name = N'TENANT_CODE'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'唯一标识',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'PROCESS_DESCRIPTOR_DIMENSIONS',
    @level2type = N'COLUMN',
    @level2name = N'UNIQUE_ID'
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_PROCESS_DESCRIPTOR_DIMENSIONS_KEY_TENANT_CODE] ON [WF].[PROCESS_DESCRIPTOR_DIMENSIONS] ([TENANT_CODE], [PROCESS_KEY])

GO

CREATE INDEX [IX_PROCESS_DESCRIPTOR_DIMENSIONS_PROCESS_KEY] ON [WF].[PROCESS_DESCRIPTOR_DIMENSIONS] ([PROCESS_KEY])
