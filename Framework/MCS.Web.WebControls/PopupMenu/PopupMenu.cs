using System;
using System.Data;
using System.Text;
using System.Configuration;
using System.Drawing.Design;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Web.UI.Design.WebControls;
using System.Web.Script;
using System.ComponentModel;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Web.Script.Serialization;


using ChinaCustoms.Framework.DeluxeWorks.Web.Library.Script;

[assembly: WebResource("ChinaCustoms.Framework.DeluxeWorks.Web.WebControls.PopupMenu.PopupMenu.js", "application/x-javascript")]
[assembly: WebResource("ChinaCustoms.Framework.DeluxeWorks.Web.WebControls.PopupMenu.PopupMenu.css", "text/css", PerformSubstitution = true)]
[assembly: WebResource("ChinaCustoms.Framework.DeluxeWorks.Web.WebControls.PopupMenu.popOutImageUrl.gif", "image/gif")]
[assembly: WebResource("ChinaCustoms.Framework.DeluxeWorks.Web.WebControls.PopupMenu.staticPopOutImageUrl.gif", "image/gif")]
[assembly: WebResource("ChinaCustoms.Framework.DeluxeWorks.Web.WebControls.PopupMenu.dynamicPopOutImageUrl.gif", "image/gif")]
[assembly: WebResource("ChinaCustoms.Framework.DeluxeWorks.Web.WebControls.PopupMenu.back.gif", "image/gif")]

namespace ChinaCustoms.Framework.DeluxeWorks.Web.WebControls
{
    /// <summary>
    /// �ָ��ߵ�����
    /// </summary>
    /// <remarks></remarks>
    public enum PopupMenuItemSeparatorMode
    {
        None = 0,
        Bottom = 1,
        Top = 2
    }

    /// <summary>
    /// ������ö��
    /// </summary>
    ///<remarks></remarks>
    public enum MenuOrientation
    {
        Vertical = 0,
        Horizontal = 1
    }


    [PersistChildren(false)]
    [ParseChildren(true)]
    [Designer(typeof(MenuItemsDesigner))]
    [Editor(typeof(MenuItemsEditor), typeof(System.ComponentModel.ComponentEditor))]
    [RequiredScript(typeof(DeluxeAjaxScript))]
    [RequiredScript(typeof(BlockingScript))]
    [RequiredScript(typeof(ControlBaseScript))]
    [RequiredScript(typeof(PopupControlScript))]
    [RequiredScript(typeof(TreeBaseScript))]
    [ClientCssResource("ChinaCustoms.Framework.DeluxeWorks.Web.WebControls.PopupMenu.PopupMenu.css")]
    [ClientScriptResource("ChinaCustoms.Framework.DeluxeWorks.Web.WebControls.PopupMenu",
      "ChinaCustoms.Framework.DeluxeWorks.Web.WebControls.PopupMenu.PopupMenu.js")]
    public class PopupMenu : ScriptControlBase
    {
        private MenuItemCollection items;
        /// <summary>
        /// PopupMenu���캯��
        /// </summary>
        /// <remarks></remarks>
        public PopupMenu()
            : base(true, HtmlTextWriterTag.Div)
        {
            this.items = new MenuItemCollection(null);
            //this.Style.Add("Width", "0");

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (this.DesignMode == false)
                PreloadAllImages();
        }

        /// <summary>
        /// ����˵�
        /// </summary>
        /// <param name="writer"></param>
        /// <remarks></remarks>
        protected override void Render(HtmlTextWriter writer)
        {

            if (DesignMode)
            {

                writer.Write(GetMenuDesignHTML(this));
            }
            else
            {

                base.Render(writer);
            }
        }

        /// <summary>
        /// һ���˵����з���
        /// </summary>
        ///<remarks></remarks>
        [Category("Appearance")]
        [Description("һ���˵����з���")]
        [ScriptControlProperty]
        public MenuOrientation Orientation
        {
            get { return GetPropertyValue("Orientation", MenuOrientation.Vertical); }
            set { SetPropertyValue("Orientation", value); }
        }

