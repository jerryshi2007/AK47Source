using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using MCS.Web.Library.Script;
using MCS.Web.Library;
using System.Web.UI.HtmlControls;
using MCS.Library.SOA.DataObjects;
using System.ComponentModel;
//using MCS.Library.Workflow.Engine;
//using MCS.Library.Workflow.Descriptors;
using MCS.Library.OGUPermission;
using System.Web.UI.WebControls;
using MCS.Library.Globalization;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library.MVC;

[assembly: WebResource("MCS.Web.WebControls.Workflow.ReturnControl.RejectActivitySelector.js", "application/x-javascript")]

namespace MCS.Web.WebControls
{
	/// <summary>
	/// 退件环节选择对话框
	/// </summary>
	[RequiredScript(typeof(ControlBaseScript), 1)]
	[RequiredScript(typeof(HBCommonScript), 2)]
	[ClientScriptResource("MCS.Web.WebControls.RejectActivitySelector",
		"MCS.Web.WebControls.Workflow.ReturnControl.RejectActivitySelector.js")]
	[DialogContent("MCS.Web.WebControls.Workflow.ReturnControl.RejectActivitySelector.htm", "MCS.Library.SOA.Web.WebControls")]
	[ToolboxData("<{0}:RejectActivitySelector runat=server></{0}:RejectActivitySelector>")]
	public class RejectActivitySelector : DialogControlBase<RejectActivitySelectorParams>
	{
		private HtmlSelect _ActivitiesList = null;
		private OpinionReasonItemCollection reasons = new OpinionReasonItemCollection();

		public RejectActivitySelector()
		{
			if (this.DesignMode == false)
			{
				JSONSerializerExecute.RegisterConverter(typeof(OguObjectConverter));
				JSONSerializerExecute.RegisterConverter(typeof(OguRoleConverter));
				JSONSerializerExecute.RegisterConverter(typeof(WfControlNextStepConverter));
			}
		}

		#region Properties
		/// <summary>
		/// 环节 ID
		/// </summary>
		public string ActivityID
		{
			get
			{
				return ControlParams.ActivityID;
			}
			set
			{
				ControlParams.ActivityID = value;
			}
		}

		/// <summary>
		/// 是否退回给操作人，如果不是，则送给流程的每个定义的人
		/// </summary>
		[DefaultValue(false)]
		public bool ReturnToOperator
		{
			get
			{
				return ControlParams.ReturnToOperator;
			}
			set
			{
				ControlParams.ReturnToOperator = value;
			}
		}

		[DefaultValue(false)]
		public bool ShowOpinionInput
		{
			get
			{
				return ControlParams.ShowOpinionInput;
			}
			set
			{
				ControlParams.ShowOpinionInput = value;
			}
		}

		[PersistenceMode(PersistenceMode.InnerProperty), Description("意见原因列表")]
		[MergableProperty(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[DefaultValue((string)null)]
		[Browsable(false)]
		public OpinionReasonItemCollection Reasons
		{
			get
			{
				return this.reasons;
			}
		}

		[Browsable(false)]
		[ScriptControlProperty()]
		[ClientPropertyName("activitiesListClientID")]
		private string activitiesListClientID
		{
			get
			{
				string result = string.Empty;

				if (_ActivitiesList != null)
					result = _ActivitiesList.ClientID;

				return result;
			}
		}
		#endregion Properties

		#region Protected
		/// <summary>
		/// 根据UserID生成弹出对话框的url地址
		/// </summary>
		/// <returns></returns>
		protected override string GetDialogUrl()
		{
			PageRenderMode pageRenderMode = new PageRenderMode(this.UniqueID, "DialogControl");

			string url = WebUtility.GetRequestExecutionUrl(pageRenderMode, "activityID");

			return url + "&" + this.ControlParams.ToRequestParams();
		}

		protected override void OnPagePreLoad(object sender, EventArgs e)
		{
			EnsureChildControls();
			base.OnPagePreLoad(sender, e);
		}

		protected override void OnPreRender(EventArgs e)
		{
			if (CurrentMode == ControlShowingMode.Dialog)
			{
				//DeluxeNameTableContextCache.Instance.Add(Define.DefaultCategory, "请选择退件的环节。");
				//DeluxeNameTableContextCache.Instance.Add(Define.DefaultCategory, "请填写退件意见。");
				DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "请选择退件的环节。");
				DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "请填写退件意见。");
			}

			base.OnPreRender(e);
		}

