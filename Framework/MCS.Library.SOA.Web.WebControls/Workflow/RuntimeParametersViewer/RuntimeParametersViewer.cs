using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using MCS.Library.Core;
using MCS.Library.Globalization;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library;
using MCS.Web.Library.MVC;
using MCS.Web.Library.Script;
using System.Web.UI.WebControls;
using MCS.Library.Principal;

[assembly: WebResource("MCS.Web.WebControls.Workflow.RuntimeParametersViewer.RuntimeParametersViewer.js", "application/x-javascript")]
[assembly: WebResource("MCS.Web.WebControls.Workflow.RuntimeParametersViewer.RuntimeParametersTemplate.htm", "text/html")]

namespace MCS.Web.WebControls
{
    [RequiredScript(typeof(ControlBaseScript), 1)]
    [RequiredScript(typeof(HBCommonScript), 2)]
    [ClientScriptResource("MCS.Web.WebControls.RuntimeParametersViewer", "MCS.Web.WebControls.Workflow.RuntimeParametersViewer.RuntimeParametersViewer.js")]
    [DialogTemplate("MCS.Web.WebControls.Workflow.RuntimeParametersViewer.RuntimeParametersTemplate.htm", "MCS.Library.SOA.Web.WebControls")]
    [ToolboxData("<{0}:RuntimeParametersViewer runat=server></{0}:RuntimeParametersViewer>")]
    public sealed class RuntimeParametersViewer : DialogControlBase<RuntimeParametersViewerParams>
    {
        private DeluxeGrid _ProcessParametersGrid = null;
        private Button _refreshButton = null;

        public RuntimeParametersViewer()
        {
            WfConverterHelper.RegisterConverters();
        }

        protected override string GetDialogFeature()
        {
            WindowFeature feature = new WindowFeature();

            feature.Center = true;
            feature.Width = 720;
            feature.Height = 480;
            feature.Resizable = true;
            feature.ShowScrollBars = false;
            feature.ShowToolBar = false;
            feature.ShowStatusBar = false;

            return feature.ToDialogFeatureClientString();
        }

        protected override Control LoadDialogTemplate()
        {
            ScriptManager sm = null;
            ScriptControlHelper.EnsureScriptManager(ref sm, this.Page);

            return base.LoadDialogTemplate();
        }

        protected override void InitDialogContent(Control container)
        {
            base.InitDialogContent(container);

            this._ProcessParametersGrid = (DeluxeGrid)this.FindControlByID("RuntimeParametersInfoDeluxeGrid", true);
            this._ProcessParametersGrid.PageIndexChanging += new System.Web.UI.WebControls.GridViewPageEventHandler(ProcessParametersGrid_PageIndexChanging);

            string editorTitle = Translator.Translate(Define.DefaultCulture, "编辑流程参数");

            HtmlInputButton editBtn = (HtmlInputButton)this.FindControlByID("editBtn", true);

            if (editBtn != null)
            {
                editBtn.Visible = CurrentProcess != null && WfClientContext.IsProcessAdmin(DeluxeIdentity.CurrentUser, CurrentProcess);
                editBtn.Attributes["onclick"] = string.Format("$find(\"{0}\").showEditParametersDialog();", this.ClientID);
            }

            this._refreshButton = (Button)this.FindControlByID("refreshButton", true);
        }

        private void ProcessParametersGrid_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
        {
            this._ProcessParametersGrid = (DeluxeGrid)this.FindControlByID("RuntimeParametersInfoDeluxeGrid", true);
            this._ProcessParametersGrid.PageIndex = e.NewPageIndex;
        }

        protected override void InitDialogTitle(HtmlHead header)
        {
            if (this.DialogTitle.IsNullOrEmpty())
                this.DialogTitle = "流程参数列表";

            base.InitDialogTitle(header);
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (this.ShowingMode == ControlShowingMode.Dialog && this._ProcessParametersGrid != null)
            {
                this.BindDataGrid(this._ProcessParametersGrid);
            }

            base.OnPreRender(e);
        }

        [ScriptControlProperty()]
        [ClientPropertyName("editParametersUrl")]
        private string EditParametersUrl
        {
            get
            {
                string result = "/MCSWebApp/OACommonPages/AppTrace/ProcessParametersEditor.aspx";

                if (ResourceUriSettings.GetConfig().Paths.ContainsKey("processParametersEditor"))
                    result = ResourceUriSettings.GetConfig().Paths["processParametersEditor"].Uri.ToString();

                if (result.IndexOf("?") != -1)
                    result += "&";
                else
                    result += "?";

                result += string.Format("processID={0}&autoClose=true&{1}={2}",
                    HttpUtility.UrlEncode(this.ProcessID),
                    GlobalizationWebHelper.LanguageParameterName,
                    HttpUtility.UrlEncode(GlobalizationWebHelper.GetCurrentHandlerLanguageID()));

                return result;
            }
        }

        [ScriptControlProperty()]
        [ClientPropertyName("refreshButtonClientID")]
        private string RefreshButtonClientID
        {
            get
            {
                string result = string.Empty;

                if (this._refreshButton != null)
                    result = this._refreshButton.ClientID;

                return result;
            }
        }

        [DefaultValue("")]
        [Category("文档")]
        [Description("流程ID")]
        public string ProcessID
        {
            get
            {
                return ControlParams.ProcessID;
            }
            set
            {
                ControlParams.ProcessID = value;
            }
        }

        /// <summary>
        /// 当前流程的实例对象。该实例从ProcessContext或ProcessID属性中恢复
        /// </summary>
        private IWfProcess CurrentProcess
        {
            get
            {
                IWfProcess result = null;

                try
                {
                    if (this.ProcessID.IsNotEmpty())
                    {
                        result = WfRuntime.GetProcessByProcessID(ProcessID);
                    }
                    else if (WfClientContext.Current.CurrentActivity != null)
                    {
                        result = WfClientContext.Current.CurrentActivity.Process;
                    }
                }
                catch (System.Exception)
                {
                }

                return result;
            }
        }

        private void BindDataGrid(DeluxeGrid dataGrid)
        {
            List<RuntimeParameterInfo> source = new List<RuntimeParameterInfo>();

            GetProcessParameters(this.CurrentProcess, source);

            dataGrid.DataSource = source;
            dataGrid.DataBind();
        }

        private void GetProcessParameters(IWfProcess process, List<RuntimeParameterInfo> source)
        {
            string processName = string.IsNullOrEmpty(process.Descriptor.Name) ? process.Descriptor.Name : process.Descriptor.Key;

            foreach (KeyValuePair<string, object> item in process.ApplicationRuntimeParameters)
            {
                RuntimeParameterInfo currentInfo = new RuntimeParameterInfo(item);
                //currentInfo.ProcessName = processName;

                source.Add(currentInfo);
            }

            /*if (process.Descriptor.Properties.GetValue("ProbeParentProcessParams", false) == true)
                GetProcessParameters(process.EntryInfo.OwnerActivity.Process, source); */
        }

        internal sealed class RuntimeParameterInfo
        {
            public RuntimeParameterInfo(KeyValuePair<string, object> runtimeInfo)
            {
                this.ParameterName = runtimeInfo.Key;

                if (runtimeInfo.Value != null)
                {
                    this.ParameterType = runtimeInfo.Value.GetType().ToString();
                    this.ParameterValue = runtimeInfo.Value.ToString();
                }
            }

            //public string ProcessName { get; set; }

            public string ParameterName { get; set; }

            public string ParameterType { get; set; }

            public string ParameterValue { get; set; }
        }
    }
}
