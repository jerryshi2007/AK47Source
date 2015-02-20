using System;
using System.Collections.Generic;
using System.Web.UI;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using MCS.Library.Core;
using MCS.Web.Library;
using MCS.Web.Library.Script;

[assembly: WebResource("MCS.Web.WebControls.SampleControl.ShowModalDialogSampleControl.js", "application/x-javascript")]

namespace MCS.Web.WebControls
{
	/// <summary>
	/// 可弹出控件
	/// </summary>
     [RequiredScript(typeof(ControlBaseScript))]
    //引用本控件脚本，第一项为客户端控件类名称
    [ClientScriptResource("MCS.Web.WebControls.ShowModalDialogSampleControl",
         "MCS.Web.WebControls.SampleControl.ShowModalDialogSampleControl.js")]
    [ParseChildren(true), PersistChildren(false),]
    public class ShowModalDialogSampleControl : ScriptControlBase
    {
        private ITemplate dialogControlTemplate;
        private TemplateControlContainer dialogControlContainer;

		/// <summary>
		/// 构造函数
		/// </summary>
        public ShowModalDialogSampleControl()
            : base(true, System.Web.UI.HtmlTextWriterTag.Span)
        {
        }


        /// <summary>
		/// 文本
        /// </summary>
        [DefaultValue("Text")]
        [Category("Appearance")]
        [ScriptControlProperty]//设置此属性要输出到客户端
        [ClientPropertyName("text")]//设置此属性对应客户端属性的名称
        [Description("文本")]
        public string Text
        {
            set { this.SetPropertyValue<string>("Text", value); }
            get { return this.GetPropertyValue<string>("Text", string.Empty); }
        }
		
		 /// <summary>
		 /// 模板属性
		 /// </summary>
        [TemplateInstance(TemplateInstance.Single),
        TemplateContainer(typeof(TemplateControlContainer)),
        PersistenceMode(PersistenceMode.Attribute), 
        Browsable(false)]
        public ITemplate DialogControlTemplate
        {
            get { return this.dialogControlTemplate; }
            set { this.dialogControlTemplate = value; }
        }

		/// <summary>
		/// 弹出控件的Url
		/// </summary>
        [ScriptControlProperty(), ClientPropertyName("dialogControlUrl")]
        public string DialogControlUrl
        {
            get
            {
                Control dialogControl = WebControlUtility.FindControl(this.dialogControlContainer, typeof(DialogSampleControl), false);

                ExceptionHelper.TrueThrow(dialogControl == null,
                    string.Format("未发现类型为{0}的DialogControl", typeof(DialogSampleControl)));

                PageRenderMode pageRenderMode = new PageRenderMode(this.UniqueID, "DialogControl");

                return WebUtility.GetRequestExecutionUrl(pageRenderMode);
            }
        }

		/// <summary>
		/// 创建子控件
		/// </summary>
        protected override void CreateChildControls()
        {
            if (!this.ChildControlsCreated)
            {
                Controls.Clear();

                this.dialogControlContainer = new TemplateControlContainer();
                this.dialogControlTemplate.InstantiateIn(this.dialogControlContainer);

                this.Controls.Add(this.dialogControlContainer); 
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
		
	    /// <summary>
	    /// 预处理输出
	    /// </summary>
	    /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (RenderMode.OnlyRenderSelf && RenderMode.RenderArgument == "DialogControl")
            {
                //如果是弹出控件，则ShowModalDialogSampleControl.js无效
                this.ReadOnly = true;
            }
            else
            {
                //如果不是弹出式控件，则不显示弹出控件
                this.dialogControlContainer.Visible = false;
            }
        }
    }
}
