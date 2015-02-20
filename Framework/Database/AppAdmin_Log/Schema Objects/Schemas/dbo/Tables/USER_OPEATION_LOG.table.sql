CREATE TABLE [dbo].[USER_OPEATION_LOG] (
    [ID]                   INT            IDENTITY (1, 1) NOT NULL,
    [OP_USER_GUID]         NVARCHAR (36)  NOT NULL,
    [OP_USER_DISPLAYNAME]  NVARCHAR (32)  NOT NULL,
    [OP_USER_DISTINCTNAME] NVARCHAR (255) NOT NULL,
    [OP_USER_LOGONNAME]    NVARCHAR (32)  NULL,
    [HOST_IP]              NVARCHAR (16)  NOT NULL,
    [HOST_NAME]            NVARCHAR (64)  NOT NULL,
    [OP_GUID]              NVARCHAR (36)  NULL,
    [GOAL_ID]              NVARCHAR (36)  NOT NULL,
    [GOAL_NAME]            NVARCHAR (36)  NOT NULL,
    [GOAL_DISNAME]         NVARCHAR (255) NULL,
    [LOG_DATE]             DATETIME       NOT NULL,
    [APP_GUID]             NVARCHAR (36)  NULL,
    [OP_URL]               NVARCHAR (255) NULL,
    [GOAL_EXPLAIN]         TEXT           NULL,
    [ORIGINAL_DATA]        TEXT           NULL,
    [LOG_SUCCED]           NVARCHAR (1)   NULL
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'标识（自增）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'USER_OPEATION_LOG', @level2type = N'COLUMN', @level2name = N'ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'操作人的标识', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'USER_OPEATION_LOG', @level2type = N'COLUMN', @level2name = N'OP_USER_GUID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'操作人的显示名称', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'USER_OPEATION_LOG', @level2type = N'COLUMN', @level2name = N'OP_USER_DISPLAYNAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'操作人的全地址', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'USER_OPEATION_LOG', @level2type = N'COLUMN', @level2name = N'OP_USER_DISTINCTNAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'用户登录名称', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'USER_OPEATION_LOG', @level2type = N'COLUMN', @level2name = N'OP_USER_LOGONNAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'客户端电脑IP', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'USER_OPEATION_LOG', @level2type = N'COLUMN', @level2name = N'HOST_IP';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'客户端电脑名称', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'USER_OPEATION_LOG', @level2type = N'COLUMN', @level2name = N'HOST_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'操作类型GUID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'USER_OPEATION_LOG', @level2type = N'COLUMN', @level2name = N'OP_GUID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'操作目标的标识', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'USER_OPEATION_LOG', @level2type = N'COLUMN', @level2name = N'GOAL_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'操作目标的名称（用于显示）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'USER_OPEATION_LOG', @level2type = N'COLUMN', @level2name = N'GOAL_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'操作目标的描述（特征性描述）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'USER_OPEATION_LOG', @level2type = N'COLUMN', @level2name = N'GOAL_DISNAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'操作执行的时间', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'USER_OPEATION_LOG', @level2type = N'COLUMN', @level2name = N'LOG_DATE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'应用GUID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'USER_OPEATION_LOG', @level2type = N'COLUMN', @level2name = N'APP_GUID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'操作执行所处的URL', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'USER_OPEATION_LOG', @level2type = N'COLUMN', @level2name = N'OP_URL';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'操作的具体含义', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'USER_OPEATION_LOG', @level2type = N'COLUMN', @level2name = N'GOAL_EXPLAIN';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'操作执行的原始数据', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'USER_OPEATION_LOG', @level2type = N'COLUMN', @level2name = N'ORIGINAL_DATA';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否登陆成功("y" 成功;"n" 失败)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'USER_OPEATION_LOG', @level2type = N'COLUMN', @level2name = N'LOG_SUCCED';

