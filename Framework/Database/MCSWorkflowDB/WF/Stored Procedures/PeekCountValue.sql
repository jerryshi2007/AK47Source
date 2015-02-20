-- =============================================
-- Description:	<获取下一个可用计数值>
-- =============================================
CREATE PROCEDURE [WF].[PeekCountValue]
	@counterID nvarchar(255)
AS
BEGIN
	DECLARE @countValue INT
	SET @countValue = 0
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT @countValue = COUNT_VALUE FROM WF.COUNTER WHERE COUNTER_ID = @counterID

	SELECT @countValue + 1 AS COUNT_VALUE
END
