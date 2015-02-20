CREATE TABLE [dbo].[DELEGATIONS_DELETE] (
    [SOURCE_ID]  NVARCHAR (36) NOT NULL,
    [TARGET_ID]  NVARCHAR (36) NOT NULL,
    [ROLE_ID]    NVARCHAR (36) NOT NULL,
    [START_TIME] DATETIME      NOT NULL,
    [END_TIME]   DATETIME      NOT NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_DELEGATIONS_DELETE]
    ON [dbo].[DELEGATIONS_DELETE]([ROLE_ID] ASC, [SOURCE_ID] ASC, [TARGET_ID] ASC) WITH (FILLFACTOR = 50, PAD_INDEX = ON);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'委派人GUID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'DELEGATIONS_DELETE', @level2type = N'COLUMN', @level2name = N'SOURCE_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'被委派人GUID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'DELEGATIONS_DELETE', @level2type = N'COLUMN', @level2name = N'TARGET_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'角色GUID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'DELEGATIONS_DELETE', @level2type = N'COLUMN', @level2name = N'ROLE_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'委派有效的开始时间', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'DELEGATIONS_DELETE', @level2type = N'COLUMN', @level2name = N'START_TIME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'委派有效的结束时间', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'DELEGATIONS_DELETE', @level2type = N'COLUMN', @level2name = N'END_TIME';

