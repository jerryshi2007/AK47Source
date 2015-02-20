using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.ComponentModel;
using System.Web;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Web.Library;
using MCS.Web.Library.Script;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.Globalization;
using MCS.Library.Principal;
using MCS.Web.Library.MVC;
using MCS.Library.SOA.DataObjects;

[assembly: WebResource("MCS.Web.WebControls.Workflow.MoveTo.wflogo.jpg", "image/jpeg")]
[assembly: WebResource("MCS.Web.WebControls.Workflow.Abstract.WfControls.js", "application/x-javascript")]

namespace MCS.Web.WebControls
{
	[RequiredScript(typeof(ControlBaseScript), 1)]
	[RequiredScript(typeof(HBCommonScript), 2)]
	[ClientScriptResource("MCS.Web.WebControls.WfMoveToControl", "MCS.Web.WebControls.Workflow.Abstract.WfControls.js")]
	[ToolboxData("<{0}:WfMoveToControl runat=server></{0}:WfMoveToControl>")]
	public class WfMoveToControl : WfControlBase
	{
		#region Private
		private IAttributeAccessor controlToMoveTo = null;
		private IAttributeAccessor controlToSave = null;
		private SubmitButton innerMoveToButton = null;
		private SubmitButton innerSaveButton = null;
		private WfControlNextStepCollection _NextSteps = null;
		private WfControlNextStep _CurrentStep = null;
		private WfMoveToSelectedResult _MoveToSelectedResult = null;
		#endregion

		#region Events
		//public event BeforeOperationEventHandler BeforeMoveToCheck;
		//public event BeforeOperationEventHandler BeforeMoveTo;
		public event ProcessChangedEventHandler ProcessChanged;
		public event ProcessChangedEventHandler AfterProcessChanged;
		public event PrepareNextStepsEventHanlder PrepareNextSteps;
		public event AfterGetNextStepResourcesEventHanlder AfterGetNextStepResources;

		/// <summary>
		/// 创建完修改流程的Executor事件
		/// </summary>
		public event AfterCreateExecutorHandler AfterCreateModifyProcessExecutor;
		#endregion

		public WfMoveToControl()
		{
			if (this.DesignMode == false)
			{
				WfConverterHelper.RegisterConverters();
				JSONSerializerExecute.RegisterConverter(typeof(WfMoveToSelectedResultConverter));
			}
		}

		#region Properties
		[DefaultValue(true)]
		[Category("Appearance")]
		[ScriptControlProperty()]
		[ClientPropertyName("autoShowResoureUserSelector")]
		[Description("下一步没有选择资源时，是否自动弹出人员选择框")]
		public bool AutoShowResoureUserSelector
		{
			get
			{
				return GetPropertyValue("AutoShowResoureUserSelector", true);
			}
			set
			{
				SetPropertyValue("AutoShowResoureUserSelector", value);
			}
		}

		[DefaultValue("")]
		[Category("流程")]
		[Description("缺省的流程描述的Key")]
		public string DefaultProcessKey
		{
			get
			{
				return GetPropertyValue("DefaultProcessKey", string.Empty);
			}
			set
			{
				SetPropertyValue("DefaultProcessKey", value);
			}
		}

		[DefaultValue(""), IDReferenceProperty(), TypeConverter(typeof(DialogStartUpControlConverter))]
		[Category("Appearance")]
		public string ControlIDToMoveTo
		{
			get
			{
				return GetPropertyValue<string>("ControlIDToMoveTo", string.Empty);
			}
			set
			{
				SetPropertyValue<string>("ControlIDToMoveTo", value);
				this.controlToMoveTo = null;
			}
		}

		[Browsable(false)]
		public IAttributeAccessor ControlToMoveTo
		{
			get
			{
				if (this.controlToMoveTo == null)
				{
					if (string.IsNullOrEmpty(ControlIDToMoveTo) == false)
						this.controlToMoveTo = (IAttributeAccessor)WebControlUtility.FindControlByID(Page, ControlIDToMoveTo, true);
				}

				return this.controlToMoveTo;
			}
			set
			{
				this.controlToMoveTo = value;
			}
		}