		protected override void OnPagePreRenderComplete(object sender, EventArgs e)
		{
			if (CurrentMode == ControlShowingMode.Dialog)
			{
				InitActivitiesListBox(_ActivitiesList);
				InitOpinionInputControl();

				HtmlTableRow reasonList = (HtmlTableRow)WebControlUtility.FindControlByHtmlIDProperty(this, "reasonList", true);

				if (reasonList != null)
				{
					if (this.Reasons.Count > 0)
					{
						reasonList.Style["display"] = "inline";
						HtmlSelect resonSelector = (HtmlSelect)WebControlUtility.FindControlByHtmlIDProperty(this, "resonSelector", true);

						if (resonSelector != null)
						{
							foreach (var reason in this.Reasons)
								resonSelector.Items.Add(new ListItem(reason.Description, reason.Key));
						}
					}
				}
			}

			base.OnPagePreRenderComplete(sender, e);
		}

		private void InitOpinionInputControl()
		{
			HtmlGenericControl opinionBody = (HtmlGenericControl)WebControlUtility.FindControlByHtmlIDProperty(this, "opinionBody", true);

			if (opinionBody != null)
			{
				if (ShowOpinionInput)
					opinionBody.Style["display"] = "inline";
			}
		}

		/// <summary>
		/// 初始化对话框内容
		/// </summary>
		/// <param name="container"></param>
		protected override void InitDialogContent(Control container)
		{
			base.InitDialogContent(container);

			this.ID = "RejectActivitySelector";

			_ActivitiesList =
				(HtmlSelect)WebControlUtility.FindControlByHtmlIDProperty(container, "activitiesList", true);
		}

		/// <summary>
		/// 获取对话框外观属性
		/// </summary>
		/// <returns></returns>
		protected override string GetDialogFeature()
		{
			WindowFeature feature = new WindowFeature();

			feature.Width = 400;

			if (ShowOpinionInput)
				feature.Height = 440;
			else
				feature.Height = 300;

			feature.Resizable = true;
			feature.ShowStatusBar = false;
			feature.ShowScrollBars = false;
			feature.Center = true;

			return feature.ToDialogFeatureClientString();
		}

		/// <summary>
		/// 初始化确认按钮
		/// </summary>
		/// <param name="confirmButton"></param>
		protected override void InitConfirmButton(HtmlInputButton confirmButton)
		{
			if (_ActivitiesList.Disabled == false)
				confirmButton.Attributes["onclick"] = "onDialogConfirm();";
			else
			{
				confirmButton.Attributes["onclick"] = "return false;";
				confirmButton.Disabled = true;
			}
		}

		#endregion Protected

