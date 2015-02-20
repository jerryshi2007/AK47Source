/* 生成所有快照数据，此操作将清空所有管理单元快照表并重新生成其中的数据 */

CREATE PROCEDURE [SC].[GenerateAllSchemaSnapshot]
AS
	DELETE SC.AdminUnitSnapshot
	DELETE SC.AUAdminScopeItemSnapshot
	DELETE SC.AUAdminScopeSnapshot
	DELETE SC.AURoleSnapshot
	DELETE SC.AUSchemaRoleSnapshot
	DELETE SC.AUSchemaSnapshot
	DELETE SC.SchemaMembersSnapshot
	DELETE SC.SchemaRelationObjectsSnapshot
	
	DELETE SC.SchemaObjectSnapshot

	DELETE SC.UserAndContainerSnapshot

	DELETE SC.ItemAndContainerSnapshot

	--Clear Table End

	--Generate Snapshot Begin

	EXECUTE SC.GenerateSchemaSnapshot 'AdminUnits'
	EXECUTE SC.GenerateSchemaSnapshot 'AUAdminScopes'
	EXECUTE SC.GenerateSchemaSnapshot 'AUSchemaRoles'
	EXECUTE SC.GenerateSchemaSnapshot 'AUSchemas'
	EXECUTE SC.GenerateSchemaSnapshot 'AURoles'
	EXECUTE SC.GenerateSchemaSnapshot 'MemberRelations'
	EXECUTE SC.GenerateSchemaSnapshot 'RelationObjects'

	EXECUTE SC.GenerateItemAndContainerSnapshot
	--Generate Snapshot End	
RETURN 0