		[DefaultValue(""), IDReferenceProperty(), TypeConverter(typeof(DialogStartUpControlConverter))]
		[Category("Appearance")]
		public string ControlIDToSave
		{
			get
			{
				return GetPropertyValue<string>("ControlIDToSave", string.Empty);
			}
			set
			{
				SetPropertyValue<string>("ControlIDToSave", value);
			}
		}

		[Browsable(false)]
		public IAttributeAccessor ControlToSave
		{
			get
			{
				if (this.controlToSave == null)
				{
					if (string.IsNullOrEmpty(ControlIDToSave) == false)
						this.controlToSave = (IAttributeAccessor)WebControlUtility.FindControlByID(Page, ControlIDToSave, true);
				}

				return this.controlToSave;
			}
			set
			{
				this.controlToSave = value;
			}
		}

		[DefaultValue(WfPrivilegeMode.Normal)]
		[Category("文档")]
		[Description("控件的工作的权限模式")]
		public WfPrivilegeMode PrivilegeMode
		{
			get
			{
				return GetPropertyValue("PrivilegeMode", WfPrivilegeMode.Normal);
			}
			set
			{
				SetPropertyValue("PrivilegeMode", value);
			}
		}

		[DefaultValue(true)]
		[Category("文档")]
		[Description("客户端是否检查下一步流转人员是否为空")]
		[ScriptControlProperty()]
		[ClientPropertyName("clientCheckSelectdUsers")]
		public bool ClientCheckSelectdUsers
		{
			get
			{
				return GetPropertyValue("ClientCheckSelectdUsers", true);
			}
			set
			{
				SetPropertyValue("ClientCheckSelectdUsers", value);
			}
		}

		[Browsable(false)]
		[ScriptControlProperty()]
		[ClientPropertyName("opinionInputControlID")]
		internal string OpinionInputControlID
		{
			get
			{
				return GetPropertyValue("OpinionInputControlID", string.Empty);
			}
			set
			{
				SetPropertyValue("OpinionInputControlID", value);
			}
		}

		[Browsable(false)]
		[ScriptControlProperty()]
		[ClientPropertyName("opinionTypeClientID")]
		internal string OpinionTypeClientID
		{
			get
			{
				return GetPropertyValue("OpinionTypeClientID", string.Empty);
			}
			set
			{
				SetPropertyValue("OpinionTypeClientID", value);
			}
		}

		[Browsable(false)]
		[ScriptControlProperty()]
		[ClientPropertyName("allowEmptyOpinion")]
		internal bool AllowEmptyOpinion
		{
			get
			{
				return GetPropertyValue("AllowEmptyOpinion", true);
			}
			set
			{
				SetPropertyValue("AllowEmptyOpinion", value);
			}
		}

		[Browsable(false)]
		[ScriptControlProperty()]
		[ClientPropertyName("innerMoveToButtonClientID")]
		private string InnerMoveToButtonClientID
		{
			get
			{
				string result = string.Empty;

				if (innerMoveToButton != null)
					result = innerMoveToButton.ClientID;

				return result;
			}
		}

		[Browsable(false)]
		[ScriptControlProperty()]
		[ClientPropertyName("innerSaveButtonClientID")]
		private string InnerSaveButtonClientID
		{
			get
			{
				string result = string.Empty;

				if (innerSaveButton != null)
					result = innerSaveButton.ClientID;

				return result;
			}
		}

		[Browsable(false)]
		[ScriptControlProperty()]
		[ClientPropertyName("resourceUserSelectorClientID")]
		private string ResourceUserSelectorClientID
		{
			get
			{
				string result = string.Empty;

				//if (this.resourceUserSelector != null)
				//	result = resourceUserSelector.ClientID;

				return result;
			}
		}

		[Browsable(false)]
		public WfControlNextStepCollection NextSteps
		{
			get
			{
				if (this._NextSteps == null)
				{
					if (WfClientContext.Current.CurrentActivity != null)
					{
						this._NextSteps = WfControlNextStepCollection.GetControlNextStepsByProcessDescriptor(
							WfClientContext.Current.CurrentActivity.Descriptor,
							PrepareNextSteps,
							AfterGetNextStepResources);
					}
					else
						this._NextSteps = new WfControlNextStepCollection();
				}

				return this._NextSteps;
			}
		}

