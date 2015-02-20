using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Web.Library.Script;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.UI;
using MCS.Library.SOA.DataObjects.Workflow.DTO;
using MCS.Web.WebControls.Configuration;
using MCS.Library.Configuration;
using MCS.Library.Core;
using System.Reflection;
using MCS.Web.Library;

[assembly: WebResource("MCS.Web.WebControls.Workflow.WfRuntimeViewer.Silverlight.js", "application/x-javascript")]
namespace MCS.Web.WebControls
{
    public class WfRuntimeViewer : WebControl, INamingContainer
    {
        public static readonly string CLIENTID_JSONINFO_SUFFIX = "_JsonInfo";
        public static readonly string CLIENTID_SILVERLIGHT_SUFFIX = "_SLP";
        public static readonly string CULTUREINFO_CATEGORY_NAME = "SOAWebControls";

        private readonly WfRuntimeViewerDialog _DialogControl = new WfRuntimeViewerDialog() { ID = "BranchProcessesDialog" };

        public WorkflowInfo InitializeValue { get; set; }

        public string SilverlightControlClientID
        {
            get
            {
                return this.ClientID + CLIENTID_SILVERLIGHT_SUFFIX;
            }
        }

        public string WorkflowInfoHideenFieldID
        {
            get
            {
                return this.ClientID + CLIENTID_JSONINFO_SUFFIX;
            }
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            this.Controls.Add(this._DialogControl);

            this.Page.ClientScript.RegisterClientScriptResource(this.GetType(), "MCS.Web.WebControls.Workflow.WfRuntimeViewer.Silverlight.js");
            this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "SilverlightInteractionScript", this.CreateClientScriptBlock().ToString(), true);

            HtmlGenericControl wrapper = new HtmlGenericControl("div");
            wrapper.InnerHtml = CreateSilverlightObject().ToString();

            Controls.Add(wrapper);
        }

        protected override void OnPreRender(EventArgs e)
        {
            this.Page.ClientScript.RegisterHiddenField(WorkflowInfoHideenFieldID, JSONSerializerExecute.Serialize(this.InitializeValue));

            InitializeCultrueInfo();

            base.OnPreRender(e);
        }

        private string CreateSilverlightObject()
        {
            var xapPath = WfRuntimeViewerSettings.GetConfig().XapUrl;

            string result = Assembly.GetExecutingAssembly().LoadStringFromResource(@"MCS.Web.WebControls.Workflow.WfRuntimeViewer.SilverlightObjectResource.htm");
            result = result.Replace("{OBJECTID}", this.SilverlightControlClientID);
            result = result.Replace("{OBJECTPATH}", xapPath);
            result = result.Replace("{INITPARAMS}", this.ClientID);

            return result;
        }

        private string CreateClientScriptBlock()
        {
            string result = Assembly.GetExecutingAssembly().LoadStringFromResource(@"MCS.Web.WebControls.Workflow.WfRuntimeViewer.InteractionFnResource.js");
            result = result.Replace("{JSONINFO}", CLIENTID_JSONINFO_SUFFIX);
            result = result.Replace("{DIALOGCONTROLID}", this._DialogControl.ClientID);
            result = result.Replace("{SILVERLIGHTOBJECTID}", CLIENTID_SILVERLIGHT_SUFFIX);
            result = result.Replace("{CULTUREINFOCATEGORY}", CULTUREINFO_CATEGORY_NAME);

            return result;
        }

        private static void InitializeCultrueInfo()
        {
            DeluxeNameTableContextCache.Instance.Add(CULTUREINFO_CATEGORY_NAME, "图例");
            DeluxeNameTableContextCache.Instance.Add(CULTUREINFO_CATEGORY_NAME, "设计的活动节点");
            DeluxeNameTableContextCache.Instance.Add(CULTUREINFO_CATEGORY_NAME, "新增的活动节点");

            DeluxeNameTableContextCache.Instance.Add(CULTUREINFO_CATEGORY_NAME, "模版活动节点");
            DeluxeNameTableContextCache.Instance.Add(CULTUREINFO_CATEGORY_NAME, "等待中的活动点");
            DeluxeNameTableContextCache.Instance.Add(CULTUREINFO_CATEGORY_NAME, "克隆已运行活动节点");
            DeluxeNameTableContextCache.Instance.Add(CULTUREINFO_CATEGORY_NAME, "克隆设计活动节点");
            DeluxeNameTableContextCache.Instance.Add(CULTUREINFO_CATEGORY_NAME, "克隆新增加活动节点");
            DeluxeNameTableContextCache.Instance.Add(CULTUREINFO_CATEGORY_NAME, "未运行活动路径");
            DeluxeNameTableContextCache.Instance.Add(CULTUREINFO_CATEGORY_NAME, "已禁用活动路径");

            DeluxeNameTableContextCache.Instance.Add(CULTUREINFO_CATEGORY_NAME, "删除的活动节点");
            DeluxeNameTableContextCache.Instance.Add(CULTUREINFO_CATEGORY_NAME, "当前活动节点");
            DeluxeNameTableContextCache.Instance.Add(CULTUREINFO_CATEGORY_NAME, "已完成的活动节点");
            DeluxeNameTableContextCache.Instance.Add(CULTUREINFO_CATEGORY_NAME, "已完成活动路径");
            DeluxeNameTableContextCache.Instance.Add(CULTUREINFO_CATEGORY_NAME, "包含分支流程的活动节点");
            DeluxeNameTableContextCache.Instance.Add(CULTUREINFO_CATEGORY_NAME, "流程信息");
            DeluxeNameTableContextCache.Instance.Add(CULTUREINFO_CATEGORY_NAME, "流程ID");
            DeluxeNameTableContextCache.Instance.Add(CULTUREINFO_CATEGORY_NAME, "资源ID");
            DeluxeNameTableContextCache.Instance.Add(CULTUREINFO_CATEGORY_NAME, "状态");
            DeluxeNameTableContextCache.Instance.Add(CULTUREINFO_CATEGORY_NAME, "创建人");
            DeluxeNameTableContextCache.Instance.Add(CULTUREINFO_CATEGORY_NAME, "所属组织");
            DeluxeNameTableContextCache.Instance.Add(CULTUREINFO_CATEGORY_NAME, "开始时间");
            DeluxeNameTableContextCache.Instance.Add(CULTUREINFO_CATEGORY_NAME, "结束时间");
            DeluxeNameTableContextCache.Instance.Add(CULTUREINFO_CATEGORY_NAME, "名称");
            DeluxeNameTableContextCache.Instance.Add(CULTUREINFO_CATEGORY_NAME, "操作人");
            DeluxeNameTableContextCache.Instance.Add(CULTUREINFO_CATEGORY_NAME, "活动");
            DeluxeNameTableContextCache.Instance.Add(CULTUREINFO_CATEGORY_NAME, "模板");
            DeluxeNameTableContextCache.Instance.Add(CULTUREINFO_CATEGORY_NAME, "运行中");
            DeluxeNameTableContextCache.Instance.Add(CULTUREINFO_CATEGORY_NAME, "已完成");
            DeluxeNameTableContextCache.Instance.Add(CULTUREINFO_CATEGORY_NAME, "被终止");
            DeluxeNameTableContextCache.Instance.Add(CULTUREINFO_CATEGORY_NAME, "未运行");
        }
    }
}
