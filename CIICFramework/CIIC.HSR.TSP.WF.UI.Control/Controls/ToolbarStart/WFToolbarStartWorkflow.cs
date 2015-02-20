using CIIC.HSR.TSP.WebComponents;
using CIIC.HSR.TSP.WebComponents.Widgets.Button;
using CIIC.HSR.TSP.WF.UI.Control.Controls.Abstract;
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
using CIIC.HSR.TSP.WF.UI.Control.Controls.StartWorkflow;
using CIIC.HSR.TSP.WebComponents.Widgets.Toolbar;
using CIIC.HSR.TSP.WF.Bizlet.Common;
using MCS.Library.WF.Contracts.Workflow.Runtime;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using MCS.Library.WF.Contracts.Proxies;
using MCS.Library.WF.Contracts.Ogu;
using System.Web;
using CIIC.HSR.TSP.WF.UI.Control.DefaultActions;
using MCS.Library.Globalization;

namespace CIIC.HSR.TSP.WF.UI.Control.Controls.ToolbarStartWorkflow
{
    /// <summary>
    /// 启动工作流控件
    /// </summary>
    [WFControlDescriptionAttribute(WFDefaultActionUrl.StartWorkflow, "$.fn.HSR.Controls.WFStartWorkflow.Click", "提交")]
    public class WFToolbarStartWorkflow : WFControlBase
    {
        private readonly ViewContext vcToolbar = null;
        private readonly ViewDataDictionary vddToolbar = null;
        private bool m_Enabled = true;
        private bool m_Enabled_Split = false;
        private ButtonTypes m_ButtonType = ButtonTypes.Submit;
        private List<WidgetBase> _InnerControls = new List<WidgetBase>();
        //private bool m_IsSelectCandidates = false;

        public WFToolbarStartWorkflow(ViewContext vc, ViewDataDictionary vdd)
            : base(vc, vdd)
        {
            this.vcToolbar = vc;
            this.vddToolbar = vdd;
        }