		[Browsable(false)]
		public WfControlNextStep CurrentStep
		{
			get
			{
				if (this._CurrentStep == null)
					if (WfClientContext.Current.OriginalActivity != null)
						this._CurrentStep = new WfControlNextStep(WfClientContext.Current.CurrentActivity);

				return this._CurrentStep;
			}
		}

		[Browsable(false)]
		public WfMoveToSelectedResult MoveToSelectedResult
		{
			get
			{
				return this._MoveToSelectedResult;
			}
		}

		public static WfMoveToControl Current
		{
			get
			{
				return (WfMoveToControl)HttpContext.Current.Items["WfMoveToControl"];
			}
		}

		/// <summary>
		/// 客户端流转之前的事件
		/// </summary>
		[DefaultValue("")]
		[ScriptControlEvent]
		[ClientPropertyName("beforeExecute")]
		[Bindable(true), Category("ClientEventsHandler"), Description("客户端流转之前的事件")]
		public string OnClientBeforeExecute
		{
			get
			{
				return GetPropertyValue("onClientBeforeExecute", string.Empty);
			}
			set
			{
				SetPropertyValue("onClientBeforeExecute", value);
			}
		}

		[DefaultValue("")]
		[ScriptControlEvent]
		[ClientPropertyName("validateBindingData")]
		[Bindable(true), Category("ClientEventsHandler"), Description("客户端校验绑定数据的事件")]
		public string OnClientValidateBindingData
		{
			get
			{
				return GetPropertyValue("onClientValidateBindingData", string.Empty);
			}
			set
			{
				SetPropertyValue("onClientValidateBindingData", value);
			}
		}

		[DefaultValue(-1)]
		[ScriptControlProperty]
		[ClientPropertyName("validateGroupWhenSave")]
		[Bindable(true), Category("文档"), Description("保存时客户端是否自动校验")]
		public int ValidateGroupWhenSave
		{
			get
			{
				return GetPropertyValue("ValidateGroupWhenSave", -1);
			}
			set
			{
				SetPropertyValue("ValidateGroupWhenSave", value);
			}
		}

		internal string RelativeOpinionListViewID
		{
			get
			{
				return GetPropertyValue("RelativeOpinionListViewID", string.Empty);
			}
			set
			{
				SetPropertyValue("RelativeOpinionListViewID", value);
			}
		}

		/// <summary>
		/// 修改流程的按钮的服务器端的Button的ID
		/// </summary>
		internal string ChangeProcessButtonID
		{
			get
			{
				return GetPropertyValue("ChangeProcessButtonID", string.Empty);
			}
			set
			{
				SetPropertyValue("ChangeProcessButtonID", value);
			}
		}

		[DefaultValue(-1)]
		[ScriptControlProperty]
		[ClientPropertyName("validateGroupWhenMoveTo")]
		[Bindable(true), Category("文档"), Description("流转时客户端是否自动校验")]
		public int ValidateGroupWhenMoveTo
		{
			get
			{
				return GetPropertyValue("ValidateGroupWhenMoveTo", -1);
			}
			set
			{
				SetPropertyValue("ValidateGroupWhenMoveTo", value);
			}
		}

		[Browsable(false)]
		private bool NeedToRender
		{
			get
			{
				bool result = this.Visible && !this.ReadOnly;

				if (result)
					result = WfClientContext.Current.InMoveToMode;

				return result;
			}
		}
		#endregion

		#region Protected
		protected override void OnInit(EventArgs e)
		{
			WfControlHelperExt.InitWfControlPostBackErrorHandler(this.Page);
			base.OnInit(e);

			ExceptionHelper.TrueThrow(WfMoveToControl.Current != null && WfMoveToControl.Current != this,
				"不能在页面上创建两个WfMoveToControl");

			HttpContext.Current.Items["WfMoveToControl"] = this;

			if (Page.IsCallback)
				EnsureChildControls();
		}

		protected override void OnPagePreLoad(object sender, EventArgs e)
		{
			if (Page.IsCallback == false && Page.IsPostBack == false)
			{
				if (WfClientContext.Current.OriginalActivity == null &&
					this.DefaultProcessKey.IsNotEmpty())
				{
					StartDefaultWorkflow();
				}
			}

			base.OnPagePreLoad(sender, e);
		}

