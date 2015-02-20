CREATE TABLE [dbo].[USERS_REPORT_LINE] (
    [USER_ID]        NVARCHAR (36) NOT NULL,
    [REPORT_TO_ID]   NVARCHAR (36) NOT NULL,
    [USER_NAME]      NVARCHAR (64) NULL,
    [REPORT_TO_NAME] NVARCHAR (64) NULL,
    CONSTRAINT [PK_USERS_REPORT_LINE_1] PRIMARY KEY CLUSTERED ([USER_ID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IDX_USERS_REPORT_LINE_REPORT_TO_ID]
    ON [dbo].[USERS_REPORT_LINE]([REPORT_TO_ID] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'用户的汇报关系（基本废弃）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'USERS_REPORT_LINE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'用户ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'USERS_REPORT_LINE', @level2type = N'COLUMN', @level2name = N'USER_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'汇报人ID', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'USERS_REPORT_LINE', @level2type = N'COLUMN', @level2name = N'REPORT_TO_ID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'用户名称', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'USERS_REPORT_LINE', @level2type = N'COLUMN', @level2name = N'USER_NAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'汇报人名称', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'USERS_REPORT_LINE', @level2type = N'COLUMN', @level2name = N'REPORT_TO_NAME';