        /// <summary>
        /// �Ƿ��Ƕ�ѡ
        /// </summary>
        ///<remarks></remarks>
        [DefaultValue(false)]
        [Category("Appearance")]
        [Description("�Ƿ��Ƕ�ѡ")]
        [ScriptControlProperty]
        public bool MultiSelect
        {
            get { return GetPropertyValue("MultiSelect", false); }
            set { SetPropertyValue("MultiSelect", value); }
        }

        /// <summary>
        /// �˵���Ŀ��
        /// </summary>
        ///<remarks></remarks>
        [DefaultValue(150)]
        [Category("Appearance")]
        [Description("�˵������ֿ��")]
        [ScriptControlProperty]
        public int ItemFontWidth
        {
            get { return GetPropertyValue("ItemFontWidth", 150); }
            set { SetPropertyValue("ItemFontWidth", value); }
        }

        /// <summary>
        /// ��̬�˵��У��Ӳ˵���������
        /// </summary>
        ///<remarks></remarks>
        [DefaultValue(10)]
        [Category("Appearance")]
        [ScriptControlProperty]
        [ClientPropertyName("subMenuIndent"), Description("��̬�˵��У��Ӳ˵���������")]
        public int SubMenuIndent
        {
            get { return GetPropertyValue("SubMenuIndent", 10); }
            set { SetPropertyValue("SubMenuIndent", value); }
        }

        /// <summary>
        /// 
        /// </summary>
        ///<remarks></remarks>
        [DefaultValue(1)]
        [Category("Appearance"), Description("��̬�˵�����Ĭ��Ϊ1")]
        [ScriptControlProperty]
        [ClientPropertyName("staticDisplayLevels")]
        public int StaticDisplayLevels
        {
            get { return GetPropertyValue("StaticDisplayLevels", 1); }
            set { SetPropertyValue("StaticDisplayLevels", value); }
        }

        //[DefaultValue("")]
        //[Category("Behavior")]
        //[ScriptControlProperty]
        //[ClientPropertyName("popOutImageUrl"), WebDescription("�����˵�ͼ��")]
        //public string PopOutImageUrl
        //{
        //    get { return (string)(ViewState["PopOutImageUrl"] ?? 
        //        this.Page.ClientScript.GetWebResourceUrl(typeof(ChinaCustoms.Framework.DeluxeWorks.Web.WebControls.PopupMenuControl), 
        //        "ChinaCustoms.Framework.DeluxeWorks.Web.WebControls.PopupMenu.popOutImageUrl.gif")); }
        //    set { ViewState["PopOutImageUrl"] = value; }
        //}

        /// <summary>
        /// ��ʾĬ����һ����̬�˵�ͼ��
        /// </summary>
        ///<remarks></remarks>
        [Browsable(false)]
        [ScriptControlProperty]
        [ClientPropertyName("defaultPopOutImageUrl")]
        public string DefaultPopOutImageUrl
        {
            get
            {
                return
                this.Page.ClientScript.GetWebResourceUrl(typeof(PopupMenu),
              "ChinaCustoms.Framework.DeluxeWorks.Web.WebControls.PopupMenu.dynamicPopOutImageUrl.gif");
            }
        }

        /// <summary>
        /// ��ʾ��һ����̬�˵�ͼ��
        /// </summary>
        ///<remarks></remarks>
        [DefaultValue("")]
        [Category("Appearance")]
        [ScriptControlProperty]
        [ClientPropertyName("staticPopOutImageUrl"), Description("��ʾ��һ����̬�˵�ͼ��")]
        [Editor("System.Web.UI.Design.ImageUrlEditor, System.Design", typeof(System.Drawing.Design.UITypeEditor))]
        public string StaticPopOutImageUrl
        {
            get
            {
                return GetPropertyValue("StaticPopOutImageUrl", string.Empty);
            }
            set { SetPropertyValue("StaticPopOutImageUrl", value); }
        }

        /// <summary>
        /// ��ʾ��һ����̬�˵�ͼ��
        /// </summary>
        ///<remarks></remarks>
        [DefaultValue("")]
        [Category("Appearance")]
        [ScriptControlProperty]
        [ClientPropertyName("dynamicPopOutImageUrl"), Description("��ʾ��һ����̬�˵�ͼ��")]
        [Editor("System.Web.UI.Design.ImageUrlEditor, System.Design", typeof(System.Drawing.Design.UITypeEditor))]
        public string DynamicPopOutImageUrl
        {
            get
            {
                return GetPropertyValue("DynamicPopOutImageUrl", string.Empty);
            }
            set { SetPropertyValue("DynamicPopOutImageUrl", value); }
        }

