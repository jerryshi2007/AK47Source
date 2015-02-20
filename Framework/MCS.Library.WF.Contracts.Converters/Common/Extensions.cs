using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.WF.Contracts.Converters.Runtime;
using MCS.Library.WF.Contracts.Ogu;
using MCS.Library.WF.Contracts.PropertyDefine;
using MCS.Library.WF.Contracts.Workflow.DataObjects;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using MCS.Library.WF.Contracts.Workflow.Runtime;

namespace MCS.Library.WF.Contracts.Converters
{
	public static class Extensions
	{
		public static PropertyDataType ToPropertyDataType(this ClientPropertyDataType cpdt)
		{
			return (PropertyDataType)cpdt;
		}

		public static ClientPropertyDataType ToClientPropertyDataType(this PropertyDataType pdt)
		{
			return (ClientPropertyDataType)pdt;
		}

		public static DataType ToVariableDataType(this WfClientVariableDataType cdt)
		{
			return (DataType)cdt;
		}

		public static WfClientVariableDataType ToClientVariableDataType(this DataType dt)
		{
			return (WfClientVariableDataType)dt;
		}

		public static WfActivityType ToActivityType(this WfClientActivityType cat)
		{
			return (WfActivityType)cat;
		}

		public static WfClientActivityType ToClientActivityType(this WfActivityType at)
		{
			return (WfClientActivityType)at;
		}

		public static WfProcessStatus ToProcessStatus(this WfClientProcessStatus status)
		{
			return (WfProcessStatus)status;
		}

		public static WfClientProcessStatus ToClientProcessStatus(this WfProcessStatus status)
		{
			return (WfClientProcessStatus)status;
		}

		public static WfActivityStatus ToActivityStatus(this WfClientActivityStatus status)
		{
			return (WfActivityStatus)status;
		}

		public static WfClientActivityStatus ToClientActivityStatus(this WfActivityStatus status)
		{
			return (WfClientActivityStatus)status;
		}

		public static DataLoadingType ToDataLoadingType(this WfClientDataLoadingType loadingType)
		{
			return (DataLoadingType)loadingType;
		}

		public static WfClientDataLoadingType ToClientDataLoadingType(this DataLoadingType loadingType)
		{
			return (WfClientDataLoadingType)loadingType;
		}

		public static WfBranchProcessExecuteSequence ToBranchProcessExecuteSequence(this WfClientBranchProcessExecuteSequence sequence)
		{
			return (WfBranchProcessExecuteSequence)sequence;
		}

		public static WfClientBranchProcessExecuteSequence ToClientBranchProcessExecuteSequence(this WfBranchProcessExecuteSequence sequence)
		{
			return (WfClientBranchProcessExecuteSequence)sequence;
		}

		public static WfBranchProcessBlockingType ToBranchProcessBlockingType(this WfClientBranchProcessBlockingType blockingType)
		{
			return (WfBranchProcessBlockingType)blockingType;
		}

		public static WfClientBranchProcessBlockingType ToClientBranchProcessBlockingType(this WfBranchProcessBlockingType blockingType)
		{
			return (WfClientBranchProcessBlockingType)blockingType;
		}

		public static WfBranchGroupBlockingType ToBranchGroupBlockingType(this WfClientBranchGroupBlockingType blockingType)
		{
			return (WfBranchGroupBlockingType)blockingType;
		}

		public static WfClientBranchGroupBlockingType ToClientBranchGroupBlockingType(this WfBranchGroupBlockingType blockingType)
		{
			return (WfClientBranchGroupBlockingType)blockingType;
		}

		public static WfProcessType ToProcessType(this WfClientProcessType processType)
		{
			return (WfProcessType)processType;
		}

		public static WfClientProcessType ToClientProcessType(this WfProcessType processType)
		{
			return (WfClientProcessType)processType;
		}

		public static BranchProcessReturnType ToBranchProcessReturnType(this WfClientBranchProcessReturnType returnType)
		{
			return (BranchProcessReturnType)returnType;
		}

		public static WfClientBranchProcessReturnType ToClientBranchProcessReturnType(this BranchProcessReturnType returnType)
		{
			return (WfClientBranchProcessReturnType)returnType;
		}

