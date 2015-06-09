using System;
using System.Web;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Collections.Generic;

using MCS.Web.Library.Script;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library;
using MCS.Library.SOA.DataObjects;
using MCS.Web.Library.MVC;
using MCS.Library.Globalization;
using MCS.Library.Principal;

namespace MCS.Web.WebControls
{
    /// <summary>
    /// 工作流流程作废控件
    /// </summary>
    [RequiredScript(typeof(ControlBaseScript), 1)]
    [RequiredScript(typeof(HBCommonScript), 2)]
    [ClientScriptResource("MCS.Web.WebControls.WfAbortControl",
        "MCS.Web.WebControls.Workflow.Abstract.WfControls.js")]
    [ToolboxData("<{0}:WfAbortControl runat=server></{0}:WfAbortControl>")]
    public class WfAbortControl : WfProcessControlBase
    {
        //public event BeforeOperationEventHandler BeforeCancelProcess;
        private OpinionInputDialog opinionInputDialog = new OpinionInputDialog() { Category = Define.DefaultCulture, EmptyOpinionPrompt = "请输入作废意见" };
        private string opinionText = string.Empty;
        private string opinionType = string.Empty;
        private OpinionReasonItemCollection reasons = new OpinionReasonItemCollection();

        public WfAbortControl()
        {
            if (this.DesignMode == false)
            {
                JSONSerializerExecute.RegisterConverter(typeof(OguObjectConverter));
                JSONSerializerExecute.RegisterConverter(typeof(OguRoleConverter));
            }
        }

        #region properties
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


        [DefaultValue(false)]
        [Category("文档")]
        [Description("是否需要输入作废的原因")]
        public bool NeedAbortReason
        {
            get
            {
                return GetPropertyValue("NeedAbortReason", false);
            }
            set
            {
                SetPropertyValue("NeedAbortReason", value);
            }
        }

        [ScriptControlProperty()]
        [ClientPropertyName("needAbortReason")]
        private bool ClientNeedAbortReason
        {
            get
            {
                bool result = NeedAbortReason;
                var originalAct = WfClientContext.Current.OriginalActivity;

                if (result && WfClientContext.Current.OriginalActivity != null)
                {
                    if (originalAct.Process != originalAct.Process.RootProcess)
                    {
                        result = true;
                    }
                    if (originalAct.ID != originalAct.Process.InitialActivity.ID)
                    {
                        result = true;
                    }
                }

                return result;
            }
        }

        [PersistenceMode(PersistenceMode.InnerProperty), Description("作废原因列表")]
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
        [Category("文档")]
        [Description("OpinionInputDialogClientID")]
        [ScriptControlProperty()]
        [ClientPropertyName("opinionInputDialogClientID")]
        private string OpinionInputDialogClientID
        {
            get
            {
                return this.opinionInputDialog.ClientID;
            }
        }

        #endregion properties

        #region overridable
        protected override void CreateChildControls()
        {
            this.opinionInputDialog.DialogTitle = "请输入作废意见";
            this.opinionInputDialog.ID = "OpinionInputDialog";

            opinionInputDialog.Reasons.Clear();

            this.Reasons.ForEach(r => opinionInputDialog.Reasons.Add(r));

            Controls.Add(this.opinionInputDialog);

            base.CreateChildControls();
        }

        protected override void OnInit(EventArgs e)
        {
            WfControlHelperExt.InitWfControlPostBackErrorHandler(this.Page);
            base.OnInit(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "确认要作废或取消流程吗？");

            base.OnPreRender(e);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (DesignMode)
                writer.Write("Abort Control");
            else
                base.Render(writer);
        }

        protected override WfExecutorBase CreateExecutor()
        {
            WfCancelProcessExecutor executor = new WfCancelProcessExecutor(WfClientContext.Current.CurrentActivity,
                WfClientContext.Current.CurrentActivity.ApprovalRootActivity.Process);

            executor.PrepareApplicationData += new ExecutorEventHandler(executor_PrepareApplicationData);
            executor.PrepareNotifyTasks += new PrepareTasksEventHandler(executor_PrepareNotifyTasks);
            executor.SaveApplicationData += new ExecutorEventHandler(executor_SaveApplicationData);

            return executor;
        }

        private void executor_PrepareNotifyTasks(WfExecutorDataContext dataContext, UserTaskCollection tasks)
        {
            GenericOpinion opnion = (GenericOpinion)dataContext["AbortOpinion"];

            tasks.ForEach(t =>
            {
                if (t.Body.IsNullOrEmpty())
                {
                    t.Body = opnion.Content;
                }
            });
        }

