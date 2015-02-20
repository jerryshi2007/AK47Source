-- =============================================
-- Author:		<沈峥>
-- Create date: <2008年3月17日>
-- Description:	<加锁并返回操作详情>
-- =============================================
CREATE PROCEDURE [WF].[SetLock]
	@lockID nvarchar(36), 
	@resourceID nvarchar(36),
	@lockPerson nvarchar(36),
	@effectiveTime int,
	@lockType int,
	@forceLock nchar(1) = 'n'
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @ORI_LOCK_ID NVARCHAR(36)
	DECLARE @ORI_RESOURCE_ID NVARCHAR(36)
	DECLARE @ORI_LOCK_PERSON_ID NVARCHAR(36)
	DECLARE @ORI_LOCK_TIME DATETIME
	DECLARE @ORI_EFFECTIVE_TIME INT
	DECLARE @ORI_LOCK_TYPE NVARCHAR(36)

	DECLARE @CurrentTime DATETIME

	SET @CurrentTime = getdate()

	DECLARE @resultTable TABLE(
				LOCK_ID NVARCHAR(36),
				RESOURCE_ID NVARCHAR(36),
				LOCK_PERSON_ID NVARCHAR(36),
				LOCK_TIME DATETIME,
				EFFECTIVE_TIME INT,
				LOCK_TYPE INT)
	BEGIN TRY
	
		INSERT INTO WF.LOCK(LOCK_ID, RESOURCE_ID, LOCK_PERSON_ID, LOCK_TIME, EFFECTIVE_TIME, LOCK_TYPE)
			VALUES(@lockID, @resourceID, @lockPerson, @CurrentTime, @effectiveTime, @lockType)

		INSERT INTO @resultTable
			SELECT @lockID, @resourceID, @lockPerson, @CurrentTime, @effectiveTime, @lockType
		INSERT INTO @resultTable	--插入空行
			SELECT @ORI_LOCK_ID, @ORI_RESOURCE_ID, @ORI_LOCK_PERSON_ID, @ORI_LOCK_TIME, @ORI_EFFECTIVE_TIME, @ORI_LOCK_TYPE

	END TRY
	BEGIN CATCH
		DECLARE 
			@ErrorMessage    NVARCHAR(4000),
			@ErrorNumber     INT,
			@ErrorSeverity   INT,
			@ErrorState      INT,
			@ErrorLine       INT,
			@ErrorProcedure  NVARCHAR(200);

			SELECT @ErrorMessage = 
				N'Error %d, Level %d, State %d, Procedure %s, Line %d, ' + 
					'Message: '+ ERROR_MESSAGE();

			SELECT 
				@ErrorNumber = ERROR_NUMBER(),
				@ErrorSeverity = ERROR_SEVERITY(),
				@ErrorState = ERROR_STATE(),
				@ErrorLine = ERROR_LINE(),
				@ErrorProcedure = ISNULL(ERROR_PROCEDURE(), '-');

			IF (@ErrorNumber = 2627)
			BEGIN
				--关键字冲突
				IF (@forceLock = 'y')
				BEGIN
					--强制加锁
					UPDATE WF.LOCK
						SET RESOURCE_ID = @resourceID, LOCK_PERSON_ID = @lockPerson, LOCK_TIME = @CurrentTime, EFFECTIVE_TIME = @effectiveTime, LOCK_TYPE = @lockType,
						@ORI_LOCK_ID = LOCK_ID, @ORI_RESOURCE_ID = RESOURCE_ID, @ORI_LOCK_PERSON_ID = LOCK_PERSON_ID, @ORI_LOCK_TIME = LOCK_TIME, @ORI_EFFECTIVE_TIME = EFFECTIVE_TIME, @ORI_LOCK_TYPE = LOCK_TYPE
					WHERE LOCK_ID = @lockID
					
				END
				ELSE
				BEGIN
					--如果级别高于现有的锁，或者锁是本人的，才会加上
					UPDATE WF.LOCK
						SET RESOURCE_ID = @resourceID, LOCK_PERSON_ID = @lockPerson, LOCK_TIME = @CurrentTime, EFFECTIVE_TIME = @effectiveTime, LOCK_TYPE = @lockType,
						@ORI_LOCK_ID = LOCK_ID, @ORI_RESOURCE_ID = RESOURCE_ID, @ORI_LOCK_PERSON_ID = LOCK_PERSON_ID, @ORI_LOCK_TIME = LOCK_TIME, @ORI_EFFECTIVE_TIME = EFFECTIVE_TIME, @ORI_LOCK_TYPE = LOCK_TYPE
					WHERE LOCK_ID = @lockID
						AND (@lockPerson = LOCK_PERSON_ID
							OR (@lockPerson <> LOCK_PERSON_ID AND DATEDIFF(s, LOCK_TIME, @CurrentTime) > EFFECTIVE_TIME))
				END
				IF (@@ROWCOUNT = 0)
				BEGIN
					INSERT INTO @resultTable
					SELECT @ORI_LOCK_ID, @ORI_RESOURCE_ID, @ORI_LOCK_PERSON_ID, @ORI_LOCK_TIME, @ORI_EFFECTIVE_TIME, @ORI_LOCK_TYPE

					INSERT INTO @resultTable
					SELECT LOCK_ID, RESOURCE_ID, LOCK_PERSON_ID, LOCK_TIME, EFFECTIVE_TIME, LOCK_TYPE
					FROM WF.LOCK
					WHERE LOCK_ID = @lockID
				END
				ELSE
				BEGIN
				
					INSERT INTO @resultTable
					SELECT @lockID, @resourceID, @lockPerson, @CurrentTime, @effectiveTime, @lockType

					INSERT INTO @resultTable
					SELECT @ORI_LOCK_ID, @ORI_RESOURCE_ID, @ORI_LOCK_PERSON_ID, @ORI_LOCK_TIME, @ORI_EFFECTIVE_TIME, @ORI_LOCK_TYPE
				END
			END
			ELSE
				RAISERROR 
				(
					@ErrorMessage, 
					@ErrorSeverity, 
					1,               
					@ErrorNumber,    -- parameter: original error number.
					@ErrorSeverity,  -- parameter: original error severity.
					@ErrorState,     -- parameter: original error state.
					@ErrorProcedure, -- parameter: original error procedure name.
					@ErrorLine       -- parameter: original error line number.
				);

	END CATCH

	SELECT * FROM @resultTable
END
