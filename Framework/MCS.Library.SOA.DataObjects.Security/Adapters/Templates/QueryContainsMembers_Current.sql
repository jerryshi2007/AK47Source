SELECT SC.*, SR.ParentID, IsNull(SR.InnerSort, 0) AS InnerSort, SR.FullPath, SR.GlobalSort, IsNull(SR.IsDefault, 1) AS IsDefault
FROM SC.SchemaMembersSnapshot_Current UC
	INNER JOIN SC.SchemaObjectSnapshot_Current SCContainer ON UC.ContainerID = SCContainer.ID 
	INNER JOIN SC.SchemaRelationObjectsSnapshot_Current SRContainer ON UC.ContainerID = SRContainer.ObjectID 
	INNER JOIN SC.SchemaRelationObjectsSnapshot_Current SR ON UC.MemberID = SR.ObjectID INNER JOIN SC.SchemaObjectSnapshot_Current SC ON SC.ID = SR.ObjectID
WHERE {0}