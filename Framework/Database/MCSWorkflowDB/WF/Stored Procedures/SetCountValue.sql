-- =============================================
-- Description:	<设置计数器的计数值>
-- =============================================
CREATE PROCEDURE [WF].[SetCountValue]
	@counterID nvarchar(255),
	@countValue int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	UPDATE WF.COUNTER SET COUNT_VALUE = @countValue WHERE COUNTER_ID = @counterID

	IF (@@ROWCOUNT = 0)
		INSERT INTO WF.COUNTER (COUNTER_ID, COUNT_VALUE) VALUES (@counterID, @countValue)
END
