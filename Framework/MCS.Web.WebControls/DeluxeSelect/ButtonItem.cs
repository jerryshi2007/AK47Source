#region
// -------------------------------------------------
// Assembly	：	
// FileName	：	ButtonItem.cs
// Remark	：  按钮Collection类
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		songhaojie	    20070706		创建
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
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using MCS.Web.Library.Script;

namespace MCS.Web.WebControls
{
	/// <summary>
	/// 按钮项描叙
	/// </summary>
	/// <remarks>按钮项描叙</remarks>
	[Serializable]
	public class ButtonItem
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		/// <remarks>构造函数</remarks>
		public ButtonItem()
		{
		}
		/// <summary>
		/// 按钮名称
		/// </summary>
		/// <remarks>按钮名称</remarks>
		private string buttonName;
		/// <summary>
		/// 按钮名称
		/// </summary>
		/// <remarks>按钮名称</remarks>
		[Description("按钮名称")]
		public string ButtonName
		{
			get { return this.buttonName; }
			set { this.buttonName = value; }
		}
		/// <summary>
		/// 按钮排列顺序号
		/// </summary>
		/// <remarks>按钮排列顺序号</remarks>
		private int buttonSortID;
		/// <summary>
		/// 按钮排列顺序号
		/// </summary>
		/// <remarks>按钮排列顺序号</remarks>
		[Description("按钮顺序号")]
		public int ButtonSortID
		{
			get { return this.buttonSortID; }
			set { this.buttonSortID = value; }
		}
		/// <summary>
		/// 某类别的最大选择数量
		/// </summary>
		/// <remarks>某类别的最大选择数量</remarks>
		private int buttonTypeMaxCount;
		/// <summary>
		/// 某类别的最大选择数量
		/// </summary>
		/// <remarks>某类别的最大选择数量</remarks>
		[Description("某类别的最大选择数量")]
		public int ButtonTypeMaxCount
		{
			get { return this.buttonTypeMaxCount; }
			set { this.buttonTypeMaxCount = value; }
		}
		private ButtonTypeMode buttonType;
		/// <summary>
		/// 按钮类型
		/// </summary>
		/// <remarks>按钮类型</remarks>
		[Description("按钮类型")]
		public ButtonTypeMode ButtonType
		{
			get { return this.buttonType; }
			set { this.buttonType = value; }
		}
		/// <summary>
		/// 按钮的图片资源
		/// </summary>
		/// <remarks>按钮的图片资源</remarks>
		private string imageSrc;
		/// <summary>
		/// 按钮的图片资源
		/// </summary>
		///<remarks>按钮的图片资源</remarks>
		[Category("Appearance"), Editor("System.Web.UI.Design.ImageUrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), DefaultValue(""), UrlProperty, Bindable(true)]
		[Description("按钮的图片资源")]
		public string ImageSrc
		{
			get { return this.imageSrc; }
			set { this.imageSrc = value; }
		}
		/// <summary>
		/// 按钮样式
		/// </summary>
		/// <remarks>按钮样式</remarks>
		private string buttonCssClass;
		/// <summary>
		/// 按钮样式
		/// </summary>
		/// <remarks>按钮样式</remarks>
		[Description("按钮样式")]
		[DefaultValue(""), Category("Appearance")]
		public string ButtonCssClass
		{
			get { return this.buttonCssClass; }
			set { this.buttonCssClass = value; }
		}

		/// <summary>
		/// 拓展属性
		/// </summary>
		private string extenderData;

		/// <summary>
		/// 拓展属性
		/// </summary>
		[Description("拓展属性")]
		public string ExtenderData
		{
			get { return this.extenderData; }
			set { this.extenderData = value; }
		}

		/// <summary>
		/// 枚举按钮的类型
		/// </summary>
		/// <remarks>按钮d的枚举类型</remarks>
		public enum ButtonTypeMode
		{
			/// <summary>
			/// 普通按钮
			/// </summary>
			/// <remarks>普通按钮</remarks>
			Button,
			/// <summary>
			/// 图片按钮
			/// </summary>
			/// <remarks>图片按钮</remarks>
			ImageButton,
			/// <summary>
			/// 链接按钮
			/// </summary>
			/// <remarks>链接按钮</remarks>
			LinkButton
		}

	}

	/// <summary>
	/// 按钮类的Collection类
	/// </summary>
	/// <remarks>按钮类的Collection,继承自Collection</remarks>
	[Serializable]
	public class ButtonItemCollection : CollectionBase
	{

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <remarks>构造函数</remarks>
		public ButtonItemCollection()
		{
		}

		/// <summary>
		/// 数组索引
		/// </summary>
		/// <param name="index">索引值</param>
		/// <returns>数组索引</returns>
		/// <remarks>数组索引</remarks>
		public ButtonItem this[int index]
		{
			get
			{
				return ((ButtonItem)List[index]);
			}
			set
			{
				List[index] = value;
			}
		}
		/// <summary>
		/// 添加Item
		/// </summary>
		/// <param name="item">Item</param>
		/// <returns>添加Item</returns>
		/// <remarks>添加Item</remarks>
		public int Add(ButtonItem item)
		{
			return (List.Add(item));
		}
		/// <summary>
		/// 取得Item索引值
		/// </summary>
		/// <param name="item">Item</param>
		/// <returns>取得Item的索引值</returns>
		public int IndexOf(ButtonItem item)
		{
			return (List.IndexOf(item));
		}
		/// <summary>
		/// 在指定位置添加Item
		/// </summary>
		/// <param name="index">索引位置</param>
		/// <param name="item">Item</param>
		public void Insert(int index, ButtonItem item)
		{
			List.Insert(index, item);
		}
		/// <summary>
		/// 移除Item
		/// </summary>
		/// <param name="item">Item</param>
		/// <remarks>移除Item</remarks>
		public void Remove(ButtonItem item)
		{
			List.Remove(item);
		}
		/// <summary>
		/// 清空Items
		/// </summary>
		/// <remarks>清空Items</remarks>
		public new void Clear()
		{
			List.Clear();
		}


	}
}
