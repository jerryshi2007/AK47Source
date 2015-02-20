/*根据全路径查询对象*/

CREATE PROCEDURE [SC].[QueryObjectsByFullPath]
	@fullPath NVARCHAR(MAX)
AS
BEGIN
	SET NOCOUNT ON
	DECLARE @i INT
	SET @i = 1

	DECLARE @first INT
	SET @first = 1

	DECLARE @tempName NVARCHAR(MAX)
	SET @tempName = ''

	DECLARE @parent TABLE(ID NVARCHAR(36), Name NVARCHAR(255), DisplayName NVARCHAR(255), CodeName NVARCHAR(64), ParentID NVARCHAR(36), InnerSort INT, IsDefault INT, Status INT, SchemaType NVARCHAR(36))
	DECLARE @temp TABLE(ID NVARCHAR(36), Name NVARCHAR(255), DisplayName NVARCHAR(255), CodeName NVARCHAR(64), ParentID NVARCHAR(36), InnerSort INT, IsDefault INT, Status INT, SchemaType NVARCHAR(36))

	DECLARE @currentTime DATETIME
	SET @currentTime = GETDATE()

	WHILE @i <= LEN(@fullPath)
	BEGIN
		IF SUBSTRING(@fullPath, @i, 1) = '\'
		BEGIN
			IF @first = 1
			BEGIN
				SET @first = 0

				INSERT INTO @parent
				SELECT SC.ID, SC.Name, SC.DisplayName, SC.CodeName, SR.ParentID, SR.InnerSort, SR.IsDefault, SC.Status, SC.SchemaType
				FROM SC.SchemaObjectSnapshot SC INNER JOIN SC.SchemaRelationObjectsSnapshot SR ON SC.ID = SR.ObjectID
				WHERE SC.Name = @tempName
					AND SC.VersionStartTime <= @currentTime AND SC.VersionEndTime > @currentTime
					AND SR.VersionStartTime <= @currentTime AND SR.VersionEndTime > @currentTime
			END
			ELSE
			BEGIN
				DELETE @temp

				INSERT INTO @temp
				SELECT SC.ID, SC.Name, SC.DisplayName, SC.CodeName, SR.ParentID, SR.InnerSort, SR.IsDefault, SC.Status, SC.SchemaType
				FROM @parent P INNER JOIN SC.SchemaRelationObjectsSnapshot SR ON P.ID = SR.ParentID INNER JOIN SC.SchemaObjectSnapshot SC ON SC.ID = SR.ObjectID
				WHERE SC.Name = @tempName
					AND SC.VersionStartTime <= @currentTime AND SC.VersionEndTime > @currentTime
					AND SR.VersionStartTime <= @currentTime AND SR.VersionEndTime > @currentTime

				DELETE @parent

				INSERT @parent SELECT * FROM @temp
			END

			SET @tempName = ''
		END
		ELSE
			SET @tempName = @tempName + SUBSTRING(@fullPath, @i, 1)

		SET @i = @i + 1
	END

	IF @tempName  <> ''
	BEGIN
		IF @first = 1
		BEGIN
			SET @first = 0

			INSERT INTO @parent
			SELECT SC.ID, SC.Name, SC.DisplayName, SC.CodeName, SR.ParentID, SR.InnerSort, SR.IsDefault, SC.Status, SC.SchemaType
			FROM SC.SchemaObjectSnapshot SC INNER JOIN SC.SchemaRelationObjectsSnapshot SR ON SC.ID = SR.ObjectID
			WHERE SC.Name = @tempName
				AND SC.VersionStartTime <= @currentTime AND SC.VersionEndTime > @currentTime
				AND SR.VersionStartTime <= @currentTime AND SR.VersionEndTime > @currentTime
		END
		ELSE
		BEGIN
			DELETE @temp

			INSERT INTO @temp
			SELECT SC.ID, SC.Name, SC.DisplayName, SC.CodeName, SR.ParentID, SR.InnerSort, SR.IsDefault, SC.Status, SC.SchemaType
			FROM @parent P INNER JOIN SC.SchemaRelationObjectsSnapshot SR ON P.ID = SR.ParentID INNER JOIN SC.SchemaObjectSnapshot SC ON SC.ID = SR.ObjectID
			WHERE SC.Name = @tempName
				AND SC.VersionStartTime <= @currentTime AND SC.VersionEndTime > @currentTime
				AND SR.VersionStartTime <= @currentTime AND SR.VersionEndTime > @currentTime

			DELETE @parent

			INSERT @parent SELECT * FROM @temp
		END
	END

	SELECT * FROM @parent
	SET NOCOUNT OFF
END
