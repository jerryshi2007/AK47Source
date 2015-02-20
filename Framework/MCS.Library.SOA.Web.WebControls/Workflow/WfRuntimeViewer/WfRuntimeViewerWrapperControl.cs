using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.Globalization;
using MCS.Library.OGUPermission;
using MCS.Library.Principal;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.SOA.DataObjects.Workflow.DTO;
using MCS.Web.Library;
using MCS.Web.Library.MVC;
using MCS.Web.Library.Script;
using MCS.Web.WebControls.Configuration;

[assembly: WebResource("MCS.Web.WebControls.Workflow.WfRuntimeViewer.wfrtvwc.css", "text/stylesheet")]

namespace MCS.Web.WebControls
{
    [Flags]
    public enum WfActionAfterOperation
    {
        None = 0,
        Close = 1,
        RefreshCurrent = 2,
        RefreshOpener = 4,
    }

    /// <summary>
    /// 流程运行时图形展示的包装控件
    /// </summary>
    [RequiredScript(typeof(ControlBaseScript), 1)]
    [RequiredScript(typeof(HBCommonScript), 2)]
    [ClientCssResource("MCS.Web.WebControls.Workflow.WfRuntimeViewer.wfrtvwc.css")]
    [ClientScriptResource("MCS.Web.WebControls.WfRuntimeViewerWrapperControl", "MCS.Web.WebControls.Workflow.Abstract.WfControls.js")]
    [DialogContent("MCS.Web.WebControls.Workflow.WfRuntimeViewer.WfRuntimeViewerWrapperControl.htm", "MCS.Library.SOA.Web.WebControls")]
    public class WfRuntimeViewerWrapperControl : DialogControlBase<WfRuntimeViewerWrapperControlParams>
    {
        private readonly HiddenButtonWrapper buttonWrapper = new HiddenButtonWrapper();
        private readonly WfRuntimeViewer viewerControl = new WfRuntimeViewer();
        private HtmlGenericControl advancedOpWrapperControl = null;
        private HtmlGenericControl normalOpWrapperControl = null;
        private readonly DeluxeMenu processPopupMenuControl = new DeluxeMenu();
        private readonly DeluxeMenu activityPopupMenuControl = new DeluxeMenu();
        private readonly DeluxeMenu transitionPopupMenuControl = new DeluxeMenu();

        /// <summary>
        /// 点击后，能够弹出对话框的控件ID
        /// </summary>
        [DefaultValue(""), IDReferenceProperty(), TypeConverter(typeof(DialogStartUpControlConverter))]
        public string TargetControlID
        {
            get
            {
                return this.buttonWrapper.TargetControlID;
            }
            set
            {
                this.buttonWrapper.TargetControlID = value;
            }
        }

        /// <summary>
        /// 点击后，能够弹出对话框的控件的实例
        /// </summary>
        [Browsable(false)]
        public IAttributeAccessor TargetControl
        {
            get
            {
                return this.buttonWrapper.TargetControl;
            }
            set
            {
                this.buttonWrapper.TargetControl = value;
            }
        }

