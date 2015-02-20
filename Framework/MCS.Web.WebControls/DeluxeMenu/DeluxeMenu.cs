#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Web.DeluxeMenu
// FileName	：	DeluxeMenu.cs
// Remark	：	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    吴伟	    20070720		创建
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
    /// 横纵向枚举
    /// </summary>
    ///<remarks></remarks>
    public enum MenuOrientation
    {
		/// <summary>
		/// 纵向
		/// </summary>
        Vertical = 0,
		/// <summary>
		/// 横向
		/// </summary>
        Horizontal = 1
    }

	/// <summary>
	/// 菜单类
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
        /// PopupMenu构造函数
        /// </summary>
        /// <remarks></remarks>
		public DeluxeMenu()
            : base(true, HtmlTextWriterTag.Div)
        {
            this.items = new MenuItemCollection(null);
            //this.Style.Add("Width", "0");

        }

        /// <summary>
        /// 判断如果是设计态则在页面上绘制菜单树
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (this.DesignMode == false)
                this.PreloadAllImages();
        }

        /// <summary>
        /// 输出菜单
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
        /// 一级菜单排列方向
        /// </summary>
		///<remarks>一级菜单排列方向</remarks>
        [Category("Appearance")]
        [Description("一级菜单排列方向")]
        [ScriptControlProperty]
        public MenuOrientation Orientation
        {
            get { return GetPropertyValue("Orientation", MenuOrientation.Vertical); }
            set { SetPropertyValue("Orientation", value); }
        }

        /// <summary>
        /// 是否是多选
        /// </summary>
		///<remarks>是否是多选</remarks>
        [DefaultValue(false)]
        [Category("Appearance")]
        [Description("是否是多选")]
        [ScriptControlProperty]
        public bool MultiSelect
        {
            get { return GetPropertyValue("MultiSelect", false); }
            set { SetPropertyValue("MultiSelect", value); }
        }

		/// <summary>
		/// 是否是多选
		/// </summary>
		///<remarks>是否是多选</remarks>
		[DefaultValue(false)]
		[Category("Appearance")]
		[Description("是否处理分隔线")]
		[ScriptControlProperty]
		public bool HasControlSeparator
		{
			get { return GetPropertyValue("HasControlSeparator", false); }
			set { SetPropertyValue("HasControlSeparator", value); }
		}

        /// <summary>
        /// 菜单项的宽度
        /// </summary>
		///<remarks>菜单项的宽度</remarks>
        [DefaultValue(150)]
        [Category("Appearance")]
        [Description("菜单项文字宽度")]
        [ScriptControlProperty]
        public int ItemFontWidth
        {
            get { return GetPropertyValue("ItemFontWidth", 150); }
            set { SetPropertyValue("ItemFontWidth", value); }
        }

        /// <summary>
        /// 静态菜单中，子菜单缩进长度
        /// </summary>
		///<remarks>静态菜单中，子菜单缩进长度</remarks>
        [DefaultValue(10)]
        [Category("Appearance")]
        [ScriptControlProperty]
        [ClientPropertyName("subMenuIndent"), Description("静态菜单中，子菜单缩进长度")]
        public int SubMenuIndent
        {
            get { return GetPropertyValue("SubMenuIndent", 10); }
            set { SetPropertyValue("SubMenuIndent", value); }
        }

        /// <summary>
		/// 静态菜单级别，默认为1
        /// </summary>
		///<remarks>静态菜单级别，默认为1</remarks>
        [DefaultValue(1)]
        [Category("Appearance"), Description("静态菜单级别，默认为1")]
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
        //[ClientPropertyName("popOutImageUrl"), WebDescription("弹出菜单图标")]
        //public string PopOutImageUrl
        //{
        //    get { return (string)(ViewState["PopOutImageUrl"] ?? 
        //        this.Page.ClientScript.GetWebResourceUrl(typeof(MCS.Web.WebControls.PopupMenuControl), 
        //        "MCS.Web.WebControls.PopupMenu.popOutImageUrl.gif")); }
        //    set { ViewState["PopOutImageUrl"] = value; }
        //}

        /// <summary>
        /// 标示默认下一级动态菜单图标
        /// </summary>
		///<remarks>标示默认下一级动态菜单图标</remarks>
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
        /// 标示下一级静态菜单图标
        /// </summary>
		///<remarks>标示下一级静态菜单图标</remarks>
        [DefaultValue("")]
        [Category("Appearance")]
        [ScriptControlProperty]
        [ClientPropertyName("staticPopOutImageUrl"), Description("标示下一级静态菜单图标")]
        [Editor("System.Web.UI.Design.ImageUrlEditor, System.Design", typeof(System.Drawing.Design.UITypeEditor))]
        public string StaticPopOutImageUrl
        {
			get { return ResolveUrl(GetPropertyValue("StaticPopOutImageUrl", string.Empty)); }
            set { SetPropertyValue("StaticPopOutImageUrl", value); }
        }

        /// <summary>
        /// 标示下一级动态菜单图标
        /// </summary>
		///<remarks>标示下一级动态菜单图标</remarks>
        [DefaultValue("")]
        [Category("Appearance")]
        [ScriptControlProperty]
        [ClientPropertyName("dynamicPopOutImageUrl"), Description("标示下一级动态菜单图标")]
        [Editor("System.Web.UI.Design.ImageUrlEditor, System.Design", typeof(System.Drawing.Design.UITypeEditor))]
        public string DynamicPopOutImageUrl
        {
			get { return ResolveUrl(GetPropertyValue("DynamicPopOutImageUrl", string.Empty)); }
            set { SetPropertyValue("DynamicPopOutImageUrl", value); }
        }

        /// <summary>
        /// 打开链接目标
        /// </summary>
		///<remarks>打开链接目标</remarks>
        [DefaultValue("")]
        [Category("Behavior")]
        [ScriptControlProperty]
        [ClientPropertyName("target"), Description("打开链接目标")]
        public string Target
        {
            get { return GetPropertyValue("Target", string.Empty); }
            set { SetPropertyValue("Target", value); }
        }

        ///// <summary>
        ///// 菜单的CssClass
        ///// </summary>
        /////<remarks></remarks>
        //[DefaultValue("")]
        //[Category("Appearance")]
        //[ScriptControlProperty]
        //[ClientPropertyName("cssClass"), WebDescription("菜单的CssClass")]
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
        /// 菜单条目的CssClass
        /// </summary>
		///<remarks>菜单条目的CssClass</remarks>
        [DefaultValue("")]
        [Category("Appearance")]
        [ScriptControlProperty]
        [ClientPropertyName("itemCssClass"), Description("菜单条目的CssClass")]
        public string ItemCssClass
        {
            get { return GetPropertyValue("ItemCssClass", string.Empty); }
            set { SetPropertyValue("ItemCssClass", value); }
        }

        /// <summary>
        /// 鼠标移到菜单条目的CssClass
        /// </summary>
		///<remarks>鼠标移到菜单条目的CssClass</remarks>
        [DefaultValue("")]
        [Category("Appearance")]
        [ScriptControlProperty]
        [ClientPropertyName("hoverItemCssClass"), Description("鼠标移到菜单条目的CssClass")]
        public string HoverItemCssClass
        {
            get { return GetPropertyValue("HoverItemCssClass", string.Empty); }
            set { SetPropertyValue("HoverItemCssClass", value); }
        }

        /// <summary>
        /// 菜单条目选择后的CssClass
        /// </summary>
		///<remarks>菜单条目选择后的CssClass</remarks>
        [DefaultValue("")]
        [Category("Appearance")]
        [ScriptControlProperty]
        [ClientPropertyName("selectedItemCssClass"), Description("菜单条目选择后的CssClass")]
        public string SelectedItemCssClass
        {
            get { return GetPropertyValue("SelectedItemCssClass", string.Empty); }
            set { SetPropertyValue("SelectedItemCssClass", value); }
        }

        /// <summary>
        /// 菜单分割线的CssClass
        /// </summary>
		///<remarks>菜单分割线的CssClass</remarks>
        [DefaultValue("")]
        [Category("Appearance")]
        [ScriptControlProperty]
        [ClientPropertyName("separatorCssClass"), Description("菜单分割线的CssClass")]
        public string SeparatorCssClass
        {
            get { return GetPropertyValue("SeparatorCssClass", string.Empty); }
            set { SetPropertyValue("SeparatorCssClass", value); }
        }

        /// <summary>
        /// 菜单文本前的空格宽度，默认为5
        /// </summary>
		///<remarks>菜单文本前的空格宽度，默认为5</remarks>
        [DefaultValue(5)]
        [Category("Appearance")]
        [ScriptControlProperty]
        [ClientPropertyName("textHeadWidth"), Description("菜单文本前的空格宽度，默认为5")]
        public int TextHeadWidth
        {
            get { return GetPropertyValue("TextHeadWidth", 5); }
            set { SetPropertyValue("TextHeadWidth", value); }
        }

        /// <summary>
        /// 菜单条目前的图标表格CssClass
        /// </summary>
		///<remarks>菜单条目前的图标表格CssClass</remarks>
        [DefaultValue("")]
        [Category("Appearance")]
        [ScriptControlProperty]
        [ClientPropertyName("imageColCssClass"),Description("菜单条目前的图标表格CssClass")]
        public string ImageColCssClass
        {
            get { return GetPropertyValue("ImageColCssClass", string.Empty); }
            set { SetPropertyValue("ImageColCssClass", value); }
        }

        /// <summary>
        /// 菜单条目前的图标是否缩进，默认为否
        /// </summary>
		///<remarks>菜单条目前的图标是否缩进，默认为否</remarks>
        [DefaultValue(false)]
        [Category("Appearance")]
        [ScriptControlProperty]
        [ClientPropertyName("isImageIndent"), Description("菜单条目前的图标是否缩进，默认为否")]
        public bool IsImageIndent
        {
            get { return GetPropertyValue("IsImageIndent", false); }
            set { SetPropertyValue("IsImageIndent", value); }
        }

        /// <summary>
        /// 菜单条目集合
        /// </summary>
		///<remarks>菜单条目集合</remarks>
        [DefaultValue((string)null)]
        [Category("Date")]
        [Description("菜单条目集合")]
        [Editor(typeof(MenuCollectionItemsEditor), typeof(UITypeEditor))]
        [ScriptControlProperty, ClientPropertyName("items")]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public MenuItemCollection Items
        {
            get { return this.items; }
        }

        /// <summary>
        /// 单击菜单项事件
        /// </summary>
		///<remarks>单击菜单项事件</remarks>
        [DefaultValue("")]
        [Category("ClientEventsHandler")]
        [ScriptControlEvent]
        [ClientPropertyName("mclick")]
        [DescriptionAttribute("单击事件")]
        public string OnMenuItemClick
        {
            get { return GetPropertyValue("OnMenuItemClick", string.Empty); }
            set { SetPropertyValue("OnMenuItemClick", value); }
        }

        /// <summary>
        /// 子菜单弹出事件
        /// </summary>
		///<remarks>子菜单弹出事件</remarks>
        [DefaultValue("")]
        [Category("ClientEventsHandler")]
        [ScriptControlEvent]
        [ClientPropertyName("mover")]
        [DescriptionAttribute("子菜单弹出事件")]
		public string OnMenuItemShown
        {
			get { return GetPropertyValue("OnMenuItemShown", string.Empty); }
			set { SetPropertyValue("OnMenuItemShown", value); }
        }

        #region  PostData

        /// <summary>
        /// 加载客户端状态
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
		/// 保存客户端状态
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
        /// 将menuitem对象的nodeid、selected加入数组如：["nodeid",selected],["nodeid",selected]
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
		/// 将客户端序列化的数据改变MenuItemCollection数据
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
        /// 展示设计时状态
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
        /// 展示设计时有菜单状态
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

        # region   预先加载图片
        /// <summary>
        /// 预先加载图片
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
    #endregion   菜单子项

  
}