        /// <summary>
        /// ������Ŀ��
        /// </summary>
        ///<remarks></remarks>
        [DefaultValue("")]
        [Category("Behavior")]
        [ScriptControlProperty]
        [ClientPropertyName("target"), Description("������Ŀ��")]
        public string Target
        {
            get { return GetPropertyValue("Target", string.Empty); }
            set { SetPropertyValue("Target", value); }
        }

        ///// <summary>
        ///// �˵���CssClass
        ///// </summary>
        /////<remarks></remarks>
        //[DefaultValue("")]
        //[Category("Appearance")]
        //[ScriptControlProperty]
        //[ClientPropertyName("cssClass"), WebDescription("�˵���CssClass")]
        //public override string CssClass
        //{
        //    get
        //    {
        //        return base.CssClass;
        //    }
        //    set
        //    {
        //        base.CssClass = value;
        //    }
        //}

        /// <summary>
        /// �˵���Ŀ��CssClass
        /// </summary>
        ///<remarks></remarks>
        [DefaultValue("")]
        [Category("Appearance")]
        [ScriptControlProperty]
        [ClientPropertyName("itemCssClass"), Description("�˵���Ŀ��CssClass")]
        public string ItemCssClass
        {
            get { return GetPropertyValue("ItemCssClass", string.Empty); }
            set { SetPropertyValue("ItemCssClass", value); }
        }

        /// <summary>
        /// ����Ƶ��˵���Ŀ��CssClass
        /// </summary>
        ///<remarks></remarks>
        [DefaultValue("")]
        [Category("Appearance")]
        [ScriptControlProperty]
        [ClientPropertyName("hoverItemCssClass"), Description("����Ƶ��˵���Ŀ��CssClass")]
        public string HoverItemCssClass
        {
            get { return GetPropertyValue("HoverItemCssClass", string.Empty); }
            set { SetPropertyValue("HoverItemCssClass", value); }
        }

        /// <summary>
        /// �˵���Ŀѡ����CssClass
        /// </summary>
        ///<remarks></remarks>
        [DefaultValue("")]
        [Category("Appearance")]
        [ScriptControlProperty]
        [ClientPropertyName("selectedItemCssClass"), Description("�˵���Ŀѡ����CssClass")]
        public string SelectedItemCssClass
        {
            get { return GetPropertyValue("SelectedItemCssClass", string.Empty); }
            set { SetPropertyValue("SelectedItemCssClass", value); }
        }

        /// <summary>
        /// �˵��ָ��ߵ�CssClass
        /// </summary>
        ///<remarks></remarks>
        [DefaultValue("")]
        [Category("Appearance")]
        [ScriptControlProperty]
        [ClientPropertyName("separatorCssClass"), Description("�˵��ָ��ߵ�CssClass")]
        public string SeparatorCssClass
        {
            get { return GetPropertyValue("SeparatorCssClass", string.Empty); }
            set { SetPropertyValue("SeparatorCssClass", value); }
        }

        /// <summary>
        /// �˵��ı�ǰ�Ŀո��ȣ�Ĭ��Ϊ5
        /// </summary>
        ///<remarks></remarks>
        [DefaultValue(5)]
        [Category("Appearance")]
        [ScriptControlProperty]
        [ClientPropertyName("textHeadWidth"), Description("�˵��ı�ǰ�Ŀո��ȣ�Ĭ��Ϊ5")]
        public int TextHeadWidth
        {
            get { return GetPropertyValue("TextHeadWidth", 5); }
            set { SetPropertyValue("TextHeadWidth", value); }
        }

        /// <summary>
        /// �˵���Ŀǰ��ͼ����CssClass
        /// </summary>
        ///<remarks></remarks>
        [DefaultValue("")]
        [Category("Appearance")]
        [ScriptControlProperty]
        [ClientPropertyName("imageColCssClass"),Description("�˵���Ŀǰ��ͼ����CssClass")]
        public string ImageColCssClass
        {
            get { return GetPropertyValue("ImageColCssClass", string.Empty); }
            set { SetPropertyValue("ImageColCssClass", value); }
        }

