using CIIC.HSR.TSP.WebComponents;
using CIIC.HSR.TSP.WebComponents.Widgets.Button;
using CIIC.HSR.TSP.WebComponents.Widgets.Toolbar;
using CIIC.HSR.TSP.WF.Bizlet.Common;
using CIIC.HSR.TSP.WF.UI.Control.Controls.Abstract;
using CIIC.HSR.TSP.WF.UI.Control.Controls.StartWorkflow;
using CIIC.HSR.TSP.WF.UI.Control.DefaultActions;
using CIIC.HSR.TSP.WF.UI.Control.Interfaces;
using MCS.Library.Globalization;
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

namespace CIIC.HSR.TSP.WF.UI.Control.Controls.ToolbarMoveToWorkflow
{
    /// <summary>
    /// 流转工作流控件
    /// </summary>
    [WFControlDescriptionAttribute(WFDefaultActionUrl.MoveToDefault, "$.fn.HSR.Controls.WFMoveTo.Click")]

    public class WFToolbarMoveToWorkflow : WFControlBase
    {
        private bool m_Enabled_Split = false;
        private ButtonTypes m_ButtonType = ButtonTypes.Submit;
        private List<WidgetBase> _InnerControls = new List<WidgetBase>();

        private bool IsAllow = false;
        private bool IsMulti = false;
        private readonly ViewContext vcToolbar;
        private readonly ViewDataDictionary vddToolbar;