        /// <summary>
        /// 下拉按钮控件
        /// </summary>
        private Toolbar InnerToolbar
        {
            get
            {
                return (Toolbar)this.Widget;
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
        /// 按钮名称
        /// </summary>
        public string Text
        {
            get;
            set;
        }

        /// <summary>
        /// 对话框名称
        /// </summary>
        public string DialogText
        {
            get;
            set;
        }

        /// <summary>
        /// 对话框
        /// </summary>
        public bool ProgressBar
        {
            get;
            set;
        }

        /// <summary>
        /// 图标
        /// </summary>
        public string IconList
        {
            get;
            set;
        }

        /// <summary>
        /// 按钮大小
        /// </summary>
        /// <summary>
        /// <summary>
        /// 大小模式
        /// </summary>
        public SizeModes SizeMode
        {
            get;
            set;
        }

        /// <summary>
        /// 是否需要保存按钮
        /// </summary>
        public bool IsShowDraft
        {
            get { return m_Enabled; }
            set { m_Enabled = value; }
        }

        /// <summary>
        /// 是否要分割按钮
        /// </summary>
        public bool IsSplit
        {
            get { return m_Enabled_Split; }
            set { m_Enabled_Split = value; }
        }

        /// <summary>
        /// 控件类型
        /// </summary>
        public ButtonTypes ButtonType
        {
            get { return m_ButtonType; }
            set { m_ButtonType = value; }
        }

        /// <summary>
        /// 内部的按钮列表
        /// </summary>
        public List<WidgetBase> InnerControls
        {
            get { return _InnerControls; }
            set { _InnerControls = value; }
        }

        /// <summary>
        /// 添加组中的控件
        /// </summary>
        /// <param name="control">控件</param>
        public void AddControl(WidgetBase control)
        {
            _InnerControls.Add(control);
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

        protected override void InitWidgetAttributes(WidgetBase widget)
        {

            WfClientJsonConverterHelper.Instance.RegisterConverters();
            string clickEvent = "$.fn.HSR.Controls.WFStartWorkflow.Click";
            //遍历图标字符串
            string[] IconListResult = new string[0];
            if (!string.IsNullOrEmpty(IconList))
            {
                IconListResult = IconList.Split(',');
            }

            this.InnerToolbar.Groups = new List<ButtonGroup>();
            ButtonGroup buttonGroup = new ButtonGroup(vcToolbar, vddToolbar);

            //启动按钮添加
            Button buttonStart = new Button(vcToolbar, vddToolbar);
            buttonStart.Enabled = this.GetEnabled();
            buttonStart.Name = this.Name + "Start";
            buttonStart.EnableDialog = true;
            buttonStart.DialogText = this.DialogText;
            buttonStart.ProgressBar = this.ProgressBar;
            buttonStart.ClientClick = clickEvent;
            buttonStart.Text = this.Text;
            buttonStart.SizeMode = this.SizeMode;
            buttonStart.ButtonType = this.ButtonType;
            buttonStart.HtmlAttributes[Consts.WFParas] = SerializedParam(true);

            if (IconListResult.Any())
            {
                buttonStart.Icon = IconListResult[0];
            }

            buttonGroup.AddControl(buttonStart);
            this.InnerToolbar.AddGroup(buttonGroup);
            if (IsShowDraft)
            {
                //保存草稿按钮
                Button buttonSave = new Button(vcToolbar, vddToolbar);
                buttonSave.Enabled = this.GetEnabled();
                buttonSave.Name = this.Name + "Save";
                buttonSave.EnableDialog = true;
                buttonSave.DialogText = Translator.Translate(CultureDefine.DefaultCulture, "您确定要保存草稿吗？");
                buttonSave.ProgressBar = this.ProgressBar;
                buttonSave.ClientClick = clickEvent;
                buttonSave.Text = Translator.Translate(CultureDefine.DefaultCulture, "保存草稿");
                buttonSave.SizeMode = this.SizeMode;
                buttonSave.ButtonType = this.ButtonType;
                buttonSave.HtmlAttributes[Consts.WFParas] = SerializedParam(false);

                if (IconListResult.Count() >= 2)
                    buttonSave.Icon = IconListResult[1];

                if (IsSplit)
                {
                    ButtonGroup buttonGroupSave = new ButtonGroup(vcToolbar, vddToolbar);
                    buttonGroupSave.AddControl(buttonSave);
                    this.InnerToolbar.AddGroup(buttonGroupSave);
                }
                else
                {
                    buttonGroup.AddControl(buttonSave);
                }

            }
            foreach (var item in InnerControls)
            {
                if (IsSplit)
                {
                    ButtonGroup buttonGroupTemp = new ButtonGroup(vcToolbar, vddToolbar);
                    buttonGroupTemp.AddControl(item);
                    this.InnerToolbar.AddGroup(buttonGroupTemp);
                }
                else
                {
                    buttonGroup.AddControl(item);
                }
            }
        }

        private string SerializedParam(bool isAutoNext)
        {
            WFStartWorkflowParameter param = new WFStartWorkflowParameter();
            base.InitBasicParameterProperties(param);
            param.ProcessStartupParams = null;
            param.TemplateKey = this.TemplateKey;
            param.TaskTitle = this.TaskTitle;
            param.BusinessUrl = this.BusinessUrl;
            param.AutoNext = isAutoNext;
            //取得意见ID
            WFUIRuntimeContext runtime = this.ViewContext.HttpContext.Request.GetWFContext();
            param.ClientOpinionId = WFUIControlCommon.GetCurrentOpinionId(runtime);

            if (isAutoNext)
                WFUIControlCommon.SetSelectCandidates(param, this.ViewContext.HttpContext.Request.GetWFContext(), this.TemplateKey, DictionaryWfClientUser);

            string serializedParam = JSONSerializerExecute.Serialize(param);

            return serializedParam;
        }

        public override void WriteHtml(System.IO.StringWriter stringWriter)
        {
            string mvcHtmlStr = WFUIControlCommon.WriteMvcHtmlString(this.Name, false);
            stringWriter.Write(mvcHtmlStr);

            base.WriteHtml(stringWriter);
        }
    }
}
