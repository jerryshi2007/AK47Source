DECLARE @time DATETIME

SET @time = {0};

WITH PARENT_OBJS(ID, ParentID, Name, DisplayName, SchemaType, CreateDate, CreatorID, CreatorName, VersionStartTime, VersionEndTime, [Status]) AS
(
	SELECT ID, ParentID, Name, DisplayName, SchemaType, CreateDate, CreatorID, CreatorName, VersionStartTime, VersionEndTime, [Status] FROM SC.[SchemaObjectAndParentView]
	WHERE [Status] = 1 AND [R_Status] = 1 AND VersionStartTime <= @time AND VersionEndTime > @time
		AND R_VersionStartTime <= @time AND R_VersionEndTime > @time AND {1}
	UNION ALL
	SELECT O.ID, O.ParentID, O.Name, O.DisplayName, O.SchemaType, O.CreateDate, O.CreatorID, O.CreatorName, O.VersionStartTime, O.VersionEndTime, O.[Status]
	FROM SC.[SchemaObjectAndParentView] O INNER JOIN PARENT_OBJS P ON O.ID = P.ParentID
	WHERE O.[Status] = 1 AND P.[Status] = 1 AND R_Status = 1 AND O.VersionStartTime <= @time AND O.VersionEndTime > @time
		AND R_VersionStartTime <= @time AND R_VersionEndTime > @time
)
SELECT * FROM PARENT_OBJS