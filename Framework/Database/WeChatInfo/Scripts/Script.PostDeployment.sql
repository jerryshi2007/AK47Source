/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
DECLARE @xml XML

SET @xml ='<Root>
<Config.AccountInfo AccountID="gh_3b6b0b46f69c" UserID="YHQ_1314_5i5j" Password="ab123456" />
<Config.AccountInfo AccountID="gh_6209c155d0b9" UserID="405347456@qq.com" Password="zbm19851201" />
<Config.AccountInfo AccountID="gh_d9aa7fc59d74" UserID="mayong43111@hotmail.com" Password="hengjia19821017" />
</Root>'

DECLARE @idoc int

EXEC sp_xml_preparedocument @idoc OUTPUT, @xml

DECLARE @tempAccountInfo TABLE(AccountID nvarchar(36), UserID nvarchar(64), [Password] nvarchar(64))

INSERT INTO @tempAccountInfo(AccountID, UserID, [Password])
SELECT AccountID, UserID, [Password] FROM OPENXML(@idoc,'/Root/Config.AccountInfo', 0)
WITH (AccountID nvarchar(36), UserID nvarchar(64), [Password] nvarchar(64))

DELETE [Config].[AccountInfo] FROM [Config].[AccountInfo] A INNER JOIN @tempAccountInfo TA ON A.AccountID = TA.AccountID

INSERT INTO [Config].[AccountInfo] (AccountID, UserID, [Password])
SELECT AccountID, UserID, [Password]
FROM @tempAccountInfo

GO