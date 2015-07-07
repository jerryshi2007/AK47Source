CREATE TABLE [SC].[SchemaPropertyDefine]
(
	[SchemaName] NVARCHAR(64) NOT NULL , 
    [Name] NVARCHAR(64) NOT NULL, 
    [DisplayName] NVARCHAR(255) NULL, 
    [Description] NVARCHAR(255) NULL, 
    [DataType] NVARCHAR(64) NULL, 
    [SnapshotMode] INT NULL, 
    [Category] NVARCHAR(64) NULL, 
    [Tab] NVARCHAR(64) NULL,
	[SnapshotFieldName] NVARCHAR(64) NULL,
	[MaxLength] INT NULL,
	[IsRequired] INT NULL DEFAULT 0,
    [Visible] INT NULL DEFAULT 1, 
    [EditorKey] NVARCHAR(255) NULL, 
    [EditorParams] NVARCHAR(MAX) NULL, 
    [DefaultValue] NVARCHAR(MAX) NULL, 
    [ReadOnly] INT NULL DEFAULT 0, 
    [SortOrder] INT NULL DEFAULT 0, 
    [PersisterKey] NVARCHAR(64) NULL, 
    [ShowTitle] INT NULL DEFAULT 1, 
    [EditorParamsSettingsKey] NVARCHAR(255) NULL, 
    PRIMARY KEY ([SchemaName], [Name])
)

GO
