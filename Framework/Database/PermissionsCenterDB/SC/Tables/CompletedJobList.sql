﻿CREATE TABLE [SC].[CompletedJobList]
(
	[ID] INT NOT NULL , 
    [SourceID] NVARCHAR(36) NULL, 
    [CreateTime] DATETIME NULL DEFAULT GETDATE(), 
    [ExecuteTime] DATETIME NULL DEFAULT GETDATE(), 
    [Type] NVARCHAR(64) NULL, 
	[Description] NVARCHAR(255) NULL,
    [Data] NVARCHAR(MAX) NULL, 
    PRIMARY KEY ([ID] DESC)
)

GO
