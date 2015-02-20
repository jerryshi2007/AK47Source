create proc [dbo].[Add_approles]
@roleName nvarchar(32),
@roleDescription nvarchar(64),
@roleEname nvarchar(32),
@appID nvarchar(36)
as 
begin
	insert ROLES (ID, APP_ID, NAME, CODE_NAME, DESCRIPTION, CLASSIFY, SORT_ID, INHERITED, ALLOW_DELEGATE, MODIFY_TIME)
	 values (
	NEWID(), 
	@appID,  
	@roleName, 
	@roleEname,
	@roleDescription,
	'n',
	'5',
	'n',
	'n',
	getdate())
end
