SELECT SC.*
FROM SC.SchemaApplicationSnapshot A
	INNER JOIN SC.SchemaMembersSnapshot M ON A.ID = M.ContainerID
	INNER JOIN SC.SchemaPermissionSnapshot P ON M.MemberID = P.ID
	INNER JOIN SC.SchemaRelationObjectsSnapshot R ON R.ObjectID = P.ID
	INNER JOIN SC.SchemaObject SC ON R.ParentID = SC.ID
WHERE {0}
ORDER BY M.InnerSort