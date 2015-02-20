CREATE PROCEDURE [WF].[ClearAllData]
AS
BEGIN
	truncate table WF.ACL
	truncate table WF.ACTIVITY_TEMPLATE
	truncate table WF.APP_PROGRAM_AUTH
	truncate table WF.APPLICATIONS
	truncate table WF.APPLICATIONS_ALIAS
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

	truncate table WF.JOB_INVOKE_SERVICE
	truncate table WF.JOB_SCHEDULE_DEF
	truncate table WF.JOB_SCHEDULES
	truncate table WF.JOB_START_WORKFLOW
	truncate table WF.JOBS

	truncate table WF.LOCK
	truncate table WF.MATERIAL
	truncate table WF.MATERIAL_CONTENT

	truncate table WF.MATRIX_CELLS
	truncate table WF.MATRIX_DEFINITION
	truncate table WF.MATRIX_DIMENSION_DEFINITION
	truncate table WF.MATRIX_MAIN
	truncate table WF.MATRIX_ROWS

	truncate table WF.PENDING_ACTIVITIES
	truncate table WF.PENDING_ACTIVITIES_SERVICE

	truncate table WF.PERSIST_QUEUE
	truncate table WF.PERSIST_QUEUE_ARCHIEVED

	truncate table WF.[PLAN]
	truncate table WF.PLAN_VERSION

	truncate table WF.PROCESS_CURRENT_ACTIVITIES
	truncate table WF.PROCESS_CURRENT_ASSIGNEES
	truncate table WF.PROCESS_DESCRIPTOR_CATEGORY
	truncate table WF.PROCESS_DESCRIPTOR_DIMENSIONS

	truncate table WF.PROCESS_DESCRIPTORS
	truncate table WF.PROCESS_DIMENSIONS
	truncate table WF.PROCESS_INSTANCES
	truncate table WF.PROCESS_RELATIVE_PARAMS
	truncate table WF.PROGRAMS

	truncate table WF.RELATIVE_PROCESSES
	
	truncate table WF.SYS_ACCOMPLISHED_TASK
	truncate table WF.SYS_TASK

	truncate table WF.TASK_ASSIGNED_OBJECTS
	truncate table WF.TASK_ASSIGNEES

	truncate table WF.UPLOAD_FILE_HISTORY
	truncate table WF.USER_ACCOMPLISHED_TASK
	truncate table WF.USER_OPERATION_LOG
	truncate table WF.USER_OPERATION_TASKS_LOG
	truncate table WF.USER_TASK
	truncate table WF.USER_TASK_CATEGORY

	truncate table WF.WORKFLOW_TASK
	truncate table WF.WORKFLOW_TASK_EXTERNALDATA
	truncate table WF.WORKFLOW_TASK_RELATED_PERSON
	truncate table WF.WORKFLOW_TASK_RELATED_TASK
	truncate table WF.WORKFLOW_TASK_TEMPLATE
	truncate table WF.WORKFLOW_TASK_TEMPLATE_DETAIL
	truncate table WF.WORKFLOW_TASK_VERSION

	truncate table WF.WORKITEM
	truncate table WF.WORKITEM_RELATED_PERSON
	truncate table WF.WORKITEM_RELATED_PERSON_STATUS

	truncate table MSG.EMAIL_ADDRESSES
	truncate table MSG.EMAIL_ATTACHMENTS
	truncate table MSG.EMAIL_MESSAGES
	truncate table MSG.SENT_EMAIL_MESSAGES

	truncate table KB.RELATIVE_LINK
	truncate table KB.RELATIVE_LINK_GROUP
	truncate table KB.TIP
END