-- =============================================
-- Author:		<沈峥>
-- Create date: <2008-8-19>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [WF].[QueryUserTaskCount]
	@userID NVARCHAR(36)
AS
BEGIN
	SET NOCOUNT ON;

    SELECT 'TOTAL' AS EXPIRED, [STATUS], COUNT(*) AS [COUNT]
	FROM WF.USER_TASK (NOLOCK)
	WHERE SEND_TO_USER = @userID
	GROUP BY [STATUS]
	UNION
	SELECT 'EXPIRED' AS EXPIRED, [STATUS], COUNT(*) AS [COUNT]
	FROM WF.USER_TASK (NOLOCK)
	WHERE SEND_TO_USER = @userID AND EXPIRE_TIME < getdate()
	GROUP BY [STATUS]

END