		#region Private
		/// <summary>
		/// 初始化用户输入框
		/// </summary>
		/// <param name="container"></param>
		private void InitActivitiesListBox(HtmlSelect activitiesList)
		{
			RejectActivityList activities = RejectActivityList.CreateFromProcess(WfClientContext.Current.CurrentActivity.Process);

			ListItemCollection list = new ListItemCollection();

			for (int i = activities.Count - 1; i >= 0; i--)
			{
				IWfActivity activity = activities[i];

				if (activity.Descriptor.Properties.GetValue("AllowToBeReturned", true))
				{
					WfControlNextStep nextSetp = new WfControlNextStep(activity);

					string displayName = string.Empty;

					if (activity.Operator != null)
					{
						displayName = activity.Operator.DisplayName;
					}
					else
					{
						if (activity.Assignees.Count > 0)
							displayName = activity.Assignees[0].User.DisplayName;
						else
							displayName = Translator.Translate(Define.DefaultCulture, "自动流转点");
					}

					WfMoveToControl.DoActionAfterRegisterContextConverter(() =>
						{
							ListItem item = new ListItem(string.Format("{0} - {1}({2})", displayName, activity.Descriptor.Name, activity.Descriptor.Key),
							   JSONSerializerExecute.Serialize(nextSetp));
							list.Add(item);
						}
					);
				}
			}

			if (list.Count > 0)
			{
				foreach (ListItem item in list)
					this._ActivitiesList.Items.Add(item);
			}
			else
			{
				this._ActivitiesList.Items.Add(Translator.Translate(Define.DefaultCulture, "没有能够退回的环节"));
				this._ActivitiesList.Disabled = true;
			}

			this._ActivitiesList.Multiple = true;
		}

		///// <summary>
		///// 初始化用户输入框
		///// </summary>
		///// <param name="container"></param>
		//private void InitActivitiesListBox(HtmlSelect activitiesList)
		//{
		//    ListItemCollection map = new ListItemCollection();
		//    IWfProcess process = WfClientContext.Current.CurrentActivity.Process;
		//    foreach (IWfActivity elapsedAct in process.ElapsedActivities)
		//    {
		//        string currAssoActKey = process.CurrentActivity.Descriptor.AssociatedActivityKey;
		//        if (!string.IsNullOrEmpty(currAssoActKey))
		//        {
		//            if (elapsedAct.ID == process.Activities.FindActivityByDescriptorKey(currAssoActKey).ID)
		//                break;
		//        }
		//        if (elapsedAct.ID == process.CurrentActivity.ID)
		//            break;

		//        if (string.IsNullOrEmpty(elapsedAct.Descriptor.AssociatedActivityKey) && elapsedAct.ID != process.CurrentActivity.ID && elapsedAct.Descriptor.Properties.GetValue("AllowToBeReturned", true) != false)
		//        {
		//            WfControlNextStep nextSetp = new WfControlNextStep(elapsedAct);
		//            //GetUserTitle(elapsedAct.Operator)
		//            string displayName = string.Empty;

		//            if (elapsedAct.Operator != null)
		//                displayName = elapsedAct.Operator.DisplayName;
		//            else if (elapsedAct.Assignees.Count > 0)
		//                displayName = elapsedAct.Assignees[0].User.DisplayName;
		//            else
		//                displayName = Translator.Translate(Define.DefaultCulture, "自动流转点");

		//            WfMoveToControl.DoActionAfterRegisterContextConverter(() =>
		//                {
		//                    ListItem item = new ListItem(string.Format("{0} - {1}", displayName, elapsedAct.Descriptor.Name),
		//                       JSONSerializerExecute.Serialize(nextSetp));
		//                    map.Add(item);
		//                }
		//            );
		//        }
		//    }
		//    if (map.Count > 0)
		//    {
		//        foreach (ListItem item in map)
		//        {
		//            _ActivitiesList.Items.Add(item);
		//        }
		//    }
		//    else
		//    {
		//        _ActivitiesList.Items.Add(Translator.Translate(Define.DefaultCulture, "没有能够退回的环节"));
		//        _ActivitiesList.Disabled = true;
		//    }

		//    _ActivitiesList.Multiple = true;
		//}

		/*
		/// <summary>
		/// 获取用户部门及职称
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		private string GetUserTitle(IUser user)
		{
			string title = string.Empty;

			if (user.Parent != null)
				title = user.Parent.DisplayName;

			if (string.IsNullOrEmpty(user.Occupation) == false)
				title += "/" + user.Occupation;

			return title;
		}
		*/
		#endregion
	}
}
