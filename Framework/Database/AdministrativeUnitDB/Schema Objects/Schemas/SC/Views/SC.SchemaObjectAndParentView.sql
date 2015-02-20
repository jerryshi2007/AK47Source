/*查询对象和父级关系的视图*/
CREATE VIEW [SC].[SchemaObjectAndParentView]
	AS 
SELECT O.*, R.ParentSchemaType, R.ChildSchemaType, R.ParentID, R.Status AS R_Status, R.VersionStartTime AS R_VersionStartTime, R.VersionEndTime AS R_VersionEndTime, R.InnerSort, R.FullPath, R.GlobalSort
FROM SC.SchemaObjectSnapshot O INNER JOIN SC.SchemaRelationObjectsSnapshot R
	ON O.ID = R.ObjectID
