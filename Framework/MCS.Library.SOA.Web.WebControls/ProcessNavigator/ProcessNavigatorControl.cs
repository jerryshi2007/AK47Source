using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library;
using MCS.Web.Library.MVC;
using MCS.Web.Library.Resources;
using MCS.Web.Library.Script;

[assembly: WebResource("MCS.Web.WebControls.ProcessNavigator.ProcessNavigator.js", "application/x-javascript")]
[assembly: WebResource("MCS.Web.WebControls.ProcessNavigator.ProcessNavigator.css", "text/css", PerformSubstitution = true)]
[assembly: WebResource("MCS.Web.WebControls.ProcessNavigator.ProcessNavigator_No_Width.css", "text/css", PerformSubstitution = true)]
[assembly: WebResource("MCS.Web.WebControls.ProcessNavigator.line.gif", "image/gif")]
[assembly: WebResource("MCS.Web.WebControls.ProcessNavigator.arrow.gif", "image/gif")]
[assembly: WebResource("MCS.Web.WebControls.ProcessNavigator.btnArrow.png", "image/png")]
[assembly: WebResource("MCS.Web.WebControls.ProcessNavigator.btnArrow.gif", "image/gif")]

namespace MCS.Web.WebControls
{
	[RequiredScript(typeof(HBCommonScript), 2)]
	[RequiredScript(typeof(AnimationsScript), 1)]
	[ClientScriptResource("MCS.Web.WebControls.ProcessNavigator", "MCS.Web.WebControls.ProcessNavigator.ProcessNavigator.js")]
	public class ProcessNavigator : ScriptControlBase
	{
		private class ActivitiesWithOffset
		{
			public IEnumerable<IWfMainStreamActivityDescriptor> MainActivities { get; set; }
			public string CurrentActivityKey { get; set; }
			public string OwerActivityId { get; set; }
			public string ProcessId { get; set; }
			public string AssociatedActivityId { get; set; }
		}

		HtmlGenericControl processDivContainer = new HtmlGenericControl("div");
		HtmlGenericControl processUlContainer = new HtmlGenericControl("div");
		HtmlGenericControl aLeft = new HtmlGenericControl("a");
		HtmlGenericControl aRight = new HtmlGenericControl("a");
		HtmlGenericControl btnDiv = new HtmlGenericControl("div");
		HtmlGenericControl mainContainer = new HtmlGenericControl("table");

