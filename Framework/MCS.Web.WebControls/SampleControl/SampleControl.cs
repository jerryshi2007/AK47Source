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

//在Assembly中需要暴露出的资源
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
        /// 构造函数
        /// </summary>
        public SampleObject()
        {
        }

        /// <summary>
        /// 构造函数
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
    /// 控件例子
    /// </summary>
    //引用基础脚本
    [RequiredScript(typeof(ControlBaseScript))]

    //引用本控件脚本，第一项为客户端控件类名称
    [ClientScriptResource("MCS.Web.WebControls.SampleControl",
          "MCS.Web.WebControls.SampleControl.SampleControl.js")]

    //引用所需的Css
    [ClientCssResource("MCS.Web.WebControls.SampleControl.SampleControl.css")]
    public class SampleControl : ScriptControlBase
    {
        private System.Web.UI.AttributeCollection inputAttribute = null;
        private string inputCssClass = string.Empty;
        private SampleObject samObject;

        /// <summary>
        /// 构造函数，可确定相关客户端对象的类型。
        /// </summary>
        public SampleControl()
            :
            base(true, HtmlTextWriterTag.Div)
        {
            //在客户端，客户端控件对应的DomElement为DIV控件
            this.inputAttribute = new System.Web.UI.AttributeCollection(this.ViewState);
            this.samObject = new SampleObject(DateTime.Now, "LiShiMin", 188);
            this.CssClass = "sampleControl";
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
        /// 输入框样式
        /// </summary>
        [Category("Appearance")]
        [ScriptControlProperty]//设置此属性要输出到客户端
        [ClientPropertyName("inputStyle")]//设置此属性对应客户端属性的名称
        //[JavaScriptConverter(typeof(StyleCollectionConverter))]
        [Browsable(false)]
        [Description("输入框样式")]
        public CssStyleCollection InputStyle
        {
            get { return this.inputAttribute.CssStyle; }
        }

        /// <summary>
        /// 输入框样式
        /// </summary>
        [Category("Appearance")]
        [ScriptControlProperty]//设置此属性要输出到客户端
        [ClientPropertyName("inputStyle2")]//设置此属性对应客户端属性的名称
        [Description("输入框样式")]
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
        /// 输入框样式CssClass
        /// </summary>
        [Category("Appearance")]
        [ScriptControlProperty]//设置此属性要输出到客户端
        [ClientPropertyName("inputCssClass")]//设置此属性对应客户端属性的名称
        [Description("输入框样式CssClass")]
        public string InputCssClass
        {
            get { return this.GetPropertyValue<string>("InputCssClass", string.Empty); }
            set { this.SetPropertyValue<string>("InputCssClass", value); }
        }

        /// <summary>
        /// SampleObject对象属性
        /// </summary>
        [Category("Data")]
        [ScriptControlProperty]//设置此属性要输出到客户端
        [ClientPropertyName("samObject")]//设置此属性对应客户端属性的名称
        [Description("SampleObject")]
        public SampleObject SamObject
        {
            get { return this.samObject; }
            set { this.samObject = value; }
        }

        /// <summary>
        /// SampleObject对象属性
        /// </summary>
        [Category("Data")]
        [ScriptControlProperty]//设置此属性要输出到客户端
        [ClientPropertyName("samTime")]//设置此属性对应客户端属性的名称
        [Description("SamTime")]
        public DateTime SamTime
        {
            get { return this.samObject.DT; }
            set { this.samObject.DT = value; }
        }

        /// <summary>
        /// 图片Url
        /// </summary>
        [ScriptControlProperty]//设置此属性要输出到客户端
        [ClientPropertyName("alertImgUrl")]//设置此属性对应客户端属性的名称
        [Description("图片Url")]
        private string AlertImgUrl
        {
            get { return Page.ClientScript.GetWebResourceUrl(typeof(SampleControl), "MCS.Web.WebControls.SampleControl.Alert.gif"); }
        }

        /// <summary>
        /// 输出到Word文档的URL
        /// </summary>
        [ScriptControlProperty]//设置此属性要输出到客户端
        [ClientPropertyName("exportToWordUrl")]//设置此属性对应客户端属性的名称
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
        /// 客户端响应callbackComplete函数
        /// </summary>
        [ScriptControlEvent]
        [ClientPropertyName("callbackComplete")]
        [Description("客户端响应callbackComplete函数")]
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
        /// 可供客户端调用的方法
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
        /// 供客户端调用的方法
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
        /// 供客户端调用的方法
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        [ScriptControlMethod]
        public string SetSampleObject(object[] o)
        {
            //MethodInfo mi = this.Page.GetType().GetMethod("SetSampleObject");

            //ExceptionHelper.TrueThrow(mi == null, "页面未定义公共方法 SetSampleObject");

            //return (int)mi.Invoke(this.Page, new object[1] { o });

            //return o.Length;
            return o[0].GetType().AssemblyQualifiedName;
        }

        /// <summary>
        /// 重载OnLoad方法
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
        /// 在此处理客户端传来的ClientState字符串值
        /// </summary>
        /// <param name="clientState"></param>
        protected override void LoadClientState(string clientState)
        {
            List<SampleObject> objs = (List<SampleObject>)JSONSerializerExecute.DeserializeObject(clientState, typeof(List<SampleObject>));
        }

        /// <summary>
        /// 在此返回ClientState字符串值，以便传回到客户端
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
        /// 输出预处理
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
        }

        /// <summary>
        /// 控制输出
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
            writer.Write("汉字");
        }
    }
}