        /// <summary>
        /// �˵���Ŀǰ��ͼ���Ƿ�������Ĭ��Ϊ��
        /// </summary>
        ///<remarks></remarks>
        [DefaultValue(false)]
        [Category("Appearance")]
        [ScriptControlProperty]
        [ClientPropertyName("isImageIndent"), Description("�˵���Ŀǰ��ͼ���Ƿ�������Ĭ��Ϊ��")]
        public bool IsImageIndent
        {
            get { return GetPropertyValue("IsImageIndent", false); }
            set { SetPropertyValue("IsImageIndent", value); }
        }

        /// <summary>
        /// �˵���Ŀ����
        /// </summary>
        ///<remarks></remarks>
        [DefaultValue((string)null)]
        [Category("Date")]
        [Description("�˵���Ŀ����")]
        [Editor(typeof(MenuCollectionItemsEditor), typeof(UITypeEditor))]
        [ScriptControlProperty, ClientPropertyName("items")]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public MenuItemCollection Items
        {
            get { return items; }
        }

        /// <summary>
        /// �����˵����¼�
        /// </summary>
        ///<remarks></remarks>
        [DefaultValue("")]
        [Category("ClientEventsHandler")]
        [ScriptControEventAttribute]
        [ClientPropertyName("mclick")]
        [DescriptionAttribute("�����¼�")]
        public string OnMenuItemClick
        {
            get { return GetPropertyValue("OnMenuItemClick", string.Empty); }
            set { SetPropertyValue("OnMenuItemClick", value); }
        }

        /// <summary>
        /// �Ӳ˵������¼�
        /// </summary>
        ///<remarks></remarks>
        [DefaultValue("")]
        [Category("ClientEventsHandler")]
        [ScriptControEventAttribute]
        [ClientPropertyName("mover")]
        [DescriptionAttribute("�Ӳ˵������¼�")]
        public string OnMenuPopup
        {
            get { return GetPropertyValue("OnMenuPopup", string.Empty); }
            set { SetPropertyValue("OnMenuPopup", value); }
        }

        #region  PostData

        /// <summary>
        /// ���ؿͻ���״̬
        /// </summary>
        /// <param name="clientState"></param>
        ///<remarks></remarks>
        protected override void LoadClientState(string clientState)
        {
            System.Web.Script.Serialization.JavaScriptSerializer js = JSONSerializerFactory.GetJavaScriptSerializer();

            object[] loadArray = (object[])js.DeserializeObject(clientState);

            if (null != loadArray)
            {
                ReceiveArray(this.items, loadArray);
            }

        }

        /// <summary>
        /// ����ͻ���״̬
        /// </summary>
        /// <param name="clientState"></param>
        ///<remarks></remarks>
        protected override string SaveClientState()
        {
            BulidArray(this.items);

            System.Web.Script.Serialization.JavaScriptSerializer js = JSONSerializerFactory.GetJavaScriptSerializer();

            string fsSerialize = js.Serialize(nodeSelectedArray);

            return fsSerialize;
        }

        List<object[]> nodeSelectedArray = new List<object[]>();
        /// <summary>
        /// ��menuitem�����nodeid��selected���������磺["nodeid",selected],["nodeid",selected]
        /// </summary>
        /// <param name="mic"></param>
        ///<remarks></remarks>
        void BulidArray(MenuItemCollection mic)
        {
            if (mic.Count != 0)
            {
                for (int i = 0; i < mic.Count; i++)
                {
                    nodeSelectedArray.Add(new string[] { mic[i].NodeID, mic[i].Selected.ToString() });

                    BulidArray(mic[i].ChildItems);
                }
            }
        }

