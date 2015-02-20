CREATE TABLE [WF].[MATERIAL] (
    [ID]                       NVARCHAR (36)   NOT NULL,
    [DEPARTMENT_ID]            NVARCHAR (36)   NULL,
    [DEPARTMENT_NAME]          NVARCHAR (255)  NULL,
    [DEPARTMENT_GLOBALSORT_ID] NVARCHAR (255)  NULL,
    [RESOURCE_ID]              NVARCHAR (36)   NULL,
    [SORT_ID]                  INT             NULL,
    [CLASS]                    NVARCHAR (255)  NULL,
    [TITLE]                    NVARCHAR (128)  NOT NULL,
    [PAGE_QUANTITY]            INT             NULL,
    [RELATIVE_FILE_PATH]       NVARCHAR (255)  NOT NULL,
    [ORIGINAL_NAME]            NVARCHAR (255)  NOT NULL,
    [CREATOR_ID]               NVARCHAR (36)   NOT NULL,
    [CREATOR_USER_NAME]        NVARCHAR (64)   NOT NULL,
    [LAST_UPLOAD_TAG]          NVARCHAR (36)   NOT NULL,
    [CREATE_DATETIME]          DATETIME        CONSTRAINT [DF_MATERIAL_CREATE_DATETIME] DEFAULT (getdate()) NOT NULL,
    [MODIFY_TIME]              DATETIME        NULL,
    [WF_PROCESS_ID]            NVARCHAR (36)   NULL,
    [WF_ACTIVITY_ID]           NVARCHAR (36)   NULL,
    [WF_ACTIVITY_NAME]         NVARCHAR (36)   NULL,
    [PARENT_ID]                NVARCHAR (36)   NULL,
    [VERSION_TYPE]             INT             CONSTRAINT [DF_MATERIAL_IS_COPY_VERSION] DEFAULT ((0)) NOT NULL,
    [EXTRA_DATA]               NVARCHAR (1024) NULL,
    CONSTRAINT [PK_MATERIAL] PRIMARY KEY NONCLUSTERED ([ID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_MATERIAL_RESOURCE_ID]
    ON [WF].[MATERIAL]([RESOURCE_ID] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'附件的ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'MATERIAL', @level2type = N'COLUMN', @level2name = N'ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'部门ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'MATERIAL', @level2type = N'COLUMN', @level2name = N'DEPARTMENT_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'部门名称', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'MATERIAL', @level2type = N'COLUMN', @level2name = N'DEPARTMENT_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'部门全局排序ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'MATERIAL', @level2type = N'COLUMN', @level2name = N'DEPARTMENT_GLOBALSORT_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'表单ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'MATERIAL', @level2type = N'COLUMN', @level2name = N'RESOURCE_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'排序号', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'MATERIAL', @level2type = N'COLUMN', @level2name = N'SORT_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'类别', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'MATERIAL', @level2type = N'COLUMN', @level2name = N'CLASS';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'文件标题', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'MATERIAL', @level2type = N'COLUMN', @level2name = N'TITLE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'页数', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'MATERIAL', @level2type = N'COLUMN', @level2name = N'PAGE_QUANTITY';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'相对路径', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'MATERIAL', @level2type = N'COLUMN', @level2name = N'RELATIVE_FILE_PATH';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'文件原始名称', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'MATERIAL', @level2type = N'COLUMN', @level2name = N'ORIGINAL_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'创建者ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'MATERIAL', @level2type = N'COLUMN', @level2name = N'CREATOR_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'创建者名称', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'MATERIAL', @level2type = N'COLUMN', @level2name = N'CREATOR_USER_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'最后上传标记', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'MATERIAL', @level2type = N'COLUMN', @level2name = N'LAST_UPLOAD_TAG';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'创建时间', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'MATERIAL', @level2type = N'COLUMN', @level2name = N'CREATE_DATETIME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改时间', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'MATERIAL', @level2type = N'COLUMN', @level2name = N'MODIFY_TIME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'对应流程ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'MATERIAL', @level2type = N'COLUMN', @level2name = N'WF_PROCESS_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'对应流程活动ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'MATERIAL', @level2type = N'COLUMN', @level2name = N'WF_ACTIVITY_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'对应流程活动名称', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'MATERIAL', @level2type = N'COLUMN', @level2name = N'WF_ACTIVITY_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'父版本ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'MATERIAL', @level2type = N'COLUMN', @level2name = N'PARENT_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'版本类型', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'MATERIAL', @level2type = N'COLUMN', @level2name = N'VERSION_TYPE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'扩展数据', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'MATERIAL', @level2type = N'COLUMN', @level2name = N'EXTRA_DATA';

