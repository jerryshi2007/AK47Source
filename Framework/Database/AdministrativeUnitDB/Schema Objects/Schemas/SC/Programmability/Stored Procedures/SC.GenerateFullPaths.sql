/*生成所有的全路径*/

CREATE PROCEDURE [SC].[GenerateFullPaths]
AS
BEGIN
	DECLARE @currentObjs [SC].[ObjectWithParentTable]
	DECLARE @time DATETIME

	SET @time = GETDATE()

	SET NOCOUNT ON
	INSERT INTO @currentObjs
		SELECT T.ID,
			T.SchemaType,
			T.Name,
			T.ParentID,
			T.InnerSort,
			'',
			''
		FROM SC.SchemaObjectAndParentView T LEFT JOIN SC.SchemaObjectAndParentView R ON R.ID = T.ParentID
		WHERE T.ParentSchemaType='AUSchemas' AND T.ChildSchemaType='AdminUnits' AND T.VersionStartTime <= @time AND T.VersionEndTime > @time AND T.R_VersionStartTime <= @time AND T.R_VersionEndTime > @time
			AND T.Status = 1 AND T.R_Status = 1
			AND (R.VersionStartTime <= @time AND R.VersionEndTime > @time AND R.R_VersionStartTime <= @time AND R.R_VersionEndTime > @time AND R.Status = 1 AND R.R_Status = 1 OR R.ID IS NULL)

	EXECUTE SC.UpdateFullPaths @currentObjs, @time

	DECLARE @children [SC].[ObjectWithParentTable]

	DECLARE @continue INT 
	SET @continue = 1

	WHILE (@continue = 1)
	BEGIN
		DELETE @children

		INSERT INTO @children
		SELECT O.ID,
				O.SchemaType,
				O.Name,
				O.ParentID,
				O.InnerSort,
				O.FullPath,
				O.GlobalSort
		FROM SC.SchemaObjectAndParentView O INNER JOIN @currentObjs T ON O.ParentID = T.ID
		WHERE O.VersionStartTime <= @time AND O.VersionEndTime > @time AND O.R_VersionStartTime <= @time AND O.R_VersionEndTime > @time AND O.Status = 1 AND O.R_Status = 1

		IF EXISTS(SELECT * FROM @children)
		BEGIN
			EXECUTE SC.UpdateFullPaths @children, @time
			DELETE @currentObjs

			INSERT INTO @currentObjs SELECT * FROM @children
		END
		ELSE
		BEGIN
			SET @continue = 0
		END
	END

	SET NOCOUNT OFF
END
