CREATE TABLE [WF].[JOBS] (
    [JOB_ID]        NVARCHAR (64)  NOT NULL,
    [JOB_NAME]      NVARCHAR (64)  NULL,
	[JOB_CATEGORY]  NVARCHAR (64)  NULL,
    [DESCRIPTION]   NVARCHAR (MAX) NULL,
    [ENABLED]       NCHAR (1)      CONSTRAINT [DF_JOB_ENABLED] DEFAULT ((1)) NULL,
    [LAST_EXE_TIME] DATETIME       NULL,
    [JOB_TYPE]      NCHAR (1)      NULL,
    [CREATE_TIME]   DATETIME       CONSTRAINT [DF_JOB_CREATE_TIME] DEFAULT (getdate()) NULL,
    [CREATOR_ID]    NVARCHAR (36)  NULL,
    [CREATOR_NAME]  NVARCHAR (64)  NULL,
    [JOB_STATUS] INT NULL DEFAULT 0, 
    [LAST_START_EXE_TIME] DATETIME NULL DEFAULT GETDATE() , 
	[TENANT_CODE] NVARCHAR(36) NULL DEFAULT 'D5561180-7617-4B67-B68B-1F0EA604B509'
    CONSTRAINT [PK_JOB] PRIMARY KEY CLUSTERED ([JOB_ID] ASC)
);

GO

CREATE INDEX [IX_JOBS_TENANT_CODE] ON [WF].[JOBS] ([TENANT_CODE])

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'计划任务基本信息表', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'JOBS';


GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'运行时状态',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'JOBS',
    @level2type = N'COLUMN',
    @level2name = 'JOB_STATUS'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'最后一次开始执行任务时间',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'JOBS',
    @level2type = N'COLUMN',
    @level2name = N'LAST_START_EXE_TIME'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'作业的类别',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'JOBS',
    @level2type = N'COLUMN',
    @level2name = N'JOB_CATEGORY'
GO

CREATE INDEX [IX_JOBS_JOB_NAME] ON [WF].[JOBS] ([JOB_NAME])

GO

CREATE INDEX [IX_JOBS_CATEGORY] ON [WF].[JOBS] ([JOB_CATEGORY])

GO

CREATE INDEX [IX_JOBS_CREATE_TIME] ON [WF].[JOBS] ([CREATE_TIME])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'作业的ID',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'JOBS',
    @level2type = N'COLUMN',
    @level2name = N'JOB_ID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'作业的名称',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'JOBS',
    @level2type = N'COLUMN',
    @level2name = N'JOB_NAME'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'作业的描述',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'JOBS',
    @level2type = N'COLUMN',
    @level2name = N'DESCRIPTION'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'是否启用',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'JOBS',
    @level2type = N'COLUMN',
    @level2name = N'ENABLED'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'最后执行完成时间',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'JOBS',
    @level2type = N'COLUMN',
    @level2name = N'LAST_EXE_TIME'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'作业类型',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'JOBS',
    @level2type = N'COLUMN',
    @level2name = N'JOB_TYPE'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'作业创建的时间',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'JOBS',
    @level2type = N'COLUMN',
    @level2name = N'CREATE_TIME'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'创建者的ID',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'JOBS',
    @level2type = N'COLUMN',
    @level2name = N'CREATOR_ID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'创建者的名称',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'JOBS',
    @level2type = N'COLUMN',
    @level2name = N'CREATOR_NAME'