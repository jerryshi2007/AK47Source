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
	/// �ɵ����ؼ�
	/// </summary>
     [RequiredScript(typeof(ControlBaseScript))]
    //���ñ��ؼ��ű�����һ��Ϊ�ͻ��˿ؼ�������
    [ClientScriptResource("MCS.Web.WebControls.ShowModalDialogSampleControl",
         "MCS.Web.WebControls.SampleControl.ShowModalDialogSampleControl.js")]
    [ParseChildren(true), PersistChildren(false),]
    public class ShowModalDialogSampleControl : ScriptControlBase
    {
        private ITemplate dialogControlTemplate;
        private TemplateControlContainer dialogControlContainer;

		/// <summary>
		/// ���캯��
		/// </summary>
        public ShowModalDialogSampleControl()
            : base(true, System.Web.UI.HtmlTextWriterTag.Span)
        {
        }


        /// <summary>
		/// �ı�
        /// </summary>
        [DefaultValue("Text")]
        [Category("Appearance")]
        [ScriptControlProperty]//���ô�����Ҫ������ͻ���
        [ClientPropertyName("text")]//���ô����Զ�Ӧ�ͻ������Ե�����
        [Description("�ı�")]
        public string Text
        {
            set { this.SetPropertyValue<string>("Text", value); }
            get { return this.GetPropertyValue<string>("Text", string.Empty); }
        }
		
		 /// <summary>
		 /// ģ������
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
		/// �����ؼ���Url
		/// </summary>
        [ScriptControlProperty(), ClientPropertyName("dialogControlUrl")]
        public string DialogControlUrl
        {
            get
            {
                Control dialogControl = WebControlUtility.FindControl(this.dialogControlContainer, typeof(DialogSampleControl), false);

                ExceptionHelper.TrueThrow(dialogControl == null,
                    string.Format("δ��������Ϊ{0}��DialogControl", typeof(DialogSampleControl)));

                PageRenderMode pageRenderMode = new PageRenderMode(this.UniqueID, "DialogControl");

                return WebUtility.GetRequestExecutionUrl(pageRenderMode);
            }
        }

		/// <summary>
		/// �����ӿؼ�
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
		/// ����OnLoad
		/// </summary>
		/// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.CreateChildControls();
        }       
		
	    /// <summary>
	    /// Ԥ�������
	    /// </summary>
	    /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (RenderMode.OnlyRenderSelf && RenderMode.RenderArgument == "DialogControl")
            {
                //����ǵ����ؼ�����ShowModalDialogSampleControl.js��Ч
                this.ReadOnly = true;
            }
            else
            {
                //������ǵ���ʽ�ؼ�������ʾ�����ؼ�
                this.dialogControlContainer.Visible = false;
            }
        }
    }
}
