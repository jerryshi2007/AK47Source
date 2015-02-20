#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Web.DeluxeMenu
// FileName	��	DeluxeMenu.cs
// Remark	��	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ��ΰ	    20070720		����
// -------------------------------------------------
#endregion

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
using System.Collections.Generic;
using System.Web.Script.Serialization;


using MCS.Web.Library.Script;

[assembly: WebResource("MCS.Web.WebControls.DeluxeMenu.DeluxeMenu.js", "application/x-javascript")]
[assembly: WebResource("MCS.Web.WebControls.DeluxeMenu.PopupMenu.css", "text/css")]
[assembly: WebResource("MCS.Web.WebControls.DeluxeMenu.popOutImageUrl.gif", "image/gif")]
[assembly: WebResource("MCS.Web.WebControls.DeluxeMenu.staticPopOutImageUrl.gif", "image/gif")]
[assembly: WebResource("MCS.Web.WebControls.DeluxeMenu.dynamicPopOutImageUrl.gif", "image/gif")]
[assembly: WebResource("MCS.Web.WebControls.DeluxeMenu.back.gif", "image/gif")]

namespace MCS.Web.WebControls
{
    /// <summary>
    /// ������ö��
    /// </summary>
    ///<remarks></remarks>
    public enum MenuOrientation
    {
		/// <summary>
		/// ����
		/// </summary>
        Vertical = 0,
		/// <summary>
		/// ����
		/// </summary>
        Horizontal = 1
    }

	/// <summary>
	/// �˵���
	/// </summary>
    [PersistChildren(false)]
    [ParseChildren(true)]
    [Designer(typeof(MenuItemsDesigner))]
    [Editor(typeof(MenuItemsEditor), typeof(System.ComponentModel.ComponentEditor))]
	[RequiredScript(typeof(MenuItem))]
    [RequiredScript(typeof(ControlBaseScript))]
	[ClientCssResource("MCS.Web.WebControls.DeluxeMenu.PopupMenu.css")]
	[ClientScriptResource("MCS.Web.WebControls.DeluxeMenu",
   "MCS.Web.WebControls.DeluxeMenu.DeluxeMenu.js")] 
	public class DeluxeMenu : ScriptControlBase
    {
        private MenuItemCollection items;
        /// <summary>
        /// PopupMenu���캯��
        /// </summary>
        /// <remarks></remarks>
		public DeluxeMenu()
            : base(true, HtmlTextWriterTag.Div)
        {
            this.items = new MenuItemCollection(null);
            //this.Style.Add("Width", "0");

        }

        /// <summary>
        /// �ж���������̬����ҳ���ϻ��Ʋ˵���
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (this.DesignMode == false)
                this.PreloadAllImages();
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
		///<remarks>һ���˵����з���</remarks>
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
		///<remarks>�Ƿ��Ƕ�ѡ</remarks>
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
		/// �Ƿ��Ƕ�ѡ
		/// </summary>
		///<remarks>�Ƿ��Ƕ�ѡ</remarks>
		[DefaultValue(false)]
		[Category("Appearance")]
		[Description("�Ƿ���ָ���")]
		[ScriptControlProperty]
		public bool HasControlSeparator
		{
			get { return GetPropertyValue("HasControlSeparator", false); }
			set { SetPropertyValue("HasControlSeparator", value); }
		}

        /// <summary>
        /// �˵���Ŀ��
        /// </summary>
		///<remarks>�˵���Ŀ��</remarks>
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
		///<remarks>��̬�˵��У��Ӳ˵���������</remarks>
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
		/// ��̬�˵�����Ĭ��Ϊ1
        /// </summary>
		///<remarks>��̬�˵�����Ĭ��Ϊ1</remarks>
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
        //        this.Page.ClientScript.GetWebResourceUrl(typeof(MCS.Web.WebControls.PopupMenuControl), 
        //        "MCS.Web.WebControls.PopupMenu.popOutImageUrl.gif")); }
        //    set { ViewState["PopOutImageUrl"] = value; }
        //}

        /// <summary>
        /// ��ʾĬ����һ����̬�˵�ͼ��
        /// </summary>
		///<remarks>��ʾĬ����һ����̬�˵�ͼ��</remarks>
        [Browsable(false)]
        [ScriptControlProperty]
        [ClientPropertyName("defaultPopOutImageUrl")]
        public string DefaultPopOutImageUrl
        {
            get
            {
                return
				this.Page.ClientScript.GetWebResourceUrl(typeof(DeluxeMenu),
			  "MCS.Web.WebControls.DeluxeMenu.dynamicPopOutImageUrl.gif");
            }
        }

        /// <summary>
        /// ��ʾ��һ����̬�˵�ͼ��
        /// </summary>
		///<remarks>��ʾ��һ����̬�˵�ͼ��</remarks>
        [DefaultValue("")]
        [Category("Appearance")]
        [ScriptControlProperty]
        [ClientPropertyName("staticPopOutImageUrl"), Description("��ʾ��һ����̬�˵�ͼ��")]
        [Editor("System.Web.UI.Design.ImageUrlEditor, System.Design", typeof(System.Drawing.Design.UITypeEditor))]
        public string StaticPopOutImageUrl
        {
			get { return ResolveUrl(GetPropertyValue("StaticPopOutImageUrl", string.Empty)); }
            set { SetPropertyValue("StaticPopOutImageUrl", value); }
        }

