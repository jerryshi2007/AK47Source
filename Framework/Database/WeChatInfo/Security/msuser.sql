CREATE USER [msuser]
	WITHOUT LOGIN
	WITH DEFAULT_SCHEMA = WeChat

GO

EXEC sp_addrolemember 'db_owner', 'msuser'
GO
