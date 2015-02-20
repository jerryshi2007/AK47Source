using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using System.Reflection;
using MCS.Library.Principal;
using System.Collections.Specialized;

namespace MCS.Web.Library.MVC
{
	/// <summary>
	/// 前置控制器的基类
	/// </summary>
	public abstract class ControllerBase : IHttpHandler
	{
		#region IHttpHandler Members

		bool IHttpHandler.IsReusable
		{
			get
			{
				return true;
			}
		}

		void IHttpHandler.ProcessRequest(HttpContext context)
		{
			context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
			context.Response.Cache.SetNoStore();

			PerformanceMonitorHelper.GetDefaultMonitor().WriteExecutionDuration("ExecuteMethodByRequest(ProcessRequest)",
				() =>
				ControllerHelper.ExecuteMethodByRequest(this));
		}

		#endregion

		#region Properties
		/// <summary>
		/// 当前的操作类
		/// </summary>
		protected ControllerOperationBase CurrentOperation
		{
			get;
			private set;
		}
		#endregion Properties

		#region 控制器方法
		[ControllerMethod(true)]
		protected virtual void DefaultOperation()
		{
			HttpContext.Current.Response.Write("Default Operation");
		}

		[ControllerMethod("activityID,processID")]
		protected void StartWorkflow(string processDescKey)
		{
			StartWorkflow(processDescKey, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
		}

		[ControllerMethod("activityID,processID")]
		protected void StartWorkflow(string processDescKey, string relativeID, string relativeURL)
		{
			StartWorkflow(processDescKey, string.Empty, string.Empty, relativeID, relativeURL, string.Empty);
		}

		[ControllerMethod]
		protected virtual void CopyWorkflow(string processDescKey, string appName, string programName, string sourceResourceID, string sourceProcessID)
		{
			CopyWorkflow(processDescKey, appName, programName, sourceResourceID, sourceProcessID, string.Empty, string.Empty);
		}

		[ControllerMethod]
		protected virtual void CopyWorkflow(string processDescKey, string appName, string programName, string sourceResourceID, string sourceProcessID, string relativeID, string relativeURL)
		{
			IWfProcess originalProcess = null;

			if (sourceProcessID.IsNotEmpty())
			{
				originalProcess = WfRuntime.GetProcessByProcessID(sourceProcessID);
			}
			else
			{
				WfProcessCollection processes = WfRuntime.GetProcessByResourceID(sourceResourceID);

				(processes.Count > 0).FalseThrow("不能根据{0}找到对应的流程", sourceResourceID);
				originalProcess = processes[0];
			}

			InnerStartWorkflow(processDescKey, appName, programName, relativeID, relativeURL, true, originalProcess.ApprovalRootProcess.RelativeParams);
		}

		protected virtual ICommandStatePersist LoadStateByResourceID(string sourceResourceID)
		{
			return null;
		}

		[ControllerMethod("activityID,processID")]
		protected virtual void StartWorkflow(string processDescKey, string appName, string programName, string relativeID, string relativeURL, string relativeParams)
		{
			InnerStartWorkflow(processDescKey, appName, programName, relativeID, relativeURL, false, UriHelper.GetUriParamsCollection(relativeParams));
		}

		private void InnerStartWorkflow(string processDescKey, string appName, string programName,
			string relativeID, string relativeURL, bool isCloned, NameValueCollection relativeParams)
		{
			StartWorkflowControllerOperation operation = new StartWorkflowControllerOperation();

			operation.StartupParams.ProcessDescriptor = WfProcessDescriptorManager.GetDescriptor(processDescKey);

			if (operation.StartupParams.ProcessDescriptor.DefaultTaskTitle.IsNullOrEmpty())
				operation.StartupParams.DefaultTaskTitle = operation.StartupParams.ProcessDescriptor.Name;
			else
				operation.StartupParams.DefaultTaskTitle = operation.StartupParams.ProcessDescriptor.DefaultTaskTitle;

			operation.StartupParams.ResourceID = UuidHelper.NewUuidString();
			operation.StartupParams.ApplicationRuntimeParameters["ProcessRequestor"] = OguUser.CreateWrapperObject(DeluxeIdentity.CurrentUser);
			operation.ApplicationName = appName;
			operation.ProgramName = programName;
			operation.StartupParams.RelativeID = relativeID;
			operation.StartupParams.RelativeURL = relativeURL;
			operation.StartupParams.Department = DeluxeIdentity.CurrentUser.TopOU;
			operation.StartupParams.Assignees.Add(DeluxeIdentity.CurrentUser);
			operation.IsCloned = isCloned;

			operation.StartupParams.RelativeParams.CopyFrom(relativeParams);

			operation.NavigationCommand = CollectNavigationCommand(this.GetType());
			operation.SceneInfo = CollectSceneInfo(this.GetType());

			OnInitOperation(operation);

			PerformanceMonitorHelper.GetDefaultMonitor().WriteExecutionDuration("DoStartWorkflowControllerOperation",
				() => operation.DoOperation());
		}

		[ControllerMethod]
		protected void OpenFromActivity(string resourceID, string activityID)
		{
			ExecuteOpenFormControllerOperation();
		}

		[ControllerMethod]
		protected void OpenFromProcess(string resourceID, string processID)
		{
			ExecuteOpenFormControllerOperation();
		}

		[ControllerMethod]
		protected void OpenFromResource(string resourceID)
		{
			ExecuteOpenFormControllerOperation();
		}

		[ControllerMethod]
		protected void OpenFromResource(string resourceID, string requestTicket)
		{
			ExecuteOpenFormControllerOperation();
		}

		[ControllerMethod]
		protected void OpenNotifyResource(string resourceID, string taskID, string _op)
		{
			string targetUrl = GetDefaultTargetUrl();

			HttpContext.Current.Server.Transfer(targetUrl);
		}

		[ControllerMethod]
		protected void ProcessRequestTicket(string requestTicket)
		{
			RelativeTicket ticket = RelativeTicket.DecryptFromString(requestTicket);

			ticket.CheckUriReferer();

			if (ticket.TargetUri.IsNotEmpty())
			{
				string redir = RelativeTicket.GetRequestTicketUrl(ticket.TargetUri, string.Empty);

				HttpContext.Current.Response.Redirect(redir);
			}
		}
		#endregion

		#region Protected
		protected virtual void OnAfterInitOperation(ControllerOperationBase operation)
		{

		}
		#endregion

		#region Private
		private string GetDefaultTargetUrl()
		{
			string url = string.Empty;

			ControllerNavigationTargetAttribute attribute = AttributeHelper.GetCustomAttribute<ControllerNavigationTargetAttribute>(this.GetType());

			if (attribute != null)
			{
				url = attribute.Url;
			}

			return url;
		}

		private void OnInitOperation(ControllerOperationBase operation)
		{
			this.CurrentOperation = operation;

			operation.ProcessReady += new ProcessReadyHandler(OnProcessReadyHandler);
			OnAfterInitOperation(operation);
		}

		private void OnProcessReadyHandler(IWfProcess process)
		{
			if (process.LoadingType == DataLoadingType.Cloned)
			{
				string sourceResourceID = WebUtility.GetRequestQueryString("sourceResourceID", string.Empty);

				ICommandStatePersist originalState = LoadStateByResourceID(sourceResourceID);

				ExceptionHelper.FalseThrow(originalState != null, "CopyForm时，必须实现LoadStateByResourceID方法");
				ICommandStatePersist persistedState = (ICommandStatePersist)originalState;

				CommandStateBase newState = (CommandStateBase)persistedState.CloneBusinessObject();

				CommandStateHelper.RegisterState(newState);
			}
		}

		private void ExecuteOpenFormControllerOperation()
		{
			IWfProcess process = WfClientContext.Current.OriginalActivity.Process;
			OpenFormControllerOperation operation = new OpenFormControllerOperation();

			string requestTicketString = HttpContext.Current.Request.QueryString.GetValue("requestTicket", string.Empty);

			if (requestTicketString.IsNotEmpty())
			{
				RelativeTicket requestTicket = RelativeTicket.DecryptFromString(requestTicketString);

				requestTicket.CheckIsValid();

				operation.IsRelativeForm = true;
			}

			operation.Process = process;
			operation.NavigationCommand = CollectNavigationCommand(this.GetType());
			operation.SceneInfo = CollectSceneInfo(this.GetType());

			OnInitOperation(operation);

			PerformanceMonitorHelper.GetDefaultMonitor().WriteExecutionDuration("DoOpenFormControllerOperation",
				() => operation.DoOperation());
		}

		private IRequestCommand CollectNavigationCommand(MemberInfo mi)
		{
			ControllerNavigationTargetAttribute attribute = AttributeHelper.GetCustomAttribute<ControllerNavigationTargetAttribute>(mi);

			PageNavigateCommandBase result = null;

			if (attribute != null)
			{
				if (attribute.TransferType == ControllerNavigationType.Transfer)
					result = new TransferCommand(attribute.Name);
				else
					result = new RedirectCommand(attribute.Name);

				result.NavigateUrl = attribute.Url;

				if (ResourceUriSettings.GetConfig().Features.ContainsKey(attribute.FeatureName))
					result.Feature = ResourceUriSettings.GetConfig().Features[attribute.FeatureName].Feature;
			}

			return result;
		}

		private SceneInfoAttribute CollectSceneInfo(MemberInfo mi)
		{
			return AttributeHelper.GetCustomAttribute<SceneInfoAttribute>(mi);
		}
		#endregion
	}
}
