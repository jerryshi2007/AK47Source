using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using MCS.Library.Core;
using MCS.Library.Globalization;
using MCS.Library.OGUPermission;
using MCS.Library.Principal;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library;
using MCS.Web.Library.MVC;
using MCS.Web.Library.Script;

[assembly: WebResource("MCS.Web.WebControls.Opinion.addApproverLogo.gif", "image/gif")]
[assembly: WebResource("MCS.Web.WebControls.Opinion.returnLogo.gif", "image/gif")]

namespace MCS.Web.WebControls
{
	[DefaultProperty("Opinions")]
	[ToolboxData("<{0}:OpinionListView runat=server></{0}:OpinionListView>")]
	public partial class OpinionListView : Control, INamingContainer
	{
		private GenericOpinionCollection _Opinions = null;
		private OpinionInput _OpinionInput = new OpinionInput();
		private Control _ActivateContainer = null;
		private IWfActivityDescriptor _ActivateDescriptor = null;
		private HtmlTable _Table = new HtmlTable();
		private SubmitButton _ChangeProcessButton = new SubmitButton();
		private SubmitButton _RefreshProcessButton = new SubmitButton();
		private SubmitButton _RefreshCurrentProcessButton = new SubmitButton();
		private SubmitButton _ChangeTransitionButton = new SubmitButton();
		private WfActivityDescriptorEditorBase _activityEditor = null;
		private WfActivityDescriptorEditorBase _groupActivityEditor = null;
		private HtmlInputHidden _HiddenData = new HtmlInputHidden();

		private GenericOpinion _CurrentOpinion = null;
		private Control _RootPanel = null;

		private WfNextStepSelectorControl _EnabledNextStepSelectorControl = null;

		private Dictionary<IWfActivityDescriptor, Control> _opinionPlaceHolders = new Dictionary<IWfActivityDescriptor, Control>();
		private Dictionary<string, Control> _activityEditorPlaceHolders = new Dictionary<string, Control>();

		public event AutoCompleteExtender.GetDataSourceDelegate GetDataSource;
		public event ValidateInputOuUserHandler ValidateInputOuUser;

		/// <summary>
		/// 意见绑定事件定义
		/// </summary>
		public event EventHandler<OpinionListViewBindEventArgs> CollectCurrentOpinion;

		/// <summary>
		/// 调用刷新流程前的操作
		/// </summary>
		public event EventHandler RefreshProcess;

		/// <summary>
		/// 意见绑定事件定义
		/// </summary>
		public event EventHandler<OpinionListViewBindEventArgs> OpinionBind;

		public OpinionListView()
		{
			JSONSerializerExecute.RegisterConverter(typeof(OguObjectConverter));
			WfConverterHelper.RegisterConverters();
		}

		#region Properties
		[Browsable(false)]
		public string RefreshProcessButtonClientID
		{
			get
			{
				return this._RefreshProcessButton.ClientID;
			}
		}

		[Browsable(false)]
		public string RefreshCurrentProcessButtonClientID
		{
			get
			{
				return this._RefreshCurrentProcessButton.ClientID;
			}
		}

		[Browsable(false)]
		public OpinionInput InputControl
		{
			get
			{
				return this._OpinionInput;
			}
		}

		/// <summary>
		/// Title的显示模式
		/// </summary>
		[DefaultValue(UserTitleShowMode.ShowActivityNameAndDeptNameAndTitle)]
		public UserTitleShowMode TitleShowMode
		{
			get
			{
				return WebControlUtility.GetViewStateValue(this.ViewState, "TitleShowMode", UserTitleShowMode.ShowActivityNameAndDeptNameAndTitle);
			}
			set
			{
				WebControlUtility.SetViewStateValue(this.ViewState, "TitleShowMode", value);
			}
		}

		/// <summary>
		/// 流程ID
		/// </summary>
		[DefaultValue("")]
		[Category("文档")]
		[Description("流程ID")]
		public string ProcessID
		{
			get
			{
				return WebControlUtility.GetViewStateValue(this.ViewState, "ProcessID", string.Empty);
			}
			set
			{
				WebControlUtility.SetViewStateValue(this.ViewState, "ProcessID", value);
			}
		}

		[DefaultValue(typeof(Unit), "100%")]
		[Bindable(true), Category("Appearance")]
		public Unit Width
		{
			get
			{

				return WebControlUtility.GetViewStateValue(ViewState, "Width", Unit.Empty);
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "Width", value);
			}
		}

