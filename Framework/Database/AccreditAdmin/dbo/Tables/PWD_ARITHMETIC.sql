CREATE TABLE [dbo].[PWD_ARITHMETIC] (
    [GUID]          NVARCHAR (36)  NOT NULL,
    [NAME]          NVARCHAR (64)  NOT NULL,
    [VERSION]       NVARCHAR (16)  NOT NULL,
    [VISIBLE]       INT            CONSTRAINT [DF_PWD_ARITHMETIC_VISIBLE] DEFAULT (1) NOT NULL,
    [ASSEMBLYCLASS] NVARCHAR (128) NULL,
    [SORT_ID]       INT            IDENTITY (3, 1) NOT NULL,
    CONSTRAINT [PK_PWD_ARITHMETIC] PRIMARY KEY CLUSTERED ([GUID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_PWD_ARITHMETIC_NAME]
    ON [dbo].[PWD_ARITHMETIC]([NAME] ASC);


GO
CREATE TRIGGER DELETE_PWD_ARITHMETIC
ON dbo.PWD_ARITHMETIC
FOR DELETE 
AS
BEGIN
	UPDATE USERS SET PWD_TYPE_GUID = NULL FROM DELETED WHERE USERS.PWD_TYPE_GUID = DELETED.GUID
END

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'加密算法的标志ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PWD_ARITHMETIC', @level2type = N'COLUMN', @level2name = N'GUID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'显示名称', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PWD_ARITHMETIC', @level2type = N'COLUMN', @level2name = N'NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'加密算法版本', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PWD_ARITHMETIC', @level2type = N'COLUMN', @level2name = N'VERSION';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'显示与否（0、不显示；1、显示）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PWD_ARITHMETIC', @level2type = N'COLUMN', @level2name = N'VISIBLE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'算法对应的CLASS全名称', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PWD_ARITHMETIC', @level2type = N'COLUMN', @level2name = N'ASSEMBLYCLASS';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'用于次序排列', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PWD_ARITHMETIC', @level2type = N'COLUMN', @level2name = N'SORT_ID';

