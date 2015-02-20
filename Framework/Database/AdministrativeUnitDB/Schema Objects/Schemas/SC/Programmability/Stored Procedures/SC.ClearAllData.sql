--清除所有数据，不会清除拼音表数据和分类数据

CREATE PROCEDURE [SC].[ClearAllData]
AS
BEGIN
	SET NOCOUNT ON;

    TRUNCATE TABLE SC.OperationLog
	TRUNCATE TABLE SC.SchemaObject
	TRUNCATE TABLE SC.SchemaMembers
	
	TRUNCATE TABLE SC.SchemaRelationObjects

	TRUNCATE TABLE SC.SchemaDefine
	TRUNCATE TABLE SC.SchemaPropertyDefine
	TRUNCATE TABLE [SC].[OperationLog]
	TRUNCATE TABLE SC.ToDoJobList
	TRUNCATE TABLE SC.CompletedJobList
	
	TRUNCATE TABLE SC.[Locks]

	DELETE SC.SchemaRelationObjectsSnapshot
	DELETE SC.SchemaMembersSnapshot
	DELETE SC.SchemaPermissionSnapshot
	DELETE [SC].[SchemaObjectSnapshot]
	DELETE [SC].[AUSchemaRoleSnapshot]
	DELETE [SC].[AUAdminScopeItemSnapshot]
	DELETE [SC].[AUSchemaSnapshot]
	DELETE [SC].[AdminUnitSnapshot]
	DELETE [SC].[UserAndContainerSnapshot]
	DELETE [SC].[ItemAndContainerSnapshot]
	
	DELETE [SC].[AURoleSnapshot]
	TRUNCATE TABLE SC.SCOperationSnapshot

	DELETE [SC].[Conditions]
	DELETE [SC].[Acl]
END
