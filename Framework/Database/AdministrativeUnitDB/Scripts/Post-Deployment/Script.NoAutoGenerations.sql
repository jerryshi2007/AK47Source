--需要手动部署的内容(不会包含在生成中)
--1、创建两级分类 2、将msuser映射到此数据库
--※请确保当前数据库为生成的数据库
--※警告：请调整后续语句以决定是否清空现有分类

EXEC SC.GenerateCategories

--设置MSUSER的登录权限


CREATE USER [msuser] FOR LOGIN [msuser]
GO

ALTER USER [msuser] WITH DEFAULT_SCHEMA=[SC]
GO

ALTER ROLE [db_datareader] ADD MEMBER [msuser]
GO

ALTER ROLE [db_datawriter] ADD MEMBER [msuser]
GO

ALTER ROLE [db_owner] ADD MEMBER [msuser]
GO