		public static WfNavigatorDisplayMode ToNavigatorDisplayMode(this WfClientNavigatorDisplayMode ndm)
		{
			return (WfNavigatorDisplayMode)ndm;
		}

		public static WfClientNavigatorDisplayMode ToClientNavigatorDisplayMode(this WfNavigatorDisplayMode ndm)
		{
			return (WfClientNavigatorDisplayMode)ndm;
		}

		public static WfSubProcessResourceMode ToSubProcessResourceMode(this WfClientSubProcessResourceMode resourceMode)
		{
			return (WfSubProcessResourceMode)resourceMode;
		}

		public static WfClientSubProcessResourceMode ToClientSubProcessResourceMode(this WfSubProcessResourceMode resourceMode)
		{
			return (WfClientSubProcessResourceMode)resourceMode;
		}

		public static WfSubProcessApprovalMode ToSubProcessApprovalMode(this WfClientSubProcessApprovalMode approvalMode)
		{
			return (WfSubProcessApprovalMode)approvalMode;
		}

		public static WfClientSubProcessApprovalMode ToClientSubProcessApprovalMode(this WfSubProcessApprovalMode approvalMode)
		{
			return (WfClientSubProcessApprovalMode)approvalMode;
		}

		public static ProcessParameterEvalMode ToProcessParameterEvalMode(this WfClientProcessParameterEvalMode evalMode)
		{
			return (ProcessParameterEvalMode)evalMode;
		}

		public static WfClientProcessParameterEvalMode ToClientProcessParameterEvalMode(this ProcessParameterEvalMode evalMode)
		{
			return (WfClientProcessParameterEvalMode)evalMode;
		}

		public static WfOpinionMode ToOpinionMode(this WfClientOpinionMode opMode)
		{
			return (WfOpinionMode)opMode;
		}

		public static WfClientOpinionMode ToClientOpinionMode(this WfOpinionMode opMode)
		{
			return (WfClientOpinionMode)opMode;
		}

		public static WfSubProcessActivityEditMode ToSubProcessActivityEditMode(this WfClientSubProcessActivityEditMode editMode)
		{
			return (WfSubProcessActivityEditMode)editMode;
		}

		public static WfClientSubProcessActivityEditMode ToClientSubProcessActivityEditMode(this WfSubProcessActivityEditMode editMode)
		{
			return (WfClientSubProcessActivityEditMode)editMode;
		}

		public static WfSearchIDMode ToSearchIDMode(this WfClientSearchIDMode searchIDMode)
		{
			return (WfSearchIDMode)searchIDMode;
		}

		public static WfClientSearchIDMode ToClientSearchIDMode(this WfSearchIDMode searchIDMode)
		{
			return (WfClientSearchIDMode)searchIDMode;
		}

		public static WfAutoSendUserTaskMode ToAutoSendUserTaskMode(this WfClientAutoSendUserTaskMode sendTaskMode)
		{
			return (WfAutoSendUserTaskMode)sendTaskMode;
		}

		public static WfClientAutoSendUserTaskMode ToClientAutoSendUserTaskMode(this WfAutoSendUserTaskMode sendTaskMode)
		{
			return (WfClientAutoSendUserTaskMode)sendTaskMode;
		}

		public static WfAddApproverMode ToAddApproverMode(WfClientAddApproverMode addApproverMode)
		{
			return (WfAddApproverMode)addApproverMode;
		}

		public static WfClientAddApproverMode ToClientAddApproverMode(WfAddApproverMode addApproverMode)
		{
			return (WfClientAddApproverMode)addApproverMode;
		}

		public static IOguObject ToOguObject(this WfClientOguObjectBase client)
		{
			OguBase result = null;

			if (client != null)
			{
				result = (OguBase)OguBase.CreateWrapperObject(client.ID, client.ObjectType.ToOguSchemaType());
				result.Name = client.Name;
				result.DisplayName = client.Name;
			}

			return result;
		}

		public static IList<TServer> ToOguObjects<TClient, TServer>(this IEnumerable<TClient> clientOguObjects)
			where TClient : WfClientOguObjectBase
			where TServer : IOguObject
		{
			List<TServer> result = new List<TServer>();

			if (clientOguObjects != null)
			{
				foreach (TClient client in clientOguObjects)
					result.Add((TServer)client.ToOguObject());
			}

			return result;
		}

