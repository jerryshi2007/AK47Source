#region
// -------------------------------------------------
// Assembly	：	MCS.Web.WebControls
// FileName	：	GenericInput.cs
// Remark	：  通用录入控件，这个控件是一个复合控件，是一个Input和一个通用录入扩展控件
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		张曦	    20070815		创建
// -------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using MCS.Web.WebControls;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Text.RegularExpressions;
using System.ComponentModel;
using MCS.Web.Library.Script;

namespace MCS.Web.WebControls
{
	/// <summary>
	/// 通用录入控件服务器端代码
	/// </summary>
	/// <remarks>
	/// 这个控件是一个复合控件，是一个Input和一个通用录入扩展控件
	/// </remarks>
	public class GenericInput : System.Web.UI.WebControls.WebControl
	{
		private GenericInputExtender oGIE = new GenericInputExtender();
		private System.Web.UI.HtmlControls.HtmlInputText oText = new System.Web.UI.HtmlControls.HtmlInputText();

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <remarks>
		/// 构造函数
		/// </remarks>
		public GenericInput()
		{ }

		/// <summary>
		/// 初始化
		/// </summary>
		/// <remarks>
		/// 只读时添加Literal否则添加Input和GenericInputExtender
		/// </remarks>
		/// <param name="e">EventArgs</param>
		protected override void OnInit(EventArgs e)
		{
			if (!this.DesignMode)
			{
				if (!ReadOnly)
				{

					string sID = System.Guid.NewGuid().ToString();
					this.oGIE.ID = "GenericInput_" + sID;
					this.oText.ID = "Text_" + sID;
					this.oGIE.TargetControlID = this.oText.ID;
					this.oText.Value = this.Text;
					this.oText.Attributes.Add("style", "border-style:None;width:" + this.Width.ToString());

					Controls.Add(this.oGIE);
					Controls.Add(this.oText);
				}
				else
				{
					Literal foLtr = new Literal();
					foLtr.Text = this.Text;
					foLtr.ID = "Ltr_" + this.ID;
					for (int i = 0; i < Controls.Count; i++)
					{
						if (Controls[i].ID == ("Ltr_" + this.ID))
						{
							Controls.RemoveAt(i);
							break;
						}
					}
					Controls.Add(foLtr);
				}
			}
			base.OnInit(e);
		}

		/// <summary>
		/// 输出时的代码
		/// </summary>
		/// <remarks>设计时输出一个Select，运行时未进行处理</remarks>
		/// <param name="writer">HtmlTextWriter</param>
		protected override void Render(System.Web.UI.HtmlTextWriter writer)
		{
			if (this.DesignMode)
			{
				System.Web.UI.HtmlControls.HtmlSelect ddl = new System.Web.UI.HtmlControls.HtmlSelect();
				ddl.ID = "ddl_Generic_Design";
				for (int i = 0; i < Controls.Count; i++)
				{
					if (Controls[i].ID == "ddl_Generic_Design")
					{
						Controls.RemoveAt(i);
						break;
					}
				}
				ddl.Attributes.Add("style", "width:" + this.Width.ToString());
				Controls.Add(ddl);
			}
			base.Render(writer);
		}

		#region Properties
		/// <summary>
		/// 控件边框的颜色
		/// </summary>
		/// <remarks>
		/// 控件边框的颜色
		/// </remarks>
		[DefaultValue(typeof(Color), "35,83,178")]//"#2353B2"
		[Category("Appearance")]
		public Color HighlightBorderColor
		{
			get { return this.oGIE.HighlightBorderColor; }
			set { this.oGIE.HighlightBorderColor = value; }
		}

		/// <summary>
		/// 选择项目默认字体颜色
		/// </summary>
		/// <remarks>
		/// 选择项目默认字体颜色
		/// </remarks>
		[DefaultValue("")]
		[Category("Appearance")]
		public string ItemCssClass
		{
			get { return this.oGIE.ItemCssClass; }
			set { this.oGIE.ItemCssClass = value; }
		}