        private void executor_PrepareApplicationData(WfExecutorDataContext dataContext)
        {
            dataContext["AbortOpinion"] = CreateAbortOpinion();
        }

        private void executor_SaveApplicationData(WfExecutorDataContext dataContext)
        {
            GenericOpinion opnion = (GenericOpinion)dataContext["AbortOpinion"];
            GenericOpinionAdapter.Instance.Update(opnion);
        }

        protected override WfControlAccessbility OnGetAccessbility(WfControlAccessbility defaultAccessbility)
        {
            WfControlAccessbility result = WfControlAccessbility.None;

            if (this.Visible && !this.ReadOnly && this.CanCancelProcess())
                result |= WfControlAccessbility.Visible | WfControlAccessbility.Enabled;

            return base.OnGetAccessbility(result);
        }

        protected override void SetTargetControlVisible(Control target)
        {
            target.Visible = (OnGetAccessbility(WfControlAccessbility.None) & WfControlAccessbility.Visible) != WfControlAccessbility.None;

            ((IAttributeAccessor)target).SetAttribute("class", "enable");

            if (target.Visible == false)
            {
                ((IAttributeAccessor)target).SetAttribute("class", "disable");
            }
            else
            {
                if (target is IButtonControl)
                {
                    IButtonControl button = (IButtonControl)target;

                    string buttonText = WfClientContext.Current.CurrentActivity.Descriptor.Properties.GetValue("AbortButtonName", "取消");

                    button.Text = button.Text.Replace("取消", HttpUtility.HtmlEncode(buttonText));
                }
            }
        }

        protected override void OnCreateHiddenButton(HiddenButtonWrapper buttonWrapper)
        {
            buttonWrapper.HiddenButton.PopupCaption = Translator.Translate(Define.DefaultCulture, "正在作废...");
            buttonWrapper.HiddenButton.ProgressMode = SubmitButtonProgressMode.BySteps;
        }

        protected override void LoadClientState(string clientState)
        {
            string[] state = (string[])JSONSerializerExecute.DeserializeObject(clientState, typeof(string[]));

            this.opinionText = state[0];
            this.opinionType = state[1];

            if (string.IsNullOrEmpty(opinionType) == false)
                OpinionInput.SetOpinionType(this.opinionType);
        }
        #endregion override

        #region Protected
        protected override void OnSuccess()
        {
            base.OnSuccess();

            WebUtility.ResponseTimeoutScriptBlock(string.Format("if (parent) parent.location.replace('{0}');",
                WfClientContext.Current.ReplaceEntryPathByProcessID()),
                ExtScriptHelper.DefaultResponseTimeout);
        }

        #endregion Protected

        #region private

        private bool CanCancelProcess()
        {
            bool result = false;

            if (CurrentProcess != null)
            {
                IWfActivity originalActivity = WfClientContext.Current.OriginalActivity;

                result = (CurrentProcess.CanCancel)
                        && originalActivity.Process.IsApprovalRootProcess;

                //首先是流程状态满足，然后再判断人和活动的权限
                if (result)
                {
                    //当前环节允许的话，要看是否是待办人或者流程已经办结
                    if (CurrentProcess.CurrentActivity.Descriptor.ActivityType == WfActivityType.CompletedActivity)
                        originalActivity = CurrentProcess.CurrentActivity;

                    result = ((WfClientContext.Current.InMoveToMode || CurrentProcess.Status == WfProcessStatus.Completed) &&
                        originalActivity.Descriptor.Properties.GetValue("AllowAbortProcess", true));

                    if (result == false)
                        result = (WfClientContext.Current.InAdminMode || this.PrivilegeMode == WfPrivilegeMode.Admin);
                }
            }

            return result;
        }

        public GenericOpinion CreateAbortOpinion()
        {
            GenericOpinion opinion = new GenericOpinion();

            opinion.ID = UuidHelper.NewUuidString();
            opinion.ResourceID = WfClientContext.Current.CurrentActivity.Process.ResourceID;
            opinion.ProcessID = "AbortProcess";
            opinion.ActivityID = WfClientContext.Current.OriginalActivity.ID;
            opinion.LevelName = WfClientContext.Current.OriginalActivity.ApprovalRootActivity.Descriptor.LevelName;
            opinion.IssuePerson = WfClientContext.Current.User;
            opinion.AppendPerson = DeluxeIdentity.CurrentRealUser;
            opinion.Content = this.opinionText;
            opinion.OpinionType = this.opinionType;

            return opinion;
        }

        #endregion private
    }
}