		protected override void OnPreRender(EventArgs e)
		{
			if (RenderMode.UseNewPage == false)
			{
				RegisterNextSteps();

				if (RenderMode.OnlyRenderSelf == false)
				{
					InitInnerControlAttributes(
						ControlToMoveTo,
						this.innerMoveToButton,
						string.Format("if (!event.srcElement.disabled && $find('{0}').doMoveTo($HBRootNS.WfControlOperationType.MoveTo))", this.ClientID),
						this.Visible && !this.ReadOnly && NeedToRender,
						true);

					if (WfClientContext.Current.CurrentActivity != null &&
						WfClientContext.Current.CurrentActivity.Descriptor.Properties.GetValue("AllowSave", true))
					{
						InitInnerControlAttributes(
							ControlToSave,
							this.innerSaveButton,
							string.Format("if (!event.srcElement.disabled && $find('{0}').doSave(false))", this.ClientID),
							this.Visible && !this.ReadOnly &&
							(this.PrivilegeMode == WfPrivilegeMode.Admin || NeedToRender),
							false);
					}
				}

				if (WfClientContext.Current.CurrentActivity != null)
				{
					Page.ClientScript.RegisterClientScriptBlock(typeof(WfControlBase),
						"wfExternalFrame",
						"<div id='wfExternalFrameContainer'><iframe id='wfExternalFrame' name='wfExternalFrame' style='display:none'></iframe></div>");

					Page.ClientScript.RegisterStartupScript(typeof(WfMoveToControl),
						"WfMoveToControl",
						"document.getElementById('wfExternalFrame').onreadystatechange = $HBRootNS.WfMoveToControl.onInnerFrameStateChange;",
						true);

					this.Page.Form.Target = "wfExternalFrame";
				}

				DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "没有选择下一步的流转人员");
				DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "已经被注销");
				DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "确认要执行吗？");

