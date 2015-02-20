SELECT SC.*, SR.ParentID, IsNull(SR.InnerSort, 0) AS InnerSort, SR.FullPath, SR.GlobalSort, IsNull(SR.IsDefault, 1) AS IsDefault
FROM SC.SchemaApplicationSnapshot A 
	INNER JOIN SC.SchemaMembersSnapshot MA ON A.ID = MA.ContainerID
	INNER JOIN SC.SchemaRoleSnapshot R ON MA.MemberID = R.ID
	INNER JOIN SC.UserAndContainerSnapshot UC ON UC.ContainerID = R.ID
	INNER JOIN SC.SchemaRelationObjectsSnapshot SR ON UC.UserID = SR.ObjectID
	INNER JOIN SC.SchemaObjectSnapshot SC ON SC.ID = SR.ObjectID
WHERE {0}
ORDER BY SR.GlobalSort
