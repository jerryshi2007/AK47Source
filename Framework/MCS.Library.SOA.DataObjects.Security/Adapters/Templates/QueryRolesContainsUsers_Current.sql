SELECT SC.*, SR.ParentID, IsNull(SR.InnerSort, 0) AS InnerSort, SR.FullPath, SR.GlobalSort, IsNull(SR.IsDefault, 1) AS IsDefault
FROM SC.SchemaApplicationSnapshot_Current A 
	INNER JOIN SC.SchemaMembersSnapshot_Current MA ON A.ID = MA.ContainerID
	INNER JOIN SC.SchemaRoleSnapshot_Current R ON MA.MemberID = R.ID
	INNER JOIN SC.UserAndContainerSnapshot_Current UC ON UC.ContainerID = R.ID
	INNER JOIN SC.SchemaRelationObjectsSnapshot_Current SR ON UC.UserID = SR.ObjectID
	INNER JOIN SC.SchemaObjectSnapshot_Current SC ON SC.ID = SR.ObjectID
WHERE {0}
ORDER BY SR.GlobalSort