		[DefaultValue(typeof(Unit), "")]
		[Bindable(true), Category("Appearance")]
		public Unit Height
		{
			get
			{

				return WebControlUtility.GetViewStateValue(ViewState, "Height", Unit.Empty);
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "Height", value);
			}
		}

		/// <summary>
		/// 编辑流程时，是否显示阅知人
		/// </summary>
		[DefaultValue(false)]
		[Bindable(true), Category("Appearance")]
		public bool ShowCirculateUsers
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "ShowCirculateUsers", false);
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "ShowCirculateUsers", value);
			}
		}

		/// <summary>
		/// 是否显示步骤的人员
		/// </summary>
		[DefaultValue(ShowStepUsersDefine.All)]
		[Bindable(true), Category("Appearance")]
		public ShowStepUsersDefine ShowStepUsers
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "ShowStepUsers", ShowStepUsersDefine.All);
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "ShowStepUsers", value);
			}
		}

		/// <summary>
		/// 流转时是否允许空意见
		/// </summary>
		[Description("流转时是否允许空意见")]
		[DefaultValue(true)]
		[Bindable(true), Category("Appearance")]
		public bool AllowEmptyOpinion
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "AllowEmptyOpinion", true);
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "AllowEmptyOpinion", value);
			}
		}

		/// <summary>
		/// 是否显示用户自定义的意见
		/// </summary>
		[DefaultValue(true)]
		[Bindable(true), Category("Appearance")]
		[Description("是否显示用户自定义的意见")]
		public bool ShowPredefinedOpinions
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "ShowPredefinedOpinions", true);
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "ShowPredefinedOpinions", value);
			}
		}

		[Description("意见为空时显示的文本")]
		[DefaultValue("（无）")]
		[Bindable(true), Category("Appearance")]
		public string EmptyOpinionText
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "EmptyOpinionText", Translator.Translate(Define.DefaultCulture, "（无）"));
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "EmptyOpinionText", value);
			}
		}

		/// <summary>
		/// 检查意见时，如果为空提示的信息
		/// </summary>
		[Description("检查意见时，如果为空提示的信息")]
		[DefaultValue("请填写意见")]
		[Bindable(true), Category("Appearance")]
		public string CheckEmptyOpinionText
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "CheckEmptyOpinionText", Translator.Translate(Define.DefaultCulture, "请填写意见"));
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "CheckEmptyOpinionText", value);
			}
		}

		/// <summary>
		/// 流转时是否允许空意见
		/// </summary>
		[Description("流转时是否显示意见输入控件")]
		[DefaultValue(true)]
		[Bindable(true), Category("Appearance")]
		public bool ShowOpinionInput
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "ShowOpinionInput", true);
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "ShowOpinionInput", value);
			}
		}

		/// <summary>
		/// 意见集合
		/// </summary>
		[Browsable(false)]
		public GenericOpinionCollection Opinions
		{
			get
			{
				return _Opinions;
			}
			set
			{
				_Opinions = value;
			}
		}

		/// <summary>
		/// 默认的意见类型
		/// </summary>
		public string DefaultOpinionType
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "DefaultOpinionType", string.Empty);
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "DefaultOpinionType", value);
			}
		}

		/// <summary>
		/// 当前操作人的意见
		/// </summary>
		[Browsable(false)]
		public GenericOpinion CurrentOpinion
		{
			get
			{
				if (_CurrentOpinion == null)
				{
					_CurrentOpinion = GetCurrentActivityOpinion();

					if (_CurrentOpinion == null)
						_CurrentOpinion = _OpinionInput.Opinion;
				}

				if (_CurrentOpinion != null)
				{
					string reqOContent = string.Format("{0}${1}${2}", this.ClientID, "OpinionInput", "OpinionText");
					string reqOType = string.Format("{0}${1}${2}", this.ClientID, "OpinionInput", "OpinionType");

					_CurrentOpinion.Content = HttpContext.Current.Request.Form.GetValue(reqOContent, _CurrentOpinion.Content);
					_CurrentOpinion.OpinionType = HttpContext.Current.Request.Form.GetValue(reqOType, DefaultOpinionType);

					string nextSelectorString = (string)ViewState["NextStepsSelectorsString"];

					if (nextSelectorString.IsNullOrEmpty())
						nextSelectorString = PrepareNextStepsSelectorsString();

					_CurrentOpinion.ExtData["NextSteps"] = SetSelectedInNextStepsSelectorString(nextSelectorString);

					_CurrentOpinion.Result = PrepareOpinionResult();

					if (_ActivateContainer != null)
					{
						OpinionListViewBindEventArgs eventArgs = new OpinionListViewBindEventArgs(_CurrentOpinion, _ActivateDescriptor, _ActivateContainer, false);

						OnCollectCurrentOpinion(eventArgs);
					}

					if (string.IsNullOrEmpty(_CurrentOpinion.Content))
						_CurrentOpinion.Content = WfClientContext.Current.DefaultOpinionText;

					_CurrentOpinion.FillPersonInfo();
				}

				if (_CurrentOpinion != null)
					if (WfClientContext.Current.InCirculateMode && _CurrentOpinion.Content == string.Empty)
						_CurrentOpinion = null;

				return _CurrentOpinion;
			}
		}

		[Bindable(true)]
		[Category("Appearance")]
		[DefaultValue(true)]
		[Localizable(true)]
		public bool EnableUserPresence
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "EnableUserPresence", true);
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "EnableUserPresence", value);
			}
		}

		[DefaultValue(false)]
		[Bindable(true), Category("Appearance")]
		public bool ReadOnly
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "ReadOnly", false);
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "ReadOnly", value);
			}
		}

		[Browsable(true)]
		[Description("是否显示机构人员树选择控件")]
		[DefaultValue(true)]
		public bool ShowTreeSelector
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "ShowTreeSelector", true);
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "ShowTreeSelector", value);
			}
		}
		#endregion Properties

		#region Protected
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			Page.PreRenderComplete += new EventHandler(Page_PreRenderComplete);

			if (this.Page.IsCallback)
				EnsureChildControls();
		}

		protected override void LoadViewState(object savedState)
		{
			_CurrentOpinion = WebControlUtility.GetViewStateValue(ViewState, "CurrentOpinion", (GenericOpinion)null);

			string reqOType = string.Format("{0}${1}${2}", this.ClientID, "OpinionInput", "OpinionType");

			string opinionType = WebUtility.GetRequestFormString(reqOType, string.Empty);

			if (string.IsNullOrEmpty(opinionType) == false)
			{
				OpinionInput.SetOpinionType(opinionType);
			}

			base.LoadViewState(savedState);
		}

		protected override void CreateChildControls()
		{
			if (this.Page.IsCallback)
				_RootPanel = new HtmlGenericControl("div");
			else
			{
				ScriptManager sm = null;
				ScriptControlHelper.EnsureScriptManager(ref sm, this.Page);

				UpdatePanel uPanel = new UpdatePanel();
				uPanel.UpdateMode = UpdatePanelUpdateMode.Conditional;

				_RootPanel = uPanel;
			}

			_RootPanel.ID = "UpdatePanel";

			Controls.Add(_RootPanel);

			_ChangeProcessButton.Style["display"] = "none";
			_ChangeProcessButton.PopupCaption = Translator.Translate(Define.DefaultCulture, "正在保存流程设置...");
			_ChangeProcessButton.ID = "changeProcessButton";

			_ChangeProcessButton.Click += new EventHandler(ChangeProcessButton_Click);
			AddControlToTemplate(_RootPanel, _ChangeProcessButton);

			//RefreshCurrentProcessButton
			_RefreshCurrentProcessButton.Style["display"] = "none";
			_RefreshCurrentProcessButton.PopupCaption = Translator.Translate(Define.DefaultCulture, "正在更新流程...");
			_RefreshCurrentProcessButton.ID = "refreshCurrentProcessButton";
			_RefreshCurrentProcessButton.Attributes["onclick"] = "onRefreshProcessButtonClick();";
			_RefreshCurrentProcessButton.Click += new EventHandler(RefreshCurrentProcessButton_Click);
			AddControlToTemplate(_RootPanel, _RefreshCurrentProcessButton);

			//_RefreshProcessButton
			_RefreshProcessButton.Style["display"] = "none";
			_RefreshProcessButton.PopupCaption = Translator.Translate(Define.DefaultCulture, "正在更新流程...");
			_RefreshProcessButton.ID = "refreshProcessButton";
			_RefreshProcessButton.Attributes["onclick"] = "onRefreshProcessButtonClick();";

			_RefreshProcessButton.Click += new EventHandler(RefreshProcessButton_Click);
			AddControlToTemplate(_RootPanel, _RefreshProcessButton);

			//_ChangeTransitionButton
			_ChangeTransitionButton.Style["display"] = "none";
			_ChangeTransitionButton.PopupCaption = Translator.Translate(Define.DefaultCulture, "正在更新流程...");
			_ChangeTransitionButton.ID = "changeTransitionProcessButton";

			_ChangeTransitionButton.Click += new EventHandler(ChangeTransitionButton_Click);
			AddControlToTemplate(_RootPanel, _ChangeTransitionButton);

			_HiddenData.ID = "descriptorData";
			AddControlToTemplate(_RootPanel, _HiddenData);

			_Table.CellPadding = 4;
			_Table.CellSpacing = 0;
			_Table.ID = "ContainerTable";
			_Table.Attributes["class"] = "opinionListView";

			_OpinionInput.Width = Unit.Percentage(100);
			_OpinionInput.ID = "OpinionInput";
			_OpinionInput.ShowPredefinedOpinions = ShowPredefinedOpinions;

			if (this.Page.IsCallback == false || WebUtility.GetRequestPageRenderMode().UseNewPage == true)
			{
				if (OriginalActivity != null)
				{
					_activityEditor = CreateActivityEditor();

					_activityEditor.ID = "activityEditor";

					_activityEditor.ShowTreeSelector = ShowTreeSelector;

					if (this.GetDataSource != null)
						_activityEditor.GetDataSource += OnGetDataSource;

					if (this.ValidateInputOuUser != null)
						_activityEditor.ValidateInputOuUser += OnValidateInputOuUser;

					Controls.Add(_activityEditor);

					_groupActivityEditor = CreateGroupActivityEditor();
					_groupActivityEditor.ID = "groupActivityEditor";

					Controls.Add(_groupActivityEditor);

					AddControlToTemplate(_RootPanel, _Table);

					CreateProcessSteps(GetAllMainStreamActivities());
				}
			}

			base.CreateChildControls();
		}

		/// <summary>
		/// 触发意见绑定事件
		/// </summary>
		/// <param name="e">意见绑定事件参数</param>
		protected virtual void OnCollectCurrentOpinion(OpinionListViewBindEventArgs e)
		{
			if (CollectCurrentOpinion != null)
				CollectCurrentOpinion(this, e);
		}

		/// <summary>
		/// 触发意见绑定事件
		/// </summary>
		/// <param name="e">意见绑定事件参数</param>
		protected virtual void OnOpinionBind(OpinionListViewBindEventArgs e)
		{
			if (OpinionBind != null)
				OpinionBind(this, e);
		}

		private void OnGetDataSource(string sPrefix, int iCount, object context, ref IEnumerable result)
		{
			if (this.GetDataSource != null)
				this.GetDataSource(sPrefix, iCount, context, ref result);
		}

		protected OguDataCollection<IOguObject> OnValidateInputOuUser(string chkString, object context = null)
		{
			OguDataCollection<IOguObject> result = null;

			if (this.ValidateInputOuUser != null)
				result = ValidateInputOuUser(chkString);
			else
				result = new OguDataCollection<IOguObject>();

			return result;
		}

		/// <summary>
		/// 负责渲染每一个流程活动上，流程编辑按钮
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPreRender(EventArgs e)
		{
			_OpinionInput.ShowPredefinedOpinions = ShowPredefinedOpinions;

			//增加一个空的UserPresence控件
			if (this.Page.IsCallback == false && EnableUserPresence)
				AddControlToTemplate(_RootPanel, new UserPresence());

			_Table.Style["width"] = Width.ToString();
			_Table.Style["height"] = Height.ToString();

			if (_activityEditor != null)
			{
				_activityEditor.ShowCirculateUsers = this.ShowCirculateUsers;
				_activityEditor.ShowTreeSelector = ShowTreeSelector;
			}

			if (OriginalActivity != null)
				RenderActivitiesEditor(GetAllMainStreamActivities());

			if (InputControl != null)
				InputControl.ClientVisible = CanShowOpinionInput();

			base.OnPreRender(e);

			Page.ClientScript.RegisterClientScriptBlock(this.GetType(),
				"OpinionListView",
				ResourceHelper.LoadStringFromResource(Assembly.GetExecutingAssembly(), "MCS.Web.WebControls.Opinion.OpinionListView.js"),
				true);

			Page.ClientScript.RegisterStartupScript(this.GetType(),
				"OpinionListViewStartup",
				"Sys.WebForms.PageRequestManager.getInstance().add_endRequest(adjustProcessDescriptorEndRequestHandler);",
				true);

			RegisterHiddenElementIDs();

			WebControlUtility.SetViewStateValue(ViewState, "CurrentOpinion", _CurrentOpinion);

			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "确认删除该环节吗？");
		}
		#endregion Protected

		#region Private
		private void CreateProcessSteps(WfMainStreamActivityDescriptorCollection msActivities)
		{
			this._opinionPlaceHolders.Clear();
			this._activityEditorPlaceHolders.Clear();
			this._OpinionInput.OpinionContainer.Controls.Clear();

			if (CurrentActivity.OpinionRootActivity.Descriptor.ActivityType == WfActivityType.InitialActivity)
			{
				CreateInitialActivityEditorRow(_Table, CurrentActivity.OpinionRootActivity.Descriptor);
			}

			OpinionListControlHelper.PrepareSignImages(this._Opinions);

			int i = 0;

			foreach (IWfMainStreamActivityDescriptor msActivity in msActivities)
			{
				//渲染每个环节的节点，结束点不渲染
				if (msActivity.Activity.ActivityType != WfActivityType.CompletedActivity)
				{
					HtmlTableRow row = new HtmlTableRow();
					row.ID = "OpinionRow" + i++.ToString();

					_Table.Rows.Add(row);

					HtmlTableCell activityCell = CreateActivityCell(row);

					RenderActivity(activityCell, msActivity);
					RenderLevelOpinionsPlaceHolders(CreateOpinionCell(row), msActivity);

					this._activityEditorPlaceHolders.Add(msActivity.Activity.Key, activityCell);
				}
			}

			ViewState["NextStepsSelectorsString"] = PrepareNextStepsSelectorsString();
		}

		/// <summary>
		/// 是否需要显示意见输入控件
		/// </summary>
		/// <returns></returns>
		private bool CanShowOpinionInput()
		{
			bool result = this.ShowOpinionInput;

			if (result)
			{
				if (WfClientContext.Current.OriginalActivity != null)
				{
					result = CurrentActivity.Descriptor.Properties.GetValue("AllowWriteOpinion", true);
				}
			}

			return result;
		}

		private WfMainStreamActivityDescriptorCollection GetAllMainStreamActivities()
		{
			WfApplicationRuntimeParametersCollector.CollectApplicationData(this.CurrentProcess);

			//沈峥注释，修改为获取流程实例中的主线活动
			WfMainStreamActivityDescriptorCollection msa = this.CurrentProcess.GetMainStreamActivities(true);

			WfMainStreamActivityDescriptorCollection result = new WfMainStreamActivityDescriptorCollection();

			foreach (IWfMainStreamActivityDescriptor ms in msa)
			{
				if (ms.Activity.Properties.GetValue("ShowingInOpinionList", true))
					result.Add(ms);
			}

			return result;
		}

		private WfActivityDescriptorEditorBase CreateActivityEditor()
		{
			return WfControlSettings.GetConfig().CreateActivityDescriptorEditor();
		}

		private WfActivityDescriptorEditorBase CreateGroupActivityEditor()
		{
			return new WfActivityDescriptorGroupResourceEditor();
		}

		private string GetActivityEditorClientID(IWfActivityDescriptor activityDescriptor)
		{
			string result = string.Empty;

			if (this._activityEditor != null)
				result = WfVariableDefine.UseGroupSelector(activityDescriptor) ?
						this._groupActivityEditor.ClientID : this._activityEditor.ClientID;

			return result;
		}

		/// <summary>
		/// 综合判断是否允许意见为空
		/// </summary>
		/// <returns></returns>
		private bool GetAllowEmptyOpinion()
		{
			bool actValue = true;

			if (WfClientContext.Current.InMoveToMode)
				actValue = WfClientContext.Current.CurrentActivity.Descriptor.Properties.GetValue("AllowEmptyOpinion", true);

			return actValue && this.AllowEmptyOpinion;
		}

		private void Page_PreRenderComplete(object sender, EventArgs e)
		{
			RenderOpinionPlaceHolders();

			if (InputControl.Visible &&
				string.IsNullOrEmpty(InputControl.TextInputClientID) == false &&
				WfMoveToControl.Current != null)
			{
				WfMoveToControl.Current.OpinionInputControlID = InputControl.TextInputClientID;
				WfMoveToControl.Current.AllowEmptyOpinion = this.GetAllowEmptyOpinion();
				WfMoveToControl.Current.OpinionTypeClientID = InputControl.OpinionTypeClientID;
				WfMoveToControl.Current.RelativeOpinionListViewID = this.ID;
				WfMoveToControl.Current.ChangeProcessButtonID = this._ChangeProcessButton.ID;

				if (InputControl.ClientVisible)
				{
					if (this.GetAllowEmptyOpinion() == false)
					{
						string script = "pushOpinionValidator({opinionInputID: \"{0}\", errorMessage: \"{1}\"});Sys.Application.add_load(function() { addMoveToValidator(\"{2}\") });";

						script = script.Replace("{0}", InputControl.TextInputClientID);
						script = script.Replace("{1}", WebUtility.CheckScriptString(this.CheckEmptyOpinionText));
						script = script.Replace("{2}", WfMoveToControl.Current.ClientID);

						Page.ClientScript.RegisterStartupScript(this.GetType(),
							"OpinionListView_" + this.ClientID,
							script,
							true);
					}
				}
			}

			WfMoveToControl.Current.RegisterRefreshProcessButton(this.RefreshProcessButtonClientID);
			WfMoveToControl.Current.RegisterRefreshCurrentProcessButton(this.RefreshCurrentProcessButtonClientID);
		}

		private void RegisterHiddenElementIDs()
		{
			StringBuilder strB = new StringBuilder();

			strB.AppendFormat("opinionListViewHiddenDataID = \"{0}\";", this._HiddenData.ClientID);
			strB.Append("\n");
			strB.AppendFormat("opinionListViewChangeProcessButtonID = \"{0}\";", this._ChangeProcessButton.ClientID);

			Page.ClientScript.RegisterStartupScript(this.GetType(),
				"HiddenElementIDs",
				strB.ToString(),
				true);
		}

		private IWfProcess CurrentProcess
		{
			get
			{
				IWfProcess result = null;

				if (string.IsNullOrEmpty(ProcessID) == false)
					result = WfRuntime.GetProcessByProcessID(ProcessID);
				else
				{
					try
					{
						if (OriginalActivity != null)
							result = OriginalActivity.Process.OpinionRootProcess;
						//result = OriginalActivity.Process.ApprovalRootProcess;  //徐磊修改 2012-2-8  传阅时的意见有问题，已修复
					}
					catch (System.Exception)
					{
					}
				}

				return result;
			}
		}

		private IWfActivity CurrentActivity
		{
			get
			{
				IWfActivity result = null;

				if (CurrentProcess != null)
					result = CurrentProcess.CurrentActivity;

				return result;
			}
		}

		private IWfActivity OriginalActivity
		{
			get
			{
				IWfActivity result = null;

				if (string.IsNullOrEmpty(ProcessID))
					result = WfClientContext.Current.OriginalCurrentActivity;
				else
					result = CurrentActivity;

				return result;
			}
		}

		private IWfActivityDescriptor GetInputActivityDescriptorFromGroup(IWfMainStreamActivityDescriptor msActDesp)
		{
			IWfActivityDescriptor result = null;

			if (CanActivityInputOpinion(msActDesp.Activity) == false)
			{
				foreach (IWfActivityDescriptor actDesp in msActDesp.AssociatedActivities)
				{
					if (CanActivityInputOpinion(actDesp))
					{
						result = actDesp;
						break;
					}
				}
			}
			else
				result = msActDesp.Activity;

			return result;
		}

		private bool CanActivityInputOpinion(IWfActivityDescriptor actDesp)
		{
			bool result = false;

			IWfActivity originalActivity = OriginalActivity;
			IWfActivity rootActivity = originalActivity.OpinionRootActivity;

			if (ReadOnly == false && originalActivity != null)
			{
				string levelName = rootActivity.Descriptor.AssociatedActivityKey != null ? rootActivity.Descriptor.AssociatedActivityKey : rootActivity.Descriptor.Key;

				result = WfClientContext.Current.InCirculateMode && string.IsNullOrEmpty(levelName) == false &&
							(levelName == actDesp.Key || levelName == actDesp.AssociatedActivityKey);

				if (!result)
				{
					result = originalActivity.Process.Status == WfProcessStatus.Running &&
							originalActivity.Status == WfActivityStatus.Running &&
							string.IsNullOrEmpty(levelName) == false &&
                            (levelName == actDesp.Key || levelName == actDesp.Instance.MainStreamActivityKey || levelName == actDesp.AssociatedActivityKey);

					if (result)
						result = WfClientContext.Current.InMoveToMode;
				}
			}

			return result;
		}

		private GenericOpinion GetCurrentActivityOpinion()
		{
			GenericOpinion opinion = null;

			if (Opinions != null
				&& OriginalActivity != null)
			{
				opinion = Opinions.Find(op => string.Compare(op.ActivityID, OriginalActivity.ID, true) == 0 &&
										string.Compare(op.IssuePerson.ID, WfClientContext.Current.User.ID, true) == 0);
			}

			return opinion;
		}

		private static void AddControlToTemplate(Control container, Control target)
		{
			if (container is UpdatePanel)
				((UpdatePanel)container).ContentTemplateContainer.Controls.Add(target);
			else
				container.Controls.Add(target);
		}

		private IWfActivity FindCurrentActivity(WfActivityDescriptorCreateParams createParams)
		{
			return CurrentActivity.Process.OpinionRootProcess.Activities.FindActivityByDescriptorKey(createParams.CurrentActivityKey);
		}
		#endregion

		#region Render Relative
		private HtmlTableCell CreateOpinionCell(Control parent)
		{
			HtmlTableCell opinionCell = new HtmlTableCell();
			parent.Controls.Add(opinionCell);

			opinionCell.Attributes["class"] = "opinions";
			opinionCell.Controls.Add(new HtmlGenericControl("span"));

			return opinionCell;
		}

		private static string GetUserDepartmentName(IUser user)
		{
			string deptName = string.Empty;

			try
			{
				if (user.Parent != null)
					deptName = user.Parent.DisplayName;
			}
			catch (System.Exception)
			{
				deptName = GetDeptNameFromFullPath(user.FullPath);
			}

			return deptName;
		}

		private static string GetDeptNameFromFullPath(string fullPath)
		{
			string[] parts = fullPath.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);

			string result = string.Empty;

			if (parts.Length > 1)
				result = parts[parts.Length - 2];

			return result;
		}

		private void RenderLevelOpinionsPlaceHolders(HtmlTableCell opinionCell, IWfMainStreamActivityDescriptor msActDesp)
		{
			IWfActivityDescriptor nextStepSelectorActDesp = WfClientContext.Current.OriginalCurrentActivity.Descriptor;

			RenderLevelOpinionsPlaceHolders(
				opinionCell,
				msActDesp.Activity,
				GetInputActivityDescriptorFromGroup(msActDesp),
				nextStepSelectorActDesp);
		}

		private string GetKeyFromActivity(IWfActivityDescriptor actDesp)
		{
			return actDesp.AssociatedActivityKey == null ? actDesp.Key : actDesp.AssociatedActivityKey;
		}

		private bool IsConsignActivity(IWfActivityDescriptor actDesp)
		{
			return actDesp.AssociatedActivityKey != null && actDesp.ClonedKey == null;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="opinionCell">表格单元格</param>
		/// <param name="msActDesp"></param>
		/// <param name="inputActDesp">需要输入的活动描述</param>
		/// <param name="nextStepSelectorActDesp"></param>
		private void RenderLevelOpinionsPlaceHolders(HtmlTableCell opinionCell, IWfActivityDescriptor msActDesp, IWfActivityDescriptor inputActDesp, IWfActivityDescriptor nextStepSelectorActDesp)
		{
			HtmlGenericControl opinionContainer = new HtmlGenericControl("div");
			opinionCell.Controls.Add(opinionContainer);

			this._opinionPlaceHolders.Add(msActDesp, opinionContainer);

			PageRenderMode pageMode = WebUtility.GetRequestPageRenderMode();

			bool flag = inputActDesp != null || (pageMode.UseNewPage == true && pageMode.RenderControlUniqueID.IndexOf("PredefinedOpinionDialog") >= 0);
			//如果当前环节可编辑，则输出意见输入控件
			if (flag)
			{
				GenericOpinion currentOpinion = GetCurrentActivityOpinion();

				OnOpinionBind(new OpinionListViewBindEventArgs(currentOpinion, inputActDesp, opinionCell, false));

				HtmlGenericControl inputContainer = new HtmlGenericControl("div");

				inputContainer.ID = "OpinionInputContainer";
				opinionCell.Controls.Add(inputContainer);

				inputContainer.Attributes["class"] = "opinionInputCell";
				inputContainer.Controls.Add(_OpinionInput);

				this._OpinionInput.OpinionContainer.Controls.Add(this.CreateNextStepSelectorControl(nextStepSelectorActDesp, flag));

				_OpinionInput.Opinion = currentOpinion;

				if (_OpinionInput.Opinion.Content.IsNullOrEmpty())
					_OpinionInput.Opinion.Content = GetActivityEmptyOpinionText(inputActDesp);

				_ActivateDescriptor = inputActDesp;
				_ActivateContainer = opinionCell;
			}
		}

		private WfNextStepSelectorControl CreateNextStepSelectorControl(IWfActivityDescriptor nextStepSelectorActDesp, bool enabledFlag)
		{
			WfNextStepSelectorControl nextStepSelector = new WfNextStepSelectorControl();

			nextStepSelector.ActivityDescriptor = nextStepSelectorActDesp;

			nextStepSelector.ID = nextStepSelector.ActivityDescriptor.Key;

			nextStepSelector.Enabled = (this.ReadOnly == false && enabledFlag);

			if (nextStepSelector.Enabled)
			{
				nextStepSelector.OnChangeClientScript = string.Format("$get('{0}').click();", this._ChangeTransitionButton.ClientID);
				this._EnabledNextStepSelectorControl = nextStepSelector;
			}

			return nextStepSelector;
		}

		private void RenderOpinionPlaceHolders()
		{
			if (Opinions != null && Opinions.Count > 0)
			{
				foreach (KeyValuePair<IWfActivityDescriptor, Control> kp in this._opinionPlaceHolders)
				{
					IWfActivityDescriptor levelActDesp = kp.Key;

					GenericOpinionCollection collectedOpinions = new GenericOpinionCollection();

					string clonedKey = levelActDesp.ClonedKey;
					while (clonedKey.IsNotEmpty())
					{
						foreach (var op in Opinions.GetOpinions(clonedKey))
						{
							collectedOpinions.Add(op);
						}

						var actDescriptor = this.CurrentProcess.Descriptor.Activities[clonedKey];
						clonedKey = actDescriptor.ClonedKey;
					}

					string levelName = levelActDesp.Instance.MainStreamActivityKey;

					if (levelName.IsNullOrEmpty())
						levelName = levelActDesp.Key;

					if (levelName.IsNotEmpty())
					{
						foreach (var op in Opinions.GetOpinions(levelName))
							collectedOpinions.Add(op);
					}

					if (collectedOpinions != null)
					{
						RenderOpinions(kp.Value, collectedOpinions, levelActDesp);
					}
				}
			}
		}

		private void RenderOpinions(Control opinionContainer, GenericOpinionCollection opinions, IWfActivityDescriptor actDesp)
		{
			bool addContentToCell = false;
			int i = 0;
			foreach (GenericOpinion opinion in opinions)
			{
				if (ReadOnly ||
					OriginalActivity == null ||
					(opinion.ActivityID != OriginalActivity.ID ||
					opinion.IssuePerson.ID != WfClientContext.Current.User.ID) ||
					OriginalActivity.Status != WfActivityStatus.Running)
				{
					object nextStepsString = string.Empty;

					if (opinion.ExtData.TryGetValue("NextSteps", out nextStepsString))
					{
						RenderOriginalOpinionSelector(opinionContainer, (string)nextStepsString);
					}

					HtmlGenericControl opDiv = new HtmlGenericControl("div");
					opDiv.Attributes["class"] = "opinion";
					opDiv.Style["padding"] = "0px";

					OpinionListViewNamingContainer container = new OpinionListViewNamingContainer();
					opDiv.Controls.Add(container);
					container.ID = "Opinion" + opinion.ID;

					if (i > 0)
						opDiv.Style["border-top"] = "1px dotted silver";

					IWfActivity act = GetWfActivityByActivityID(opinion.ActivityID);

					if (act != null)
					{
						OnOpinionBind(new OpinionListViewBindEventArgs(opinion, actDesp, container, true));

						string opText = GetActivityEmptyOpinionText(actDesp);

						if (string.IsNullOrEmpty(opinion.Content) == false)
							opText = opinion.Content;

						HtmlGenericControl div = new HtmlGenericControl("div");
						div.Style["padding"] = "6px 8px";

						if (EnableUserPresence == false)
						{
							RenderOneOpinionWithoutPrecense(act, opinion, opText, div);
						}
						else
						{
							RenderOneOpinionWithPrecense(act, opinion, opText, div);
						}

						opDiv.Controls.Add(div);
						opinionContainer.Controls.Add(opDiv);
						addContentToCell = true;
					}

					i++;
				}
			}

			if (addContentToCell)
			{
				HtmlGenericControl div = new HtmlGenericControl("div");
				div.Attributes["class"] = "opinion";
				div.Style["padding"] = "0";
				opinionContainer.Controls.Add(div);
			}
		}

		private string GetActivityEmptyOpinionText(IWfActivityDescriptor actDesp)
		{
			string opText = actDesp != null ? actDesp.Properties.GetValue("EmptyOpinionText", string.Empty) : string.Empty;

			if (string.IsNullOrEmpty(opText))
				opText = this.EmptyOpinionText;

			return opText;
		}

		private void CreateUserPrefixElement(IWfActivity activity, Control container)
		{
			Control control = null;

			if (activity.Descriptor.Variables.GetValue("IsReturn", false))
				control = CreateUserPrefixLogo(Page.ClientScript.GetWebResourceUrl(typeof(OpinionListView),
						"MCS.Web.WebControls.Opinion.returnLogo.gif"), Translator.Translate(Define.DefaultCulture, activity.Descriptor.Name));
			else
				if (WfVariableDefine.IsAddAppoverActivity(activity.Descriptor))
					control = CreateUserPrefixLogo(Page.ClientScript.GetWebResourceUrl(typeof(OpinionListView),
						"MCS.Web.WebControls.Opinion.addApproverLogo.gif"), string.Empty);

			if (control != null)
				container.Controls.Add(control);
		}

		private static Control CreateUserPrefixLogo(string imgUrl, string altText)
		{
			HtmlImage img = new HtmlImage();

			img.Src = imgUrl;
			img.Style["vertical-align"] = "middle";
			img.Alt = Translator.Translate(Define.DefaultCulture, altText);
			img.Style["width"] = "20px";
			img.Style["height"] = "20px";
			img.Style["margin"] = "2px";

			return img;
		}

		private void RenderOneOpinionWithPrecense(IWfActivity activity, GenericOpinion opinion, string opText, Control container)
		{
			HtmlGenericControl opTextContainer = new HtmlGenericControl("div");

			opTextContainer.Attributes["class"] = "text";
			opTextContainer.InnerHtml = HttpUtility.HtmlEncode(opText).Replace("\r\n", "<br/>");
			container.Controls.Add(opTextContainer);

			HtmlGenericControl signName = new HtmlGenericControl("div");
			signName.Attributes["class"] = "signName";
			container.Controls.Add(signName);

			CreateUserPrefixElement(activity, signName);

			UserPresence presence = new UserPresence();

			if (OpinionListControlHelper.UserSignatures.ContainsKey(opinion.IssuePerson.ID))
			{
				HtmlImage sigImage = new HtmlImage();

				sigImage.Src = OpinionListControlHelper.UserSignatures[opinion.IssuePerson.ID];
				sigImage.Alt = opinion.IssuePerson.DisplayName;

				signName.Controls.Add(sigImage);

				presence.ShowUserDisplayName = false;
			}

			signName.Controls.Add(presence);

			presence.UserID = opinion.IssuePerson.ID;
			presence.UserDisplayName = opinion.IssuePerson.DisplayName;	//防止人员离职
			presence.EnsureInUserList();

			if (opinion.IssuePerson.ID != opinion.AppendPerson.ID)
			{
				HtmlGenericControl sp1 = new HtmlGenericControl("span");
				sp1.InnerText = "(";
				signName.Controls.Add(sp1);

				UserPresence presence2 = new UserPresence();
				signName.Controls.Add(presence2);
				presence2.UserID = opinion.AppendPerson.ID;
				presence2.UserDisplayName = opinion.AppendPerson.DisplayName;	//防止人员离职
				presence2.EnsureInUserList();

				HtmlGenericControl sp2 = new HtmlGenericControl("span");
				sp2.InnerText = string.Format(" {0})", Translator.Translate(Define.DefaultCulture, "代写"));
				signName.Controls.Add(sp2);
			}

			HtmlGenericControl dateContainer = new HtmlGenericControl("div");

			dateContainer.Attributes["class"] = "signDate";
			dateContainer.InnerText = opinion.AppendDatetime.ToString("yyyy-MM-dd HH:mm:ss");
			container.Controls.Add(dateContainer);
		}

		private void RenderOneOpinionWithoutPrecense(IWfActivity activity, GenericOpinion opinion, string opText, Control container)
		{
			string signature = HttpUtility.HtmlEncode(opinion.IssuePerson.DisplayName);

			if (opinion.IssuePerson.ID != opinion.AppendPerson.ID)
			{
				signature = string.Format("{0}({1} {2})",
					HttpUtility.HtmlEncode(opinion.IssuePerson.DisplayName),
					HttpUtility.HtmlEncode(opinion.AppendPerson.DisplayName),
					Translator.Translate(Define.DefaultCulture, "代写"));
			}

			if (OpinionListControlHelper.UserSignatures.ContainsKey(opinion.IssuePerson.ID))
			{
				signature = string.Format("<img src=\"{0}\" alt=\"{1}\" title=\"{1}\" />",
					OpinionListControlHelper.UserSignatures[opinion.IssuePerson.ID],
					signature);
			}

			HtmlGenericControl divOpinion = new HtmlGenericControl("div");
			divOpinion.Attributes["class"] = "text";
			divOpinion.InnerHtml = HttpUtility.HtmlEncode(opText).Replace("\r\n", "<br/>");

			container.Controls.Add(divOpinion);

			HtmlGenericControl divSignName = new HtmlGenericControl("div");
			divSignName.Attributes["class"] = "signName";

			CreateUserPrefixElement(activity, divSignName);

			divSignName.Controls.Add(new LiteralControl(signature));

			container.Controls.Add(divSignName);

			HtmlGenericControl divSignDate = new HtmlGenericControl("div");
			divSignDate.Attributes["class"] = "signDate";
			divSignDate.InnerText = opinion.AppendDatetime.ToString("yyyy-MM-dd HH:mm:ss");

			container.Controls.Add(divSignDate);
		}

		/// <summary>
		/// 是否可以编辑环节的，根据AllowModifyActivities变量的值
		/// </summary>
		/// <param name="actDesp"></param>
		/// <returns></returns>
		private bool CanEditActivityDescriptor(IWfActivityDescriptor actDesp)
		{
			bool result = false;

			if (CurrentActivity != null)
			{
				/*
				List<IWfActivityDescriptor> activities = actDesp.Process.FindActivitiesByKeys(
					CurrentActivity.Descriptor.Variables.GetValue("AllowModifyActivities", string.Empty).Split(',', ';'));

				result = activities.Exists(a => a.Key == actDesp.Key);
				 */
			}

			return result;
		}

		private static string PrepareNextStepsSelectorsString()
		{
			XElement nextStepsRoot = new XElement("NextSteps");

			WfControlNextStepCollection nextSteps = WfNextStepSelectorControl.GetNextSteps(WfClientContext.Current.OriginalCurrentActivity.Descriptor);

			nextSteps.ToXElement(nextStepsRoot);

			return nextStepsRoot.ToString();
		}

		private static string SetSelectedInNextStepsSelectorString(string nextStepSelectorString)
		{
			XElement nextStepsRoot = XElement.Parse(nextStepSelectorString);

			if (WfMoveToControl.Current != null)
			{
				if (WfMoveToControl.Current.MoveToSelectedResult != null)
				{
					IWfTransitionDescriptor fromTransition = WfMoveToControl.Current.MoveToSelectedResult.FromTransitionDescriptor;

					if (fromTransition != null)
						nextStepsRoot.SetAttributeValue("selected", fromTransition.Key);
				}
			}

			return nextStepsRoot.ToString();
		}

		private static ApprovalResult PrepareOpinionResult()
		{
			ApprovalResult result = ApprovalResult.Disagree;

			if (WfMoveToControl.Current != null)
			{
				if (WfMoveToControl.Current.MoveToSelectedResult != null)
				{
					IWfTransitionDescriptor fromTransition = WfMoveToControl.Current.MoveToSelectedResult.FromTransitionDescriptor;

					if (fromTransition != null)
					{
						if (fromTransition.AffectedProcessReturnValue)
							result = ApprovalResult.Agree;
					}
				}
			}

			return result;
		}

		internal static void RenderOriginalOpinionSelector(Control container, string nextStepsString)
		{
			if (nextStepsString.IsNotEmpty())
			{
				XElement nextStepsNode = XElement.Parse(nextStepsString);

				if (nextStepsNode.Elements().Count() > 1)
				{
					foreach (XElement stepNode in nextStepsNode.Elements())
					{
						string tranRadioValue = stepNode.AttributeValue("transitionDescription").IsNotEmpty() ? stepNode.AttributeValue("transitionDescription") : stepNode.AttributeValue("transitionName");
						string actRadioValue = stepNode.AttributeValue("activityDescription").IsNotEmpty() ? stepNode.AttributeValue("transitionDescription") : stepNode.AttributeValue("activityName");

						string html = string.Format("<input type=\"radio\" onclick=\"return false;\" disabled {0} \"/><label style=\"cursor: hand;font-family:'宋体',Arial,Helvetica,sans-serif;margin-right:4px;font-size: 12px;font-weight:bold;\">{1}</label>",
							stepNode.AttributeValue("transitionKey") == nextStepsNode.AttributeValue("selected") ? "checked" : string.Empty,
							HttpUtility.HtmlEncode(tranRadioValue.IsNotEmpty() ? tranRadioValue : actRadioValue)
						);

						container.Controls.Add(new LiteralControl(html));
					}
				}
			}
		}
		#endregion

		#region
		public event EventHandler<RenderOneActivityEventArgs> RenderOneActivityEvent;

		private void OnRenderOneActivity(IWfActivity currentActivity, IWfActivityDescriptor activityDesctiptor, RenderOneActivityEventArgs eventArgs)
		{
			if (RenderOneActivityEvent != null)
				RenderOneActivityEvent(this, eventArgs);
		}

		private static IWfActivity GetWfActivityByActivityID(string activityID)
		{
			IWfActivity act = null;

			try
			{
				act = WfRuntime.GetProcessByActivityID(activityID).Activities[activityID];
			}
			catch (System.Exception)
			{
			}

			return act;
		}
		#endregion
	}

	class OpinionListViewNamingContainer : Control, INamingContainer
	{
	}
}
