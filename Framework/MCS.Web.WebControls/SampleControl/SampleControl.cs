using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Reflection;
using MCS.Library.Core;
using MCS.Web.Library;
using MCS.Web.Library.Script;

//��Assembly����Ҫ��¶������Դ
[assembly: WebResource("MCS.Web.WebControls.SampleControl.SampleControl.js", "application/x-javascript")]
[assembly: WebResource("MCS.Web.WebControls.SampleControl.SampleControl.css", "text/css", PerformSubstitution = true)]
[assembly: WebResource("MCS.Web.WebControls.SampleControl.Alert.gif", "image/gif")]

namespace MCS.Web.WebControls
{
    /// <summary>
    /// SampleObject
    /// </summary>
    public class SampleObject
    {
        private DateTime dt;
        private string name;
        private int length;

        /// <summary>
        /// ���캯��
        /// </summary>
        public SampleObject()
        {
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="name"></param>
        /// <param name="length"></param>
        public SampleObject(DateTime dt, string name, int length)
        {
            this.dt = dt;
            this.name = name;
            this.length = length;
        }

        /// <summary>
        /// DT
        /// </summary>
        public DateTime DT
        {
            get { return this.dt; }
            set { this.dt = value; }
        }
        /// <summary>
        /// Name
        /// </summary>
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }
        /// <summary>
        /// Length
        /// </summary>
        public int Length
        {
            get { return this.length; }
            set { this.length = value; }
        }
    }
    /// <summary>
    /// �ؼ�����
    /// </summary>
    //���û����ű�
    [RequiredScript(typeof(ControlBaseScript))]

    //���ñ��ؼ��ű�����һ��Ϊ�ͻ��˿ؼ�������
    [ClientScriptResource("MCS.Web.WebControls.SampleControl",
          "MCS.Web.WebControls.SampleControl.SampleControl.js")]

    //���������Css
    [ClientCssResource("MCS.Web.WebControls.SampleControl.SampleControl.css")]
    public class SampleControl : ScriptControlBase
    {
        private System.Web.UI.AttributeCollection inputAttribute = null;
        private string inputCssClass = string.Empty;
        private SampleObject samObject;

        /// <summary>
        /// ���캯������ȷ����ؿͻ��˶�������͡�
        /// </summary>
        public SampleControl()
            :
            base(true, HtmlTextWriterTag.Div)
        {
            //�ڿͻ��ˣ��ͻ��˿ؼ���Ӧ��DomElementΪDIV�ؼ�
            this.inputAttribute = new System.Web.UI.AttributeCollection(this.ViewState);
            this.samObject = new SampleObject(DateTime.Now, "LiShiMin", 188);
            this.CssClass = "sampleControl";
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
        /// �������ʽ
        /// </summary>
        [Category("Appearance")]
        [ScriptControlProperty]//���ô�����Ҫ������ͻ���
        [ClientPropertyName("inputStyle")]//���ô����Զ�Ӧ�ͻ������Ե�����
        //[JavaScriptConverter(typeof(StyleCollectionConverter))]
        [Browsable(false)]
        [Description("�������ʽ")]
        public CssStyleCollection InputStyle
        {
            get { return this.inputAttribute.CssStyle; }
        }

        /// <summary>
        /// �������ʽ
        /// </summary>
        [Category("Appearance")]
        [ScriptControlProperty]//���ô�����Ҫ������ͻ���
        [ClientPropertyName("inputStyle2")]//���ô����Զ�Ӧ�ͻ������Ե�����
        [Description("�������ʽ")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty)]
        public Style InputStyle2
        {
            get
            {
                Style style = this.GetPropertyValue<Style>("InputStyle2", null);
                if (style == null)
                {
                    style = new Style();
                    this.SetPropertyValue<Style>("InputStyle2", style);
                }
                return style;
            }
        }

        /// <summary>
        /// �������ʽCssClass
        /// </summary>
        [Category("Appearance")]
        [ScriptControlProperty]//���ô�����Ҫ������ͻ���
        [ClientPropertyName("inputCssClass")]//���ô����Զ�Ӧ�ͻ������Ե�����
        [Description("�������ʽCssClass")]
        public string InputCssClass
        {
            get { return this.GetPropertyValue<string>("InputCssClass", string.Empty); }
            set { this.SetPropertyValue<string>("InputCssClass", value); }
        }

        /// <summary>
        /// SampleObject��������
        /// </summary>
        [Category("Data")]
        [ScriptControlProperty]//���ô�����Ҫ������ͻ���
        [ClientPropertyName("samObject")]//���ô����Զ�Ӧ�ͻ������Ե�����
        [Description("SampleObject")]
        public SampleObject SamObject
        {
            get { return this.samObject; }
            set { this.samObject = value; }
        }

        /// <summary>
        /// SampleObject��������
        /// </summary>
        [Category("Data")]
        [ScriptControlProperty]//���ô�����Ҫ������ͻ���
        [ClientPropertyName("samTime")]//���ô����Զ�Ӧ�ͻ������Ե�����
        [Description("SamTime")]
        public DateTime SamTime
        {
            get { return this.samObject.DT; }
            set { this.samObject.DT = value; }
        }

