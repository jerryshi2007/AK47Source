CREATE TABLE [SC].[SchemaUserSnapshot] (
    [ID]               NVARCHAR (36)  NOT NULL,
    [VersionStartTime] DATETIME       NOT NULL,
    [VersionEndTime]   DATETIME       CONSTRAINT [DF_SchemaUser_VersionEndTime] DEFAULT ('99990909 00:00:00') NULL,
    [Status]           INT            CONSTRAINT [DF_SchemaUser_Status] DEFAULT ((1)) NULL,
	[CreateDate]	   DATETIME NULL DEFAULT GETDATE(), 
    [Name]             NVARCHAR (255) NULL,
    [DisplayName]      NVARCHAR (255) NULL,
	[CodeName]		   NVARCHAR (64) NULL,
    [FirstName]        NVARCHAR (64)  NULL,
    [LastName]         NVARCHAR (64)  NULL,
    [SearchContent]    NVARCHAR (MAX) NULL,
    [RowUniqueID]      NVARCHAR (36)  CONSTRAINT [DF_SchemaUser_RowUniqueID] DEFAULT (CONVERT([nvarchar](36),newid())) NOT NULL,
    [SchemaType] NVARCHAR(36) NULL, 
    [Mail] NVARCHAR(255) NULL, 
    [Sip] NVARCHAR(255) NULL, 
    [MP] NVARCHAR(36) NULL, 
    [WP] NVARCHAR(36) NULL, 
    [Address] NVARCHAR(MAX) NULL, 
	[CreatorID] NVARCHAR(36) NULL, 
    [CreatorName] NVARCHAR(255) NULL,
	[OwnerID] NVARCHAR(36) NULL, 
    [OwnerName] NVARCHAR(255) NULL,
    [AccountDisabled] INT NULL DEFAULT 0, 
    [PasswordNotRequired] INT NULL DEFAULT 0, 
    [DontExpirePassword] INT NULL DEFAULT 0, 
    [AccountExpires] DATETIME NULL, 
    [AccountInspires] DATETIME NULL, 
    [Comment] NVARCHAR(255) NULL, 
    CONSTRAINT [PK_SchemaUser] PRIMARY KEY CLUSTERED ([ID] ASC, [VersionStartTime] DESC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_SchemaUser_RowID]
    ON [SC].[SchemaUserSnapshot]([RowUniqueID] ASC);


GO


CREATE INDEX [IX_SchemaUserSnapshot_CodeName] ON [SC].[SchemaUserSnapshot] ([CodeName])

GO


CREATE INDEX [IX_SchemaUserSnapshot_OwnerID] ON [SC].[SchemaUserSnapshot] ([OwnerID])

GO
