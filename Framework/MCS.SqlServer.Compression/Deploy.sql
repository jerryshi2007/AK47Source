USE MCS_WORKFLOW
/*
DROP FUNCTION dbo.AboutCompression
DROP FUNCTION dbo.CompressString
DROP FUNCTION dbo.CompressStringWithEncoding
DROP FUNCTION dbo.ExtractString
DROP FUNCTION dbo.ExtractStringWithEncoding
DROP ASSEMBLY MCSCompression
*/
--Precondition
SP_CONFIGURE 'clr enabled', 1
GO
RECONFIGURE
GO


CREATE ASSEMBLY MCSCompression FROM 'D:\MCS2010\02.Develop\Bin\MCS.SqlServer.Compression.dll'

GO

CREATE FUNCTION dbo.AboutCompression()
RETURNS NVARCHAR(256)
AS EXTERNAL NAME MCSCompression.[MCS.SqlServer.Compression.CompressManager].AboutCompression

GO

CREATE FUNCTION dbo.CompressString(@content NVARCHAR(MAX))
RETURNS VARBINARY(MAX)
AS EXTERNAL NAME MCSCompression.[MCS.SqlServer.Compression.CompressManager].CompressString

GO

CREATE FUNCTION dbo.CompressStringWithEncoding(@content NVARCHAR(MAX), @encodingName NVARCHAR(64))
RETURNS VARBINARY(MAX)
AS EXTERNAL NAME MCSCompression.[MCS.SqlServer.Compression.CompressManager].CompressStringWithEncoding

GO

CREATE FUNCTION dbo.ExtractString(@binary VARBINARY(MAX))
RETURNS NVARCHAR(MAX)
AS EXTERNAL NAME MCSCompression.[MCS.SqlServer.Compression.CompressManager].ExtractString

GO

CREATE FUNCTION dbo.ExtractStringWithEncoding(@binary VARBINARY(MAX), @encodingName NVARCHAR(64))
RETURNS NVARCHAR(MAX)
AS EXTERNAL NAME MCSCompression.[MCS.SqlServer.Compression.CompressManager].ExtractStringWithEncoding

GO