        /// <summary>
        /// ��ʾ��һ����̬�˵�ͼ��
        /// </summary>
		///<remarks>��ʾ��һ����̬�˵�ͼ��</remarks>
        [DefaultValue("")]
        [Category("Appearance")]
        [ScriptControlProperty]
        [ClientPropertyName("dynamicPopOutImageUrl"), Description("��ʾ��һ����̬�˵�ͼ��")]
        [Editor("System.Web.UI.Design.ImageUrlEditor, System.Design", typeof(System.Drawing.Design.UITypeEditor))]
        public string DynamicPopOutImageUrl
        {
			get { return ResolveUrl(GetPropertyValue("DynamicPopOutImageUrl", string.Empty)); }
            set { SetPropertyValue("DynamicPopOutImageUrl", value); }
        }

        /// <summary>
        /// ������Ŀ��
        /// </summary>
		///<remarks>������Ŀ��</remarks>
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
		///<remarks>�˵���Ŀ��CssClass</remarks>
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
		///<remarks>����Ƶ��˵���Ŀ��CssClass</remarks>
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
		///<remarks>�˵���Ŀѡ����CssClass</remarks>
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
		///<remarks>�˵��ָ��ߵ�CssClass</remarks>
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
		///<remarks>�˵��ı�ǰ�Ŀո��ȣ�Ĭ��Ϊ5</remarks>
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
		///<remarks>�˵���Ŀǰ��ͼ����CssClass</remarks>
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
		///<remarks>�˵���Ŀǰ��ͼ���Ƿ�������Ĭ��Ϊ��</remarks>
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
		///<remarks>�˵���Ŀ����</remarks>
        [DefaultValue((string)null)]
        [Category("Date")]
        [Description("�˵���Ŀ����")]
        [Editor(typeof(MenuCollectionItemsEditor), typeof(UITypeEditor))]
        [ScriptControlProperty, ClientPropertyName("items")]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public MenuItemCollection Items
        {
            get { return this.items; }
        }

        /// <summary>
        /// �����˵����¼�
        /// </summary>
		///<remarks>�����˵����¼�</remarks>
        [DefaultValue("")]
        [Category("ClientEventsHandler")]
        [ScriptControlEvent]
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
		///<remarks>�Ӳ˵������¼�</remarks>
        [DefaultValue("")]
        [Category("ClientEventsHandler")]
        [ScriptControlEvent]
        [ClientPropertyName("mover")]
        [DescriptionAttribute("�Ӳ˵������¼�")]
		public string OnMenuItemShown
        {
			get { return GetPropertyValue("OnMenuItemShown", string.Empty); }
			set { SetPropertyValue("OnMenuItemShown", value); }
        }

        #region  PostData

        /// <summary>
        /// ���ؿͻ���״̬
        /// </summary>
        /// <param name="clientState"></param>
        ///<remarks></remarks>
        protected override void LoadClientState(string clientState)
        {
            object[] loadArray = JSONSerializerExecute.Deserialize<object[]>(clientState);

            if (null != loadArray)
            {
                ReceiveArray(this.items, loadArray);
            }

        }
		/// <summary>
		/// ����ͻ���״̬
		/// </summary>
		/// <returns></returns>
        protected override string SaveClientState()
        {
            BulidArray(this.items);

            string fsSerialize = JSONSerializerExecute.Serialize(nodeSelectedArray);

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
		/// ���ͻ������л������ݸı�MenuItemCollection����
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

                    mic[i].NodeID = ((object[])saveList[this.counter])[0].ToString();

                    mic[i].Selected = Convert.ToBoolean(((object[])saveList[this.counter])[1]);

                    ++this.counter;

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
		public string GetMenuDesignHTML(DeluxeMenu menu)
        {
            StringBuilder strB = new StringBuilder();
            if (menu.items.Count == 0)
            {
                strB.Append("<div>");
                strB.Append(MCS.Web.WebControls.Properties.Resources.EnptyMenuDesignTimeHtml);
                strB.Append("</div>");
            }
            else
            {
                strB.Append("<Table>");
                strB.Append(this.GetMenuItemsDesignHTML(menu.Items, 0));
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
        private string GetMenuItemsDesignHTML(IList<MCS.Web.WebControls.MenuItem> items, int level)
        {
            StringBuilder strB = new StringBuilder();
            foreach (MCS.Web.WebControls.MenuItem item in items)
            {
                strB.Append("<TR>");
                strB.AppendFormat("<TD style='text-indent:{0}px'>{1}</TD>", level * 10, item.Text);
                strB.Append("</TR>");
                strB.Append(this.GetMenuItemsDesignHTML(item.ChildItems, level + 1));
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
            this.PreloadImage(this.DefaultPopOutImageUrl, this.DefaultPopOutImageUrl);
            this.PreloadImage(this.DynamicPopOutImageUrl, this.DynamicPopOutImageUrl);
            this.PreloadImage(this.StaticPopOutImageUrl, this.StaticPopOutImageUrl);

            this.PreloadMenuItemsImages(this.Items);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="menuItems"></param>
        private void PreloadMenuItemsImages(MenuItemCollection menuItems)
        {
            foreach (MenuItem item in menuItems)
            {
                this.PreloadImage(item.ImageUrl, item.ImageUrl);
                this.PreloadImage(item.DynamicPopOutImageUrl, item.DynamicPopOutImageUrl);
                this.PreloadImage(item.StaticPopOutImageUrl, item.StaticPopOutImageUrl);

                this.PreloadMenuItemsImages(item.ChildItems);
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
                String valString = value as string;

                if (valString != null)
                {
                    JavaScriptSerializer ser = new JavaScriptSerializer();
                    MenuItemCollection items = ser.Deserialize<MenuItemCollection>(valString);
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

  
}
