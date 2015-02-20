CREATE TABLE [WF].[PROCESS_INSTANCES] (
    [INSTANCE_ID]         NVARCHAR (36)   NOT NULL,
    [OWNER_ACTIVITY_ID]   NVARCHAR (36)   NULL,
    [OWNER_TEMPLATE_KEY]  NVARCHAR (64)   NULL,
    [CURRENT_ACTIVITY_ID] NVARCHAR (36)   NULL,
    [SEQUENCE]            INT             NULL,
    [STATUS]              NVARCHAR (64)   CONSTRAINT [DF_PROCESS_INSTANCES_STATUS] DEFAULT ((0)) NULL,
    [RESOURCE_ID]         NVARCHAR (36)   NULL,
    [DESCRIPTOR_KEY]      NVARCHAR (64)   NULL,
    [PROCESS_NAME]        NVARCHAR (255)   NULL,
    [APPLICATION_NAME]    NVARCHAR (64)   NULL,
    [PROGRAM_NAME]        NVARCHAR (64)   NULL,
	[TENANT_CODE]         NVARCHAR (36)   NULL DEFAULT 'D5561180-7617-4B67-B68B-1F0EA604B509',
    [DATA]                XML             NULL,
	[EXT_DATA]            XML             NULL,		
    [CREATE_TIME]         DATETIME        CONSTRAINT [DF_PROCESS_INSTANCES_CREATE_TIME] DEFAULT (getdate()) NULL,
    [UPDATE_TAG]          INT             CONSTRAINT [DF_PROCESS_INSTANCES_UPDATE_TAG] DEFAULT ((0)) NULL,
	[COMMITTED]			  NCHAR(1)		  CONSTRAINT [DF_PROCESS_INSTANCES_COMMITTED] DEFAULT (('1')) NULL,
    [START_TIME]          DATETIME        NULL,
    [END_TIME]            DATETIME        NULL,
    [CREATOR_ID]          NVARCHAR (36)   NULL,
    [CREATOR_NAME]        NVARCHAR (64)   NULL,
    [CREATOR_PATH]        NVARCHAR (414)  NULL,
    [DEPARTMENT_ID]       NVARCHAR (36)   NULL,
    [DEPARTMENT_NAME]     NVARCHAR (64)   NULL,
    [DEPARTMENT_PATH]     NVARCHAR (414)  NULL,
    [BIN_DATA]            VARBINARY (MAX) NULL,
    CONSTRAINT [PK_PROCESS_INSTANCES] PRIMARY KEY CLUSTERED ([INSTANCE_ID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_PROCESS_INSTANCES_ACTIVITY_ID]
    ON [WF].[PROCESS_INSTANCES]([CURRENT_ACTIVITY_ID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_PROCESS_INSTANCES_RESOURCE_ID]
    ON [WF].[PROCESS_INSTANCES]([RESOURCE_ID] ASC, [TENANT_CODE] ASC);


GO
CREATE NONCLUSTERED INDEX [IDX_PROCESS_INSTANCE_OWNER_ACTIVITY_ID]
    ON [WF].[PROCESS_INSTANCES]([OWNER_ACTIVITY_ID] ASC, [OWNER_TEMPLATE_KEY] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'流程实例类', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'PROCESS_INSTANCES';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'流程实例ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'PROCESS_INSTANCES', @level2type = N'COLUMN', @level2name = N'INSTANCE_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'拥有流程的活动点ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'PROCESS_INSTANCES', @level2type = N'COLUMN', @level2name = N'OWNER_ACTIVITY_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'拥有当前流程实例的模板KEY', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'PROCESS_INSTANCES', @level2type = N'COLUMN', @level2name = N'OWNER_TEMPLATE_KEY';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'流程当前活动点', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'PROCESS_INSTANCES', @level2type = N'COLUMN', @level2name = N'CURRENT_ACTIVITY_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'分支流程的启动序号', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'PROCESS_INSTANCES', @level2type = N'COLUMN', @level2name = N'SEQUENCE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'流程的状态，即运行中；已完成；被终止；未运行；', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'PROCESS_INSTANCES', @level2type = N'COLUMN', @level2name = N'STATUS';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'资源ID，即基于应用上划分的某件事情为资源的ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'PROCESS_INSTANCES', @level2type = N'COLUMN', @level2name = N'RESOURCE_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'流程描述Key', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'PROCESS_INSTANCES', @level2type = N'COLUMN', @level2name = N'DESCRIPTOR_KEY';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'流程名称', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'PROCESS_INSTANCES', @level2type = N'COLUMN', @level2name = N'PROCESS_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'应用名称', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'PROCESS_INSTANCES', @level2type = N'COLUMN', @level2name = N'APPLICATION_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'模块名称', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'PROCESS_INSTANCES', @level2type = N'COLUMN', @level2name = N'PROGRAM_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'存放为流程实例XMLSerialize的字符串', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'PROCESS_INSTANCES', @level2type = N'COLUMN', @level2name = N'DATA';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'流程实例的创建时间', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'PROCESS_INSTANCES', @level2type = N'COLUMN', @level2name = N'CREATE_TIME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'一个标识，每次对这条记录操作成功则加1.目的：防止多人在取到相同此标识时，分别在对这条记录执行写入操作。', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'PROCESS_INSTANCES', @level2type = N'COLUMN', @level2name = N'UPDATE_TAG';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'开始时间', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'PROCESS_INSTANCES', @level2type = N'COLUMN', @level2name = N'START_TIME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'结束时间', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'PROCESS_INSTANCES', @level2type = N'COLUMN', @level2name = N'END_TIME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'流程启动人', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'PROCESS_INSTANCES', @level2type = N'COLUMN', @level2name = N'CREATOR_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'流程启动人名称', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'PROCESS_INSTANCES', @level2type = N'COLUMN', @level2name = N'CREATOR_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'启动人的路径，相对于人员机构系统', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'PROCESS_INSTANCES', @level2type = N'COLUMN', @level2name = N'CREATOR_PATH';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'起草人ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'PROCESS_INSTANCES', @level2type = N'COLUMN', @level2name = N'DEPARTMENT_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'起草人名称', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'PROCESS_INSTANCES', @level2type = N'COLUMN', @level2name = N'DEPARTMENT_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'起草人的路径', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'PROCESS_INSTANCES', @level2type = N'COLUMN', @level2name = N'DEPARTMENT_PATH';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'DATA的压缩二进制版本', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'PROCESS_INSTANCES', @level2type = N'COLUMN', @level2name = N'BIN_DATA';


GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'扩展描述信息',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'PROCESS_INSTANCES',
    @level2type = N'COLUMN',
    @level2name = N'EXT_DATA'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'流程是否已经正式提交。如果为0，则表示启动了流程，但是没有保存和流转',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'PROCESS_INSTANCES',
    @level2type = N'COLUMN',
    @level2name = N'COMMITTED'
GO

CREATE INDEX [IX_PROCESS_INSTANCES_CREATE_TIME] ON [WF].[PROCESS_INSTANCES] ([CREATE_TIME], [TENANT_CODE])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'租户代码',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'PROCESS_INSTANCES',
    @level2type = N'COLUMN',
    @level2name = N'TENANT_CODE'
GO

CREATE INDEX [IX_PROCESS_INSTANCES_TENANT_CODE] ON [WF].[PROCESS_INSTANCES] ([TENANT_CODE])
