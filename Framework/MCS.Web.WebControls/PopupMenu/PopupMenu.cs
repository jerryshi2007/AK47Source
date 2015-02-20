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
    /// 分割线的类型
    /// </summary>
    /// <remarks></remarks>
    public enum PopupMenuItemSeparatorMode
    {
        None = 0,
        Bottom = 1,
        Top = 2
    }

    /// <summary>
    /// 横纵向枚举
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
        /// PopupMenu构造函数
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
        ///<remarks></remarks>
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
        ///<remarks></remarks>
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
        /// 菜单项的宽度
        /// </summary>
        ///<remarks></remarks>
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
        ///<remarks></remarks>
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
        /// 
        /// </summary>
        ///<remarks></remarks>
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
        //        this.Page.ClientScript.GetWebResourceUrl(typeof(ChinaCustoms.Framework.DeluxeWorks.Web.WebControls.PopupMenuControl), 
        //        "ChinaCustoms.Framework.DeluxeWorks.Web.WebControls.PopupMenu.popOutImageUrl.gif")); }
        //    set { ViewState["PopOutImageUrl"] = value; }
        //}

        /// <summary>
        /// 标示默认下一级动态菜单图标
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
        /// 标示下一级静态菜单图标
        /// </summary>
        ///<remarks></remarks>
        [DefaultValue("")]
        [Category("Appearance")]
        [ScriptControlProperty]
        [ClientPropertyName("staticPopOutImageUrl"), Description("标示下一级静态菜单图标")]
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
        /// 标示下一级动态菜单图标
        /// </summary>
        ///<remarks></remarks>
        [DefaultValue("")]
        [Category("Appearance")]
        [ScriptControlProperty]
        [ClientPropertyName("dynamicPopOutImageUrl"), Description("标示下一级动态菜单图标")]
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
        /// 打开链接目标
        /// </summary>
        ///<remarks></remarks>
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
        ///<remarks></remarks>
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
        ///<remarks></remarks>
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
        ///<remarks></remarks>
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
        ///<remarks></remarks>
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
        ///<remarks></remarks>
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
        ///<remarks></remarks>
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
        ///<remarks></remarks>
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
        ///<remarks></remarks>
        [DefaultValue((string)null)]
        [Category("Date")]
        [Description("菜单条目集合")]
        [Editor(typeof(MenuCollectionItemsEditor), typeof(UITypeEditor))]
        [ScriptControlProperty, ClientPropertyName("items")]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public MenuItemCollection Items
        {
            get { return items; }
        }

        /// <summary>
        /// 单击菜单项事件
        /// </summary>
        ///<remarks></remarks>
        [DefaultValue("")]
        [Category("ClientEventsHandler")]
        [ScriptControEventAttribute]
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
        ///<remarks></remarks>
        [DefaultValue("")]
        [Category("ClientEventsHandler")]
        [ScriptControEventAttribute]
        [ClientPropertyName("mover")]
        [DescriptionAttribute("子菜单弹出事件")]
        public string OnMenuPopup
        {
            get { return GetPropertyValue("OnMenuPopup", string.Empty); }
            set { SetPropertyValue("OnMenuPopup", value); }
        }

        #region  PostData

        /// <summary>
        /// 加载客户端状态
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
        /// 保存客户端状态
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
        /// 展示设计时状态
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
        /// 展示设计时有菜单状态
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

        # region   预先加载图片
        /// <summary>
        /// 预先加载图片
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
    #endregion   菜单子项

    [ParseChildren(true, "ChildItems")]
    public class MenuItem : IStateManager
    {
        private MenuItem parent;
        private MenuItem previous;
        private MenuItem next;
        private MenuItemCollection childItems;
        private StateBag viewState;

        /// <summary>
        /// MenuItem构造函数
        /// </summary>
        ///<remarks></remarks>
        public MenuItem()
        {
            childItems = new MenuItemCollection(this);
            viewState = new StateBag();
        }

        /// <summary>
        /// 节点的编号
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
        /// 是否是分隔线
        /// </summary>
        ///<remarks>是否是分隔线</remarks>
        [DefaultValue(false)]
        [Category("Appearance")]
        [ClientPropertyName("isSeparator")]
        [Description("是否是分隔线")]
        public bool IsSeparator
        {
            get { return (bool)(viewState["IsSeparator"] ?? false); }
            set { viewState["IsSeparator"] = value; }
        }

        /// <summary>
        /// 该菜单项是否可用
        /// </summary>
        ///<remarks></remarks>
        [DefaultValue(true)]
        [Category("Appearance")]
        [ClientPropertyName("enable")]
        [Description("该菜单项是否可用")]
        public bool Enable
        {
            get { return (bool)(viewState["Enable"] ?? true); }
            set { viewState["Enable"] = value; }
        }

        /// <summary>
        /// 该菜单项是否可见
        /// </summary>
        ///<remarks></remarks>
        [DefaultValue(true)]
        [Category("Appearance")]
        [ClientPropertyName("visible")]
        [Description("该菜单项是否可见")]
        public bool Visible
        {
            get { return (bool)(viewState["Visible"] ?? true); }
            set { viewState["Visible"] = value; }
        }

        /// <summary>
        /// 菜单条目文本
        /// </summary>
        /// <remarks></remarks>
        [DefaultValue("")]
        [Category("Appearance")]
        [ClientPropertyName("text")]
        [Description("菜单条目文本")]
        public string Text
        {
            get { return (string)(viewState["Text"] ?? string.Empty); }
            set { viewState["Text"] = value; }
        }

        /// <summary>
        /// 菜单条目值
        /// </summary>
        /// <remarks></remarks>
        [DefaultValue("")]
        [Category("Appearance")]
        [Description("菜单条目值")]
        [ClientPropertyName("value")]
        public string Value
        {
            get { return (string)(viewState["Value"] ?? string.Empty); }
            set { viewState["Value"] = value; }
        }

        /// <summary>
        /// 菜单条目链接目标
        /// </summary>
        /// <remarks></remarks>
        [DefaultValue("")]
        [Category("Appearance")]
        [Description("菜单条目链接目标")]
        [ClientPropertyName("target")]
        public string Target
        {
            get { return (string)(viewState["Target"] ?? string.Empty); }
            set { viewState["Target"] = value; }
        }

        //[WebDescription("弹出菜单图标")]
        //public string PopOutImageUrl
        //{
        //    get { return (string)(_ViewState["PopOutImageUrl"] ?? string.Empty); }
        //    set { _ViewState["PopOutImageUrl"] = value; }
        //}

        /// <summary>
        /// 弹出静态菜单图标
        /// </summary>
        /// <remarks></remarks>
        [DefaultValue("")]
        [Category("Appearance")]
        [Description("弹出静态菜单图标")]
        [ClientPropertyName("staticPopOutImageUrl")]
        [Editor("System.Web.UI.Design.ImageUrlEditor, System.Design", typeof(System.Drawing.Design.UITypeEditor))]
        public string StaticPopOutImageUrl
        {
            get { return (string)(viewState["StaticPopOutImageUrl"] ?? string.Empty); }
            set { viewState["StaticPopOutImageUrl"] = value; }
        }

        /// <summary>
        /// 弹出动态菜单图标
        /// </summary>
        /// <remarks></remarks>
        [DefaultValue("")]
        [Category("Appearance")]
        [Description("弹出动态菜单图标")]
        [ClientPropertyName("dynamicPopOutImageUrl")]
        [Editor("System.Web.UI.Design.ImageUrlEditor, System.Design", typeof(System.Drawing.Design.UITypeEditor))]
        public string DynamicPopOutImageUrl
        {
            get { return (string)(viewState["DynamicPopOutImageUrl"] ?? string.Empty); }
            set { viewState["DynamicPopOutImageUrl"] = value; }
        }

        /// <summary>
        /// 菜单条目链接Url
        /// </summary>
        /// <remarks></remarks>
        [DefaultValue("")]
        [Category("Appearance")]
        [Description("菜单条目链接Url")]
        [ClientPropertyName("navigateUrl")]
        public string NavigateUrl
        {
            get { return (string)(viewState["NavigateUrl"] ?? string.Empty); }
            set { viewState["NavigateUrl"] = value; }
        }

        /// <summary>
        /// 菜单条目前图标
        /// </summary>
        /// <remarks></remarks>
        [DefaultValue("")]
        [Category("Appearance")]
        [Description("菜单条目前图标")]
        [ClientPropertyName("imageUrl")]
        public string ImageUrl
        {
            get { return (string)(viewState["ImageUrl"] ?? string.Empty); }
            set { viewState["ImageUrl"] = value; }
        }

        /// <summary>
        /// 菜单条目提示
        /// </summary>
        /// <remarks></remarks>
        [DefaultValue("")]
        [Category("Appearance")]
        [Description("菜单条目提示")]
        [ClientPropertyName("toolTip")]
        public string ToolTip
        {
            get { return (string)(viewState["ToolTip"] ?? string.Empty); }
            set { viewState["ToolTip"] = value; }
        }

        /// <summary>
        /// 菜单条目是否选择
        /// </summary>
        /// <remarks></remarks>
        [DefaultValue(false)]
        [Category("Appearance")]
        [Description("菜单条目是否选择")]
        [ClientPropertyName("selected")]
        public bool Selected
        {
            get { return (bool)(viewState["Selected"] ?? false); }
            set { viewState["Selected"] = value; }
        }

        /// <summary>
        /// 菜单条目分割线样式
        /// </summary>
        /// <remarks></remarks>
        [Description("菜单条目分割线样式")]
        [ClientPropertyName("separatorMode")]
        public PopupMenuItemSeparatorMode SeparatorMode
        {
            get { return (PopupMenuItemSeparatorMode)(viewState["SeparatorMode"] ?? PopupMenuItemSeparatorMode.None); }
            set { viewState["SeparatorMode"] = value; }
        }

        /// <summary>
        /// 父节点
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
        /// 前一兄弟节点
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
        /// 后一兄弟节点
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
        /// 子菜单集合
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
        /// 存储视图状态
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
        /// 跟踪视图状态
        /// </summary>
        /// <remarks></remarks>
        void IStateManager.TrackViewState()
        {
            ((IStateManager)viewState).TrackViewState();
        }

        /// <summary>
        /// 加载视图状态
        /// </summary>
        /// <param name="saveState"></param>
        /// <remarks></remarks>
        void IStateManager.LoadViewState(object saveState)
        {
            if (saveState != null)
                ((IStateManager)viewState).LoadViewState(saveState);
        }

        /// <summary>
        /// 保存视图状态
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
        /// MenuItemCollection构造函数
        /// </summary>
        /// <remarks></remarks>
        public MenuItemCollection()
            : this(null)
        {
        }

        /// <summary>
        ///  MenuItemCollection构造函数
        /// </summary>
        /// <param name="ownerItem"></param>
        /// <remarks></remarks>
        public MenuItemCollection(MenuItem ownerItem)
        {
            this.ownerItem = ownerItem;
        }

        ///// <summary>
        ///// 清除菜单列表
        ///// </summary>
        ///// <remarks></remarks>
        //protected override void ClearItems()
        //{
        //    foreach (MenuItem item in this)
        //        item.Parent = null;
        //    base.ClearItems();
        //}

		/// <summary>
		/// 在列表中填加菜单项
		/// </summary>
		/// <param name="item"></param>
		/// <remarks></remarks>
		new public void Add(MenuItem item)
		{
			this.AddAt(this.Count, item);
		}

        /// <summary>
        /// 在指定索引处填加菜单项
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
            //如果最后一个
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
        /// 根据索引删除列表中指定菜单项
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
        ///// 删除列表中指定菜单项
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
