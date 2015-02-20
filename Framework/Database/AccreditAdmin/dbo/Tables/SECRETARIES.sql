CREATE TABLE [dbo].[SECRETARIES] (
    [LEADER_GUID]    NVARCHAR (36)  NOT NULL,
    [SECRETARY_GUID] NVARCHAR (36)  NOT NULL,
    [START_TIME]     DATETIME       CONSTRAINT [DF_SECRETARYS_START_TIME] DEFAULT (0) NOT NULL,
    [END_TIME]       DATETIME       NULL,
    [DESCRIPTION]    NVARCHAR (255) NULL,
    [CREATE_TIME]    DATETIME       CONSTRAINT [DF_SECRETARYS_CREATE_TIME] DEFAULT (getdate()) NOT NULL,
    [MODIFY_TIME]    DATETIME       CONSTRAINT [DF_SECRETARYS_MODIFY_TIME] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_SECRETARY] PRIMARY KEY CLUSTERED ([LEADER_GUID] ASC, [SECRETARY_GUID] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'领导标识', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SECRETARIES', @level2type = N'COLUMN', @level2name = N'LEADER_GUID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'秘书标识', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SECRETARIES', @level2type = N'COLUMN', @level2name = N'SECRETARY_GUID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'开始时间', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SECRETARIES', @level2type = N'COLUMN', @level2name = N'START_TIME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'结束时间', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SECRETARIES', @level2type = N'COLUMN', @level2name = N'END_TIME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'关系描述', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SECRETARIES', @level2type = N'COLUMN', @level2name = N'DESCRIPTION';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'关系建立时间', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SECRETARIES', @level2type = N'COLUMN', @level2name = N'CREATE_TIME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'关系修改时间', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SECRETARIES', @level2type = N'COLUMN', @level2name = N'MODIFY_TIME';

