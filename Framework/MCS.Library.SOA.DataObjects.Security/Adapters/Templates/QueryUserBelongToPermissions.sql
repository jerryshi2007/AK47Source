SELECT SC.*, A.ID AS AppID
FROM SC.SchemaApplicationSnapshot A 
	INNER JOIN SC.SchemaMembersSnapshot MA ON A.ID = MA.ContainerID
	INNER JOIN SC.SchemaRoleSnapshot R ON MA.MemberID = R.ID
	INNER JOIN SC.SchemaRelationObjectsSnapshot SR ON SR.ParentID = R.ID
	INNER JOIN SC.SchemaObject SC ON SR.ObjectID = SC.ID
	INNER JOIN SC.UserAndContainerSnapshot UC ON UC.ContainerID = R.ID
WHERE {0}
