using CIIC.HSR.TSP.WebComponents;
using CIIC.HSR.TSP.WebComponents.Widgets.Button;
using CIIC.HSR.TSP.WebComponents.Widgets.DropDownButton;
using CIIC.HSR.TSP.WF.Bizlet.Impl;
using CIIC.HSR.TSP.WF.UI.Control.Controls;
using CIIC.HSR.TSP.WF.UI.Control.Controls.Abstract;
using CIIC.HSR.TSP.WF.UI.Control.Controls.StartWorkflow;
using CIIC.HSR.TSP.WF.UI.Control.DefaultActions;
using CIIC.HSR.TSP.WF.UI.Control.Interfaces;
using CIIC.HSR.TSP.WF.UI.Control.ModelBinding;
using MCS.Library.WF.Contracts.Json.Converters;
using MCS.Library.WF.Contracts.Ogu;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using MCS.Library.WF.Contracts.Workflow.Runtime;
using MCS.Web.Library.Script;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Translator = MCS.Library.Globalization.Translator;

namespace CIIC.HSR.TSP.WF.UI.Control.Controls.MoveTo
{
	[WFControlDescription(WFDefaultActionUrl.MoveToDefault, "$.fn.HSR.Controls.WFMoveTo.Click")]
	public class WFMoveTo : WFControlBase
	{
        private bool IsAllow = false;
        private bool IsMulti = false;
		public WFMoveTo(ViewContext vc, ViewDataDictionary vdd)
			: base(vc, vdd)
		{
            WFUIRuntimeContext runtime = this.ViewContext.HttpContext.Request.GetWFContext();
            if (runtime != null && runtime.Process != null)
            {
                //是否可选择审批人
                IsAllow = runtime.Process.CurrentActivity.Descriptor.Properties.GetValue("AllowSelectCandidates", false);
                IsMulti = runtime.Process.CurrentActivity.Descriptor.Properties.GetValue("AllowAssignToMultiUsers", true);
            }

		}

		/// <summary>
		/// 下拉按钮控件
		/// </summary>
		private DropDownButton InnerDropDownButton
		{
			get
			{
				return (DropDownButton)this.Widget;
			}
		}

		/// <summary>
		/// 流程ID
		/// </summary>
		public string ProcessId
		{
			get;
			set;
		}

		/// <summary>
		/// 业务Id
		/// </summary>
		public string ResourceId
		{
			get;
			set;
		}

		/// <summary>
		/// 当前节点ID
		/// </summary>
		public string ActivityId
		{
			get;
			set;
		}

		/// <summary>
		/// 待办标题
		/// </summary>
		public string TaskTitle
		{
			get;
			set;
		}

		/// <summary>
		/// 表单地址
		/// </summary>
		public string BusinessUrl
		{
			get;
			set;
        }

        /// <summary>
        /// 动态角色审批人列表
        /// </summary>
        public Dictionary<string, List<WfClientUser>> DictionaryWfClientUser
        {
            get;
            set;
        }

		protected override WFParameterWithResponseBase PrepareParameters()
		{
			WFMoveToParameter param = new WFMoveToParameter();

			param.IsDefault = true;
			param.TransferParameter = null;
			param.TaskTitle = this.TaskTitle;
			param.BusinessUrl = this.BusinessUrl;

            //取得意见ID
            WFUIRuntimeContext runtime = this.ViewContext.HttpContext.Request.GetWFContext();
            param.ClientOpinionId = WFUIControlCommon.GetCurrentOpinionId(runtime);

			if (runtime != null && runtime.Process != null)
			{
				WfClientNextActivity nextActivity = runtime.Process.NextActivities.FirstOrDefault();

				if (nextActivity != null)
				{
					param.Target.ActivityKey = nextActivity.Activity.DescriptorKey;
					param.Target.TransitionKey = nextActivity.Transition.Key;

					string codeName = nextActivity.Transition.Properties.GetValue("CodeName", nextActivity.Transition.Name);

					param.Target.ActionResult = string.IsNullOrEmpty(codeName) ? nextActivity.Transition.Key : codeName;

                    int tempCandidatesCnt = 0;
					nextActivity.Activity.Candidates.ForEach(assignee =>
					{
						param.Target.Candidates.Add(assignee);
                        tempCandidatesCnt = tempCandidatesCnt + 1;
					});


                    tempCandidatesCnt = WFUIControlCommon.AddWfClientUser(this.DictionaryWfClientUser, param.Target, tempCandidatesCnt);

                    if (tempCandidatesCnt > 1 && IsAllow)
                    {
                        param.IsSelectCandidates = true;
                        //单选还是多选
                        if (IsMulti)
                            param.IsAssignToMultiUsers = true;
                    }
				}

			}

			return param;
		}

