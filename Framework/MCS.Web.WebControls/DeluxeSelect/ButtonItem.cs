#region
// -------------------------------------------------
// Assembly	��	
// FileName	��	ButtonItem.cs
// Remark	��  ��ťCollection��
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		songhaojie	    20070706		����
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
	/// ��ť������
	/// </summary>
	/// <remarks>��ť������</remarks>
	[Serializable]
	public class ButtonItem
	{
		/// <summary>
		/// ���캯��
		/// </summary>
		/// <remarks>���캯��</remarks>
		public ButtonItem()
		{
		}
		/// <summary>
		/// ��ť����
		/// </summary>
		/// <remarks>��ť����</remarks>
		private string buttonName;
		/// <summary>
		/// ��ť����
		/// </summary>
		/// <remarks>��ť����</remarks>
		[Description("��ť����")]
		public string ButtonName
		{
			get { return this.buttonName; }
			set { this.buttonName = value; }
		}
		/// <summary>
		/// ��ť����˳���
		/// </summary>
		/// <remarks>��ť����˳���</remarks>
		private int buttonSortID;
		/// <summary>
		/// ��ť����˳���
		/// </summary>
		/// <remarks>��ť����˳���</remarks>
		[Description("��ť˳���")]
		public int ButtonSortID
		{
			get { return this.buttonSortID; }
			set { this.buttonSortID = value; }
		}
		/// <summary>
		/// ĳ�������ѡ������
		/// </summary>
		/// <remarks>ĳ�������ѡ������</remarks>
		private int buttonTypeMaxCount;
		/// <summary>
		/// ĳ�������ѡ������
		/// </summary>
		/// <remarks>ĳ�������ѡ������</remarks>
		[Description("ĳ�������ѡ������")]
		public int ButtonTypeMaxCount
		{
			get { return this.buttonTypeMaxCount; }
			set { this.buttonTypeMaxCount = value; }
		}
		private ButtonTypeMode buttonType;
		/// <summary>
		/// ��ť����
		/// </summary>
		/// <remarks>��ť����</remarks>
		[Description("��ť����")]
		public ButtonTypeMode ButtonType
		{
			get { return this.buttonType; }
			set { this.buttonType = value; }
		}
		/// <summary>
		/// ��ť��ͼƬ��Դ
		/// </summary>
		/// <remarks>��ť��ͼƬ��Դ</remarks>
		private string imageSrc;
		/// <summary>
		/// ��ť��ͼƬ��Դ
		/// </summary>
		///<remarks>��ť��ͼƬ��Դ</remarks>
		[Category("Appearance"), Editor("System.Web.UI.Design.ImageUrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), DefaultValue(""), UrlProperty, Bindable(true)]
		[Description("��ť��ͼƬ��Դ")]
		public string ImageSrc
		{
			get { return this.imageSrc; }
			set { this.imageSrc = value; }
		}
		/// <summary>
		/// ��ť��ʽ
		/// </summary>
		/// <remarks>��ť��ʽ</remarks>
		private string buttonCssClass;
		/// <summary>
		/// ��ť��ʽ
		/// </summary>
		/// <remarks>��ť��ʽ</remarks>
		[Description("��ť��ʽ")]
		[DefaultValue(""), Category("Appearance")]
		public string ButtonCssClass
		{
			get { return this.buttonCssClass; }
			set { this.buttonCssClass = value; }
		}

		/// <summary>
		/// ��չ����
		/// </summary>
		private string extenderData;

		/// <summary>
		/// ��չ����
		/// </summary>
		[Description("��չ����")]
		public string ExtenderData
		{
			get { return this.extenderData; }
			set { this.extenderData = value; }
		}

		/// <summary>
		/// ö�ٰ�ť������
		/// </summary>
		/// <remarks>��ťd��ö������</remarks>
		public enum ButtonTypeMode
		{
			/// <summary>
			/// ��ͨ��ť
			/// </summary>
			/// <remarks>��ͨ��ť</remarks>
			Button,
			/// <summary>
			/// ͼƬ��ť
			/// </summary>
			/// <remarks>ͼƬ��ť</remarks>
			ImageButton,
			/// <summary>
			/// ���Ӱ�ť
			/// </summary>
			/// <remarks>���Ӱ�ť</remarks>
			LinkButton
		}

	}

	/// <summary>
	/// ��ť���Collection��
	/// </summary>
	/// <remarks>��ť���Collection,�̳���Collection</remarks>
	[Serializable]
	public class ButtonItemCollection : CollectionBase
	{

		/// <summary>
		/// ���캯��
		/// </summary>
		/// <remarks>���캯��</remarks>
		public ButtonItemCollection()
		{
		}

		/// <summary>
		/// ��������
		/// </summary>
		/// <param name="index">����ֵ</param>
		/// <returns>��������</returns>
		/// <remarks>��������</remarks>
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
		/// ���Item
		/// </summary>
		/// <param name="item">Item</param>
		/// <returns>���Item</returns>
		/// <remarks>���Item</remarks>
		public int Add(ButtonItem item)
		{
			return (List.Add(item));
		}
		/// <summary>
		/// ȡ��Item����ֵ
		/// </summary>
		/// <param name="item">Item</param>
		/// <returns>ȡ��Item������ֵ</returns>
		public int IndexOf(ButtonItem item)
		{
			return (List.IndexOf(item));
		}
		/// <summary>
		/// ��ָ��λ�����Item
		/// </summary>
		/// <param name="index">����λ��</param>
		/// <param name="item">Item</param>
		public void Insert(int index, ButtonItem item)
		{
			List.Insert(index, item);
		}
		/// <summary>
		/// �Ƴ�Item
		/// </summary>
		/// <param name="item">Item</param>
		/// <remarks>�Ƴ�Item</remarks>
		public void Remove(ButtonItem item)
		{
			List.Remove(item);
		}
		/// <summary>
		/// ���Items
		/// </summary>
		/// <remarks>���Items</remarks>
		public new void Clear()
		{
			List.Clear();
		}


	}
}