        private int counter = 0;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mic"></param>
        /// <param name="saveList"></param>
        ///<remarks></remarks>
        void ReceiveArray(MenuItemCollection mic, object[] saveList)//List<string[]> saveList)
        {

            if (mic.Count != 0)
            {

                for (int i = 0; i < mic.Count; i++)
                {

                    mic[i].NodeID = ((object[])saveList[counter])[0].ToString();

                    mic[i].Selected = Convert.ToBoolean(((object[])saveList[counter])[1]);

                    ++counter;

                    ReceiveArray(mic[i].ChildItems, saveList);

                }
            }

        }


        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="descriptor"></param>
        ///<remarks></remarks>
        protected override void DescribeComponent(ScriptComponentDescriptor descriptor)
        {
            base.DescribeComponent(descriptor);
            //descriptor.AddProperty("items", (new MenuCollectionTypeConvert()).ConvertToString(this.Items));
        }

        /// <summary>
        /// չʾ���ʱ״̬
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        ///<remarks></remarks>
        public string GetMenuDesignHTML(PopupMenu menu)
        {
            StringBuilder strB = new StringBuilder();
            if (menu.items.Count == 0)
            {
                strB.Append("<div>");
                strB.Append(ChinaCustoms.Framework.DeluxeWorks.Web.WebControls.Properties.Resources.EnptyMenuDesignTimeHtml);
                strB.Append("</div>");
            }
            else
            {
                strB.Append("<Table>");
                strB.Append(GetMenuItemsDesignHTML(menu.Items, 0));
                strB.Append("</Table>");
            }
            return strB.ToString();
        }

        /// <summary>
        /// չʾ���ʱ�в˵�״̬
        /// </summary>
        /// <param name="items"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        ///<remarks></remarks>
        private string GetMenuItemsDesignHTML(IList<ChinaCustoms.Framework.DeluxeWorks.Web.WebControls.MenuItem> items, int level)
        {
            StringBuilder strB = new StringBuilder();
            foreach (ChinaCustoms.Framework.DeluxeWorks.Web.WebControls.MenuItem item in items)
            {
                strB.Append("<TR>");
                strB.AppendFormat("<TD style='text-indent:{0}px'>{1}</TD>", level * 10, item.Text);
                strB.Append("</TR>");
                strB.Append(GetMenuItemsDesignHTML(item.ChildItems, level + 1));
            }

            return strB.ToString();
        }

        # region   Ԥ�ȼ���ͼƬ
        /// <summary>
        /// Ԥ�ȼ���ͼƬ
        /// </summary>
        /// <remarks></remarks>
        private void PreloadAllImages()
        {
            PreloadImage(this.DefaultPopOutImageUrl, this.DefaultPopOutImageUrl);
            PreloadImage(this.DynamicPopOutImageUrl, this.DynamicPopOutImageUrl);
            PreloadImage(this.StaticPopOutImageUrl, this.StaticPopOutImageUrl);

            PreloadMenuItemsImages(this.Items);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="menuItems"></param>
        private void PreloadMenuItemsImages(MenuItemCollection menuItems)
        {
            foreach (MenuItem item in menuItems)
            {
                PreloadImage(item.ImageUrl, item.ImageUrl);
                PreloadImage(item.DynamicPopOutImageUrl, item.DynamicPopOutImageUrl);
                PreloadImage(item.StaticPopOutImageUrl, item.StaticPopOutImageUrl);

                PreloadMenuItemsImages(item.ChildItems);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="imgSrc"></param>
        private void PreloadImage(string key, string imgSrc)
        {
            if (string.IsNullOrEmpty(imgSrc) == false)
            {
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), key,
                    string.Format("<img src=\"{0}\" style=\"display:none\"/>", HttpUtility.UrlPathEncode(imgSrc)));
            }
        }

        #endregion
    }



    #region   TypeConverter
    /*
        public class MenuCollectionTypeConvert : TypeConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                return sourceType == typeof(String);
            }
            public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
            {
                String str = value as string;

                if (str != null)
                {
                    JavaScriptSerializer ser = new JavaScriptSerializer();
                    MenuItemCollection items = ser.Deserialize<MenuItemCollection>(str);
                    return items;
                }

                return null;
            }

            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                return destinationType == typeof(MenuItemCollection);
            }
            public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
            {
                MenuItemCollection itemList = value as MenuItemCollection;

                if (itemList != null)
                {
                    JavaScriptSerializer ser = new JavaScriptSerializer();
                    ser.RegisterConverters(new JavaScriptConverter[] { new MenuCollectionJavaScriptConvert() });
                    string r = ser.Serialize(itemList);

                    return r;
                }

                return null;
            }
        }
        */
    #endregion   �˵�����

