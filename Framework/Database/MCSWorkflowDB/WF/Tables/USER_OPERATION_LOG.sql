CREATE TABLE [WF].[USER_OPERATION_LOG] (
    [ID]                  BIGINT             IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [RESOURCE_ID]         NVARCHAR (64)   NULL,
    [TENANT_CODE]         NVARCHAR (36)   NULL DEFAULT 'D5561180-7617-4B67-B68B-1F0EA604B509',
	[SUBJECT]             NVARCHAR (255)  NULL,
    [APPLICATION_NAME]    NVARCHAR (64)   NULL,
    [PROGRAM_NAME]        NVARCHAR (64)   NULL,
    [PROCESS_ID]          NVARCHAR (36)   NULL,
    [ACTIVITY_ID]         NVARCHAR (36)   NULL,
    [ACTIVITY_NAME]       NVARCHAR (64)   NULL,
    [OPERATOR_ID]         NVARCHAR (36)   NULL,
    [OPERATOR_NAME]       NVARCHAR (64)   NULL,
    [TOP_DEPT_ID]         NVARCHAR (36)   NULL,
    [TOP_DEPT_NAME]       NVARCHAR (64)   NULL,
    [REAL_USER_ID]        NVARCHAR (36)   NULL,
    [REAL_USER_NAME]      NVARCHAR (64)   NULL,
    [OPERATE_DATETIME]    DATETIME        NOT NULL,
    [OPERATE_NAME]        NVARCHAR (64)   NULL,
    [OPERATE_TYPE]        NCHAR (1)       NULL,
    [OPERATE_DESCRIPTION] NVARCHAR (1024) NULL,
    [HTTP_CONTEXT]        NVARCHAR (MAX)  NULL,
    [CORRELATION_ID]      NVARCHAR (36)   NULL,
    CONSTRAINT [PK_OPERATION_LOG] PRIMARY KEY CLUSTERED ([ID] DESC)
);


GO
CREATE NONCLUSTERED INDEX [IDX_USER_OPERATION_LOG_RESOURCE_ID]
    ON [WF].[USER_OPERATION_LOG]([RESOURCE_ID], [TENANT_CODE]);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'用户 操作日志', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'USER_OPERATION_LOG';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'文件ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'USER_OPERATION_LOG', @level2type = N'COLUMN', @level2name = N'RESOURCE_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'文件名称', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'USER_OPERATION_LOG', @level2type = N'COLUMN', @level2name = N'SUBJECT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'应用名称', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'USER_OPERATION_LOG', @level2type = N'COLUMN', @level2name = N'APPLICATION_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'程序名称', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'USER_OPERATION_LOG', @level2type = N'COLUMN', @level2name = N'PROGRAM_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'流程ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'USER_OPERATION_LOG', @level2type = N'COLUMN', @level2name = N'PROCESS_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'流程节点ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'USER_OPERATION_LOG', @level2type = N'COLUMN', @level2name = N'ACTIVITY_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'流程节点名称', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'USER_OPERATION_LOG', @level2type = N'COLUMN', @level2name = N'ACTIVITY_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'操作者ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'USER_OPERATION_LOG', @level2type = N'COLUMN', @level2name = N'OPERATOR_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'操作者名称', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'USER_OPERATION_LOG', @level2type = N'COLUMN', @level2name = N'OPERATOR_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'顶级部门ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'USER_OPERATION_LOG', @level2type = N'COLUMN', @level2name = N'TOP_DEPT_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'顶级部门名称', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'USER_OPERATION_LOG', @level2type = N'COLUMN', @level2name = N'TOP_DEPT_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'真实用户ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'USER_OPERATION_LOG', @level2type = N'COLUMN', @level2name = N'REAL_USER_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'真实用户名称', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'USER_OPERATION_LOG', @level2type = N'COLUMN', @level2name = N'REAL_USER_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'操作时间', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'USER_OPERATION_LOG', @level2type = N'COLUMN', @level2name = N'OPERATE_DATETIME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'操作名称', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'USER_OPERATION_LOG', @level2type = N'COLUMN', @level2name = N'OPERATE_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'操作类型。如添加、修改、删除文件及修改意见与异常处理修改', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'USER_OPERATION_LOG', @level2type = N'COLUMN', @level2name = N'OPERATE_TYPE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'操作描述', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'USER_OPERATION_LOG', @level2type = N'COLUMN', @level2name = N'OPERATE_DESCRIPTION';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'客户端类型', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'USER_OPERATION_LOG', @level2type = N'COLUMN', @level2name = N'HTTP_CONTEXT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'相关ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'USER_OPERATION_LOG', @level2type = N'COLUMN', @level2name = N'CORRELATION_ID';


GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'租户代码',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'USER_OPERATION_LOG',
    @level2type = N'COLUMN',
    @level2name = N'TENANT_CODE'
GO

CREATE INDEX [IX_USER_OPERATION_LOG_TENANT_CODE] ON [WF].[USER_OPERATION_LOG] ([TENANT_CODE])