		/// <summary>
		/// 鼠标移动到选项项目上时的字体颜色
		/// </summary>
		/// <remarks>
		/// 鼠标移动到选项项目上时的字体颜色
		/// </remarks>
		[DefaultValue("")]
		[Category("Appearance")]
		public string ItemHoverCssClass
		{
			get { return this.oGIE.ItemHoverCssClass; }
			set { this.oGIE.ItemHoverCssClass = value; }
		}

		/// <summary>
		/// 下拉箭头区域的背景色
		/// </summary>
		/// <remarks>
		/// 下拉箭头区域的背景色
		/// </remarks>
		[DefaultValue(typeof(Color), "198, 225, 255")]//"#C6E1FF"
		[Category("Appearance")]
		public Color DropArrowBackgroundColor
		{
			get { return this.oGIE.DropArrowBackgroundColor; }
			set { this.oGIE.DropArrowBackgroundColor = value; }
		}


		/// <summary>
		/// 输入框边框顶部宽度
		/// </summary>
		[Category("Appearance")]
		public int HighlightBorderTopWidth
		{
			get { return this.oGIE.HighlightBorderTopWidth; }
			set { this.oGIE.HighlightBorderTopWidth = value; }
		}

		/// <summary>
		/// 输入框边框左侧宽度
		/// </summary>
		[Category("Appearance")]
		public int HighlightBorderLeftWidth
		{
			get { return this.oGIE.HighlightBorderLeftWidth; }
			set { this.oGIE.HighlightBorderLeftWidth = value; }
		}

		/// <summary>
		/// 输入框边框右侧宽度
		/// </summary>
		[Category("Appearance")]
		public int HighlightBorderRightWidth
		{
			get { return this.oGIE.HighlightBorderRightWidth; }
			set { this.oGIE.HighlightBorderRightWidth = value; }
		}

		/// <summary>
		/// 输入框边框底部宽度
		/// </summary>
		[Category("Appearance")]
		[ScriptControlProperty]//设置此属性要输出到客户端
		[ClientPropertyName("highlightBorderTopWidth")]
		public int HighlightBorderBottomWidth
		{
			get { return this.oGIE.HighlightBorderBottomWidth; }
			set { this.oGIE.HighlightBorderBottomWidth = value; }
		}

		

		/// <summary>
		/// 控件的当前文本
		/// </summary>
		/// <remarks>
		/// 控件的当前文本
		/// </remarks>
		public string Text
		{
			get
			{
				return this.oGIE.Text;
			}
			set
			{
				this.oGIE.Text = value;
			}
		}

		/// <summary>
		/// 文本改变的事件
		/// </summary>
		[DefaultValue("")]
		public string OnChange
		{
			get { return this.oGIE.OnChange; }
			set { this.oGIE.OnChange = value; }
		}

		/// <summary>
		/// 选择项目的事件
		/// </summary>
		[DefaultValue("")]
		public string OnSelectedItem
		{
			get { return this.oGIE.OnSelectedItem; }
			set { this.oGIE.OnSelectedItem = value; }
		}

		/// <summary>
		/// 选择项目前的事件
		/// </summary>
		[DefaultValue("")]
		public string OnSelectItem
		{
			get { return this.oGIE.OnSelectItem; }
			set { this.oGIE.OnSelectItem = value; }
		}

		/// <summary>
		/// 当前选择的Index值
		/// </summary>
		/// <remarks>
		/// 当前选择的Index值
		/// </remarks>
		[DefaultValue(-1)]
		[Browsable(false)]
		public int SelectedIndex
		{
			get { return this.oGIE.SelectedIndex; }
			set { this.oGIE.SelectedIndex = value; }
		}

		/// <summary>
		/// 控件中的选择项目集合
		/// </summary>
		/// <remarks>
		/// 控件中的选择项目集合
		/// </remarks>
		public ListItemCollection Items
		{
			get
			{
				return this.oGIE.Items;
			}
		}
		#endregion

		/// <summary>
		/// 控件是否只读
		/// </summary>
		/// <remarks>
		/// 控件是否只读
		/// </remarks>
		public bool ReadOnly
		{
			get
			{
				if (ViewState["IsReadOnly"] != null)
				{
					return (bool)ViewState["IsReadOnly"];
				}
				return false;
			}
			set
			{
				ViewState["IsReadOnly"] = value;
			}
		}
	}
}
