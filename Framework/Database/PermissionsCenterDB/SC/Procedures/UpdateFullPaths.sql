/*根据查询的对象，更新全路径*/
CREATE PROCEDURE [SC].[UpdateFullPaths]
	@currentObjs [SC].[ObjectWithParentTable] READONLY,
	@time DATETIME
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @calculatedObjs [SC].[ObjectWithParentTable]

	INSERT INTO @calculatedObjs
	SELECT T.ID,
		T.SchemaType,
		T.Name,
		T.ParentID,
		T.InnerSort,
		CASE ISNULL(R.FullPath, '')
			WHEN '' THEN ISNULL(R.Name, T.Name)
			ELSE R.FullPath + '\' + T.Name
		END,
		CASE ISNULL(R.GlobalSort, '')
			WHEN '' THEN SC.FormatInteger(T.InnerSort, 6)
			ELSE R.GlobalSort + SC.FormatInteger(T.InnerSort, 6)
		END
	FROM @currentObjs T LEFT JOIN SC.SchemaObjectAndParentView R ON R.ID = T.ParentID
	WHERE (R.VersionStartTime <= @time AND R.VersionEndTime > @time AND R.R_VersionStartTime <= @time AND R.R_VersionEndTime > @time AND R.Status = 1 AND R.R_Status = 1) OR R.ID IS NULL

	--准备用来替换Data字段的临时表
	DECLARE @originalData TABLE(ObjectID NVARCHAR(36), ParentID NVARCHAR(36), OldFullPath NVARCHAR(414), FullPath NVARCHAR(414), OldGlobalSort NVARCHAR(414), GlobalSort NVARCHAR(414), Data NVARCHAR(MAX))

	INSERT INTO @originalData
		SELECT R.ObjectID, R.ParentID, R.FullPath, T.FullPath, R.GlobalSort, T.GlobalSort, CAST(R.Data AS NVARCHAR(MAX))
		FROM SC.SchemaRelationObjects R INNER JOIN @calculatedObjs T ON R.ObjectID = T.ID
		WHERE R.VersionStartTime <= @time AND R.VersionEndTime > @time AND (R.FullPath IS NULL OR R.FullPath <> T.FullPath OR R.GlobalSort IS NULL OR R.GlobalSort <> T.GlobalSort)

	UPDATE @originalData
	SET Data = REPLACE(Data, 'FullPath="' + OldFullPath + '"', 'FullPath="' + FullPath + '"')
	
	UPDATE @originalData
	SET Data = REPLACE(Data, 'GlobalSort="' + OldGlobalSort + '"', 'GlobalSort="' + GlobalSort + '"')

	UPDATE SC.SchemaRelationObjects
	SET FullPath = T.FullPath, GlobalSort = T.GlobalSort, Data = OT.Data
	FROM SC.SchemaRelationObjects R INNER JOIN @calculatedObjs T ON R.ObjectID = T.ID INNER JOIN @originalData OT ON OT.ObjectID = R.ObjectID AND OT.ParentID = R.ParentID AND R.ParentID = T.ParentID
	WHERE R.VersionStartTime <= @time AND R.VersionEndTime > @time AND (R.FullPath IS NULL OR R.FullPath <> T.FullPath OR R.GlobalSort IS NULL OR R.GlobalSort <> T.GlobalSort)

	UPDATE SC.SchemaRelationObjectsSnapshot
	SET FullPath = T.FullPath, GlobalSort = T.GlobalSort
	FROM SC.SchemaRelationObjectsSnapshot R INNER JOIN @calculatedObjs T ON R.ObjectID = T.ID AND R.ParentID = T.ParentID
	WHERE R.VersionStartTime <= @time AND R.VersionEndTime > @time AND (R.FullPath IS NULL OR R.FullPath <> T.FullPath OR R.GlobalSort IS NULL OR R.GlobalSort <> T.GlobalSort)

	SET NOCOUNT OFF;
END
