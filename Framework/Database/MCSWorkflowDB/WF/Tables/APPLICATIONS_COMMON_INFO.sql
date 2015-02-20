CREATE TABLE [WF].[APPLICATIONS_COMMON_INFO] (
    [APPLICATION_NAME]      NVARCHAR (64)  NULL,
    [PROGRAM_NAME]          NVARCHAR (64)  NULL,
    [RESOURCE_ID]           NVARCHAR (36)  NOT NULL,
    [TOPOU_ID]              NVARCHAR (36)  NULL,
    [DEPT_ID]               NVARCHAR (36)  NULL,
    [CREATOR_ID]            NVARCHAR (36)  NULL,
    [SUBJECT]               NVARCHAR (255) NULL,
    [CONTENT]               NVARCHAR (MAX) NULL,
    [EMERGENCY]             NCHAR (1)      NULL,
    [SECRET]                NCHAR (1)      NULL,
    [SN_TITLE]              NVARCHAR (32)  NULL,
    [SN_YEAR]               NVARCHAR (4)   NULL,
    [SN_NO]                 NVARCHAR (32)  NULL,
    [ARCHIVE_STATUS]        NCHAR (1)      NULL,
    [COMPLETED_FLAG]        NCHAR (1)      NULL,
    [URL]                   NVARCHAR (512) NULL,
    [TOPOU_NAME]            NVARCHAR (64)  NULL,
    [DEPT_NAME]             NVARCHAR (64)  NULL,
    [CREATOR_NAME]          NVARCHAR (64)  NULL,
    [BARCODE]               NVARCHAR (32)  NULL,
    [REGISTER_NO]           INT            NULL,
    [CREATE_TIME]           DATETIME       CONSTRAINT [DF_APPLICATIONS_COMMON_INFO_CREATE_TIME] DEFAULT (getdate()) NULL,
    [DRAFT_DEPARTMENT_NAME] NVARCHAR (512) NULL,
    CONSTRAINT [PK_APPLICATIONS_COMMON_INFO] PRIMARY KEY CLUSTERED ([RESOURCE_ID] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'应用程序的数据', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'APPLICATIONS_COMMON_INFO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'应用程序的类别', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'APPLICATIONS_COMMON_INFO', @level2type = N'COLUMN', @level2name = N'APPLICATION_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'应用程序模块的名称', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'APPLICATIONS_COMMON_INFO', @level2type = N'COLUMN', @level2name = N'PROGRAM_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'应用数据的ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'APPLICATIONS_COMMON_INFO', @level2type = N'COLUMN', @level2name = N'RESOURCE_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'应用数据对应的顶级部门的ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'APPLICATIONS_COMMON_INFO', @level2type = N'COLUMN', @level2name = N'TOPOU_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'对应的部门的ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'APPLICATIONS_COMMON_INFO', @level2type = N'COLUMN', @level2name = N'DEPT_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'应用数据的创建者', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'APPLICATIONS_COMMON_INFO', @level2type = N'COLUMN', @level2name = N'CREATOR_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'应用数据的主题', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'APPLICATIONS_COMMON_INFO', @level2type = N'COLUMN', @level2name = N'SUBJECT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'可全文检索的应用数据', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'APPLICATIONS_COMMON_INFO', @level2type = N'COLUMN', @level2name = N'CONTENT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'应用数据的缓急程度', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'APPLICATIONS_COMMON_INFO', @level2type = N'COLUMN', @level2name = N'EMERGENCY';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'应用数据的密级', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'APPLICATIONS_COMMON_INFO', @level2type = N'COLUMN', @level2name = N'SECRET';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'应用文件的文件代字', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'APPLICATIONS_COMMON_INFO', @level2type = N'COLUMN', @level2name = N'SN_TITLE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'应用数据所对应的年号', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'APPLICATIONS_COMMON_INFO', @level2type = N'COLUMN', @level2name = N'SN_YEAR';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'应用的文件编号', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'APPLICATIONS_COMMON_INFO', @level2type = N'COLUMN', @level2name = N'SN_NO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否归档', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'APPLICATIONS_COMMON_INFO', @level2type = N'COLUMN', @level2name = N'ARCHIVE_STATUS';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否办结', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'APPLICATIONS_COMMON_INFO', @level2type = N'COLUMN', @level2name = N'COMPLETED_FLAG';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'应用界面所对应的URL', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'APPLICATIONS_COMMON_INFO', @level2type = N'COLUMN', @level2name = N'URL';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'应用数据所对应的顶级部门的名称', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'APPLICATIONS_COMMON_INFO', @level2type = N'COLUMN', @level2name = N'TOPOU_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'应用数据所对应的部门名称', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'APPLICATIONS_COMMON_INFO', @level2type = N'COLUMN', @level2name = N'DEPT_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'应用数据的创建者的名称', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'APPLICATIONS_COMMON_INFO', @level2type = N'COLUMN', @level2name = N'CREATOR_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'应用数据所对应的条码', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'APPLICATIONS_COMMON_INFO', @level2type = N'COLUMN', @level2name = N'BARCODE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'应用数据所对应的文件登记号', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'APPLICATIONS_COMMON_INFO', @level2type = N'COLUMN', @level2name = N'REGISTER_NO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'应用数据的创建时间', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'APPLICATIONS_COMMON_INFO', @level2type = N'COLUMN', @level2name = N'CREATE_TIME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'应用数据对应的起草部门名称', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'APPLICATIONS_COMMON_INFO', @level2type = N'COLUMN', @level2name = N'DRAFT_DEPARTMENT_NAME';

