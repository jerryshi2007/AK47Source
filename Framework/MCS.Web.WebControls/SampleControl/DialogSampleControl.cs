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
    /// �ؼ�����
    /// </summary>
    //���û����ű�
    [RequiredScript(typeof(ControlBaseScript))]

    //���ñ��ؼ��ű�����һ��Ϊ�ͻ��˿ؼ�������
    [ClientScriptResource("MCS.Web.WebControls.DialogSampleControl",
         "MCS.Web.WebControls.SampleControl.DialogSampleControl.js")]
    [ParseChildren(true), PersistChildren(true),]
    public class DialogSampleControl : ScriptControlBase, INamingContainer
    {
        private ITemplate dataControlTemplate;
        private TemplateControlContainer dataControlContainer;

		/// <summary>
		/// ���캯��
		/// </summary>
        public DialogSampleControl()
            : base(true, System.Web.UI.HtmlTextWriterTag.Span)
        {
        }

		/// <summary>
		/// DataControl�ͻ���ID
		/// </summary>
        [ScriptControlProperty(), ClientPropertyName("dataControlID")]
        public string DataControlClientID
        {
            get
            {
                Control dataControl = WebControlUtility.FindControl(this.dataControlContainer, typeof(SampleControl), false);
                ExceptionHelper.TrueThrow(dataControl == null,
                    string.Format("δ��������Ϊ{0}��DataControl", typeof(SampleControl)));

                return dataControl != null ? dataControl.ClientID : string.Empty;
            }
        }

	   /// <summary>
	   /// ģ������
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
		/// �����ӿؼ�
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
		/// ����OnLoad
		/// </summary>
		/// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.CreateChildControls();
        }       
    }
}
