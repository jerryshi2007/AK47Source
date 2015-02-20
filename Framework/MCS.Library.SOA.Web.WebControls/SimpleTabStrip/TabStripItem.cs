using System;
using System.Text;
using System.Web.UI;
using System.Collections;
using System.Web.UI.Design;
using System.ComponentModel;
using System.Drawing.Design;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using MCS.Library.Core;

namespace MCS.Web.WebControls
{
	/// <summary>
	/// TabStrip的每一项
	/// </summary>
	public class TabStripItem
	{
		private string key = string.Empty;
		private string text = string.Empty;
		private string logo = string.Empty;
		private Unit width = Unit.Pixel(80);
		private string controlID = string.Empty;
		private string tag = string.Empty;

		/// <summary>
		/// TabStrip页签上的图标
		/// </summary>
		[DefaultValue("")]
		[UrlProperty, Editor(typeof(UrlEditor), typeof(UITypeEditor))] 
		public string Logo
		{
			get { return this.logo; }
			set { this.logo = value; }
		}

		/// <summary>
		/// TabStrip的标签文本
		/// </summary>
		[DefaultValue("")]
		public string Text
		{
			get { return this.text; }
			set { this.text = value; }
		}

		/// <summary>
		/// TabStrip的Key
		/// </summary>
		[DefaultValue("")]
		public string Key
		{
			get { return this.key; }
			set { this.key = value; }
		}

		/// <summary>
		/// 对应的服务器端控件ID
		/// </summary>
		[DefaultValue("")]
		public string ControlID
		{
			get
			{
				return this.controlID;
			}
			set
			{
				this.controlID = value;
			}
		}

		/// <summary>
		/// 对应的一些特殊标识
		/// </summary>
		[DefaultValue("")]
		public string Tag
		{
			get
			{
				return this.tag;
			}
			set
			{
				this.tag = value;
			}
		}

		/// <summary>
		/// 页签的宽度
		/// </summary>
		public Unit Width
		{
			get
			{
				return this.width;
			}
			set
			{
				this.width = value;
			}
		}
	}

	/// <summary>
	/// TabStrip的集合
	/// </summary>
	public class TabStripItemCollection : CollectionBase
	{
		public void Add(TabStripItem item)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(item != null, "item");

			List.Add(item);
		}

		public TabStripItem this[int index]
		{
			get
			{
				return (TabStripItem)List[index];
			}
			set
			{
				List[index] = value;
			}
		}
	}
}
