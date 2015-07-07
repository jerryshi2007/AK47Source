CREATE TABLE [SC].[SchemaObject] (
    [ID]               NVARCHAR (36) NOT NULL,
    [VersionStartTime] DATETIME      NOT NULL,
    [VersionEndTime]   DATETIME      CONSTRAINT [DF_SchemaObject_VersionEndTime] DEFAULT ('99990909 00:00:00') NULL,
    [Status]           INT           CONSTRAINT [DF_SchemaObject_Status] DEFAULT ((1)) NULL,
    [Data]             XML           NULL,
    [SchemaType]       NVARCHAR (64) NULL,
    [CreateDate] DATETIME NULL DEFAULT GETDATE(), 
    [CreatorID] NVARCHAR(36) NULL, 
    [CreatorName] NVARCHAR(255) NULL, 
    CONSTRAINT [PK_SchemaObject] PRIMARY KEY CLUSTERED ([ID] ASC, [VersionStartTime] DESC)
);


GO

CREATE INDEX [IX_SchemaObject_SchemaType] ON [SC].[SchemaObject] ([SchemaType])

GO