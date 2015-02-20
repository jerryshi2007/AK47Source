SELECT SC.ID, SC.Name, SC.DisplayName, SC.CodeName, SR.ParentID, SR.InnerSort, SR.GlobalSort, SR.IsDefault, SC.Status, SC.SchemaType,SR.FullPath
FROM SC.SchemaRelationObjectsSnapshot SR
	INNER JOIN SC.SchemaObjectSnapshot SC ON SC.ID = SR.ObjectID
WHERE {0}
ORDER BY SR.GlobalSort