    [ParseChildren(true, "ChildItems")]
    public class MenuItem : IStateManager
    {
        private MenuItem parent;
        private MenuItem previous;
        private MenuItem next;
        private MenuItemCollection childItems;
        private StateBag viewState;

        /// <summary>
        /// MenuItem���캯��
        /// </summary>
        ///<remarks></remarks>
        public MenuItem()
        {
            childItems = new MenuItemCollection(this);
            viewState = new StateBag();
        }

        /// <summary>
        /// �ڵ�ı��
        /// </summary>
        ///<remarks></remarks>
        [DefaultValue("")]
        [Category("Appearance")]
        [ClientPropertyName("nodeID")]
        [Browsable(false)]
        public string NodeID
        {
            get { return (string)(viewState["NodeID"] ?? ""); }
            set { viewState["NodeID"] = value; }
        }

        /// <summary>
        /// �Ƿ��Ƿָ���
        /// </summary>
        ///<remarks>�Ƿ��Ƿָ���</remarks>
        [DefaultValue(false)]
        [Category("Appearance")]
        [ClientPropertyName("isSeparator")]
        [Description("�Ƿ��Ƿָ���")]
        public bool IsSeparator
        {
            get { return (bool)(viewState["IsSeparator"] ?? false); }
            set { viewState["IsSeparator"] = value; }
        }

        /// <summary>
        /// �ò˵����Ƿ����
        /// </summary>
        ///<remarks></remarks>
        [DefaultValue(true)]
        [Category("Appearance")]
        [ClientPropertyName("enable")]
        [Description("�ò˵����Ƿ����")]
        public bool Enable
        {
            get { return (bool)(viewState["Enable"] ?? true); }
            set { viewState["Enable"] = value; }
        }

        /// <summary>
        /// �ò˵����Ƿ�ɼ�
        /// </summary>
        ///<remarks></remarks>
        [DefaultValue(true)]
        [Category("Appearance")]
        [ClientPropertyName("visible")]
        [Description("�ò˵����Ƿ�ɼ�")]
        public bool Visible
        {
            get { return (bool)(viewState["Visible"] ?? true); }
            set { viewState["Visible"] = value; }
        }

        /// <summary>
        /// �˵���Ŀ�ı�
        /// </summary>
        /// <remarks></remarks>
        [DefaultValue("")]
        [Category("Appearance")]
        [ClientPropertyName("text")]
        [Description("�˵���Ŀ�ı�")]
        public string Text
        {
            get { return (string)(viewState["Text"] ?? string.Empty); }
            set { viewState["Text"] = value; }
        }

        /// <summary>
        /// �˵���Ŀֵ
        /// </summary>
        /// <remarks></remarks>
        [DefaultValue("")]
        [Category("Appearance")]
        [Description("�˵���Ŀֵ")]
        [ClientPropertyName("value")]
        public string Value
        {
            get { return (string)(viewState["Value"] ?? string.Empty); }
            set { viewState["Value"] = value; }
        }

        /// <summary>
        /// �˵���Ŀ����Ŀ��
        /// </summary>
        /// <remarks></remarks>
        [DefaultValue("")]
        [Category("Appearance")]
        [Description("�˵���Ŀ����Ŀ��")]
        [ClientPropertyName("target")]
        public string Target
        {
            get { return (string)(viewState["Target"] ?? string.Empty); }
            set { viewState["Target"] = value; }
        }

        //[WebDescription("�����˵�ͼ��")]
        //public string PopOutImageUrl
        //{
        //    get { return (string)(_ViewState["PopOutImageUrl"] ?? string.Empty); }
        //    set { _ViewState["PopOutImageUrl"] = value; }
        //}

        /// <summary>
        /// ������̬�˵�ͼ��
        /// </summary>
        /// <remarks></remarks>
        [DefaultValue("")]
        [Category("Appearance")]
        [Description("������̬�˵�ͼ��")]
        [ClientPropertyName("staticPopOutImageUrl")]
        [Editor("System.Web.UI.Design.ImageUrlEditor, System.Design", typeof(System.Drawing.Design.UITypeEditor))]
        public string StaticPopOutImageUrl
        {
            get { return (string)(viewState["StaticPopOutImageUrl"] ?? string.Empty); }
            set { viewState["StaticPopOutImageUrl"] = value; }
        }

