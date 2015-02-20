CREATE TABLE [WF].[PENDING_ACTIVITIES_SERVICE] (
    [ACTIVITY_ID]       NVARCHAR (36) NOT NULL,
    [LAST_SERVICE_TIME] DATETIME      NULL,
    CONSTRAINT [PK_PENDING_ACTIVITIES_SERVICE] PRIMARY KEY CLUSTERED ([ACTIVITY_ID] ASC)
);


GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'不太明确用途',
    @level0type = N'SCHEMA',
    @level0name = N'WF',
    @level1type = N'TABLE',
    @level1name = N'PENDING_ACTIVITIES_SERVICE',
    @level2type = NULL,
    @level2name = NULL