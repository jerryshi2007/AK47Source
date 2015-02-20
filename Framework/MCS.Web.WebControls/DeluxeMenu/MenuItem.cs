#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Web.MenuItem
// FileName	：	MenuItem.cs
// Remark	：	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    吴伟	    20070720		创建
// -------------------------------------------------
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing.Design;
using System.Text;
using System.Web;
using System.Web.Script;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.Design.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using MCS.Web.Library.Script;

[assembly: WebResource("MCS.Web.WebControls.DeluxeMenu.MenuItem.js", "application/x-javascript")]

namespace MCS.Web.WebControls
{
	/// <summary>
	/// 菜单项类
	/// </summary>
	[PersistChildren(false)]
	[Designer(typeof(MenuItemsDesigner))]
	[Editor(typeof(MenuItemsEditor), typeof(System.ComponentModel.ComponentEditor))]
	[RequiredScript(typeof(ControlBaseScript))]
	[ClientScriptResource("", "MCS.Web.WebControls.DeluxeMenu.MenuItem.js")]
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
			this.childItems = new MenuItemCollection(this);
			this.viewState = new StateBag();
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
			get { return (string)(this.viewState["NodeID"] ?? ""); }
			set { this.viewState["NodeID"] = value; }
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
			get { return (bool)(this.viewState["IsSeparator"] ?? false); }
			set { this.viewState["IsSeparator"] = value; }
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
			get { return (bool)(this.viewState["Enable"] ?? true); }
			set { this.viewState["Enable"] = value; }
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
			get { return (bool)(this.viewState["Visible"] ?? true); }
			set { this.viewState["Visible"] = value; }
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
			get { return (string)(this.viewState["Text"] ?? string.Empty); }
			set { this.viewState["Text"] = value; }
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
			get { return (string)(this.viewState["Value"] ?? string.Empty); }
			set { this.viewState["Value"] = value; }
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
			get { return (string)(this.viewState["Target"] ?? string.Empty); }
			set { this.viewState["Target"] = value; }
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
			get { return (string)(this.viewState["StaticPopOutImageUrl"] ?? string.Empty); }
			set { this.viewState["StaticPopOutImageUrl"] = value; }
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
			get { return (string)(this.viewState["DynamicPopOutImageUrl"] ?? string.Empty); }
			set { this.viewState["DynamicPopOutImageUrl"] = value; }
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
			get { return (string)(this.viewState["NavigateUrl"] ?? string.Empty); }
			set { this.viewState["NavigateUrl"] = value; }
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
			get { return (string)(this.viewState["ImageUrl"] ?? string.Empty); }
			set { this.viewState["ImageUrl"] = value; }
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
			get { return (string)(this.viewState["ToolTip"] ?? string.Empty); }
			set { this.viewState["ToolTip"] = value; }
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
			get { return (bool)(this.viewState["Selected"] ?? false); }
			set { this.viewState["Selected"] = value; }
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
				return ((IStateManager)this.viewState).IsTrackingViewState;
			}
		}

		/// <summary>
		/// 跟踪视图状态
		/// </summary>
		/// <remarks></remarks>
		void IStateManager.TrackViewState()
		{
			((IStateManager)this.viewState).TrackViewState();
		}

		/// <summary>
		/// 加载视图状态
		/// </summary>
		/// <param name="saveState"></param>
		/// <remarks></remarks>
		void IStateManager.LoadViewState(object saveState)
		{
			if (saveState != null)
				((IStateManager)this.viewState).LoadViewState(saveState);
		}

		/// <summary>
		/// 保存视图状态
		/// </summary>
		/// <returns></returns>
		/// <remarks></remarks>
		object IStateManager.SaveViewState()
		{
			return ((IStateManager)this.viewState).SaveViewState();
		}
	}

	/// <summary>
	/// 菜单项集合类
	/// </summary>
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

		/// <summary>
		/// 清除菜单列表
		/// </summary>
		/// <remarks></remarks>
		protected override void ClearItems()
		{
			foreach (MenuItem item in this)
				item.Parent = null;
			base.ClearItems();
		}

		///// <summary>
		///// 在列表中填加菜单项
		///// </summary>
		///// <param name="item"></param>
		///// <remarks></remarks>
		//new public void Add(MenuItem item)
		//{
		//    this.AddAt(this.Count, item);
		//}

		/// <summary>
		/// 在指定索引处填加菜单项
		/// </summary>
		/// <param name="index"></param>
		/// <param name="item"></param>
		/// <remarks>在指定索引处填加菜单项</remarks>
		protected override void InsertItem(int index, MenuItem item)
		{
			item.Parent = this.ownerItem;
			base.InsertItem(index, item);
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
		/// <remarks>根据索引删除列表中指定菜单项</remarks>
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