		public static WfClientOguObjectBase ToClientOguObject(this IOguObject oguObject)
		{
			WfClientOguObjectBase result = null;

			if (oguObject != null)
			{
				OguBase wrappedObj = (OguBase)OguBase.CreateWrapperObject(oguObject);

				string name = wrappedObj.IsNameInitialized() ? wrappedObj.Name : null;

				result = WfClientOguObjectBase.CreateWrapperObject(wrappedObj.ID, name, wrappedObj.ObjectType.ToClientOguSchemaType());
			}

			return result;
		}

		public static IList<TClient> ToClientOguObjects<TClient, TServer>(this IEnumerable<TServer> clientOguObjects)
			where TClient : WfClientOguObjectBase
			where TServer : IOguObject
		{
			List<TClient> result = new List<TClient>();

			if (clientOguObjects != null)
			{
				foreach (TServer server in clientOguObjects)
					result.Add((TClient)server.ToClientOguObject());
			}

			return result;
		}

		public static WfAssigneeType ToAssigneeType(this WfClientAssigneeType assigneeType)
		{
			return (WfAssigneeType)assigneeType;
		}

		public static WfClientAssigneeType ToClientAssigneeType(this WfAssigneeType assigneeType)
		{
			return (WfClientAssigneeType)assigneeType;
		}

		public static WfClientProcessInfo ToClientProcessInfo(this IWfProcess process)
		{
			return ToClientProcessInfo(process, process.CurrentActivity, (WfClientUser)null);
		}

		public static WfClientProcessInfo ToClientProcessInfo(this IWfProcess process, IUser user)
		{
			return ToClientProcessInfo(process, process.CurrentActivity, (WfClientUser)user.ToClientOguObject());
		}

		public static WfClientProcessInfo ToClientProcessInfo(this IWfProcess process, IWfActivity originalActivity, IUser user)
		{
			return ToClientProcessInfo(process, originalActivity, (WfClientUser)user.ToClientOguObject());
		}

		public static WfClientProcessInfo ToClientProcessInfo(this IWfProcess process, WfClientUser clientUser)
		{
			return ToClientProcessInfo(process, process.CurrentActivity, clientUser);
		}

		public static WfClientProcessInfo ToClientProcessInfo(this IWfProcess process, IWfActivity originalActivity, WfClientUser clientUser)
		{
			process.NullCheck("process");

			WfClientProcessInfo clientProcessInfo = null;

			WfClientProcessInfoConverter.Instance.ServerToClient(process, ref clientProcessInfo);

			clientProcessInfo.AuthorizationInfo = WfClientProcessInfoBaseConverter.Instance.GetAuthorizationInfo(process, originalActivity, clientUser);

			return clientProcessInfo;
		}

		public static T FillCurrentOpinion<T>(this T processInfo, IWfActivity originalActivity, WfClientUser user) where T : WfClientProcessInfoBase
		{
			if (processInfo != null)
				processInfo.CurrentOpinion = WfClientProcessInfoBaseConverter.Instance.GetUserActivityOpinion(originalActivity, user);

			return processInfo;
		}

		public static WfClientRoleOperatorType ToClientRoleOperatorType(this SOARoleOperatorType opType)
		{
			return (WfClientRoleOperatorType)opType;
		}

		public static SOARoleOperatorType ToRoleOperatorType(this WfClientRoleOperatorType opType)
		{
			return (SOARoleOperatorType)opType;
		}

		public static WfClientProbeApplicationRuntimeParameterMode ToClientProbeApplicationRuntimeParameterMode(this WfProbeApplicationRuntimeParameterMode probeMode)
		{
			return (WfClientProbeApplicationRuntimeParameterMode)probeMode;
		}

		public static WfProbeApplicationRuntimeParameterMode ToProbeApplicationRuntimeParameterMode(this WfClientProbeApplicationRuntimeParameterMode probeMode)
		{
			return (WfProbeApplicationRuntimeParameterMode)probeMode;
		}

		private static SchemaType ToOguSchemaType(this ClientOguSchemaType schemaType)
		{
			return (SchemaType)schemaType;
		}

		private static ClientOguSchemaType ToClientOguSchemaType(this SchemaType schemaType)
		{
			return (ClientOguSchemaType)schemaType;
		}
	}
}
