CREATE TABLE [SC].[OperationLog]
(
	[ID] INT NOT NULL IDENTITY, 
    [ResourceID] NVARCHAR(36) NULL, 
    [CorrelationID] NVARCHAR(36) NULL, 
    [Category] NVARCHAR(64) NULL, 
    [OperatorID] NVARCHAR(36) NULL, 
    [OperatorName] NVARCHAR(128) NULL, 
    [RealOperatorID] NVARCHAR(36) NULL, 
    [RealOperatorName] NVARCHAR(128) NULL, 
    [RequestContext] NVARCHAR(MAX) NULL, 
    [Subject] NVARCHAR(MAX) NULL, 
    [SchemaType] NVARCHAR(64) NULL, 
    [OperationType] NVARCHAR(64) NULL, 
    [SearchContent] NVARCHAR(MAX) NULL, 
    [CreateTime] DATETIME NULL DEFAULT getdate(), 
	CONSTRAINT [PK_OperationLog] PRIMARY KEY CLUSTERED ([ID] DESC)
)

GO


CREATE INDEX [IX_OperationLog_ResourceID] ON [SC].[OperationLog] ([ResourceID])

GO

