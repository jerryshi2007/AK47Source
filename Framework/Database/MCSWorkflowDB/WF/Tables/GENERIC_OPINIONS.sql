CREATE TABLE [WF].[GENERIC_OPINIONS] (
    [ID]                 NVARCHAR (36)  NOT NULL,
    [RESOURCE_ID]        NVARCHAR (36)  NULL,
    [CONTENT]            NVARCHAR (MAX) NULL,
    [ISSUE_PERSON_ID]    NVARCHAR (36)  NULL,
    [ISSUE_PERSON_NAME]  NVARCHAR (64)  NULL,
    [ISSUE_PERSON_LEVEL] NVARCHAR (36)  NULL,
    [APPEND_PERSON_ID]   NVARCHAR (36)  NULL,
    [APPEND_PERSON_NAME] NVARCHAR (64)  NULL,
    [ISSUE_DATETIME]     DATETIME       CONSTRAINT [DF_GENERIC_OPINIONS_ISSUE_DATETIME] DEFAULT (getdate()) NULL,
    [APPEND_DATETIME]    DATETIME       CONSTRAINT [DF_GENERIC_OPINIONS_APPEND_DATETIME] DEFAULT (getdate()) NULL,
    [PROCESS_ID]         NVARCHAR (36)  NULL,
    [ACTIVITY_ID]        NVARCHAR (36)  NULL,
    [LEVEL_NAME]         NVARCHAR (64)  NULL,
    [LEVEL_DESP]         NVARCHAR (64)  NULL,
    [OPINION_TYPE]       NVARCHAR (64)  NULL,
    [EVALUE]             INT            NULL,
    [RESULT]             INT            NULL,
    [EXT_DATA]           NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_GENERIC_OPINIONS] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IDX_GENERIC_OPINIONS_RESOURCE_ID]
    ON [WF].[GENERIC_OPINIONS]([RESOURCE_ID] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'业务意见表', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'GENERIC_OPINIONS';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'对应的文件ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'GENERIC_OPINIONS', @level2type = N'COLUMN', @level2name = N'RESOURCE_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'意见的内容', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'GENERIC_OPINIONS', @level2type = N'COLUMN', @level2name = N'CONTENT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'发布人ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'GENERIC_OPINIONS', @level2type = N'COLUMN', @level2name = N'ISSUE_PERSON_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'发布人名称', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'GENERIC_OPINIONS', @level2type = N'COLUMN', @level2name = N'ISSUE_PERSON_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'发布人在机构人员里的级别', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'GENERIC_OPINIONS', @level2type = N'COLUMN', @level2name = N'ISSUE_PERSON_LEVEL';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'填写人ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'GENERIC_OPINIONS', @level2type = N'COLUMN', @level2name = N'APPEND_PERSON_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'填写人名称', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'GENERIC_OPINIONS', @level2type = N'COLUMN', @level2name = N'APPEND_PERSON_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'创建意见的时间', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'GENERIC_OPINIONS', @level2type = N'COLUMN', @level2name = N'ISSUE_DATETIME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改意见的时间', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'GENERIC_OPINIONS', @level2type = N'COLUMN', @level2name = N'APPEND_DATETIME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'对应流程的ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'GENERIC_OPINIONS', @level2type = N'COLUMN', @level2name = N'PROCESS_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'对应流程环节的ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'GENERIC_OPINIONS', @level2type = N'COLUMN', @level2name = N'ACTIVITY_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'对应业务环节的名称', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'GENERIC_OPINIONS', @level2type = N'COLUMN', @level2name = N'LEVEL_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'对应业务环节的描述', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'GENERIC_OPINIONS', @level2type = N'COLUMN', @level2name = N'LEVEL_DESP';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'意见的类型', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'GENERIC_OPINIONS', @level2type = N'COLUMN', @level2name = N'OPINION_TYPE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'打分和评价', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'GENERIC_OPINIONS', @level2type = N'COLUMN', @level2name = N'EVALUE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'结果', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'GENERIC_OPINIONS', @level2type = N'COLUMN', @level2name = N'RESULT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'扩展数据', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'GENERIC_OPINIONS', @level2type = N'COLUMN', @level2name = N'EXT_DATA';


GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'唯一ID',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'GENERIC_OPINIONS',
    @level2type = N'COLUMN',
    @level2name = N'ID'