				base.OnPreRender(e);
			}
		}

		protected override void CreateChildControls()
		{
			CreateButtons();

			//this.resourceUserSelector.ID = "resourceUserSelector";
			//this.resourceUserSelector.DialogTitle = Translator.Translate(Define.DefaultCategory, "请选择下一步的人员");

			//Controls.Add(this.resourceUserSelector);

			base.CreateChildControls();
		}

		protected override void Render(HtmlTextWriter writer)
		{
			if (this.DesignMode)
				writer.Write(string.Format("<img src=\"{0}\"/>",
					Page.ClientScript.GetWebResourceUrl(this.GetType(),
						"MCS.Web.WebControls.Workflow.MoveTo.wflogo.jpg")));
			else
				base.Render(writer);
		}

		internal void RegisterRefreshProcessButton(string buttonID)
		{
			if (buttonID.IsNotEmpty())
			{
				Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "RefreshProcessButton",
					string.Format("var _wfRefreshButtonID = \"{0}\";", buttonID), true);
			}
		}

        internal void RegisterRefreshCurrentProcessButton(string buttonID)
        {
            if (buttonID.IsNotEmpty())
            {
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "RefreshCurrentProcessButton",
                    string.Format("var _wfRefreshCurrentButtonID = \"{0}\";", buttonID), true);
            }
        }

		internal void OnProcessChanged(IWfProcess process)
		{
			if (ProcessChanged != null)
				ProcessChanged(process);
		}

		internal void OnAfterProcessChanged(IWfProcess process)
		{
			if (AfterProcessChanged != null)
				AfterProcessChanged(process);
		}

		internal void OnAfterCreateModifyProcessExecutor(WfExecutorBase executor)
		{
			if (AfterCreateModifyProcessExecutor != null)
				AfterCreateModifyProcessExecutor(executor);
		}

		internal static void DoActionAfterRegisterContextConverter(Action action)
		{
			JSONSerializerExecute.BeginRegisterContextConverters();
			try
			{
				JSONSerializerExecute.RegisterContextConverter(typeof(EasyPropertyValueConverter));
				JSONSerializerExecute.RegisterContextConverter(typeof(EasyWfBackwardTransitionDescriptorConverter));
				JSONSerializerExecute.RegisterContextConverter(typeof(EasyWfForwardTransitionDescriptorConverter));
				JSONSerializerExecute.RegisterContextConverter(typeof(EasyWfActivityDescriptorConverter));

				action();
			}
			finally
			{
				JSONSerializerExecute.EndRegisterContextConverters();
			}
		}

		protected override void LoadClientState(string clientState)
		{
			if (string.IsNullOrEmpty(clientState) == false)
			{
				DoActionAfterRegisterContextConverter(()=>
					this._MoveToSelectedResult = (WfMoveToSelectedResult)JSONSerializerExecute.DeserializeObject(clientState, typeof(WfMoveToSelectedResult)));
			}
		}
		#endregion

		#region Private
		private void RegisterNextSteps()
		{
			DoActionAfterRegisterContextConverter(() =>
			{
				Page.ClientScript.RegisterHiddenField("processNextStepsHidden",
					JSONSerializerExecute.Serialize(this.NextSteps));

				Page.ClientScript.RegisterHiddenField("processCurrentStepsHidden",
					JSONSerializerExecute.Serialize(this.CurrentStep));

				Page.ClientScript.RegisterHiddenField("processInMoveToMode",
					WfClientContext.Current.InMoveToMode.ToString());
			});
		}

		private void CreateButtons()
		{
			this.innerMoveToButton = CreateHiddenButton(
				"innerMoveToButton",
				Translator.Translate(Define.DefaultCulture, "正在流转..."),
				this.ControlIDToMoveTo,
				new EventHandler(innerMoveToButton_Click));
			Controls.Add(this.innerMoveToButton);

			this.innerSaveButton = CreateHiddenButton(
				"innerSaveButton",
				Translator.Translate(Define.DefaultCulture, "正在保存..."),
				this.ControlIDToSave,
				new EventHandler(innerSaveButton_Click));
			Controls.Add(this.innerSaveButton);
		}

		private void InitInnerControlAttributes(IAttributeAccessor target, Button innerBtn, string script, bool visible, bool changeText)
		{
			if (target != null)
			{
				string clickScript = string.Format("event.returnValue = false; {0}return false;",
					string.IsNullOrEmpty(script) ? string.Empty : script + " ");

				target.SetAttribute("onclick", clickScript);

				((Control)target).Visible = visible;
				target.SetAttribute("class", "enable");

				if (changeText)
				{
					WfControlNextStep step = null;

					if (NextSteps.Count > 0)
						step = NextSteps[0];

					if (step != null)
					{
						if (WfClientContext.Current.CurrentActivity.Descriptor.Properties.GetValue("MoveToButtonNameSameAsTransitionName", false))
						{
							if (step.TransitionDescriptor != null)
							{
								string btnName = step.TransitionDescriptor.Name;

								if (string.IsNullOrEmpty(btnName))
									btnName = step.TransitionDescriptor.Description;

								if (btnName.IsNotEmpty())
								{
									string originalText = ((IButtonControl)target).Text;
									((IButtonControl)target).Text = originalText.Replace("送签", HttpUtility.HtmlEncode(btnName));
								}
							}
						}
					}
				}
			}
		}

		private WfExecutorBase PrepareExecutor()
		{
			MoveToSelectedResult.NullCheck<WfRuntimeException>(Translator.Translate(Define.DefaultCulture, "MoveToSelectedResult属性为空，请确认代码是否运行在流程提交的过程中"));

			return null;
		}

		private void innerMoveToButton_Click(object sender, EventArgs e)
		{
			MoveToSelectedResult.NullCheck<WfRuntimeException>(Translator.Translate(Define.DefaultCulture, "MoveToSelectedResult属性为空，请确认代码是否运行在流程提交的过程中"));

			WfExecutorBase executor = MoveToSelectedResult.CreateExecutor();

			OnAfterCreateExecutor(executor);

			ExecuteOperation("流转", MoveToSelectedResult.OperationType, executor);
		}

		private void innerSaveButton_Click(object sender, EventArgs e)
		{
			WfSaveDataExecutor executor = new WfSaveDataExecutor(WfClientContext.Current.OriginalActivity, WfClientContext.Current.OriginalActivity);

			OnAfterCreateExecutor(executor);

			ExecuteOperation("保存", WfControlOperationType.Save, executor);
		}

		/// <summary>
		/// 执行流转或保存操作。
		/// </summary>
		/// <param name="actionName">操作名称</param>
		/// <param name="action">实际操作</param>
		private void ExecuteOperation(string actionName, WfControlOperationType operationType, WfExecutorBase executor)
		{
			StringBuilder strScript = new StringBuilder();

			ExceptionHelper.FalseThrow(WfClientContext.Current.HasProcessChanged == false && WfClientContext.Current.InMoveToMode,
				Translator.Translate(Define.DefaultCulture, "当前流程的状态已经改变，不能流转"));

			try
			{
				if (executor != null)
					WfClientContext.Current.Execute(executor);

				HttpContext.Current.Response.Write(ExtScriptHelper.GetRefreshBridgeScript());

				strScript.Append(GetAfterExecutionScript(operationType));

				ResponsePostBackPlaceHolder();
			}
			catch (System.Exception ex)
			{
				ResponsePostBackPlaceHolder();

				System.Exception innerEx = ExceptionHelper.GetRealException(ex);

				if (innerEx is ApplicationInformationException)
					WebUtility.ResponseShowClientMessageScriptBlock(innerEx.Message, innerEx.StackTrace, Translator.Translate(Define.DefaultCulture, "流转"));
				else
					WebUtility.ResponseShowClientErrorScriptBlock(innerEx.Message, innerEx.StackTrace, Translator.Translate(Define.DefaultCulture, "流转"));
			}
			finally
			{
				strScript.Insert(0, "if (parent.document.all('wfOperationNotifier')) parent.document.all('wfOperationNotifier').value = '';");

				WebUtility.ResponseTimeoutScriptBlock(strScript.ToString(), ExtScriptHelper.DefaultResponseTimeout);

				Page.Response.End();
			}
		}

		private string GetAfterExecutionScript(WfControlOperationType operationType)
		{
			string script = "top.$HBRootNS.WfProcessControlBase.close()";

			switch (operationType)
			{
				case WfControlOperationType.Save:
					script = string.Format("if (parent) parent.location.replace('{0}');",
						WfClientContext.Current.ReplaceEntryPathByActivityID()
						);
					break;
				case WfControlOperationType.MoveTo:
					if (WfClientContext.Current.OriginalActivity.Descriptor.Properties.GetValue("ForceCloseFormAfterMoveTo", false) == false)
					{
						UserTaskCollection userTasks = UserTaskAdapter.Instance.GetUserTasks(UserTaskIDType.ActivityID,
							UserTaskFieldDefine.SendToUserID | UserTaskFieldDefine.Status | UserTaskFieldDefine.Url,
							WfClientContext.Current.CurrentActivity.ID);

						UserTask task = userTasks.Find(t => t.SendToUserID == DeluxeIdentity.CurrentUser.ID && t.Status == TaskStatus.Ban);

						if (task != null)
							script = string.Format("if (parent) parent.location.replace('{0}');", task.Url);
					}
					break;
			}

			return script;
		}

		/// <summary>
		/// 启动默认的流程
		/// </summary>
		private void StartDefaultWorkflow()
		{
			WfProcessStartupParams startupParams = new WfProcessStartupParams();

			startupParams.ResourceID = UuidHelper.NewUuidString();
			startupParams.ProcessDescriptor = WfProcessDescriptorManager.GetDescriptor(DefaultProcessKey);

			if (string.IsNullOrEmpty(startupParams.ProcessDescriptor.Url))
				startupParams.ProcessDescriptor.Url = WfClientContext.Current.EntryUri.ToString();

			if (startupParams.ProcessDescriptor.InitialActivity != null)
				startupParams.ProcessDescriptor.InitialActivity.Properties.SetValue("AutoSendUserTask", false);

			startupParams.Department = DeluxeIdentity.CurrentUser.TopOU;
			startupParams.Assignees.Add(DeluxeIdentity.CurrentUser);

			WfStartWorkflowExecutor executor = new WfStartWorkflowExecutor(null, startupParams);

			WfClientContext.Current.Execute(executor);
		}
		#endregion
	}
}