		public string ProcessID
		{
			get
			{
				return this.ViewState.GetViewStateValue("ProcessId", string.Empty);
			}
			set
			{
				this.ViewState.SetViewStateValue("ProcessId", value);
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

		protected IWfProcess CurrentProcess
		{
			get
			{
				IWfProcess result = null;

				if (string.IsNullOrEmpty(ProcessID) == false)
				{
					result = WfRuntime.GetProcessByProcessID(ProcessID);
				}
				else
				{
					try
					{
						if (WfClientContext.Current.OriginalActivity != null)
							result = WfClientContext.Current.OriginalActivity.Process;
					}
					catch (System.Exception)
					{
					}
				}

				return result;
			}
		}

		public ProcessNavigator()
			: base(HtmlTextWriterTag.Div)
		{ }

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			if (WfMoveToControl.Current != null)
				WfMoveToControl.Current.AfterProcessChanged += new ProcessChangedEventHandler(AfterCurrent_ProcessChanged);
		}

		private void AfterCurrent_ProcessChanged(IWfProcess process)
		{
			Table tbl = GetProcessNavTableContainer();
			this.Controls.Add(tbl);

			StringBuilder strB = new StringBuilder();

			using (StringWriter sw = new StringWriter(strB))
			{
				HtmlTextWriter newWriter = new HtmlTextWriter(sw, "\t");

				RenderContents(newWriter);
			}

			string script = string.Format("$find(\"{0}\")._refreshProcess(\"{1}\");  ",
				this.ClientID, WebUtility.CheckScriptString(strB.ToString(), false));

			ScriptManager.RegisterStartupScript(this.Page, this.GetType(), this.ID,
				script,
				true);
		}

		protected override void Render(HtmlTextWriter writer)
		{
			if (this.DesignMode)
			{
				writer.Write("Process Navigator Control");
			}
			else
			{
				base.Render(writer);
			}
		}

		private TableCell GetLeftCell()
		{
			string btnArrowImgPath = Page.ClientScript.GetWebResourceUrl(typeof(ProcessNavigator), "MCS.Web.WebControls.ProcessNavigator.btnArrow.gif");

			TableCell cell = new TableCell();
			cell.Attributes["class"] = "btnArrowContainer";

			aLeft.Attributes["style"] = string.Format("background-image: url(\"{0}\"); no-repeat 0px 0px", btnArrowImgPath);
			aLeft.Attributes["class"] = "arrow back";
			aLeft.ID = "btnLeft";
			cell.Controls.Add(aLeft);

			return cell;
		}

		private TableCell GetRightCell()
		{
			string btnArrowImgPath = Page.ClientScript.GetWebResourceUrl(typeof(ProcessNavigator), "MCS.Web.WebControls.ProcessNavigator.btnArrow.gif");

			TableCell cell = new TableCell();

			cell.Attributes["class"] = "btnArrowContainer";
			aRight.Attributes["style"] = string.Format("background-image: url(\"{0}\");", btnArrowImgPath);
			aRight.Attributes["class"] = "arrow forward";
			aRight.ID = "btnRight";
			cell.Controls.Add(aRight);

			return cell;
		}

		private HtmlGenericControl GetProcessNavigatorHtml()
		{
			processDivContainer.ID = "processDivContainer";
			processUlContainer.ID = "processUlContainer";
			processDivContainer.Attributes["class"] = "process_div_container1";

			processUlContainer.Attributes["class"] = "div_nav";

			List<ActivitiesWithOffset> actIds = GetProcessHierachy(this.ProcessID);

			for (int i = actIds.Count - 1; i >= 0; i--)
			{
				if (i == 0)
				{
					processUlContainer.Controls.Add(GetProcessNavigatorUl(actIds[i], true));
				}
				else
				{
					processUlContainer.Controls.Add(GetProcessNavigatorUl(actIds[i], false));
				}
			}

			processDivContainer.Controls.Add(processUlContainer);

			return processDivContainer;
		}

		private HtmlGenericControl GetProcessNavigatorUl(ActivitiesWithOffset actWithOffset, bool isLastProcess)
		{
			HtmlGenericControl processUl = new HtmlGenericControl("ul");
			processUl.Attributes["ownerActivityId"] = actWithOffset.OwerActivityId;

			if (actWithOffset.OwerActivityId != string.Empty)
			{
				HtmlGenericControl connectorLi = new HtmlGenericControl("li");
				HtmlImage connector = new HtmlImage();

				connector.Src = Page.ClientScript.GetWebResourceUrl(typeof(ProcessNavigator),
					"MCS.Web.WebControls.ProcessNavigator.line.gif");
				connectorLi.Controls.Add(connector);
				processUl.Controls.Add(connectorLi);

				processUl.Attributes["associatedOwnerActivityId"] =
					GetAssociatedActivity(actWithOffset.OwerActivityId).Descriptor.GetAssociatedActivity().ProcessInstance.Activities.FindActivityByDescriptorKey(GetAssociatedActivity(actWithOffset.OwerActivityId).Descriptor.GetAssociatedActivity().Key).ID;
			}

			string currentKey = string.Empty;

			foreach (HtmlGenericControl item in GetProcessActivitiesLi(actWithOffset.MainActivities, actWithOffset.CurrentActivityKey, isLastProcess))
			{
				processUl.Controls.Add(item);
			}

			return processUl;
		}

		private List<HtmlGenericControl> GetProcessActivitiesLi(IEnumerable<IWfMainStreamActivityDescriptor> actDescs, string currActKey, bool islastProcess)
		{
			List<HtmlGenericControl> processActsLi = new List<HtmlGenericControl>();
			int i = 0;

			foreach (var act in actDescs)
			{
				HtmlGenericControl activityLi = new HtmlGenericControl("li");
				HtmlGenericControl activityNameSpan = new HtmlGenericControl("span");

				activityNameSpan.InnerText = string.Empty;
				activityNameSpan.Style["vertical-align"] = "middle";

				IWfActivity wfActivity = act.Activity.ProcessInstance.Activities.FindActivityByDescriptorKey(act.Activity.Key);

				activityLi.Attributes["activityId"] = wfActivity.ID;

				if (!(act.Activity.ActivityType == WfActivityType.CompletedActivity))
				{
					Control activityDisplayControl = GetActivityDisplayControl(act);

					if (activityDisplayControl != null)
						activityNameSpan.Controls.Add(activityDisplayControl);

					activityNameSpan.Attributes["title"] = GetActivityTooltipText(act);
					if (act.Activity.Key == currActKey || act.Activity.AssociatedActivityKey == currActKey)
					{
						activityNameSpan.Attributes["class"] = "currActivity";
					}
					else
					{
						string actkey = act.Activity.Key;
						string trueKey = act.Activity.Key;

						if (act.Activity.GetAssociatedActivity() != null)
							actkey = act.Activity.GetAssociatedActivity().Key;

						if (act.Activity.ProcessInstance.Activities.FindActivityByDescriptorKey(trueKey).Status == WfActivityStatus.Completed
							|| act.Activity.ProcessInstance.Activities.FindActivityByDescriptorKey(actkey).Status == WfActivityStatus.Completed)
						{
							activityNameSpan.Style["background-color"] = "yellow";
						}

						activityNameSpan.Attributes["class"] = "normalAvtivity";
					}

					activityLi.Controls.Add(activityNameSpan);
					HtmlGenericControl imgLi = new HtmlGenericControl("li");
					HtmlImage img = new HtmlImage();
					img.Src = Page.ClientScript.GetWebResourceUrl(typeof(ProcessNavigator), "MCS.Web.WebControls.ProcessNavigator.arrow.gif");
					activityLi.Controls.Add(img);
				}
				else
				{
					activityNameSpan.InnerText = act.Activity.Name;
					HtmlGenericControl imgLi = new HtmlGenericControl("li");
					HtmlGenericControl imgSpan = new HtmlGenericControl("span");
					imgSpan.Attributes["class"] = "normalAvtivity";
					HtmlImage img = new HtmlImage();
					img.Style["vertical-align "] = "middle";
					img.Src = ControlResources.CompletedActivityLogoUrl;
					img.Alt = "结束";
					activityLi.Controls.Add(imgSpan);
					imgSpan.Controls.Add(img);
				}
				processActsLi.Add(activityLi);
				i++;
			}

			return processActsLi;
		}

		private string GetActivityTooltipText(IWfMainStreamActivityDescriptor mainStreamActivity)
		{
			IWfActivity wfActivity = mainStreamActivity.Activity.ProcessInstance.Activities.
				FindActivityByDescriptorKey(mainStreamActivity.Activity.Key);

			return GetActivityPeopleNameDisplayText(wfActivity, mainStreamActivity);
		}

		private HtmlGenericControl GetButtonContainer()
		{
			btnDiv = new HtmlGenericControl("div");
			btnDiv.Attributes["class"] = "buttonDiv";
			btnDiv.Style["height"] = "50px";

			aLeft = new HtmlGenericControl("a");
			btnDiv.ID = "btn_div";

			string btnArrowImgPath = Page.ClientScript.GetWebResourceUrl(typeof(ProcessNavigator), "MCS.Web.WebControls.ProcessNavigator.btnArrow.gif");

			aLeft.Attributes["style"] = string.Format("background-image: url(\"{0}\"); no-repeat 0px 0px", btnArrowImgPath);
			aLeft.Attributes["class"] = "arrow back";
			aLeft.ID = "btnLeft";
			aRight = new HtmlGenericControl("a");
			aRight.Attributes["style"] = string.Format("background-image: url(\"{0}\");", btnArrowImgPath);
			aRight.Attributes["class"] = "arrow forward";
			aRight.ID = "btnRight";

			btnDiv.Controls.Add(aLeft);
			btnDiv.Controls.Add(aRight);

			return btnDiv;
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			WebUtility.RequiredScript(typeof(AnimationsScript));
			RegisterCssExpressionMethod();

			RegisterCSS();
			Controls.Clear();

			if (CurrentProcess != null && this.Page.IsPostBack == false)
			{
				this.Attributes["class"] = "mainContainer";
				Table tbl = GetProcessNavTableContainer();

				this.Controls.Add(tbl);
			}
		}

		private Table GetProcessNavTableContainer()
		{
			Table contentTable = new Table();
			contentTable.Attributes.Add("style", "table-layout:fixed");
			TableRow row = new TableRow();
			TableCell cell = new TableCell();
			contentTable.Style["width"] = "100%";
			contentTable.Style["height"] = "100%";
			cell.Controls.Add(GetProcessNavigatorHtml());
			row.Controls.Add(GetLeftCell());
			row.Controls.Add(cell);
			row.Controls.Add(GetRightCell());
			contentTable.Controls.Add(row);

			return contentTable;
		}

		private string GetOwerActivityID(IWfProcess proc)
		{
			string result = string.Empty;

			//得到父流程的主线活动
			WfMainStreamActivityDescriptorCollection mainActs = GetMainStreamActivitiesByProcess(proc.EntryInfo.OwnerActivity.Process);

			IWfActivityDescriptor ownerActDesp = proc.EntryInfo.OwnerActivity.Descriptor.GetAssociatedActivity();

			string ownerDespKey = ownerActDesp.Key;

			var actDesp = mainActs.Find(act => act.Activity.Key == ownerDespKey || act.Activity.AssociatedActivityKey == ownerDespKey);

			if (actDesp != null)
				result = actDesp.Activity.Instance.ID;

			return result;
		}

		private List<ActivitiesWithOffset> GetProcessHierachy(string processID)
		{
			List<ActivitiesWithOffset> acts = new List<ActivitiesWithOffset>();

			if (CurrentProcess != null)
			{
				IWfProcess proc = CurrentProcess;

				string owerActID = string.Empty;

				//如果当前流程是子流程
				if (proc.EntryInfo != null)
					owerActID = GetOwerActivityID(proc);

				WfApplicationRuntimeParametersCollector.CollectApplicationData(proc);

				//添加当前流程的主线活动
				acts.Add(
					new ActivitiesWithOffset()
					{
						MainActivities = GetMainStreamActivitiesByProcess(proc),
						CurrentActivityKey = GetCurrentActivityKey(proc),
						OwerActivityId = owerActID,
						ProcessId = proc.ID
					});

				//添加父流程的主线活动
				while (owerActID != string.Empty)
				{
					IWfProcess parentProcess = proc.EntryInfo.OwnerActivity.Process;

					ActivitiesWithOffset actWithOffset = new ActivitiesWithOffset();
					actWithOffset.ProcessId = parentProcess.ID;
					actWithOffset.CurrentActivityKey = GetCurrentActivityKey(parentProcess);

					owerActID = parentProcess.EntryInfo != null ? GetOwerActivityID(parentProcess) : string.Empty;
					actWithOffset.OwerActivityId = owerActID;

					WfApplicationRuntimeParametersCollector.CollectApplicationData(parentProcess);
					actWithOffset.MainActivities = GetMainStreamActivitiesByProcess(parentProcess);
					acts.Add(actWithOffset);

					proc = parentProcess;
				}
			}

			return acts;
		}

		private string GetCurrentActivityKey(IWfProcess proc)
		{
			if (string.IsNullOrEmpty(proc.CurrentActivity.Descriptor.AssociatedActivityKey))
				return proc.CurrentActivity.Descriptor.Key;
			else
				return proc.CurrentActivity.Descriptor.AssociatedActivityKey;
		}

		private IWfActivity GetAssociatedActivity(string ownerActivityId)
		{
			IWfProcess proc1 = WfRuntime.GetProcessByActivityID(ownerActivityId);

			return proc1.Activities[ownerActivityId];
		}

		private Control GetActivityDisplayControl(IWfMainStreamActivityDescriptor mainStreamActivity)
		{
			Control result = null;

			IWfActivity wfActivity = mainStreamActivity.Activity.ProcessInstance.Activities.
				FindActivityByDescriptorKey(mainStreamActivity.Activity.Key);

			if (NeedToShowActivityName(mainStreamActivity.Activity))
				result = new LiteralControl(HttpUtility.HtmlEncode(mainStreamActivity.Activity.Name));
			else
				result = GetActivityPeopleNameDisplayControl(wfActivity, mainStreamActivity);

			return result;
		}

		private static bool NeedToShowActivityName(IWfActivityDescriptor actDesp)
		{
			bool result = false;

			switch (actDesp.NavigatorDisplayMode)
			{
				case WfNavigatorDisplayMode.DependsOnProcess:
					result = actDesp.Process.ProcessType != WfProcessType.Approval;
					break;
				case WfNavigatorDisplayMode.ShowActivityName:
					result = true;
					break;
				case WfNavigatorDisplayMode.ShowCandidates:
					result = false;
					break;
			}

			return result;
		}

		private Control GetActivityPeopleNameDisplayControl(IWfActivity wfActivity, IWfMainStreamActivityDescriptor mainStreamActivity)
		{
			Control userNames = null;

			if (wfActivity.Operator != null && wfActivity.Status != WfActivityStatus.NotRunning)
			{
				userNames = GetUserNameControl(wfActivity.Operator);
			}
			else
				if (wfActivity.Assignees.Count != 0)
				{
					userNames = GetCandidatesOrAssigneesControl(wfActivity.Assignees, mainStreamActivity.Activity.Name);
				}
				else
				{
					WfAssigneeCollection candidates = wfActivity.Candidates.GetSelectedAssignees();

					if (candidates.Count != 0)
						userNames = GetCandidatesOrAssigneesControl(candidates, mainStreamActivity.Activity.Name);
					else
						userNames = new HtmlGenericControl("span") { InnerText = string.Empty };
				}

			return userNames;
		}

		private string GetActivityPeopleNameDisplayText(IWfActivity wfActivity, IWfMainStreamActivityDescriptor mainStreamActivity)
		{
			string result = string.Empty;

			if (wfActivity.Operator != null && wfActivity.Status != WfActivityStatus.NotRunning)
			{
				result = wfActivity.Operator.DisplayName;
			}
			else
				if (wfActivity.Assignees.Count != 0)
				{
					result = GetCandidatesOrAssigneesText(wfActivity.Assignees, mainStreamActivity.Activity.Name);
				}
				else
				{
					WfAssigneeCollection candidates = wfActivity.Candidates.GetSelectedAssignees();

					if (candidates.Count != 0)
						result = GetCandidatesOrAssigneesText(candidates, mainStreamActivity.Activity.Name);
				}
			return result;
		}

		private Control GetUserNameControl(IUser user)
		{
			HtmlGenericControl userName = new HtmlGenericControl("span");

			if (user != null)
			{
				if (EnableUserPresence)
				{
					UserPresence presence = new UserPresence();

					presence.UserID = user.ID;
					presence.UserDisplayName = user.DisplayName;

					userName.Controls.Add(presence);

					presence.EnsureInUserList();
				}
				else
					userName.InnerText = user.DisplayName;
			}

			return userName;
		}

		private Control GetCandidatesOrAssigneesControl(WfAssigneeCollection assignees, string defaultValue)
		{
			HtmlGenericControl userNames = new HtmlGenericControl("span");
			string splitter = ",";

			if (this.EnableUserPresence)
				splitter = "&nbsp";

			if (assignees != null && assignees.Count > 0)
			{
				for (int i = 0; i < assignees.Count; i++)
				{
					if (i > 0)
						userNames.Controls.Add(new LiteralControl(splitter));

					userNames.Controls.Add(GetUserNameControl(assignees[i].User));
				}
			}
			else
			{
				userNames.Controls.Add(new LiteralControl(HttpUtility.HtmlEncode(defaultValue)));
			}

			return userNames;
		}

		private string GetCandidatesOrAssigneesText(WfAssigneeCollection assignees, string defaultValue)
		{
			StringBuilder strB = new StringBuilder();
			if (assignees != null && assignees.Count > 0)
			{
				for (int i = 0; i < assignees.Count; i++)
				{
					strB.Append(assignees[i].User.DisplayName + (i < assignees.Count - 1 ? "," : ""));
				}
			}
			else
			{
				strB.Append(defaultValue);
			}

			return strB.ToString();
		}

		private void RegisterCSS()
		{
			//string css = string.Empty;
			//if (!this.Width.IsEmpty)
			//{
			//    css = ResourceHelper.LoadStringFromResource(
			//    Assembly.GetExecutingAssembly(),
			//    "MCS.Web.WebControls.ProcessNavigator.ProcessNavigator_No_Width.css");

			//}
			//else
			//{
			//    css = ResourceHelper.LoadStringFromResource(
			//    Assembly.GetExecutingAssembly(),
			//    "MCS.Web.WebControls.ProcessNavigator.ProcessNavigator.css");

			//}
			string css = ResourceHelper.LoadStringFromResource(
				Assembly.GetExecutingAssembly(),
				"MCS.Web.WebControls.ProcessNavigator.ProcessNavigator.css");

			Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "StatusCSS",
				string.Format("<style type='text/css'>{0}</style>", css));
		}

