CREATE USER [msuser]
	WITHOUT LOGIN
	WITH DEFAULT_SCHEMA = [dbo]
GO

EXEC sp_addrolemember 'db_owner', 'msuser'
GO