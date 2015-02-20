CREATE TABLE [WF].[LOCK] (
    [LOCK_ID]        NVARCHAR (36) NOT NULL,
    [RESOURCE_ID]    NVARCHAR (36) NULL,
    [LOCK_PERSON_ID] NVARCHAR (36) NOT NULL,
    [LOCK_TIME]      DATETIME      NOT NULL,
    [EFFECTIVE_TIME] INT           NOT NULL,
    [LOCK_TYPE]      INT           NOT NULL,
    CONSTRAINT [PK_LOCK] PRIMARY KEY CLUSTERED ([LOCK_ID] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'设置表单并发锁', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'LOCK';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'锁ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'LOCK', @level2type = N'COLUMN', @level2name = N'LOCK_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'锁所对应的资源ID  即基本应用的产生的资源的ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'LOCK', @level2type = N'COLUMN', @level2name = N'RESOURCE_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'加锁人ID', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'LOCK', @level2type = N'COLUMN', @level2name = N'LOCK_PERSON_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'锁创建时间', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'LOCK', @level2type = N'COLUMN', @level2name = N'LOCK_TIME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'锁有效时间', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'LOCK', @level2type = N'COLUMN', @level2name = N'EFFECTIVE_TIME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'锁类型 即管理员锁；表单锁；流转锁', @level0type = N'SCHEMA', @level0name = N'WF', @level1type = N'TABLE', @level1name = N'LOCK', @level2type = N'COLUMN', @level2name = N'LOCK_TYPE';

