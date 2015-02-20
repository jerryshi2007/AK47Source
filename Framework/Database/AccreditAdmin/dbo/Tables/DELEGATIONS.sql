CREATE TABLE [dbo].[DELEGATIONS] (
    [SOURCE_ID]  NVARCHAR (36) NOT NULL,
    [TARGET_ID]  NVARCHAR (36) NOT NULL,
    [ROLE_ID]    NVARCHAR (36) NOT NULL,
    [START_TIME] DATETIME      CONSTRAINT [DF_DELEGATION_START_TIME] DEFAULT (getdate()) NOT NULL,
    [END_TIME]   DATETIME      NOT NULL,
    CONSTRAINT [PK_DELEGATIONS] PRIMARY KEY CLUSTERED ([SOURCE_ID] ASC, [TARGET_ID] ASC, [ROLE_ID] ASC, [START_TIME] ASC, [END_TIME] ASC)
);


GO
CREATE TRIGGER [TRIG_DELEGATIONS_DELETE] ON [dbo].[DELEGATIONS] 
FOR DELETE 
AS
BEGIN
	INSERT INTO DELEGATIONS_DELETE SELECT * FROM deleted
END

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'委派人GUID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'DELEGATIONS', @level2type = N'COLUMN', @level2name = N'SOURCE_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'被委派人GUID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'DELEGATIONS', @level2type = N'COLUMN', @level2name = N'TARGET_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'角色GUID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'DELEGATIONS', @level2type = N'COLUMN', @level2name = N'ROLE_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'委派有效的开始时间', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'DELEGATIONS', @level2type = N'COLUMN', @level2name = N'START_TIME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'委派有效的结束时间', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'DELEGATIONS', @level2type = N'COLUMN', @level2name = N'END_TIME';

