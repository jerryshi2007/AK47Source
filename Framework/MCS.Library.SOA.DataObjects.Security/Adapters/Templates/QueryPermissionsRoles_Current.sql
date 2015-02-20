SELECT SC.*
FROM SC.SchemaApplicationSnapshot_Current A
	INNER JOIN SC.SchemaMembersSnapshot_Current M ON A.ID = M.ContainerID
	INNER JOIN SC.SchemaPermissionSnapshot_Current P ON M.MemberID = P.ID
	INNER JOIN SC.SchemaRelationObjectsSnapshot_Current R ON R.ObjectID = P.ID
	INNER JOIN SC.SchemaObject_Current SC ON R.ParentID = SC.ID
WHERE {0}
ORDER BY M.InnerSort