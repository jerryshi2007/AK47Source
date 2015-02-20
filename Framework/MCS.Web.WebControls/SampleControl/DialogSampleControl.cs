using System;
using System.Collections.Generic;
using System.Web.UI;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using MCS.Library.Core;
using MCS.Web.Library;
using MCS.Web.Library.Script;

[assembly: WebResource("MCS.Web.WebControls.SampleControl.DialogSampleControl.js", "application/x-javascript")]

namespace MCS.Web.WebControls
{
	/// <summary>
	/// TemplateControlContainer
	/// </summary>
    public class TemplateControlContainer : WebControl, INamingContainer
    {
		/// <summary>
		/// TagKey
		/// </summary>
        protected override HtmlTextWriterTag TagKey
        {
            get
            {
                return HtmlTextWriterTag.Div;
            }
        }
    }

    /// <summary>
    /// 控件例子
    /// </summary>
    //引用基础脚本
    [RequiredScript(typeof(ControlBaseScript))]

    //引用本控件脚本，第一项为客户端控件类名称
    [ClientScriptResource("MCS.Web.WebControls.DialogSampleControl",
         "MCS.Web.WebControls.SampleControl.DialogSampleControl.js")]
    [ParseChildren(true), PersistChildren(true),]
    public class DialogSampleControl : ScriptControlBase, INamingContainer
    {
        private ITemplate dataControlTemplate;
        private TemplateControlContainer dataControlContainer;

		/// <summary>
		/// 构造函数
		/// </summary>
        public DialogSampleControl()
            : base(true, System.Web.UI.HtmlTextWriterTag.Span)
        {
        }

		/// <summary>
		/// DataControl客户端ID
		/// </summary>
        [ScriptControlProperty(), ClientPropertyName("dataControlID")]
        public string DataControlClientID
        {
            get
            {
                Control dataControl = WebControlUtility.FindControl(this.dataControlContainer, typeof(SampleControl), false);
                ExceptionHelper.TrueThrow(dataControl == null,
                    string.Format("未发现类型为{0}的DataControl", typeof(SampleControl)));

                return dataControl != null ? dataControl.ClientID : string.Empty;
            }
        }

	   /// <summary>
	   /// 模板属性
	   /// </summary>
       [TemplateInstance(TemplateInstance.Single),
       TemplateContainer(typeof(TemplateControlContainer)),
        PersistenceMode(PersistenceMode.Attribute), 
        Browsable(false)]
        public ITemplate DataControlTemplate
        {
            get { return this.dataControlTemplate; }
            set { this.dataControlTemplate = value; }
        }

		/// <summary>
		/// 创建子控件
		/// </summary>
        protected override void CreateChildControls()
        {
            if (!this.ChildControlsCreated)
            {
                Controls.Clear();
                this.dataControlContainer = new TemplateControlContainer();
                this.dataControlTemplate.InstantiateIn(this.dataControlContainer);

                this.Controls.Add(this.dataControlContainer);
                this.ChildControlsCreated = true;
            }
        }

		/// <summary>
		/// 重载OnLoad
		/// </summary>
		/// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.CreateChildControls();
        }       
    }
}
