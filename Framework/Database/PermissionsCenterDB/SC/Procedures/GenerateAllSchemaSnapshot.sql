/* 生成所有快照数据，此操作将清空所有权限中心快照表并重新生成其中的数据 */

CREATE PROCEDURE [SC].[GenerateAllSchemaSnapshot]
AS


	SET NOCOUNT ON;
	--Clear Tables Begin
	DELETE SC.SchemaApplicationSnapshot
	DELETE SC.SchemaGroupSnapshot
	DELETE SC.SchemaOrganizationSnapshot
	DELETE SC.SchemaUserSnapshot
	DELETE SC.SchemaMembersSnapshot
	DELETE SC.SchemaPermissionSnapshot
	DELETE SC.SchemaRelationObjectsSnapshot
	DELETE SC.SchemaRoleSnapshot

	DELETE SC.SchemaObjectSnapshot
	DELETE SC.UserAndContainerSnapshot

	--Clear Table End

	--Generate Snapshot Begin
	
		EXECUTE SC.GenerateSchemaSnapshot 'Applications'
		EXECUTE SC.GenerateSchemaSnapshot 'Groups'
			EXECUTE SC.GenerateUserAndContainerSnapshot 'Groups'
		EXECUTE SC.GenerateSchemaSnapshot 'MemberRelations'
		EXECUTE SC.GenerateSchemaSnapshot 'Organizations'
		EXECUTE SC.GenerateSchemaSnapshot 'Permissions'
		EXECUTE SC.GenerateSchemaSnapshot 'RelationObjects'
		EXECUTE SC.GenerateSchemaSnapshot 'Roles'
			EXECUTE SC.GenerateUserAndContainerSnapshot 'Roles'
		EXECUTE SC.GenerateSchemaSnapshot 'SecretaryRelations'
		EXECUTE SC.GenerateSchemaSnapshot 'Users'

	--Generate Snapshot End	
RETURN 0
