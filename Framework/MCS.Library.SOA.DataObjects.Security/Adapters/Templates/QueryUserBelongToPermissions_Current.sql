SELECT SC.*, A.ID AS AppID
FROM SC.SchemaApplicationSnapshot A 
	INNER JOIN SC.SchemaMembersSnapshot_Current MA ON A.ID = MA.ContainerID
	INNER JOIN SC.SchemaRoleSnapshot_Current R ON MA.MemberID = R.ID
	INNER JOIN SC.SchemaRelationObjectsSnapshot_Current SR ON SR.ParentID = R.ID
	INNER JOIN SC.SchemaObject_Current SC ON SR.ObjectID = SC.ID
	INNER JOIN SC.UserAndContainerSnapshot_Current UC ON UC.ContainerID = R.ID
WHERE {0}
