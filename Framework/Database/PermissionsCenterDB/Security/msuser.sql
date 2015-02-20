/*创建用户msuser，这是可选的*/
CREATE USER [msuser]
	WITHOUT LOGIN
	WITH DEFAULT_SCHEMA = [SC]
GO

EXEC sp_addrolemember 'db_owner', 'msuser'
GO
