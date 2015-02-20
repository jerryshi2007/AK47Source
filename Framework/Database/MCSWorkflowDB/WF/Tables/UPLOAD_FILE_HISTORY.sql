CREATE TABLE [WF].[UPLOAD_FILE_HISTORY] (
    [ID]                 INT            IDENTITY (1, 1) NOT NULL,
    [APPLICATION_NAME]   NVARCHAR (64)  NOT NULL,
    [PROGRAM_NAME]       NVARCHAR (64)  NOT NULL,
    [OPERATOR_ID]        NVARCHAR (36)  NULL,
    [OPERATOR_NAME]      NVARCHAR (64)  NULL,
    [STATUS]             INT            CONSTRAINT [DF_UPLOAD_FILE_HISTORY_STATUS] DEFAULT ((0)) NULL,
    [ORIGINAL_FILE_NAME] NVARCHAR (255) NULL,
    [CURRENT_FILE_NAME]  NVARCHAR (255) NULL,
    [CREATE_TIME]        DATETIME       CONSTRAINT [DF_UPLOAD_FILE_HISTORY_CREATE_TIME] DEFAULT (getdate()) NULL,
    [STATUS_TEXT]        NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_UPLOAD_FILE_HISTORY] PRIMARY KEY CLUSTERED ([ID] DESC)
);


GO
CREATE NONCLUSTERED INDEX [IDX_UPLOAD_OPERATOR_ID]
    ON [WF].[UPLOAD_FILE_HISTORY]([OPERATOR_ID] ASC);


GO
CREATE NONCLUSTERED INDEX [IDX_UPLOAD_FILE_HISTORY_APP_PROG_NAME]
    ON [WF].[UPLOAD_FILE_HISTORY]([APPLICATION_NAME] ASC, [PROGRAM_NAME] ASC, [OPERATOR_ID] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'文件上传的历史', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'UPLOAD_FILE_HISTORY';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'操作人的ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'UPLOAD_FILE_HISTORY', @level2type = N'COLUMN', @level2name = N'OPERATOR_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'操作人的名称', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'UPLOAD_FILE_HISTORY', @level2type = N'COLUMN', @level2name = N'OPERATOR_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'操作结果的状态（0：失败，1：成功）', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'UPLOAD_FILE_HISTORY', @level2type = N'COLUMN', @level2name = N'STATUS';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'创建时间', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'UPLOAD_FILE_HISTORY', @level2type = N'COLUMN', @level2name = N'CREATE_TIME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'日志', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'UPLOAD_FILE_HISTORY', @level2type = N'COLUMN', @level2name = N'STATUS_TEXT';

