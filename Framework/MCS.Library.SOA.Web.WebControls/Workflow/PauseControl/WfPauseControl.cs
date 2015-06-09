using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library;
using MCS.Web.Library.MVC;
using MCS.Web.Library.Script;

namespace MCS.Web.WebControls
{
    /// <summary>
    /// 工作流流程暂停控件
    /// </summary>
    [RequiredScript(typeof(ControlBaseScript), 1)]
    [RequiredScript(typeof(HBCommonScript), 2)]
    [ClientScriptResource("MCS.Web.WebControls.WfPauseControl",
        "MCS.Web.WebControls.Workflow.Abstract.WfControls.js")]
    public class WfPauseControl : WfProcessControlBase
    {
        public WfPauseControl()
        {
            if (this.DesignMode == false)
            {
                JSONSerializerExecute.RegisterConverter(typeof(OguObjectConverter));
                JSONSerializerExecute.RegisterConverter(typeof(OguRoleConverter));
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

        [Browsable(false)]
        [Category("文档")]
        [Description("操作的描述")]
        [ScriptControlProperty()]
        [ClientPropertyName("operationText")]
        private string OperationText
        {
            get
            {
                return GetOperationText(CurrentProcess);
            }
        }

        protected override void OnInit(EventArgs e)
        {
            WfControlHelperExt.InitWfControlPostBackErrorHandler(this.Page);
            base.OnInit(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "确认要暂停流程吗？");
            DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "确认要恢复流程吗？");

            base.OnPreRender(e);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (DesignMode)
                writer.Write("Pause Control");
            else
                base.Render(writer);
        }

        protected override WfExecutorBase CreateExecutor()
        {
            WfExecutorBase executor = null;

            switch (CurrentProcess.Status)
            {
                case WfProcessStatus.Paused:
                    executor = new WfResumeProcessExecutor(WfClientContext.Current.CurrentActivity, CurrentProcess);
                    break;
                case WfProcessStatus.Running:
                    executor = new WfPauseProcessExecutor(WfClientContext.Current.CurrentActivity, CurrentProcess);
                    break;
                default:
                    throw new NotSupportedException(string.Format("暂停和恢复控件不支持根据流程状态为{0}构造执行器", CurrentProcess.Status));
            }

            return executor;
        }

        protected override WfControlAccessbility OnGetAccessbility(WfControlAccessbility defaultAccessbility)
        {
            WfControlAccessbility result = WfControlAccessbility.None;

            if (this.Visible && !this.ReadOnly && this.CanPauseOrResumeProcess())
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

                    string buttonText = GetOperationText(CurrentProcess);

                    button.Text = button.Text.Replace("暂停", HttpUtility.HtmlEncode(buttonText));
                }
            }
        }

        protected override void OnSuccess()
        {
            base.OnSuccess();

            WebUtility.ResponseTimeoutScriptBlock(string.Format("if (parent) parent.location.replace('{0}');",
                WfClientContext.Current.ReplaceEntryPathByProcessID()),
                ExtScriptHelper.DefaultResponseTimeout);
        }

        private bool CanPauseOrResumeProcess()
        {
            bool result = false;

            if (CurrentProcess != null)
            {
                IWfActivity originalActivity = WfClientContext.Current.OriginalActivity;

                result = (CurrentProcess.CanPause || CurrentProcess.CanResume);

                //首先是流程状态满足，然后再判断人和活动的权限
                if (result)
                    result = (WfClientContext.Current.InAdminMode || this.PrivilegeMode == WfPrivilegeMode.Admin);
            }

            return result;
        }

        private static string GetOperationText(IWfProcess process)
        {
            string result = "暂停";

            if (process != null)
            {
                switch (process.Status)
                {
                    case WfProcessStatus.Paused:
                        result = "恢复";
                        break;
                    case WfProcessStatus.Running:
                        result = "暂停";
                        break;
                }
            }

            return result;
        }
    }
}
