--将属性值的字符串转换为SQL类型

CREATE FUNCTION [SC].[ConvertPropertyValue]
(
	@propertyValue NVARCHAR(MAX),
	@dataType NVARCHAR(64)
)
RETURNS NVARCHAR(MAX)
AS
BEGIN
	DECLARE @result NVARCHAR(MAX)

	SET @result = @propertyValue

	IF LOWER(@dataType) = 'boolean'
	BEGIN
		IF LOWER( @propertyValue) = 'true'
			SET @result = 1
		ELSE
			SET @result = 0
	END

	RETURN @result
END
