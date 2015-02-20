#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Web.MenuItem
// FileName	��	MenuItem.cs
// Remark	��	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ��ΰ	    20070720		����
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
	/// �˵�����
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
		/// MenuItem���캯��
		/// </summary>
		///<remarks></remarks>
		public MenuItem()
		{
			this.childItems = new MenuItemCollection(this);
			this.viewState = new StateBag();
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
			get { return (string)(this.viewState["NodeID"] ?? ""); }
			set { this.viewState["NodeID"] = value; }
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
			get { return (bool)(this.viewState["IsSeparator"] ?? false); }
			set { this.viewState["IsSeparator"] = value; }
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
			get { return (bool)(this.viewState["Enable"] ?? true); }
			set { this.viewState["Enable"] = value; }
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
			get { return (bool)(this.viewState["Visible"] ?? true); }
			set { this.viewState["Visible"] = value; }
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
			get { return (string)(this.viewState["Text"] ?? string.Empty); }
			set { this.viewState["Text"] = value; }
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
			get { return (string)(this.viewState["Value"] ?? string.Empty); }
			set { this.viewState["Value"] = value; }
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
			get { return (string)(this.viewState["Target"] ?? string.Empty); }
			set { this.viewState["Target"] = value; }
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
			get { return (string)(this.viewState["StaticPopOutImageUrl"] ?? string.Empty); }
			set { this.viewState["StaticPopOutImageUrl"] = value; }
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
			get { return (string)(this.viewState["DynamicPopOutImageUrl"] ?? string.Empty); }
			set { this.viewState["DynamicPopOutImageUrl"] = value; }
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
			get { return (string)(this.viewState["NavigateUrl"] ?? string.Empty); }
			set { this.viewState["NavigateUrl"] = value; }
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
			get { return (string)(this.viewState["ImageUrl"] ?? string.Empty); }
			set { this.viewState["ImageUrl"] = value; }
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
			get { return (string)(this.viewState["ToolTip"] ?? string.Empty); }
			set { this.viewState["ToolTip"] = value; }
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
			get { return (bool)(this.viewState["Selected"] ?? false); }
			set { this.viewState["Selected"] = value; }
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
				return ((IStateManager)this.viewState).IsTrackingViewState;
			}
		}

		/// <summary>
		/// ������ͼ״̬
		/// </summary>
		/// <remarks></remarks>
		void IStateManager.TrackViewState()
		{
			((IStateManager)this.viewState).TrackViewState();
		}

		/// <summary>
		/// ������ͼ״̬
		/// </summary>
		/// <param name="saveState"></param>
		/// <remarks></remarks>
		void IStateManager.LoadViewState(object saveState)
		{
			if (saveState != null)
				((IStateManager)this.viewState).LoadViewState(saveState);
		}

		/// <summary>
		/// ������ͼ״̬
		/// </summary>
		/// <returns></returns>
		/// <remarks></remarks>
		object IStateManager.SaveViewState()
		{
			return ((IStateManager)this.viewState).SaveViewState();
		}
	}

	/// <summary>
	/// �˵������
	/// </summary>
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

		/// <summary>
		/// ����˵��б�
		/// </summary>
		/// <remarks></remarks>
		protected override void ClearItems()
		{
			foreach (MenuItem item in this)
				item.Parent = null;
			base.ClearItems();
		}

		///// <summary>
		///// ���б�����Ӳ˵���
		///// </summary>
		///// <param name="item"></param>
		///// <remarks></remarks>
		//new public void Add(MenuItem item)
		//{
		//    this.AddAt(this.Count, item);
		//}

		/// <summary>
		/// ��ָ����������Ӳ˵���
		/// </summary>
		/// <param name="index"></param>
		/// <param name="item"></param>
		/// <remarks>��ָ����������Ӳ˵���</remarks>
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
		/// <remarks>��������ɾ���б���ָ���˵���</remarks>
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
