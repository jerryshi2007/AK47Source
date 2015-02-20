SELECT SC.*, A.ID AS AppID
FROM SC.SchemaApplicationSnapshot A 
	INNER JOIN SC.SchemaMembersSnapshot MA ON A.ID = MA.ContainerID
	INNER JOIN SC.SchemaObject SC ON MA.MemberID = SC.ID
	INNER JOIN SC.UserAndContainerSnapshot UC ON UC.ContainerID = SC.ID
WHERE {0}
ORDER BY MA.InnerSort
