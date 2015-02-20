CREATE TABLE [WF].[PLAN_VERSION] (
    [ID]           NVARCHAR (36) NOT NULL,
    [OLD_PLAN_ID]  NVARCHAR (36) NOT NULL,
    [Created_Date] DATETIME      NULL,
    [VERSION]      INT           NULL,
    CONSTRAINT [PK_WF.PLAN_VERSION] PRIMARY KEY CLUSTERED ([ID] ASC, [OLD_PLAN_ID] ASC)
);


GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'基本不用了',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'PLAN_VERSION',
    @level2type = NULL,
    @level2name = NULL