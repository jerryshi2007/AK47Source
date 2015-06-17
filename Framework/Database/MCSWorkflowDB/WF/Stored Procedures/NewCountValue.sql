
-- =============================================
-- Author:		<郑博>
-- Create date: <2007.08.02>
-- Description:	<获取下一个可用计数值，并在计数器中记录>
-- =============================================
CREATE PROCEDURE [WF].[NewCountValue]
	@counterID nvarchar(255),
	@tenantCode NVARCHAR(36) = N'D5561180-7617-4B67-B68B-1F0EA604B509'
AS
BEGIN
	DECLARE @countValue int
	SET @countValue = 0
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	--在Update的同时记录计数值，可以减少检索数据库的次数
	UPDATE WF.COUNTER SET @countValue = COUNT_VALUE, COUNT_VALUE = COUNT_VALUE + 1 WHERE COUNTER_ID = @counterID AND TENANT_CODE = @tenantCode

	IF (@@ROWCOUNT = 0)
		INSERT INTO WF.COUNTER (COUNTER_ID, COUNT_VALUE, TENANT_CODE) VALUES (@counterID, 1, @tenantCode)

	SELECT @countValue + 1 AS COUNT_VALUE
END