        public WFToolbarMoveToWorkflow(ViewContext vc, ViewDataDictionary vdd)
            : base(vc, vdd)
        {
            WFUIRuntimeContext runtime = this.ViewContext.HttpContext.Request.GetWFContext();
            if (runtime != null && runtime.Process != null)
            {
                //是否可选择审批人
                IsAllow = runtime.Process.CurrentActivity.Descriptor.Properties.GetValue("AllowSelectCandidates", false);
                IsMulti = runtime.Process.CurrentActivity.Descriptor.Properties.GetValue("AllowAssignToMultiUsers", true);
            }
            vcToolbar = vc;
            vddToolbar = vdd;
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
        /// 大小模式
        /// </summary>
        public SizeModes SizeMode
        {
            get;
            set;
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
        /// 动态角色审批人列表
        /// </summary>
        public Dictionary<string, List<WfClientUser>> DictionaryWfClientUser
        {
            get;
            set;
        }

        /// <summary>
        /// 添加组中的控件
        /// </summary>
        /// <param name="control">控件</param>
        public void AddControl(WidgetBase control)
        {
            _InnerControls.Add(control);
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
            //图标
            string icon = "";
            //按钮文字
            string btnText;
            //属性序列化
            string serializedParam;
            //确认框文本
            string dialogText;
            WfClientJsonConverterHelper.Instance.RegisterConverters();
            WFUIRuntimeContext runtime = this.ViewContext.HttpContext.Request.GetWFContext();
            this.InnerToolbar.Groups = new List<ButtonGroup>();
            this.InnerToolbar.Id = !string.IsNullOrEmpty(this.Id) ? this.Id : this.Name;
            ButtonGroup buttonGroup = new ButtonGroup(vcToolbar, vddToolbar);
            //通用属性设置
            WFMoveToParameter param = new WFMoveToParameter();
            base.InitBasicParameterProperties(param);
            param.IsDefault = true;
            param.TransferParameter = null;
            param.TaskTitle = this.TaskTitle;
            param.BusinessUrl = this.BusinessUrl;
            //取得意见ID
            param.ClientOpinionId = WFUIControlCommon.GetCurrentOpinionId(runtime);
            //遍历图标字符串
            string[] IconListResult = new string[0];
            if (!string.IsNullOrEmpty(IconList))
            {
                IconListResult = IconList.Split(',');
            }

            //流程设计器线的属性
            if (runtime != null && runtime.Process != null)
            {
                //是否可选择审批人
                for (int i = 0; i < runtime.Process.NextActivities.Count; i++)
                {
                    WfClientNextActivity nextActivity = runtime.Process.NextActivities[i];

                    //参数配置
                    if (nextActivity != null)
                    {
                        param.Target.ActivityKey = nextActivity.Activity.DescriptorKey;
                        param.Target.TransitionKey = nextActivity.Transition.Key;

                        string codeName = nextActivity.Transition.Properties.GetValue("CodeName", nextActivity.Transition.Name);

                        param.Target.ActionResult = string.IsNullOrEmpty(codeName) ? nextActivity.Transition.Key : codeName;

                        int tempCandidatesCnt = 0;
                        param.Target.Candidates.Clear();
                        nextActivity.Activity.Candidates.ForEach(assignee =>
                        {
                            param.Target.Candidates.Add(assignee);
                            tempCandidatesCnt = tempCandidatesCnt + 1;
                        });

                        tempCandidatesCnt = WFUIControlCommon.AddWfClientUser(this.DictionaryWfClientUser, param.Target, tempCandidatesCnt);

                        //判断是否符合允许从候选人中选择执行人
                        param.IsSelectCandidates = false;
                        param.IsAssignToMultiUsers = false;

                        if (tempCandidatesCnt > 1 && IsAllow)
                        {
                            param.IsSelectCandidates = true;
                            //单选还是多选
                            if (IsMulti)
                                param.IsAssignToMultiUsers = true;
                        }
                    }

                    if (IconListResult.Count() >= i + 1)
                    {
                        icon = IconListResult[i];
                    }

                    if (i == 0)
                    {
                        serializedParam = JSONSerializerExecute.Serialize(param);
                        btnText = GetButtonName(runtime.Process.CurrentActivity, nextActivity.Transition, this.Text, true);
                    }
                    else
                    {
                        param.IsDefault = false;
                        serializedParam = JSONSerializerExecute.Serialize(param);
                        btnText = GetButtonName(runtime.Process.CurrentActivity, nextActivity.Transition, "送签", true);
                    }

                    if (string.IsNullOrEmpty(this.DialogText))
                        dialogText = Translator.Translate(CultureDefine.DefaultCulture, "您确定要{0}吗？", btnText);
                    else
                        dialogText = this.DialogText;

                    if (IsSplit)
                    {
                        ButtonGroup buttonGroupTemp = new ButtonGroup(vcToolbar, vddToolbar);
                        buttonGroupTemp.AddControl(CreateButtonInstance("btnMoveTo" + i.ToString(), btnText, serializedParam, icon, dialogText));
                        this.InnerToolbar.AddGroup(buttonGroupTemp);
                    }
                    else
                    {
                        buttonGroup.AddControl(CreateButtonInstance("btnMoveTo" + i.ToString(), btnText, serializedParam, icon, dialogText));
                        if (i == runtime.Process.NextActivities.Count - 1)
                            this.InnerToolbar.AddGroup(buttonGroup);
                    }
                }
            }
            //添加额外控件
            AddControlExtra(buttonGroup);
           
        }
      
        private void AddControlExtra(ButtonGroup buttonGroup)
        {
            foreach (var item in InnerControls)
            {
                if (IsSplit)
                {
                    ButtonGroup buttonGroupTempList = new ButtonGroup(vcToolbar, vddToolbar);
                    buttonGroupTempList.AddControl(item);
                    this.InnerToolbar.AddGroup(buttonGroupTempList);
                }
                else
                {
                    buttonGroup.AddControl(item);
                }
            }
        }

        private Button CreateButtonInstance(string btnName, string btnText, string serializedParam, string icon, string dialogText)
        {
            Button buttonTemp = new Button(vcToolbar, vddToolbar);

            buttonTemp.Enabled = this.GetEnabled();
            buttonTemp.Visible = this.GetEnabled();
            buttonTemp.Name = btnName;
            buttonTemp.EnableDialog = true;
            buttonTemp.DialogText = Translator.Translate(CultureDefine.DefaultCulture, dialogText);
            buttonTemp.ProgressBar = this.ProgressBar;
            buttonTemp.ClientClick = "$.fn.HSR.Controls.WFMoveTo.Click";
            buttonTemp.Text = Translator.Translate(CultureDefine.DefaultCulture, btnText); ;
            buttonTemp.SizeMode = this.SizeMode;
            buttonTemp.HtmlAttributes[Consts.WFParas] = serializedParam;
            buttonTemp.Icon = icon;
            buttonTemp.ButtonType = this.ButtonType;

            return buttonTemp;
        }

        private static string GetButtonName(WfClientActivity currentActivity, WfClientTransitionDescriptor transition, string defaultButtonName, bool isFirstButton)
        {
            string result = string.Empty;

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

            //result = Translator.Translate(CultureDefine.DefaultCulture, result);

            return result;
        }

        public override void WriteHtml(System.IO.StringWriter stringWriter)
        {
            if (IsAllow)
            {
                string mvcHtmlStr = WFUIControlCommon.WriteMvcHtmlString(this.Name, false);

                stringWriter.Write(mvcHtmlStr);
            }

            base.WriteHtml(stringWriter);
        }
    }
}