        /// <summary>
        /// ͼƬUrl
        /// </summary>
        [ScriptControlProperty]//���ô�����Ҫ������ͻ���
        [ClientPropertyName("alertImgUrl")]//���ô����Զ�Ӧ�ͻ������Ե�����
        [Description("ͼƬUrl")]
        private string AlertImgUrl
        {
            get { return Page.ClientScript.GetWebResourceUrl(typeof(SampleControl), "MCS.Web.WebControls.SampleControl.Alert.gif"); }
        }

        /// <summary>
        /// �����Word�ĵ���URL
        /// </summary>
        [ScriptControlProperty]//���ô�����Ҫ������ͻ���
        [ClientPropertyName("exportToWordUrl")]//���ô����Զ�Ӧ�ͻ������Ե�����
        [Browsable(false)]
        public string ExportToWordUrl
        {
            get
            {
                PageRenderMode pageRenderMode = new PageRenderMode(ResponseContentTypeKey.WORD, ResponseDispositionType.Inline, "SampleControl.doc", this.UniqueID, "");
                return WebUtility.GetRequestExecutionUrl(pageRenderMode);
            }
        }

        /// <summary>
        /// �ͻ�����ӦcallbackComplete����
        /// </summary>
        [ScriptControlEvent]
        [ClientPropertyName("callbackComplete")]
        [Description("�ͻ�����ӦcallbackComplete����")]
        public string ClientCallbackCompleteEventHanlder
        {
            get
            {
                return this.GetPropertyValue<string>("ClientCallbackCompleteEventHanlder", string.Empty);
            }

            set
            {
                this.SetPropertyValue<string>("ClientCallbackCompleteEventHanlder", value);
            }
        }

        /// <summary>
        /// �ɹ��ͻ��˵��õķ���
        /// </summary>
        /// <param name="head"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        [ScriptControlMethod]
        public string GetMyText(string head, int index)
        {
            return head + "_" + index.ToString() + "_" + this.Text;
        }

        /// <summary>
        /// ���ͻ��˵��õķ���
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="name"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        [ScriptControlMethod]
        public SampleObject GetSampleObject(DateTime dt, string name, int length)
        {
            SampleObject o = new SampleObject(dt, name, length);
            return o;
        }

        /// <summary>
        /// ���ͻ��˵��õķ���
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        [ScriptControlMethod]
        public string SetSampleObject(object[] o)
        {
            //MethodInfo mi = this.Page.GetType().GetMethod("SetSampleObject");

            //ExceptionHelper.TrueThrow(mi == null, "ҳ��δ���幫������ SetSampleObject");

            //return (int)mi.Invoke(this.Page, new object[1] { o });

            //return o.Length;
            return o[0].GetType().AssemblyQualifiedName;
        }

        /// <summary>
        /// ����OnLoad����
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            JSONSerializerExecute.RegisterTypeToClient(this.Page, "SampleObjectTypeKey", typeof(SampleControl));
            JSONSerializerExecute.RegisterConverter(typeof(StyleConverter));
            JSONSerializerExecute.RegisterConverter(typeof(StyleCollectionConverter));
        }

        /// <summary>
        /// �ڴ˴���ͻ��˴�����ClientState�ַ���ֵ
        /// </summary>
        /// <param name="clientState"></param>
        protected override void LoadClientState(string clientState)
        {
            List<SampleObject> objs = (List<SampleObject>)JSONSerializerExecute.DeserializeObject(clientState, typeof(List<SampleObject>));
        }

        /// <summary>
        /// �ڴ˷���ClientState�ַ���ֵ���Ա㴫�ص��ͻ���
        /// </summary>
        /// <returns></returns>
        protected override string SaveClientState()
        {
            List<SampleObject> objs = new List<SampleObject>();
            objs.Add(new SampleObject(DateTime.Now, "1234", 111));
            objs.Add(new SampleObject(DateTime.Now, "5566", 222));
            return JSONSerializerExecute.Serialize(objs);// (new System.Web.Script.Serialization.JavaScriptSerializer()).Serialize(this.samObject);// base.SaveClientState();
        }

        /// <summary>
        /// ���Ԥ����
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
        }

        /// <summary>
        /// �������
        /// </summary>
        /// <param name="writer"></param>
        protected override void Render(HtmlTextWriter writer)
        {
            if (DesignMode)
            {
                Image img = new Image();
                img.ImageUrl = AlertImgUrl;
                Controls.Add(img);
                TextBox tb = new TextBox();
                Controls.Add(tb);
                base.Render(writer);
            }
            else
            {
                if (RenderMode.IsHtmlRender)
                    base.Render(writer);
                else
                    RenderToFile(writer);
            }
        }

        private void RenderToFile(HtmlTextWriter writer)
        {
            writer.Write("����");
        }
    }
}
