CREATE TABLE [SC].[SchemaDefine]
(
	[Name] NVARCHAR(64) NOT NULL PRIMARY KEY,
	[TableName] NVARCHAR(255) NULL, 
    [SnapshotTable] NVARCHAR(255) NULL, 
    [IsUsersContainer] INT NULL DEFAULT 0, 
    [Category] NVARCHAR(64) NULL,
	[CodeNameKey] NVARCHAR(64) NULL,
	[CodeNameValidationMethod] INT NULL DEFAULT 0,
	[FullPathValidationMethod] INT NULL DEFAULT 0,
    [SortOrder] INT NULL DEFAULT 0, 
    [IsUsersContainerMember] INT NULL DEFAULT 0, 
    [ToSchemaObjectSnapshot] INT NULL DEFAULT 1, 
    [LogoImage] NVARCHAR(MAX) NULL, 
    [IsRelation] INT NULL DEFAULT 0
)

GO
