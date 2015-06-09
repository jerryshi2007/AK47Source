/* 执行生成全路径的作业 */
CREATE PROCEDURE [SC].[ExecuteJobList]
AS
BEGIN
	--执行JobList。目前只处理一种类型（GenerateFullPaths）的任务
	DECLARE @tempList TABLE(ID INT)
	DECLARE @todoCount INT

	BEGIN TRANSACTION
		INSERT INTO @tempList
			SELECT ID FROM SC.ToDoJobList WITH(UPDLOCK READPAST) WHERE Type = 'GenerateFullPaths'

		SET @todoCount = @@ROWCOUNT

		INSERT INTO SC.CompletedJobList(ID, SourceID, CreateTime, ExecuteTime, [Type], Description, Data)
			SELECT D.ID, D.SourceID, D.CreateTime, GETDATE(), D.Type, D.Description, D.Data
			FROM SC.ToDoJobList D INNER JOIN @tempList T ON D.ID = T.ID

		DELETE SC.ToDoJobList FROM @tempList T WHERE T.ID = SC.ToDoJobList.ID
	COMMIT

	PRINT @todoCount

	IF (@todoCount > 0)
	BEGIN
		DECLARE @sourceID NVARCHAR(36)

		DECLARE JobCursor CURSOR FOR 
		SELECT data.SourceID
		FROM SC.CompletedJobList data INNER JOIN @tempList todo ON data.ID = todo.ID

		OPEN JobCursor
  
		FETCH NEXT FROM JobCursor INTO @sourceID

		WHILE @@FETCH_STATUS = 0
		BEGIN
			EXEC SC.GenerateFullPaths @sourceID

			FETCH NEXT FROM JobCursor INTO @sourceID
		END

		CLOSE JobCursor;
		DEALLOCATE JobCursor;
	END
END