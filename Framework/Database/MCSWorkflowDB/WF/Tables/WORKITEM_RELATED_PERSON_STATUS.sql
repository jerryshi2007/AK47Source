CREATE TABLE [WF].[WORKITEM_RELATED_PERSON_STATUS] (
    [PERSON_ID]      NVARCHAR (36)  NOT NULL,
    [PERSON_NAME]    NVARCHAR (64)  NOT NULL,
    [PERSON_TYPE]    INT            NOT NULL,
    [START_TIME]     DATETIME       NULL,
    [END_TIME]       DATETIME       NULL,
    [OCCUPANCY_TYPE] INT            NOT NULL,
    [SOURCE_KEY]     NVARCHAR (36)  NOT NULL,
    [SOURCE_NAME]    NVARCHAR (128) NULL,
    CONSTRAINT [PK_WORKITEM_RELATED_PERSON_STATUS] PRIMARY KEY CLUSTERED ([PERSON_ID] ASC, [SOURCE_KEY] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'计划安排处理人,负责人人员状态(占用)表', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'WORKITEM_RELATED_PERSON_STATUS';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'人员ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'WORKITEM_RELATED_PERSON_STATUS', @level2type = N'COLUMN', @level2name = N'PERSON_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'人员名称', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'WORKITEM_RELATED_PERSON_STATUS', @level2type = N'COLUMN', @level2name = N'PERSON_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'人员类型(0:处理人 1:负责人)', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'WORKITEM_RELATED_PERSON_STATUS', @level2type = N'COLUMN', @level2name = N'PERSON_TYPE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'占用开始时间', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'WORKITEM_RELATED_PERSON_STATUS', @level2type = N'COLUMN', @level2name = N'START_TIME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'占用结束时间', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'WORKITEM_RELATED_PERSON_STATUS', @level2type = N'COLUMN', @level2name = N'END_TIME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'占用类型(1:应用平台;2:HR..等等)', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'WORKITEM_RELATED_PERSON_STATUS', @level2type = N'COLUMN', @level2name = N'OCCUPANCY_TYPE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'更新被占用的类型的信息SOURCE.目的修改相应的信息.如计划的结束时间', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'WORKITEM_RELATED_PERSON_STATUS', @level2type = N'COLUMN', @level2name = N'SOURCE_KEY';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'更新状态引发的源名称，如计划安排名称等.', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'WORKITEM_RELATED_PERSON_STATUS', @level2type = N'COLUMN', @level2name = N'SOURCE_NAME';

