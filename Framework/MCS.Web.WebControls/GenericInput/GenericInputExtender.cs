#region
// -------------------------------------------------
// Assembly	：	MCS.Web.WebControls
// FileName	：	GenericInputExtender.cs
// Remark	：  通用录入控件Extender形式
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		张曦	    20070815		创建
// -------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Web.UI.WebControls.WebParts;
using MCS.Web.Library.Script;

[assembly: WebResource("MCS.Web.WebControls.GenericInput.GenericInputExtender.js", "application/x-javascript")]
[assembly: WebResource("MCS.Web.WebControls.GenericInput.GenericInput.css", "text/css", PerformSubstitution = true)]
[assembly: WebResource("MCS.Web.WebControls.GenericInput.drop-arrow.gif", "image/gif")]
namespace MCS.Web.WebControls
{
	/// <summary>
	/// 通用录入的扩展形式控件
	/// </summary>
	/// <remarks>
	/// 通用录入的扩展形式控件
	/// 需要配合文本框,将这个控件的TargetID指向这个文本框即可
	/// </remarks>
    [PersistChildren(false)]
    [ParseChildren(true, "Items")]
    [RequiredScript(typeof(ControlBaseScript))]
    [Designer(typeof(GenericInputExtenderDesigner))]
    [TargetControlType(typeof(Control))]
    [ClientCssResource("MCS.Web.WebControls.GenericInput.GenericInput.css")]
    [ClientScriptResource("MCS.Web.WebControls.GenericInput",
     "MCS.Web.WebControls.GenericInput.GenericInputExtender.js")]
    [PersistenceMode(PersistenceMode.InnerProperty)]
    public class GenericInputExtender : ExtenderControlBase
    {
        private ListItemCollection items = new ListItemCollection();
        private string text = "";

        #region Properties
       
        /// <summary>
        /// 控件边框的颜色
        /// </summary>
        /// <remarks>
        /// 控件边框的颜色
        /// </remarks>
        [DefaultValue(typeof(Color), "35,83,178")]//"#2353B2"
        [Category("Appearance")]
        [ScriptControlProperty]//设置此属性要输出到客户端
        [ClientPropertyName("highlightBorderColor")]
        public Color HighlightBorderColor
        {
            get { return GetPropertyValue("HighlightBorderColor", Color.FromArgb(35, 83, 178)); }
            set { SetPropertyValue("HighlightBorderColor", value); }
        }

		/// <summary>
		/// 输入框边框顶部宽度
		/// </summary>
		[Category("Appearance")]
		[ScriptControlProperty]//设置此属性要输出到客户端
		[ClientPropertyName("highlightBorderTopWidth")]
		public int HighlightBorderTopWidth
		{
			get { return GetPropertyValue("HighlightBorderTopWidth", 1); }
			set { SetPropertyValue("HighlightBorderTopWidth", value); }
		}

		/// <summary>
		/// 输入框边框左侧宽度
		/// </summary>
		[Category("Appearance")]
		[ScriptControlProperty]//设置此属性要输出到客户端
		[ClientPropertyName("highlightBorderLeftWidth")]
		public int HighlightBorderLeftWidth
		{
			get { return GetPropertyValue("HighlightBorderLeftWidth", 1); }
			set { SetPropertyValue("HighlightBorderLeftWidth", value); }
		}

		/// <summary>
		/// 输入框边框右侧宽度
		/// </summary>
		[Category("Appearance")]
		[ScriptControlProperty]//设置此属性要输出到客户端
		[ClientPropertyName("highlightBorderRightWidth")]
		public int HighlightBorderRightWidth
		{
			get { return GetPropertyValue("HighlightBorderRightWidth", 1); }
			set { SetPropertyValue("HighlightBorderRightWidth", value); }
		}

		/// <summary>
		/// 输入框边框底部宽度
		/// </summary>
		[Category("Appearance")]
		[ScriptControlProperty]//设置此属性要输出到客户端
		[ClientPropertyName("highlightBorderBottomWidth")]
		public int HighlightBorderBottomWidth
		{
			get { return GetPropertyValue("HighlightBorderBottomWidth", 1); }
			set { SetPropertyValue("HighlightBorderBottomWidth", value); }
		}

        /// <summary>
        /// 选择项目默认字体颜色
        /// </summary>
        /// <remarks>
        /// 选择项目默认字体颜色
        /// </remarks>
        [DefaultValue("")]
        [Category("Appearance")]
        [ScriptControlProperty]//设置此属性要输出到客户端
        [ClientPropertyName("itemCssClass")]
        public string ItemCssClass
        {
            get { return GetPropertyValue("ItemCssClass", ""); }
            set { SetPropertyValue("ItemCssClass", value); }
        }

        /// <summary>
        /// 鼠标移动到选项项目上时的字体颜色
        /// </summary>
        /// <remarks>
        /// 鼠标移动到选项项目上时的字体颜色
        /// </remarks>
        [DefaultValue("")]
        [Category("Appearance")]
        [ScriptControlProperty]//设置此属性要输出到客户端
        [ClientPropertyName("itemHoverCssClass")]
        public string ItemHoverCssClass
        {
            get { return GetPropertyValue("ItemHoverCssClass", ""); }
            set { SetPropertyValue("ItemHoverCssClass", value); }
        }

