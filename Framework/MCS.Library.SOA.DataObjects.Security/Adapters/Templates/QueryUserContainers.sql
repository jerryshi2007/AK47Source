SELECT SCContainer.*, SRContainer.ParentID, IsNull(SRContainer.InnerSort, 0) AS InnerSort, SRContainer.FullPath, SRContainer.GlobalSort, IsNull(SRContainer.IsDefault, 1) AS IsDefault
FROM SC.UserAndContainerSnapshot UC INNER JOIN SC.SchemaObjectSnapshot SCContainer ON UC.ContainerID = SCContainer.ID 
	INNER JOIN SC.SchemaRelationObjectsSnapshot SRContainer ON UC.ContainerID = SRContainer.ObjectID 
	INNER JOIN SC.SchemaRelationObjectsSnapshot SR ON UC.UserID = SR.ObjectID INNER JOIN SC.SchemaObjectSnapshot SC ON SC.ID = SR.ObjectID
WHERE {0}