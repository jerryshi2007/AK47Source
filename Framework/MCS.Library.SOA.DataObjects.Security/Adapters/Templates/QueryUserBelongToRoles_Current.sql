SELECT SC.*, A.ID AS AppID
FROM SC.SchemaApplicationSnapshot_Current A 
	INNER JOIN SC.SchemaMembersSnapshot_Current MA ON A.ID = MA.ContainerID
	INNER JOIN SC.SchemaObject_Current SC ON MA.MemberID = SC.ID
	INNER JOIN SC.UserAndContainerSnapshot_Current UC ON UC.ContainerID = SC.ID
WHERE {0}
ORDER BY MA.InnerSort