        /// <summary>
        /// ������̬�˵�ͼ��
        /// </summary>
        /// <remarks></remarks>
        [DefaultValue("")]
        [Category("Appearance")]
        [Description("������̬�˵�ͼ��")]
        [ClientPropertyName("dynamicPopOutImageUrl")]
        [Editor("System.Web.UI.Design.ImageUrlEditor, System.Design", typeof(System.Drawing.Design.UITypeEditor))]
        public string DynamicPopOutImageUrl
        {
            get { return (string)(viewState["DynamicPopOutImageUrl"] ?? string.Empty); }
            set { viewState["DynamicPopOutImageUrl"] = value; }
        }

        /// <summary>
        /// �˵���Ŀ����Url
        /// </summary>
        /// <remarks></remarks>
        [DefaultValue("")]
        [Category("Appearance")]
        [Description("�˵���Ŀ����Url")]
        [ClientPropertyName("navigateUrl")]
        public string NavigateUrl
        {
            get { return (string)(viewState["NavigateUrl"] ?? string.Empty); }
            set { viewState["NavigateUrl"] = value; }
        }

        /// <summary>
        /// �˵���Ŀǰͼ��
        /// </summary>
        /// <remarks></remarks>
        [DefaultValue("")]
        [Category("Appearance")]
        [Description("�˵���Ŀǰͼ��")]
        [ClientPropertyName("imageUrl")]
        public string ImageUrl
        {
            get { return (string)(viewState["ImageUrl"] ?? string.Empty); }
            set { viewState["ImageUrl"] = value; }
        }

        /// <summary>
        /// �˵���Ŀ��ʾ
        /// </summary>
        /// <remarks></remarks>
        [DefaultValue("")]
        [Category("Appearance")]
        [Description("�˵���Ŀ��ʾ")]
        [ClientPropertyName("toolTip")]
        public string ToolTip
        {
            get { return (string)(viewState["ToolTip"] ?? string.Empty); }
            set { viewState["ToolTip"] = value; }
        }

        /// <summary>
        /// �˵���Ŀ�Ƿ�ѡ��
        /// </summary>
        /// <remarks></remarks>
        [DefaultValue(false)]
        [Category("Appearance")]
        [Description("�˵���Ŀ�Ƿ�ѡ��")]
        [ClientPropertyName("selected")]
        public bool Selected
        {
            get { return (bool)(viewState["Selected"] ?? false); }
            set { viewState["Selected"] = value; }
        }

        /// <summary>
        /// �˵���Ŀ�ָ�����ʽ
        /// </summary>
        /// <remarks></remarks>
        [Description("�˵���Ŀ�ָ�����ʽ")]
        [ClientPropertyName("separatorMode")]
        public PopupMenuItemSeparatorMode SeparatorMode
        {
            get { return (PopupMenuItemSeparatorMode)(viewState["SeparatorMode"] ?? PopupMenuItemSeparatorMode.None); }
            set { viewState["SeparatorMode"] = value; }
        }

        /// <summary>
        /// ���ڵ�
        /// </summary>
        /// <remarks></remarks>
        [ScriptIgnore()]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public MenuItem Parent
        {
            get { return this.parent; }
            set { this.parent = value; }
        }

        /// <summary>
        /// ǰһ�ֵܽڵ�
        /// </summary>
        /// <remarks></remarks>
        [ScriptIgnore()]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public MenuItem Previous
        {
            get { return this.previous; }
            set { this.previous = value; }
        }

        /// <summary>
        /// ��һ�ֵܽڵ�
        /// </summary>
        /// <remarks></remarks>
        [ScriptIgnore()]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public MenuItem Next
        {
            get { return this.next; }
            set { this.next = value; }
        }

        /// <summary>
        /// �Ӳ˵�����
        /// </summary>
        /// <remarks></remarks>
        [Browsable(false), MergableProperty(false), PersistenceMode(PersistenceMode.InnerDefaultProperty)]
        [ClientPropertyName("childItems")]
        public MenuItemCollection ChildItems
        {
            get
            {
                return this.childItems;
            }
        }

