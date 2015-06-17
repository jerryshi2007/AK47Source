﻿CREATE TABLE [WF].[JOB_INVOKE_SERVICE] (
    [JOB_ID]           NVARCHAR (64) NOT NULL,
    [SERVICE_DEF_DATA] XML           NOT NULL,
	[TENANT_CODE] NVARCHAR(36) NULL DEFAULT 'D5561180-7617-4B67-B68B-1F0EA604B509'
    CONSTRAINT [PK_JOB_INVOKE_SERVICE] PRIMARY KEY CLUSTERED ([JOB_ID] ASC)
);

GO

CREATE INDEX [IX_JOB_INVOKE_SERVICE_TENANT_CODE] ON [WF].[JOB_INVOKE_SERVICE] ([TENANT_CODE])

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'计划任务-调用web服务表', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'JOB_INVOKE_SERVICE';


GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Web服务的定义数据',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'JOB_INVOKE_SERVICE',
    @level2type = N'COLUMN',
    @level2name = N'SERVICE_DEF_DATA'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'作业的ID',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'JOB_INVOKE_SERVICE',
    @level2type = N'COLUMN',
    @level2name = N'JOB_ID'