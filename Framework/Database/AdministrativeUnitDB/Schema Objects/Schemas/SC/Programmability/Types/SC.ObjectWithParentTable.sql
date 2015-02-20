/*表类型，表示对象和父对象的关系*/

CREATE TYPE [SC].[ObjectWithParentTable] AS TABLE
(
	ID NVARCHAR(36) NOT NULL,
	SchemaType NVARCHAR(64) NULL,
	Name NVARCHAR(64) NULL,
	ParentID NVARCHAR(36) NOT NULL,
	InnerSort INT NULL,
	FullPath NVARCHAR(414) NULL,
	GlobalSort NVARCHAR(414) NULL --,
	--PRIMARY KEY ([ID] ASC, [ParentID] ASC)
)