		protected override bool GetEnabled()
		{
			bool result = false;

			WFUIRuntimeContext runtime = this.ViewContext.HttpContext.
				Request.GetWFContext();

			if (runtime != null)
				result = runtime.Process.AuthorizationInfo.InMoveToMode;

			return result;
		}

		protected override void InitWidgetAttributes(WidgetBase widget)
		{
			WfClientJsonConverterHelper.Instance.RegisterConverters();

			WFUIRuntimeContext runtime = this.ViewContext.HttpContext.
				Request.GetWFContext();

			if (runtime != null && runtime.Process != null)
			{
				for (int i = 0; i < runtime.Process.NextActivities.Count; i++)
				{
					WfClientNextActivity nextActivity = runtime.Process.NextActivities[i];

					if (i == 0)
					{
						this.InnerDropDownButton.Text = GetButtonName(runtime.Process.CurrentActivity, nextActivity.Transition, this.InnerDropDownButton.Text, true);
					}
					else
					{
						WFMoveToTargetParameter target = new WFMoveToTargetParameter();

						string codeName = nextActivity.Transition.Properties.GetValue("CodeName", nextActivity.Transition.Name);

						target.ActionResult = string.IsNullOrEmpty(codeName) ? nextActivity.Transition.Key : codeName;
						target.ActivityKey = nextActivity.Activity.DescriptorKey;
						target.TransitionKey = nextActivity.Transition.Key;
                        int tempCandidatesCnt = 0;
						nextActivity.Activity.Candidates.ForEach(assignee =>
						{
							target.Candidates.Add(assignee);
                            tempCandidatesCnt = tempCandidatesCnt + 1;
						});

                        tempCandidatesCnt = WFUIControlCommon.AddWfClientUser(this.DictionaryWfClientUser, target, tempCandidatesCnt);

                        if (tempCandidatesCnt > 1 && IsAllow)
                        {
                            target.IsSelectCandidates = true;
                            //单选还是多选
                            if (IsMulti)
                                target.IsAssignToMultiUsers = true;
                        }

						string serializedParam = JSONSerializerExecute.Serialize(target);

						this.InnerDropDownButton.Items.Add(new DropItem()
						{
							ClientHandler = "$.fn.HSR.Controls.WFMoveTo.ItemClick",
							Command = serializedParam,
							EnableDialog = false,
							Title = GetButtonName(runtime.Process.CurrentActivity, nextActivity.Transition, "送签", false),
							Enabled = true
						});
					}
				}

				this.InnerDropDownButton.ClientClick = this.ClientButtonClickScript;
				this.InnerDropDownButton.Enabled = this.GetEnabled();
                this.InnerDropDownButton.Visible = this.GetEnabled();
			}
		}

		private static string GetButtonName(WfClientActivity currentActivity, WfClientTransitionDescriptor transition, string defaultButtonName, bool isFirstButton)
		{
			string result = "";
			string btnName = transition.Name;
			bool isSameAsTransBtnName = currentActivity.Descriptor.Properties.GetValue("MoveToButtonNameSameAsTransitionName", false);

			if (isSameAsTransBtnName)
			{
				if (string.IsNullOrEmpty(btnName))
					btnName = transition.Description;

				if (string.IsNullOrEmpty(btnName) == false)
					result = btnName;
			}

			if (string.IsNullOrEmpty(result))
			{
				result = defaultButtonName;
			}

			if (string.IsNullOrEmpty(result) && isFirstButton)
			{
				result = "送签";
			}

		    result = Translator.Translate(CultureDefine.DefaultCulture, result);

			return result;
		}

        public override void WriteHtml(System.IO.StringWriter stringWriter)
        {
            if (IsAllow)
            {
                string mvcHtmlStr = WFUIControlCommon.WriteMvcHtmlString(this.Name, true);
                stringWriter.Write(mvcHtmlStr);
            }

            base.WriteHtml(stringWriter);
        }



	}
}
