CREATE PROCEDURE [WF].[ClearProcessInstanceData]
AS
BEGIN
	--清除流程实例相关的数据，同时也包括锁、计数器，表单数据，待办、已办数据
	truncate table WF.ACL
	truncate table WF.APPLICATIONS_COMMON_INFO
	truncate table WF.[COUNTER]
	truncate table WF.COUNTRY_CODE
	truncate table WF.DELEGATIONS
	truncate table WF.GENERIC_FORM_DATA
	truncate table WF.GENERIC_FORM_RELATIVE_DATA
	truncate table WF.GENERIC_OPINIONS
	truncate table WF.GLOBAL_PARAMETERS

	truncate table WF.[IMAGE]
	truncate table WF.INITIALIZATION_USER_AD_IMAGE
	truncate table WF.INVALID_ASSIGNEES

	truncate table WF.LOCK
	truncate table WF.MATERIAL
	truncate table WF.MATERIAL_CONTENT

	truncate table WF.PENDING_ACTIVITIES
	truncate table WF.PENDING_ACTIVITIES_SERVICE

	truncate table WF.PERSIST_QUEUE
	truncate table WF.PERSIST_QUEUE_ARCHIEVED

	truncate table WF.PROCESS_CURRENT_ACTIVITIES
	truncate table WF.PROCESS_CURRENT_ASSIGNEES
	truncate table WF.PROCESS_DESCRIPTOR_CATEGORY

	truncate table WF.PROCESS_DIMENSIONS
	truncate table WF.PROCESS_INSTANCES
	truncate table WF.PROCESS_RELATIVE_PARAMS

	truncate table WF.RELATIVE_PROCESSES

	truncate table WF.UPLOAD_FILE_HISTORY
	truncate table WF.USER_ACCOMPLISHED_TASK
	truncate table WF.USER_OPERATION_LOG
	truncate table WF.USER_OPERATION_TASKS_LOG
	truncate table WF.USER_TASK
	truncate table WF.USER_TASK_CATEGORY
END