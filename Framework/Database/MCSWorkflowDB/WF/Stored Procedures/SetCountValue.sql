-- =============================================
-- Description:	<设置计数器的计数值>
-- =============================================
CREATE PROCEDURE [WF].[SetCountValue]
	@counterID nvarchar(255),
	@countValue int,
	@tenantCode NVARCHAR(36) = N'D5561180-7617-4B67-B68B-1F0EA604B509'
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	UPDATE WF.COUNTER SET COUNT_VALUE = @countValue WHERE COUNTER_ID = @counterID AND TENANT_CODE = @tenantCode

	IF (@@ROWCOUNT = 0)
		INSERT INTO WF.COUNTER (COUNTER_ID, COUNT_VALUE, TENANT_CODE) VALUES (@counterID, @countValue, @tenantCode)
END
