SELECT SC.*
FROM SC.SchemaApplicationSnapshot_Current A
	INNER JOIN SC.SchemaMembersSnapshot_Current M ON A.ID = M.ContainerID
	INNER JOIN SC.SchemaObject_Current SC ON M.MemberID = SC.ID
WHERE {0}
ORDER BY M.InnerSort
