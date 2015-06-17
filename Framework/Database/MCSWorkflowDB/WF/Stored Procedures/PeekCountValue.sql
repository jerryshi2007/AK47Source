-- =============================================
-- Description:	<获取下一个可用计数值>
-- =============================================
CREATE PROCEDURE [WF].[PeekCountValue]
	@counterID nvarchar(255),
	@tenantCode NVARCHAR(36) = N'D5561180-7617-4B67-B68B-1F0EA604B509'
AS
BEGIN
	DECLARE @countValue INT
	SET @countValue = 0
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT @countValue = COUNT_VALUE FROM WF.COUNTER WHERE COUNTER_ID = @counterID AND TENANT_CODE = @tenantCode

	SELECT @countValue + 1 AS COUNT_VALUE
END
