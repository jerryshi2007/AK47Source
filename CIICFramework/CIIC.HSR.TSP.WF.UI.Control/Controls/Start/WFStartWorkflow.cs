using CIIC.HSR.TSP.WebComponents;
using CIIC.HSR.TSP.WebComponents.Widgets.Button;
using CIIC.HSR.TSP.WF.Bizlet.Impl;
using CIIC.HSR.TSP.WF.UI.Control.Controls.Abstract;
using CIIC.HSR.TSP.WebComponents.Widgets.DropDownButton;
using CIIC.HSR.TSP.WF.UI.Control.Interfaces;
using MCS.Library.WF.Contracts.Json.Converters;
using MCS.Web.Library.Script;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using CIIC.HSR.TSP.WF.UI.Control.DefaultActions;
using Translator = MCS.Library.Globalization.Translator;
using MCS.Library.WF.Contracts.Ogu;

namespace CIIC.HSR.TSP.WF.UI.Control.Controls.StartWorkflow
{
    /// <summary>
    /// 启动工作流控件
    /// </summary>
    [WFControlDescriptionAttribute(WFDefaultActionUrl.StartWorkflow, "$.fn.HSR.Controls.WFStartWorkflow.Click", "提交")]
    public class WFStartWorkflow : WFControlBase
    {
        public WFStartWorkflow(ViewContext vc, ViewDataDictionary vdd)
            : base(vc, vdd)
        {

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
        /// 流程模板ID
        /// </summary>
        public string TemplateKey
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

        protected override bool GetEnabled()
        {
            return string.IsNullOrEmpty(this.TemplateKey) == false;
        }

        protected override WFParameterWithResponseBase PrepareParameters()
        {
            WFStartWorkflowParameter param = new WFStartWorkflowParameter();
            WFUIControlCommon.SetSelectCandidates(param, this.ViewContext.HttpContext.Request.GetWFContext(), this.TemplateKey, DictionaryWfClientUser);
            param.ProcessStartupParams = null;
            param.TemplateKey = this.TemplateKey;
            param.TaskTitle = this.TaskTitle;
            param.BusinessUrl = this.BusinessUrl;

            //取得意见ID
            WFUIRuntimeContext runtime = this.ViewContext.HttpContext.Request.GetWFContext();
            param.ClientOpinionId = WFUIControlCommon.GetCurrentOpinionId(runtime);

            return param;
        }

        protected override void InitWidgetAttributes(WidgetBase widget)
        {
            WfClientJsonConverterHelper.Instance.RegisterConverters();
            this.InnerDropDownButton.Items.Add(new DropItem()
            {
                ClientHandler = "$.fn.HSR.Controls.WFStartWorkflow.ItemClick",
                EnableDialog = false,
                Title = Translator.Translate(CultureDefine.DefaultCulture, "保存草稿"),
                Enabled = true
            });

            this.InnerDropDownButton.ClientClick = this.ClientButtonClickScript;
            this.InnerDropDownButton.Enabled = this.GetEnabled();
            this.InnerDropDownButton.Visible = this.GetEnabled();
        }

        public override void WriteHtml(System.IO.StringWriter stringWriter)
        {
            string mvcHtmlStr = WFUIControlCommon.WriteMvcHtmlString(this.Name, true);
            stringWriter.Write(mvcHtmlStr);

            base.WriteHtml(stringWriter);
        }
    }
}