        /// <summary>
        /// �洢��ͼ״̬
        /// </summary>
        /// <remarks></remarks>
        bool IStateManager.IsTrackingViewState
        {
            get
            {
                return ((IStateManager)viewState).IsTrackingViewState;
            }
        }

        /// <summary>
        /// ������ͼ״̬
        /// </summary>
        /// <remarks></remarks>
        void IStateManager.TrackViewState()
        {
            ((IStateManager)viewState).TrackViewState();
        }

        /// <summary>
        /// ������ͼ״̬
        /// </summary>
        /// <param name="saveState"></param>
        /// <remarks></remarks>
        void IStateManager.LoadViewState(object saveState)
        {
            if (saveState != null)
                ((IStateManager)viewState).LoadViewState(saveState);
        }

        /// <summary>
        /// ������ͼ״̬
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        object IStateManager.SaveViewState()
        {
            return ((IStateManager)viewState).SaveViewState();
        }
    }

    public class MenuItemCollection : Collection<MenuItem>
    {
        private MenuItem ownerItem;

        /// <summary>
        /// MenuItemCollection���캯��
        /// </summary>
        /// <remarks></remarks>
        public MenuItemCollection()
            : this(null)
        {
        }

        /// <summary>
        ///  MenuItemCollection���캯��
        /// </summary>
        /// <param name="ownerItem"></param>
        /// <remarks></remarks>
        public MenuItemCollection(MenuItem ownerItem)
        {
            this.ownerItem = ownerItem;
        }

        ///// <summary>
        ///// ����˵��б�
        ///// </summary>
        ///// <remarks></remarks>
        //protected override void ClearItems()
        //{
        //    foreach (MenuItem item in this)
        //        item.Parent = null;
        //    base.ClearItems();
        //}

		/// <summary>
		/// ���б�����Ӳ˵���
		/// </summary>
		/// <param name="item"></param>
		/// <remarks></remarks>
		new public void Add(MenuItem item)
		{
			this.AddAt(this.Count, item);
		}

        /// <summary>
        /// ��ָ����������Ӳ˵���
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        /// <remarks></remarks>
        public void AddAt(int index, MenuItem item)
        {
            item.Parent = this.ownerItem;
            this.Insert(index, item);
            if (index == 0)
            {
                item.Previous = null;
                if (this.Count > 1)
                {
                    item.Next = this[1];
                    this[1].Previous = item;
                }
                else
                {
                    item.Next = null;
                }
            }
            //������һ��
            else if (index == (this.Count - 1))
            {
                item.Next = null;
                if (this.Count > 1)
                {
                    item.Previous = this[index - 1];
                    this[index - 1].Next = item;
                }
                else
                {
                    item.Previous = null;
                }
            }
            else if (index > 0 && index < this.Count)
            {
                item.Previous = this[index - 1];
                this[index - 1].Next = item;
                item.Next = this[index + 1];
                this[index + 1].Previous = item;
            }

        }

        /// <summary>
        /// ��������ɾ���б���ָ���˵���
        /// </summary>
        /// <param name="index"></param>
        /// <remarks></remarks>
        protected override void RemoveItem(int index)
        {
            if (index == 0)
            {
                if (this.Count > 1)
                {
                    this[1].Previous = null;
                }
            }
            else if (index == (this.Count - 1))
            {
                if (this.Count > 1)
                {
                    this[index - 1].Next = null;
                }
            }
            else if (index > 0 && index < (this.Count - 1))
            {
                this[index - 1].Next = this[index + 1];
                this[index + 1].Previous = this[index - 1];
            }
            this[index].Parent = this[index].Previous = this[index].Next = null;
            base.RemoveItem(index);

        }

        ///// <summary>
        ///// ɾ���б���ָ���˵���
        ///// </summary>
        ///// <param name="menuItem"></param>
        ///// <remarks></remarks>
        //new public void Remove(MenuItem menuItem)
        //{
        //    int index = this.IndexOf(menuItem);
        //    if (index != -1)
        //    {
        //        this.RemoveItem(index);
        //    }
        //}

    }
}
