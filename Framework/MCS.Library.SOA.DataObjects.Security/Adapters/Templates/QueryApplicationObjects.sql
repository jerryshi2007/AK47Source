SELECT SC.*
FROM SC.SchemaApplicationSnapshot A
	INNER JOIN SC.SchemaMembersSnapshot M ON A.ID = M.ContainerID
	INNER JOIN SC.SchemaObject SC ON M.MemberID = SC.ID
WHERE {0}
ORDER BY M.InnerSort
