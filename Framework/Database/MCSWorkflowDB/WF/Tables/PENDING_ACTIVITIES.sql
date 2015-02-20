CREATE TABLE [WF].[PENDING_ACTIVITIES] (
    [ACTIVITY_ID]      NVARCHAR (36) NOT NULL,
    [PROCESS_ID]       NVARCHAR (36) NOT NULL,
    [APPLICATION_NAME] NVARCHAR (32) NULL,
    [PROGRAM_NAME]     NVARCHAR (32) NULL,
    [CREATE_TIME]      DATETIME      CONSTRAINT [DF_PENDING_ACTIVITIES_CREATE_TIME] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_PENDING_ACTIVITIES] PRIMARY KEY CLUSTERED ([ACTIVITY_ID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IDX_PENDING_ACTIVITIES_PROCESS_ID]
    ON [WF].[PENDING_ACTIVITIES]([PROCESS_ID] ASC);


GO
CREATE NONCLUSTERED INDEX [IDX_PENDING_ACTIVITIES_APPLICATION_NAME]
    ON [WF].[PENDING_ACTIVITIES]([APPLICATION_NAME], [PROGRAM_NAME]);


GO
CREATE NONCLUSTERED INDEX [IDX_PENDING_ACTIVITIES_PROGRAM_NAME]
    ON [WF].[PENDING_ACTIVITIES]([PROGRAM_NAME] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'由于条件没有满足，挂起的流程活动', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'PENDING_ACTIVITIES';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'挂起的活动点ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'PENDING_ACTIVITIES', @level2type = N'COLUMN', @level2name = N'ACTIVITY_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'挂起的流程ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'PENDING_ACTIVITIES', @level2type = N'COLUMN', @level2name = N'PROCESS_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'应用名称', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'PENDING_ACTIVITIES', @level2type = N'COLUMN', @level2name = N'APPLICATION_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'模块名称', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'PENDING_ACTIVITIES', @level2type = N'COLUMN', @level2name = N'PROGRAM_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'创建时间', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'PENDING_ACTIVITIES', @level2type = N'COLUMN', @level2name = N'CREATE_TIME';

