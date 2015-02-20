CREATE TABLE [WF].[PROCESS_DESCRIPTORS] (
    [PROCESS_KEY]      NVARCHAR (64) NOT NULL,
	[TENANT_CODE]      NVARCHAR (36) NOT NULL DEFAULT 'D5561180-7617-4B67-B68B-1F0EA604B509',
    [APPLICATION_NAME] NVARCHAR (64) NULL,
    [PROGRAM_NAME]     NVARCHAR (64) NULL,
    [PROCESS_NAME]     NVARCHAR (64) NULL,
    [DATA]             XML           NULL,
    [CREATE_TIME]      DATETIME      CONSTRAINT [DF_PROCESS_DESCRIPTORS_CREATE_TIME] DEFAULT (getdate()) NULL,
    [CREATOR_ID]       NVARCHAR (36) NULL,
    [CREATOR_NAME]     NVARCHAR (64) NULL,
    [MODIFY_TIME]      DATETIME      NULL,
    [MODIFIER_ID]      NVARCHAR (36) NULL,
    [MODIFIER_NAME]    NVARCHAR (64) NULL,
    [ENABLED]          NCHAR (1)     CONSTRAINT [DF_PROCESS_DESCRIPTORS_ENABLED] DEFAULT ((1)) NULL,
    [IMPORT_TIME]      DATETIME      NULL,
    [IMPORT_USER_ID]   NVARCHAR (36) NULL,
    [IMPORT_USER_NAME] NVARCHAR (64) NULL, 
    CONSTRAINT [PK_PROCESS_DESCRIPTORS] PRIMARY KEY CLUSTERED ([PROCESS_KEY] ASC, [TENANT_CODE] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'流程描述', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'PROCESS_DESCRIPTORS';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'流程描述KEY', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'PROCESS_DESCRIPTORS', @level2type = N'COLUMN', @level2name = N'PROCESS_KEY';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'应用名称', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'PROCESS_DESCRIPTORS', @level2type = N'COLUMN', @level2name = N'APPLICATION_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'模块名称', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'PROCESS_DESCRIPTORS', @level2type = N'COLUMN', @level2name = N'PROGRAM_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'流程描述名称', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'PROCESS_DESCRIPTORS', @level2type = N'COLUMN', @level2name = N'PROCESS_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'存放流程描述的附加信息', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'PROCESS_DESCRIPTORS', @level2type = N'COLUMN', @level2name = N'DATA';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'创建时间', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'PROCESS_DESCRIPTORS', @level2type = N'COLUMN', @level2name = N'CREATE_TIME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'创建人ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'PROCESS_DESCRIPTORS', @level2type = N'COLUMN', @level2name = N'CREATOR_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'创建人名称', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'PROCESS_DESCRIPTORS', @level2type = N'COLUMN', @level2name = N'CREATOR_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改时间', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'PROCESS_DESCRIPTORS', @level2type = N'COLUMN', @level2name = N'MODIFY_TIME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改人ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'PROCESS_DESCRIPTORS', @level2type = N'COLUMN', @level2name = N'MODIFIER_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改人名称', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'PROCESS_DESCRIPTORS', @level2type = N'COLUMN', @level2name = N'MODIFIER_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否启用', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'PROCESS_DESCRIPTORS', @level2type = N'COLUMN', @level2name = N'ENABLED';


GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'租户代码',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'PROCESS_DESCRIPTORS',
    @level2type = N'COLUMN',
    @level2name = N'TENANT_CODE'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'导入时间',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'PROCESS_DESCRIPTORS',
    @level2type = N'COLUMN',
    @level2name = N'IMPORT_TIME'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'导入用户ID',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'PROCESS_DESCRIPTORS',
    @level2type = N'COLUMN',
    @level2name = N'IMPORT_USER_ID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'导入用户名称',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'PROCESS_DESCRIPTORS',
    @level2type = N'COLUMN',
    @level2name = N'IMPORT_USER_NAME'
GO

CREATE INDEX [IX_PROCESS_DESCRIPTORS_TENANT_CODE] ON [WF].[PROCESS_DESCRIPTORS] ([TENANT_CODE])