		private void RegisterCssExpressionMethod()
		{
			string strJs = "var CssExpressionMethod = function (el) {";
			strJs += " el.className = 'process_div_container';";
			strJs += " el.width = el.parentElement.offsetWidth - 10; };";

			if (!Page.ClientScript.IsClientScriptBlockRegistered(this.GetType(), "cssExpression"))
				Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "cssExpression", strJs, true);
		}

		[Browsable(false)]
		[ScriptControlProperty()]
		[ClientPropertyName("mainContainerClientID")]
		private string MainContainerID
		{
			get
			{
				return this.mainContainer.ClientID;
			}

		}

		[Browsable(false)]
		[ScriptControlProperty()]
		[ClientPropertyName("processDivContainerClientID")]
		private string ProcessDivContainerClientID
		{
			get
			{
				return this.processDivContainer.ClientID;
			}
		}

		[Browsable(false)]
		[ScriptControlProperty()]
		[ClientPropertyName("processUlContainerClientID")]
		private string ProcessUlContainerClientID
		{
			get
			{
				return this.processUlContainer.ClientID;
			}
		}

		[Browsable(false)]
		[ScriptControlProperty()]
		[ClientPropertyName("buttonContainerClientID")]
		private string ButtonContainerClientID
		{
			get
			{
				return this.btnDiv.ClientID;
			}
		}

		[Browsable(false)]
		[ScriptControlProperty()]
		[ClientPropertyName("buttonLeftClientID")]
		private string ButtonLeftClientID
		{
			get
			{
				return this.aLeft.ClientID;
			}
		}

		[Browsable(false)]
		[ScriptControlProperty()]
		[ClientPropertyName("buttonRightClientID")]
		private string ButtonRightClientID
		{
			get
			{
				return this.aRight.ClientID;
			}
		}

		private static WfMainStreamActivityDescriptorCollection GetMainStreamActivitiesByProcess(IWfProcess process)
		{
			//沈峥注释，修改为获取流程实例中的主线活动
			return process.GetMainStreamActivities(false);
			//return ((WfProcessDescriptor)process.Descriptor).GetMainStreamForDisplayActivities();
		}
	}
}