        /// <summary>
        /// 下拉箭头区域的背景色
        /// </summary>
        /// <remarks>
        /// 下拉箭头区域的背景色
        /// </remarks>
        [DefaultValue(typeof(Color), "198, 225, 255")]//"#C6E1FF"
        [Category("Appearance")]
        [ScriptControlProperty]//设置此属性要输出到客户端
        [ClientPropertyName("dropArrowBackgroundColor")]
        public Color DropArrowBackgroundColor
        {
            get { return GetPropertyValue("DropArrowBackgroundColor", Color.FromArgb(198, 225, 255)); }
            set { SetPropertyValue("DropArrowBackgroundColor", value); }
        }

        /// <summary>
        /// 控件的当前文本
        /// </summary>
        /// <remarks>
        /// 控件的当前文本
        /// </remarks>
        [ScriptControlProperty]//设置此属性要输出到客户端
        [ClientPropertyName("text")]
        public string Text
        {
            get
            {
                return this.text;
            }
            set
            {
                this.text = value;
            }
        }

        /// <summary>
        /// 文本改变的事件
        /// </summary>
        [DefaultValue("")]
        public string OnChange
        {
            get { return GetPropertyValue("OnChange", ""); }
            set { SetPropertyValue("OnChange", value); }
        }

        /// <summary>
        /// 选择项目的事件
        /// </summary>
        [DefaultValue("")]
        public string OnSelectedItem
        {
            get { return GetPropertyValue("OnSelectedItem", ""); }
            set { SetPropertyValue("OnSelectedItem", value); }
        }

        /// <summary>
        /// 选择项目前的事件
        /// </summary>
        [DefaultValue("")]
        public string OnSelectItem
        {
            get { return GetPropertyValue("OnSelectItem", ""); }
            set { SetPropertyValue("OnSelectItem", value); }
        }

        /// <summary>
        /// 当前选择的Index值
        /// </summary>
        /// <remarks>
        /// 当前选择的Index值
        /// </remarks>
        [DefaultValue(-1)]
        [Browsable(false)]
        [ScriptControlProperty]//设置此属性要输出到客户端
        [ClientPropertyName("selectIndex")]
        public int SelectedIndex
        {
            get { return GetPropertyValue("SelectedIndex", -1); }
            set { SetPropertyValue("SelectedIndex", value); }
        }

        /// <summary>
        /// 控件中的选择项目集合
        /// </summary>
        /// <remarks>
        /// 控件中的选择项目集合
        /// </remarks>
        [ScriptControlProperty]//设置此属性要输出到客户端
        [ClientPropertyName("items")]//设置此属性对应客户端属性的名称
        [Editor("System.Web.UI.Design.WebControls.ListItemsCollectionEditor,System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(System.Drawing.Design.UITypeEditor))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ListItemCollection Items
        {
            get
            {
                return this.items;
            }
        }
        #endregion

        //---------------------------------我就素那无耻的分割线---------------------------------//

        #region ClientState
        /// <summary>
        /// 加载ClientState
        /// </summary>
        /// <remarks>
        /// 加载ClientState
        ///     ClientState中保存的是一个长度为3的一维数组
        ///         第一个为选中项目的索引值，如果没有选中则为-1
        ///         第二个为选中项目的文本，或者输入的文本
        ///         第三个是Items
        /// </remarks>
        /// <param name="clientState">序列化后的clientState</param>
        protected override void LoadClientState(string clientState)
        {
            base.LoadClientState(clientState);

            object[] foArray = JSONSerializerExecute.Deserialize<object[]>(clientState);

            if (null != foArray && foArray.Length > 0)
            {
                this.SelectedIndex = Convert.ToInt32(foArray[0]);
                if (foArray.Length > 1 && null != foArray[1])
                    this.Text = foArray[1].ToString();
                else
                    this.Text = "";

                
                if (foArray.Length > 2 && null != foArray[2])
                    this.items = (ListItemCollection)JSONSerializerExecute.DeserializeObject(foArray[2], typeof(ListItemCollection));
                else
                    this.items = new ListItemCollection();
            }
            else
            {
                this.SelectedIndex = -1;
                this.Text = "";
                this.items = new ListItemCollection();
            }
        }

        /// <summary>
        /// 保存ClientState
        /// </summary>
        /// <remarks>
        /// 保存ClientState
        ///     ClientState中保存的是一个长度为2的一维数组
        ///         第一个为选中项目的索引值，如果没有选中则为-1
        ///         第二个为选中项目的文本，或者输入的文本
        /// </remarks>
        /// <returns>序列化后的CLientState字符串</returns>
        protected override string SaveClientState()
        {
            object[] foArray = new object[] { this.SelectedIndex, this.Text ,this.items};

            string fsSerialize = JSONSerializerExecute.Serialize(foArray);

            return fsSerialize;
        }
        #endregion
    }
}