        #region Protected
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
                return this.ControlParams.ProcessID;
            }
            set
            {
                this.ControlParams.ProcessID = value;
            }
        }

        [ScriptControlProperty(), ClientPropertyName("wfRuntimeViewerSLID")]
        private string WfRuntimeViewerSLID
        {
            get
            {
                return viewerControl.ClientID + WfRuntimeViewer.CLIENTID_SILVERLIGHT_SUFFIX;
            }
        }

        /// <summary>
        /// 流程信息隐藏域ID
        /// </summary>
        [ScriptControlProperty(), ClientPropertyName("wfRuntimeViewerWorkflowInfoHiddenFieldID")]
        private string WfRuntimeViewerWorkflowInfoHiddenFieldID
        {
            get
            {
                return viewerControl.WorkflowInfoHideenFieldID;
            }
        }

        [ScriptControlProperty]
        [ClientPropertyName("openWindowFeature")]
        private string OpenWindowFeature
        {
            get
            {
                return GetPropertyValue("OpenWindowFeature", string.Empty);
            }
            set
            {
                SetPropertyValue("OpenWindowFeature", value);
            }
        }

        [ScriptControlProperty]
        [ClientPropertyName("useIndependentPage")]
        [Description("页面以链接方式打开")]
        public bool UseIndependentPage
        {
            get
            {
                return GetPropertyValue("useIndependentPage", false);
            }
            set
            {
                SetPropertyValue("useIndependentPage", value);
            }
        }

        private string _LinkPageUrl = string.Empty;
        [ScriptControlProperty]
        [ClientPropertyName("linkPageUrl")]
        [Description("打开连接的配置页面")]
        private string LinkPageUrl
        {
            get
            {
                if (string.IsNullOrEmpty(this._LinkPageUrl) == true && this.UseIndependentPage == true)
                {
                    string strUrl = ResourceUriSettings.GetConfig().Paths["wfRuntimeViewerlinkPageUrl"].Uri.ToString();

                    NameValueCollection urlParams = UriHelper.GetUriParamsCollection(strUrl);

                    urlParams["resourceID"] = CurrentProcess.ResourceID;
                    urlParams["processID"] = CurrentProcess.ID;

                    this._LinkPageUrl = UriHelper.CombineUrlParams(strUrl, urlParams);
                }

                return this._LinkPageUrl;
            }
        }

        [ScriptControlProperty]
        [ClientPropertyName("appLogViewUrl")]
        private string AppLogViewUrl
        {
            get
            {
                string result = string.Empty;

                if (ResourceUriSettings.GetConfig().Paths.ContainsKey("appLogView"))
                {
                    result = ResourceUriSettings.GetConfig().Paths["appLogView"].Uri.ToString();

                    NameValueCollection otherParams = new NameValueCollection();

                    otherParams.Add("simulation", WfRuntime.ProcessContext.EnableSimulation.ToString().ToLower());
                    otherParams.Add(GlobalizationWebHelper.LanguageParameterName, GlobalizationWebHelper.GetCurrentHandlerLanguageID());

                    result = UriHelper.CombineUrlParams(result, otherParams);
                }

                return result;
            }
        }

        [ScriptControlProperty]
        [ClientPropertyName("editProcessUrl")]
        private string EditProcessUrl
        {
            get
            {
                string result = string.Empty;

                if (ResourceUriSettings.GetConfig().Paths.ContainsKey("editProcess"))
                {
                    result = ResourceUriSettings.GetConfig().Paths["editProcess"].Uri.ToString();

                    NameValueCollection otherParams = new NameValueCollection();

                    otherParams.Add("simulation", WfRuntime.ProcessContext.EnableSimulation.ToString().ToLower());

                    result = UriHelper.CombineUrlParams(result, otherParams);
                }

                return result;
            }
        }

        [ScriptControlProperty]
        [ClientPropertyName("adminAddTransitionUrl")]
        private string AdminAddTransitionUrl
        {
            get
            {
                string result = string.Empty;

                if (ResourceUriSettings.GetConfig().Paths.ContainsKey("adminAddTransitionUrl"))
                {
                    result = ResourceUriSettings.GetConfig().Paths["adminAddTransitionUrl"].Uri.ToString();

                    NameValueCollection otherParams = new NameValueCollection();

                    otherParams.Add("simulation", WfRuntime.ProcessContext.EnableSimulation.ToString().ToLower());

                    result = UriHelper.CombineUrlParams(result, otherParams);
                }

                return result;
            }
        }

        [ScriptControlProperty]
        [ClientPropertyName("adminAddActivityUrl")]
        private string AdminAddActivityUrl
        {
            get
            {
                string result = string.Empty;

                if (ResourceUriSettings.GetConfig().Paths.ContainsKey("adminAddActivityUrl"))
                {
                    result = ResourceUriSettings.GetConfig().Paths["adminAddActivityUrl"].Uri.ToString();

                    NameValueCollection otherParams = new NameValueCollection();

                    otherParams.Add("simulation", WfRuntime.ProcessContext.EnableSimulation.ToString().ToLower());

                    result = UriHelper.CombineUrlParams(result, otherParams);
                }

                return result;
            }
        }

        [ScriptControlProperty]
        [ClientPropertyName("adminDeleteObjectUrl")]
        private string DeleteObjectUrl
        {
            get
            {
                string result = string.Empty;

                if (ResourceUriSettings.GetConfig().Paths.ContainsKey("adminDeleteObjectUrl"))
                {
                    result = ResourceUriSettings.GetConfig().Paths["adminDeleteObjectUrl"].Uri.ToString();

                    NameValueCollection otherParams = new NameValueCollection();

                    otherParams.Add("simulation", WfRuntime.ProcessContext.EnableSimulation.ToString().ToLower());

                    result = UriHelper.CombineUrlParams(result, otherParams);
                }

                return result;
            }
        }

        [ScriptControlProperty]
        [ClientPropertyName("adminAutoMoveToUrl")]
        private string AutoMoveToUrl
        {
            get
            {
                string result = string.Empty;

                if (ResourceUriSettings.GetConfig().Paths.ContainsKey("adminAutoMoveToUrl"))
                {
                    result = ResourceUriSettings.GetConfig().Paths["adminAutoMoveToUrl"].Uri.ToString();

                    NameValueCollection otherParams = new NameValueCollection();

                    otherParams.Add("simulation", WfRuntime.ProcessContext.EnableSimulation.ToString().ToLower());

                    result = UriHelper.CombineUrlParams(result, otherParams);
                }

                return result;
            }
        }

        private string AdvancedOpWrapperClientID
        {
            get
            {
                string result = string.Empty;

                if (this.advancedOpWrapperControl != null)
                    result = this.advancedOpWrapperControl.ClientID;

                return result;
            }
        }

        [ScriptControlProperty]
        [ClientPropertyName("normalOpWrapperClientID")]
        private string NormalOpWrapperClientID
        {
            get
            {
                string result = string.Empty;

                if (this.normalOpWrapperControl != null)
                    result = this.normalOpWrapperControl.ClientID;

                return result;
            }
        }

        [ScriptControlProperty]
        [ClientPropertyName("processPopupMenuClientID")]
        private string ProcessPopupMenuClientID
        {
            get
            {
                string result = string.Empty;

                if (this.processPopupMenuControl != null)
                    result = this.processPopupMenuControl.ClientID;

                return result;
            }
        }

        [ScriptControlProperty]
        [ClientPropertyName("activityPopupMenuClientID")]
        private string ActivityPopupMenuClientID
        {
            get
            {
                string result = string.Empty;

                if (this.activityPopupMenuControl != null)
                    result = this.activityPopupMenuControl.ClientID;

                return result;
            }
        }

        [ScriptControlProperty]
        [ClientPropertyName("transitionPopupMenuClientID")]
        private string TransitionPopupMenuClientID
        {
            get
            {
                string result = string.Empty;

                if (this.transitionPopupMenuControl != null)
                    result = this.transitionPopupMenuControl.ClientID;

                return result;
            }
        }

        [ScriptControlProperty]
        [ClientPropertyName("showMainStream")]
        private bool ShowMainStream
        {
            get
            {
                return WebUtility.GetRequestQueryValue("mainStream", false);
            }
        }

        protected override void OnInit(EventArgs e)
        {
            WfRuntime.ProcessContext.EnableSimulation = WfClientContext.SimulationEnabled;

            base.OnInit(e);
        }

        protected override void LoadViewState(object savedState)
        {
            base.LoadViewState(savedState);

            HiddenButtonWrapper bw = GetPropertyValue("ButtonWrapper", (HiddenButtonWrapper)null);

            if (bw != null)
            {
                buttonWrapper.TargetControlID = bw.TargetControlID;
            }
        }

        protected override object SaveViewState()
        {
            SetPropertyValue("ButtonWrapper", this.buttonWrapper);

            return base.SaveViewState();
        }

        protected override void OnPagePreLoad(object sender, EventArgs e)
        {
            EnsureChildControls();
            base.OnPagePreLoad(sender, e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            WindowFeature feature = GetWindowFeature();

            if (CurrentMode == ControlShowingMode.Normal)
            {
                InitShowDialogControl();
            }
            else
            {
                if (CurrentProcess != null)
                {
                    viewerControl.InitializeValue = GetWorkflowInfo(CurrentProcess, ShowMainStream);
                    viewerControl.Attributes["wrapperControlID"] = this.ClientID;
                }
            }

            OpenWindowFeature = feature.ToWindowFeatureClientString();

            RegisterClientStringTable();
            RegisterHover();
            RegisterPopupMenu();

            WfControlHelperExt.RegisterCurrentUserAppAuthInfoScript(this.Page);

            base.OnPreRender(e);
        }

        private void RegisterHover()
        {
            this.Page.ClientScript.RegisterStartupScript(this.GetType(), this.ID + "_HoverScript", "$HBRootNS.WfRuntimeViewerWrapperControl.registerHoverHelper('" + this.AdvancedOpWrapperClientID + "','" + this.NormalOpWrapperClientID + "');", true);
        }

        private static void RegisterClientStringTable()
        {
            DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "确定要作废或取消流程吗？");
            DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "请指定该活动资源！");
            DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "修改活动操作人失败，原因：");
            DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "流程不在运行中，无法作废。");
            DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "操作成功！");
            DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "无法修改已完成的活动！");
            DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "请先选择需要修改的活动！");
        }

        private void RegisterPopupMenu()
        {
            this.Page.ClientScript.RegisterStartupScript(this.GetType(), this.ID + "_PopupMenuScript", "$HBRootNS.WfRuntimeViewerWrapperControl.registerPopupMenu('" + this.viewerControl.SilverlightControlClientID + "','" + this.ClientID + "');", true);
        }

        protected override void InitDialogContent(Control container)
        {
            base.InitDialogContent(container);

            this.Page.Form.Target = "innerFrame";

            viewerControl.Width = Unit.Percentage(100);
            viewerControl.Height = Unit.Percentage(100);

            InitProcessPopupMenu(this.processPopupMenuControl);
            InitActivityPopupMenu(this.activityPopupMenuControl);
            InitTransitionPopupMenu(this.transitionPopupMenuControl);

            container.Controls.Add(this.viewerControl);
            container.Controls.Add(this.processPopupMenuControl);
            container.Controls.Add(this.activityPopupMenuControl);
            container.Controls.Add(this.transitionPopupMenuControl);
        }

        protected override void InitDialogHeader(HtmlGenericControl titleContainer)
        {
            base.InitDialogHeader(titleContainer);

            if (CurrentProcess == null)
                return;

            if (CurrentProcess != CurrentProcess.RootProcess)
                ShowParentProcessLink(titleContainer);

            ShowAdvancedOperation(titleContainer);

            if ((WfClientContext.Current.InAdminMode && ShowMainStream == false)
                && WfRuntime.ProcessContext.EnableSimulation == false)
                this.advancedOpWrapperControl.Attributes["class"] = "wfrtvwc-normal";
            else
                this.advancedOpWrapperControl.Attributes["class"] = "wfrtvwc-none";

            this.ShowRuntimeParameters(titleContainer);
        }

        private WorkflowInfo GetPostedProcessInfo()
        {
            string data = this.Page.Request.Form[this.WfRuntimeViewerWorkflowInfoHiddenFieldID];

            WorkflowInfo result = null;

            if (data.IsNotEmpty())
                result = JSONSerializerExecute.Deserialize<WorkflowInfo>(data);

            return result;
        }

        private void ShowRuntimeParameters(Control container)
        {
            this.normalOpWrapperControl = CreateAdvancedOpWrapperControl(container, "运行时操作↓", "normalOpWrapper");

            var span = new HtmlGenericControl("span");
            span.Attributes["class"] = "wfrtvwc-container";
            this.normalOpWrapperControl.Controls.AddAt(0, span);

            var ul = new HtmlGenericControl("ul");
            ul.Attributes["class"] = "wfrtvwc-wrapper";
            span.Controls.Add(ul);

            RuntimeParametersViewer parametersControl = (RuntimeParametersViewer)this.FindControlByID("runtimeParametersViewer", true);

            AddAdvancedOpLink(
                ul,
                "查看流程参数",
                "#",
                string.Format("$find('{0}').openProcessParameters('{1}'); return false;", this.ClientID, parametersControl.ClientID));

            AddAdvancedOpLink(
                ul,
                "查看主线流程",
                "#",
                string.Format("$find('{0}').open(true); return false;", this.ClientID));


            container.Controls.Add(this.normalOpWrapperControl);
        }

        private void ShowParentProcessLink(Control container)
        {
            HtmlAnchor rootLink = new HtmlAnchor() { HRef = "#" };
            rootLink.Title = "点击查看主流程跟踪信息";
            rootLink.InnerText = CurrentProcess.RootProcess.Descriptor.Name;
            rootLink.Attributes["onclick"] =
                string.Format("$find('{0}').getWorkflowInfo('{1}'); return false;",
                this.ClientID, CurrentProcess.RootProcess.ID);

            HtmlAnchor currentLink = new HtmlAnchor() { HRef = "#" };
            currentLink.Style.Add("margin-left", "10px");
            currentLink.InnerText = CurrentProcess.Descriptor.Name;
            currentLink.Attributes["onclick"] =
                string.Format("$find('{0}').getWorkflowInfo('{1}'); return false;",
                this.ClientID, CurrentProcess.ID);

            HtmlGenericControl wrapper = new HtmlGenericControl("span");
            wrapper.InnerText = "相关流程：";
            wrapper.Style.Add("margin-left", "20px");
            wrapper.Style.Add("font-size", "12px");
            wrapper.Style.Add("font-weight", "normal");
            wrapper.Controls.Add(rootLink);
            wrapper.Controls.Add(currentLink);

            container.Controls.Add(wrapper);
        }

        private void ShowAdvancedOperation(Control container)
        {
            if (this.CurrentProcess == null)
                return;

            this.advancedOpWrapperControl = CreateAdvancedOpWrapperControl(container, "相关操作↓", "advancedOpWrapper");

            var span = new HtmlGenericControl("span");
            span.Attributes["class"] = "wfrtvwc-container";
            this.advancedOpWrapperControl.Controls.AddAt(0, span);

            var ul = new HtmlGenericControl("ul");
            ul.Attributes["class"] = "wfrtvwc-wrapper";
            span.Controls.Add(ul);

            AddAdvancedOpLink(ul, "编辑流程模板", "#",
                string.Format("$find('{0}').openDesigner('{1}'); return false;",
                    this.ClientID, WfRuntimeViewerSettings.GetConfig().DesignerUrl));

            //AddAdvancedOpLink(ul, "作废", "#",
            //    string.Format("$find('{0}').abortCurrentProcess(); return false;", this.ClientID));
            AddAdvancedOpLinkWithSubmitButton(ul, "作废", "#", "abortProcess", "确定要作废或取消流程吗？",
                "正在作废...",
                 WfActionAfterOperation.RefreshCurrent | WfActionAfterOperation.RefreshOpener, new EventHandler(AbortProcessButton_Click));

            //AddAdvancedOpLink(ul, "还原", "#",
            //    string.Format("$find('{0}').restoreCurrentProcess(); return false;", this.ClientID));
            AddAdvancedOpLinkWithSubmitButton(ul, "还原", "#", "restoreProcess", "确定要还原作废(取消)的流程吗？",
                "正在还原...",
                 WfActionAfterOperation.RefreshCurrent | WfActionAfterOperation.RefreshOpener, new EventHandler(RestoreProcessButton_Click));

            //AddAdvancedOpLink(ul, "暂停", "#",
            //    string.Format("$find('{0}').pauseCurrentProcess(); return false;", this.ClientID));
            AddAdvancedOpLinkWithSubmitButton(ul, "暂停", "#", "pauseProcess", "确定要暂停流程吗？",
                "正在暂停...",
                 WfActionAfterOperation.RefreshCurrent | WfActionAfterOperation.RefreshOpener, new EventHandler(PauseProcessButton_Click));

            //AddAdvancedOpLink(ul, "恢复", "#",
            //    string.Format("$find('{0}').resumeCurrentProcess(); return false;", this.ClientID));
            AddAdvancedOpLinkWithSubmitButton(ul, "恢复", "#", "resumeProcess", "确定要恢复暂停的流程吗？",
                "正在恢复...",
                 WfActionAfterOperation.RefreshCurrent | WfActionAfterOperation.RefreshOpener, new EventHandler(ResumeProcessButton_Click));

            //WithdrawProcessButton_Click
            //AddAdvancedOpLink(ul, "撤回", "#",
            //    string.Format("$find('{0}').withdrawCurrentProcess(); return false;", this.ClientID));
            AddAdvancedOpLinkWithSubmitButton(ul, "撤回", "#", "withdrawProcess", "确认要撤回吗？",
                "正在撤回...",
                 WfActionAfterOperation.RefreshCurrent | WfActionAfterOperation.RefreshOpener, new EventHandler(WithdrawProcessButton_Click));

            //AddAdvancedOpLink(ul, "退出维护模式", "#",
            //    string.Format("$find('{0}').exitCurrentProcessMaintainingStatus(); return false;", this.ClientID));
            AddAdvancedOpLinkWithSubmitButton(ul, "退出维护模式", "#", "exitCurrentProcessMaintainingStatus",
                "确认要退出维护状态吗？",
                "正在执行...",
                WfActionAfterOperation.RefreshCurrent | WfActionAfterOperation.RefreshOpener,
                new EventHandler(ExitCurrentProcessMaintainingStatus_Click));

            AddAdvancedOpLinkWithSubmitButton(ul, "异步作废", "#", "asyncCancelProcess",
                "确认要异步作废流程吗？",
                "正在执行...",
                WfActionAfterOperation.RefreshCurrent | WfActionAfterOperation.RefreshOpener,
                new EventHandler(AsyncCancelProcess_Click));

            AddAdvancedOpLinkWithSubmitButton(ul, "异步撤回", "#", "asyncWithdrawProcess",
                "确认要异步撤回流程吗？",
                "正在执行...",
                WfActionAfterOperation.RefreshCurrent | WfActionAfterOperation.RefreshOpener,
                new EventHandler(AsyncWithdrawProcess_Click));

            HtmlAnchor modifyAssigneeLink = AddAdvancedOpLink(ul, "修改活动操作人", "#",
                string.Empty, "在图中选择需要修改的流程活动点，然后点击此链接进行修改");

            Control userSelector = this.FindControlByID("userSelector", true);

            modifyAssigneeLink.Attributes["onclick"] = string.Format("$find('{0}').onChangeAssigneeClick('{1}');", this.ClientID, userSelector.ClientID);

            AddAdvancedOpLink(ul, "处理等待中的活动", "#",
                string.Format("$find('{0}').onChangePendingActivity(); return false;", this.ClientID));
        }

        private HtmlAnchor AddAdvancedOpLink(Control wrapper, string text, string url, string clickScript)
        {
            return AddAdvancedOpLink(wrapper, text, url, clickScript, string.Empty);
        }

        private HtmlAnchor AddAdvancedOpLink(Control wrapper, string text, string url, string clickScript, string title)
        {
            HtmlAnchor anchor = new HtmlAnchor();

            anchor.HRef = url;
            anchor.InnerText = Translator.Translate(Define.DefaultCulture, text);
            anchor.Attributes["onclick"] = clickScript;
            anchor.Attributes["title"] = Translator.Translate(Define.DefaultCulture, title);

            HtmlGenericControl li = new HtmlGenericControl("li");

            wrapper.Controls.Add(li);

            li.Controls.Add(anchor);

            return anchor;
        }

        private HtmlAnchor AddAdvancedOpLinkWithSubmitButton(Control wrapper, string text, string url, string buttonID, string promptText, string progressText, WfActionAfterOperation afterOperation, EventHandler serverClickHandler)
        {
            SubmitButton button = new SubmitButton();

            button.ID = buttonID;
            button.Style["display"] = "none";
            button.PopupCaption = Translator.Translate(Define.DefaultCulture, progressText);
            button.ProgressMode = SubmitButtonProgressMode.BySteps;
            button.CommandArgument = afterOperation.ToString();
            button.Click += serverClickHandler;
            wrapper.Controls.Add(button);

            promptText = Translator.Translate(Define.DefaultCulture, promptText);

            string script = string.Empty;
            string confirmedScript = string.Format("$get(\"{0}\").click()", button.ClientID);

            if (promptText.IsNotEmpty())
                script = string.Format("if (window.confirm(\"{0}\")) {1};return false", promptText, confirmedScript);
            else
                script = string.Format("{0}; return false;", confirmedScript);

            return AddAdvancedOpLink(wrapper, text, url, script);
        }

        #region Process Button Click Events
        private void AbortProcessButton_Click(object sender, EventArgs e)
        {
            DoProcessButtonAction(sender, e, () =>
                DoProcessCallbackOp(GetPostedProcessInfo(), currentProcess =>
                {
                    (currentProcess.Status == WfProcessStatus.NotRunning).TrueThrow("流程未启动，无法作废");

                    return new WfCancelProcessExecutor(
                        currentProcess.CurrentActivity,
                        currentProcess);
                }));
        }

        private void WithdrawProcessButton_Click(object sender, EventArgs e)
        {
            DoProcessButtonAction(sender, e, () =>
                DoProcessCallbackOp(GetPostedProcessInfo(), currentProcess =>
                {
                    (currentProcess.Status == WfProcessStatus.Running).FalseThrow("流程不在运行中，无法撤回");

                    return new WfWithdrawExecutor(
                        currentProcess.CurrentActivity,
                        currentProcess.CurrentActivity);
                }));
        }

        private void ExitCurrentProcessMaintainingStatus_Click(object sender, EventArgs e)
        {
            DoProcessButtonAction(sender, e, () =>
                DoProcessCallbackOp(GetPostedProcessInfo(), currentProcess =>
                {
                    (currentProcess.Status == WfProcessStatus.Maintaining).FalseThrow("流程不在维护状态，无法执行此操作");

                    return new WfExitMaintainingStatusExecutor(currentProcess.CurrentActivity, currentProcess);
                }));
        }

        private void AsyncCancelProcess_Click(object sender, EventArgs e)
        {
            DoProcessButtonAction(sender, e, () =>
                DoProcessCallbackOp(GetPostedProcessInfo(), currentProcess =>
                {
                    (currentProcess.Status == WfProcessStatus.Running).FalseThrow("流程不在运行中，无法作废");

                    return new WfAsyncCancelProcessExecutor(currentProcess.CurrentActivity, currentProcess);
                }));
        }

        private void AsyncWithdrawProcess_Click(object sender, EventArgs e)
        {
            DoProcessButtonAction(sender, e, () =>
                DoProcessCallbackOp(GetPostedProcessInfo(), currentProcess =>
                {
                    (currentProcess.Status == WfProcessStatus.Running).FalseThrow("流程不在运行中，无法撤回");

                    return new WfAsyncWithdrawExecutor(currentProcess.CurrentActivity, currentProcess);
                }));
        }

        private void RestoreProcessButton_Click(object sender, EventArgs e)
        {
            DoProcessButtonAction(sender, e, () =>
                DoProcessCallbackOp(GetPostedProcessInfo(), currentProcess =>
                {
                    (currentProcess.Status == WfProcessStatus.Aborted).FalseThrow("流程不是已经作废的，无法还原");

                    return new WfRestoreProcessExecutor(
                        currentProcess.CurrentActivity,
                        currentProcess);
                }));
        }

        private void PauseProcessButton_Click(object sender, EventArgs e)
        {
            DoProcessButtonAction(sender, e, () =>
                DoProcessCallbackOp(GetPostedProcessInfo(), currentProcess =>
                {
                    (currentProcess.Status == WfProcessStatus.Running).FalseThrow("流程不在运行中，无法暂停");

                    return new WfPauseProcessExecutor(
                        currentProcess.CurrentActivity,
                        currentProcess);
                }));
        }

        private void ResumeProcessButton_Click(object sender, EventArgs e)
        {
            DoProcessButtonAction(sender, e, () =>
                DoProcessCallbackOp(GetPostedProcessInfo(), currentProcess =>
                {
                    (currentProcess.Status == WfProcessStatus.Paused).FalseThrow("流程不在暂停中，无法恢复");

                    return new WfResumeProcessExecutor(
                        currentProcess.CurrentActivity,
                        currentProcess);
                }));
        }
        #endregion Process Button Click Events

        private void DoProcessButtonAction(object sender, EventArgs e, Action action)
        {
            action.NullCheck("action");

            ProcessProgress.Current.RegisterResponser(SubmitButtonProgressResponser.Instance);
            try
            {
                Button btn = (Button)sender;

                action();

                WfActionAfterOperation operations = (WfActionAfterOperation)Enum.Parse(typeof(WfActionAfterOperation), btn.CommandArgument);

                if ((operations & WfActionAfterOperation.RefreshOpener) != WfActionAfterOperation.None)
                    this.Page.Response.Write("<script type=\"text/javascript\">if (parent.opener) parent.opener.location.reload();</script>");

                if ((operations & WfActionAfterOperation.RefreshCurrent) != WfActionAfterOperation.None)
                    this.Page.Response.Write("<script type=\"text/javascript\">parent.location.reload();</script>");

                if ((operations & WfActionAfterOperation.Close) != WfActionAfterOperation.None)
                    WebUtility.ResponseCloseWindowScriptBlock();

                string script = "<script type=\"text/javascript\">parent.location.reload();</script>";
                this.Page.Response.Write(script);

            }
            catch (System.Exception ex)
            {
                WebUtility.ResponseShowClientErrorScriptBlock(ex);

            }
            finally
            {
                this.Page.Response.Write(SubmitButton.GetResetAllParentButtonsScript(true));
                this.Page.Response.End();
            }
        }

        private static HtmlGenericControl CreateAdvancedOpWrapperControl(Control container, string text, string controlID)
        {
            HtmlGenericControl wrapper = new HtmlGenericControl("span");

            wrapper.ID = controlID;
            HtmlGenericControl label = new HtmlGenericControl("a");
            label.Attributes["class"] = "wfrtvwc-lable";
            label.Attributes["href"] = "javascript:void(0);";
            label.InnerText = Translator.Translate(Define.DefaultCulture, text);
            wrapper.Controls.Add(label);
            //wrapper.Style.Add("margin-left", "20px");
            //wrapper.Style.Add("font-size", "12px");
            //wrapper.Style.Add("font-weight", "normal");
            wrapper.Attributes["class"] = "wfrtvwc-normal";

            container.Controls.Add(wrapper);

            return wrapper;
        }

        protected override void InitCancelButton(System.Web.UI.HtmlControls.HtmlInputButton cancelButton)
        {
            cancelButton.Value = "关闭(C)";
        }

        protected override void InitConfirmButton(System.Web.UI.HtmlControls.HtmlInputButton confirmButton)
        {
            confirmButton.Value = "日志(L)";
            confirmButton.Attributes["accesskey"] = "L";
            confirmButton.Attributes["onclick"] = string.Format("$find(\"{0}\").openLogView();", this.ClientID);
            confirmButton.Attributes["category"] = Define.DefaultCulture;
        }

        protected override string GetDialogFeature()
        {
            return GetWindowFeature().ToDialogFeatureClientString();
        }
        #endregion

        #region Private
        private WindowFeature GetWindowFeature()
        {
            WindowFeature feature = new WindowFeature();

            feature.Center = true;
            feature.Width = 800;
            feature.LeftScript = "(window.screen.width - 800) / 2";
            feature.Height = 600;
            feature.TopScript = "(window.screen.height - 600) / 2";
            feature.Resizable = true;
            feature.ShowScrollBars = false;
            feature.ShowToolBar = false;
            feature.ShowStatusBar = false;

            return feature;
        }

        /// <summary>
        /// 当前流程的实例对象。该实例从ProcessContext或ProcessID属性中恢复
        /// </summary>
        protected IWfProcess CurrentProcess
        {
            get
            {
                IWfProcess result = null;
                try
                {
                    if (WfClientContext.Current.CurrentActivity != null)
                    {
                        result = WfClientContext.Current.CurrentActivity.Process;
                    }
                }
                catch (System.Exception)
                {

                }

                if (result == null && this.ProcessID.IsNotEmpty())
                {
                    result = WfRuntime.GetProcessByProcessID(ProcessID);
                }

                return result;
            }
        }

        private void InitShowDialogControl()
        {
            if (TargetControl != null)
            {
                IAttributeAccessor target = (IAttributeAccessor)TargetControl;

                if (NeedToRender == false)
                {
                    target.SetAttribute("disabled", "true");
                    target.SetAttribute("class", "disable");
                }
                else
                {
                    target.SetAttribute("class", "enable");
                    target.SetAttribute("onclick", string.Format("$find('{0}').open(); return false;", this.ClientID));
                }
            }
        }

        [Browsable(false)]
        private bool NeedToRender
        {
            get
            {
                return this.Visible && WfClientContext.Current.OriginalActivity != null;
            }
        }
        #endregion

        [ScriptControlMethod]
        public string GetWorkflowInfo(string key)
        {
            List<WorkflowInfo> list = new List<WorkflowInfo>();

            if (string.Compare(CurrentProcess.ID, key, true) == 0)
            {
                list.Add(GetWorkflowInfo(CurrentProcess, ShowMainStream));
            }

            if (string.Compare(CurrentProcess.RootProcess.ID, key, true) == 0)
            {
                list.Add(GetWorkflowInfo(CurrentProcess.RootProcess, ShowMainStream));
            }

            return JSONSerializerExecute.Serialize(list);
        }

        [ScriptControlMethod]
        public string ChangeActivityAssignee(string userJsonStr, string activityID)
        {
            if (activityID.IsNullOrEmpty())
                return Translator.Translate(Define.DefaultCulture, "请先选择相应的活动！");

            WfConverterHelper.RegisterConverters();

            try
            {
                var userList = JSONSerializerExecute.Deserialize<List<OguUser>>(userJsonStr);
                var process = WfRuntime.GetProcessByActivityID(activityID);
                var activityInstance = process.Activities[activityID];

                if (activityInstance == null)
                    return Translator.Translate(Define.DefaultCulture, "未找到活动ID为{0}的流程！", activityID);

                WfReplaceAssigneesExecutor executor =
                    new WfReplaceAssigneesExecutor(
                        WfClientContext.Current.CurrentActivity,
                        activityInstance,
                        null,
                        userList);

                WfClientContext.Current.Execute(executor);

                return string.Empty;
            }
            catch (Exception ex)
            {
                ex = ExceptionHelper.GetRealException(ex);

                return Translator.Translate(Define.DefaultCulture, "修改当前活动点操作人失败！失败原因：{0}", ex.Message);
            }
        }

        [ScriptControlMethod]
        public string GetActivityRelatedUsers(string activityID)
        {
            var activityInstance = WfClientContext.Current.CurrentActivity.Process.Activities[activityID];

            if (activityInstance == null)
                return string.Empty;

            if (activityInstance.Assignees.Count > 0)
            {
                WfConverterHelper.RegisterConverters();

                var userList = activityInstance.Assignees.ToUsers();
                return JSONSerializerExecute.Serialize(userList);
            }

            WfAssigneeCollection candidates = activityInstance.Candidates.GetSelectedAssignees();

            if (candidates.Count > 0)
            {
                WfConverterHelper.RegisterConverters();

                IEnumerable<IUser> userList = candidates.ToUsers();
                return JSONSerializerExecute.Serialize(userList);
            }

            return string.Empty;
        }

        /// <summary>
        /// 作废当前流程
        /// </summary>
        [ScriptControlMethod]
        public void AbortCurrentProcess(string processID)
        {
            DoProcessCallbackOp(processID, currentProcess =>
            {
                (currentProcess.Status == WfProcessStatus.NotRunning).TrueThrow(Translator.Translate(Define.DefaultCulture, "流程未启动，无法作废"));

                return new WfCancelProcessExecutor(
                    currentProcess.CurrentActivity,
                    currentProcess);
            });
        }

        /// <summary>
        /// 还原当前流程
        /// </summary>
        [ScriptControlMethod]
        public void RestoreCurrentProcess(string processID)
        {
            DoProcessCallbackOp(processID, currentProcess =>
            {
                (currentProcess.Status == WfProcessStatus.Aborted).FalseThrow(Translator.Translate(Define.DefaultCulture, "流程不是已经作废的，无法还原"));

                return new WfRestoreProcessExecutor(
                    currentProcess.CurrentActivity,
                    currentProcess);
            });
        }

        [ScriptControlMethod]
        public void PauseCurrentProcess(string processID)
        {
            DoProcessCallbackOp(processID, currentProcess =>
            {
                (currentProcess.Status == WfProcessStatus.Running).FalseThrow(Translator.Translate(Define.DefaultCulture, "流程不在运行中，无法暂停"));

                return new WfPauseProcessExecutor(
                    currentProcess.CurrentActivity,
                    currentProcess);
            });
        }

        [ScriptControlMethod]
        public void ResumeCurrentProcess(string processID)
        {
            DoProcessCallbackOp(processID, currentProcess =>
            {
                (currentProcess.Status == WfProcessStatus.Paused).FalseThrow(Translator.Translate(Define.DefaultCulture, "流程不在暂停中，无法恢复"));

                return new WfResumeProcessExecutor(
                    currentProcess.CurrentActivity,
                    currentProcess);
            });
        }

        [ScriptControlMethod]
        public void GenerateCurrentProcessCandidates(string processID)
        {
            DoProcessCallbackAction(processID, currentProcess =>
            {
                (currentProcess.Status != WfProcessStatus.Aborted && currentProcess.Status != WfProcessStatus.Completed).
                    FalseThrow(Translator.Translate(Define.DefaultCulture, "流程已经作废或者完成，无法重新计算"));

                currentProcess.GenerateCandidatesFromResources();

                WfRuntime.ProcessContext.AffectedProcesses.AddOrReplace(currentProcess);

                WfRuntime.PersistWorkflows();
            });
        }

        /// <summary>
        /// 撤回当前流程
        /// </summary>
        /// <returns></returns>
        [ScriptControlMethod]
        public void WithdrawCurrentProcess(string processID)
        {
            DoProcessCallbackOp(processID, currentProcess =>
            {
                (currentProcess.Status == WfProcessStatus.Running).
                    FalseThrow(Translator.Translate(Define.DefaultCulture, "流程不在运行中，无法撤回"));

                return new WfWithdrawExecutor(
                    currentProcess.CurrentActivity,
                    currentProcess.CurrentActivity);
            });
        }

        /// <summary>
        /// 退出当前流程的维护状态
        /// </summary>
        /// <returns></returns>
        [ScriptControlMethod]
        public void ExitCurrentProcessMaintainingStatus(string processID)
        {
            DoProcessCallbackOp(processID, currentProcess =>
            {
                (currentProcess.Status == WfProcessStatus.Maintaining).FalseThrow("流程不在维护状态，无法执行此操作");

                return new WfExitMaintainingStatusExecutor(currentProcess.CurrentActivity, currentProcess);
            });
        }

        /// <summary>
        /// 处理当前Pending的活动
        /// </summary>
        /// <returns></returns>
        [ScriptControlMethod]
        public void ChangeProcessPendingActivity(string processID)
        {
            DoProcessCallbackAction(processID, currentProcess =>
            {
                currentProcess.ProcessPendingActivity();

                WfRuntime.ProcessContext.AffectedProcesses.AddOrReplace(this.CurrentProcess);
                WfRuntime.PersistWorkflows();
            });
        }

        private static void DoProcessCallbackOp(WorkflowInfo workflowInfo, Func<IWfProcess, WfExecutorBase> getExecutor)
        {
            (workflowInfo != null).FalseThrow(Translator.Translate(Define.DefaultCulture, "不能获取到当前流程信息"));

            IWfProcess currentProcess = WfRuntime.GetProcessByProcessID(workflowInfo.Key);

            WfClientContext.Current.ChangeTo(currentProcess.CurrentActivity);

            WfClientContext.Current.InAdminMode.FalseThrow(Translator.Translate(Define.DefaultCulture, "您没有权限执行此操作"));

            WfExecutorBase executor = getExecutor(currentProcess);

            WfClientContext.Current.Execute(executor);
        }

        /// <summary>
        /// 执行流程的回调操作
        /// </summary>
        /// <param name="processID"></param>
        /// <param name="getExecutor"></param>
        private static void DoProcessCallbackOp(string processID, Func<IWfProcess, WfExecutorBase> getExecutor)
        {
            IWfProcess currentProcess = WfRuntime.GetProcessByProcessID(processID);

            WfClientContext.Current.ChangeTo(currentProcess.CurrentActivity);

            WfClientContext.Current.InAdminMode.FalseThrow(Translator.Translate(Define.DefaultCulture, "您没有权限执行此操作"));

            WfExecutorBase executor = getExecutor(currentProcess);

            WfClientContext.Current.Execute(executor);
        }

        private static void DoProcessCallbackAction(string processID, Action<IWfProcess> action)
        {
            IWfProcess currentProcess = WfRuntime.GetProcessByProcessID(processID);

            WfClientContext.Current.ChangeTo(currentProcess.CurrentActivity);

            WfClientContext.Current.InAdminMode.FalseThrow(Translator.Translate(Define.DefaultCulture, "您没有权限执行此操作"));

            action(currentProcess);
        }

        private static void DoProcessCallbackAction(WorkflowInfo workflowInfo, Action<IWfProcess> action)
        {
            (workflowInfo != null).FalseThrow(Translator.Translate(Define.DefaultCulture, "不能获取到当前流程信息"));

            IWfProcess currentProcess = WfRuntime.GetProcessByProcessID(workflowInfo.Key);

            WfClientContext.Current.ChangeTo(currentProcess.CurrentActivity);

            WfClientContext.Current.InAdminMode.FalseThrow(Translator.Translate(Define.DefaultCulture, "您没有权限执行此操作"));

            action(currentProcess);
        }

        private static WorkflowInfo GetWorkflowInfo(IWfProcess process, bool isMainStream)
        {
            return WorkflowInfo.ProcessAdapter(process, isMainStream);
        }

        /// <summary>
        /// 初始化流程菜单菜单项
        /// </summary>
        /// <param name="menu"></param>
        private void InitProcessPopupMenu(DeluxeMenu menu)
        {
            menu.StaticDisplayLevels = 0;

            menu.Items.Add(new MenuItem() { Text = "编辑流程属性", NavigateUrl = string.Format("javascript:$find('{0}').editProcessProperties()", this.ClientID) });
            menu.Items.Add(new MenuItem() { Text = "增加活动", NavigateUrl = string.Format("javascript:$find('{0}').addActivity()", this.ClientID) });

            if (this.ShowMainStream == false)
                menu.Items.Add(new MenuItem() { Text = "自动流转", NavigateUrl = string.Format("javascript:$find('{0}').autoMoveTo()", this.ClientID) });

            menu.Items.Add(new MenuItem() { Text = "重新计算候选人", NavigateUrl = string.Format("javascript:$find('{0}').generateCurrentProcessCandidates()", this.ClientID) });
        }

        /// <summary>
        /// 初始化活动菜单的菜单项
        /// </summary>
        /// <param name="menu"></param>
        private void InitActivityPopupMenu(DeluxeMenu menu)
        {
            menu.StaticDisplayLevels = 0;

            menu.Items.Add(new MenuItem() { Text = "编辑活动属性", NavigateUrl = string.Format("javascript:$find('{0}').editActivityProperties()", this.ClientID) });
            menu.Items.Add(new MenuItem() { Text = "增加活动", NavigateUrl = string.Format("javascript:$find('{0}').addActivity()", this.ClientID) });
            menu.Items.Add(new MenuItem() { Text = "增加连线", NavigateUrl = string.Format("javascript:$find('{0}').addTransition()", this.ClientID) });
            menu.Items.Add(new MenuItem() { Text = "删除活动", NavigateUrl = string.Format("javascript:$find('{0}').deleteActivity()", this.ClientID) });
        }

        /// <summary>
        /// 初始化线的菜单
        /// </summary>
        /// <param name="menu"></param>
        private void InitTransitionPopupMenu(DeluxeMenu menu)
        {
            menu.StaticDisplayLevels = 0;

            menu.Items.Add(new MenuItem() { Text = "编辑连线属性", NavigateUrl = string.Format("javascript:$find('{0}').editTransitionProperties()", this.ClientID) });
            menu.Items.Add(new MenuItem() { Text = "删除连线", NavigateUrl = string.Format("javascript:$find('{0}').deleteTransition()", this.ClientID) });
        }
    }
}
