-- 清除所有数据，不会清除拼音表数据

CREATE PROCEDURE [SC].[ClearAllData]
AS
BEGIN
	SET NOCOUNT ON;

    TRUNCATE TABLE SC.OperationLog
	DELETE SC.SchemaObject
	TRUNCATE TABLE SC.SchemaMembers
	
	TRUNCATE TABLE SC.SchemaRelationObjects

	TRUNCATE TABLE SC.SchemaDefine
	TRUNCATE TABLE SC.SchemaPropertyDefine
	TRUNCATE TABLE SC.UserPassword
	TRUNCATE TABLE SC.ToDoJobList
	TRUNCATE TABLE SC.CompletedJobList
	
	TRUNCATE TABLE SC.PermissionCenter_AD_IDMapping
	TRUNCATE TABLE SC.ADSynchronizeLog
	TRUNCATE TABLE SC.ADSynchronizeLogDetail
	TRUNCATE TABLE SC.ADReverseSynchronizeLog
	TRUNCATE TABLE SC.ADReverseSynchronizeLogDetail
	TRUNCATE TABLE SC.[Locks]
	TRUNCATE TABLE SC.ConditionCalculateResult
	TRUNCATE TABLE SC.ADReverseSynchronizeLog
	TRUNCATE TABLE SC.ADReverseSynchronizeLogDetail
	TRUNCATE TABLE SC.ADSynchronizeLog
	TRUNCATE TABLE SC.ADSynchronizeLogDetail

	TRUNCATE TABLE SC.SCOperationSnapshot

	DELETE SC.SchemaOrganizationSnapshot
	DELETE SC.SchemaRelationObjectsSnapshot
	DELETE SC.SchemaUserSnapshot
	DELETE SC.SchemaApplicationSnapshot
	DELETE SC.SchemaGroupSnapshot
	DELETE SC.SchemaMembersSnapshot
	DELETE SC.SchemaPermissionSnapshot
	DELETE SC.SchemaRoleSnapshot
	DELETE SC.SchemaObjectSnapshot
	DELETE SC.UserAndContainerSnapshot
	DELETE SC.Conditions
	DELETE SC.Acl
END
