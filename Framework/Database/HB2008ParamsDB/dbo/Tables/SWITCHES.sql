CREATE TABLE [dbo].[SWITCHES] (
    [SWITCH_NAME]  NVARCHAR (64)  NOT NULL,
    [SWITCH_VALUE] NCHAR (1)      CONSTRAINT [DF_SWITCHES_SWITCH_VALUE] DEFAULT ((1)) NULL,
    [DESCRIPTION]  NVARCHAR (255) NULL,
    CONSTRAINT [PK_SWITCHES] PRIMARY KEY CLUSTERED ([SWITCH_NAME] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'开关表', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'SWITCHES';

