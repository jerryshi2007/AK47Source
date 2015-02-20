SELECT SC.ID, SC.Name, SC.DisplayName, SC.CodeName, SR.ParentID, SR.InnerSort, SR.GlobalSort, SR.IsDefault, SC.Status, SC.SchemaType,SR.FullPath
FROM SC.SchemaObjectSnapshot_Current SCContainer
	INNER JOIN SC.SchemaRelationObjectsSnapshot_Current SRContainer ON SCContainer.ID = SRContainer.ObjectID,
	SC.SchemaRelationObjectsSnapshot_Current SR
	INNER JOIN SC.SchemaObjectSnapshot_Current SC ON SC.ID = SR.ObjectID
WHERE {0}
ORDER BY SR.GlobalSort