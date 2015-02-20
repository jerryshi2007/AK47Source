#region
// -------------------------------------------------
// Assembly	��	MCS.Web.WebControls
// FileName	��	GenericInput.cs
// Remark	��  ͨ��¼��ؼ�������ؼ���һ�����Ͽؼ�����һ��Input��һ��ͨ��¼����չ�ؼ�
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		����	    20070815		����
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
	/// ͨ��¼��ؼ��������˴���
	/// </summary>
	/// <remarks>
	/// ����ؼ���һ�����Ͽؼ�����һ��Input��һ��ͨ��¼����չ�ؼ�
	/// </remarks>
	public class GenericInput : System.Web.UI.WebControls.WebControl
	{
		private GenericInputExtender oGIE = new GenericInputExtender();
		private System.Web.UI.HtmlControls.HtmlInputText oText = new System.Web.UI.HtmlControls.HtmlInputText();

		/// <summary>
		/// ���캯��
		/// </summary>
		/// <remarks>
		/// ���캯��
		/// </remarks>
		public GenericInput()
		{ }

		/// <summary>
		/// ��ʼ��
		/// </summary>
		/// <remarks>
		/// ֻ��ʱ���Literal�������Input��GenericInputExtender
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
		/// ���ʱ�Ĵ���
		/// </summary>
		/// <remarks>���ʱ���һ��Select������ʱδ���д���</remarks>
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
		/// �ؼ��߿����ɫ
		/// </summary>
		/// <remarks>
		/// �ؼ��߿����ɫ
		/// </remarks>
		[DefaultValue(typeof(Color), "35,83,178")]//"#2353B2"
		[Category("Appearance")]
		public Color HighlightBorderColor
		{
			get { return this.oGIE.HighlightBorderColor; }
			set { this.oGIE.HighlightBorderColor = value; }
		}

		/// <summary>
		/// ѡ����ĿĬ��������ɫ
		/// </summary>
		/// <remarks>
		/// ѡ����ĿĬ��������ɫ
		/// </remarks>
		[DefaultValue("")]
		[Category("Appearance")]
		public string ItemCssClass
		{
			get { return this.oGIE.ItemCssClass; }
			set { this.oGIE.ItemCssClass = value; }
		}

		/// <summary>
		/// ����ƶ���ѡ����Ŀ��ʱ��������ɫ
		/// </summary>
		/// <remarks>
		/// ����ƶ���ѡ����Ŀ��ʱ��������ɫ
		/// </remarks>
		[DefaultValue("")]
		[Category("Appearance")]
		public string ItemHoverCssClass
		{
			get { return this.oGIE.ItemHoverCssClass; }
			set { this.oGIE.ItemHoverCssClass = value; }
		}

		/// <summary>
		/// ������ͷ����ı���ɫ
		/// </summary>
		/// <remarks>
		/// ������ͷ����ı���ɫ
		/// </remarks>
		[DefaultValue(typeof(Color), "198, 225, 255")]//"#C6E1FF"
		[Category("Appearance")]
		public Color DropArrowBackgroundColor
		{
			get { return this.oGIE.DropArrowBackgroundColor; }
			set { this.oGIE.DropArrowBackgroundColor = value; }
		}


		/// <summary>
		/// �����߿򶥲����
		/// </summary>
		[Category("Appearance")]
		public int HighlightBorderTopWidth
		{
			get { return this.oGIE.HighlightBorderTopWidth; }
			set { this.oGIE.HighlightBorderTopWidth = value; }
		}

		/// <summary>
		/// �����߿������
		/// </summary>
		[Category("Appearance")]
		public int HighlightBorderLeftWidth
		{
			get { return this.oGIE.HighlightBorderLeftWidth; }
			set { this.oGIE.HighlightBorderLeftWidth = value; }
		}

		/// <summary>
		/// �����߿��Ҳ���
		/// </summary>
		[Category("Appearance")]
		public int HighlightBorderRightWidth
		{
			get { return this.oGIE.HighlightBorderRightWidth; }
			set { this.oGIE.HighlightBorderRightWidth = value; }
		}

		/// <summary>
		/// �����߿�ײ����
		/// </summary>
		[Category("Appearance")]
		[ScriptControlProperty]//���ô�����Ҫ������ͻ���
		[ClientPropertyName("highlightBorderTopWidth")]
		public int HighlightBorderBottomWidth
		{
			get { return this.oGIE.HighlightBorderBottomWidth; }
			set { this.oGIE.HighlightBorderBottomWidth = value; }
		}

		

		/// <summary>
		/// �ؼ��ĵ�ǰ�ı�
		/// </summary>
		/// <remarks>
		/// �ؼ��ĵ�ǰ�ı�
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
		/// �ı��ı���¼�
		/// </summary>
		[DefaultValue("")]
		public string OnChange
		{
			get { return this.oGIE.OnChange; }
			set { this.oGIE.OnChange = value; }
		}

		/// <summary>
		/// ѡ����Ŀ���¼�
		/// </summary>
		[DefaultValue("")]
		public string OnSelectedItem
		{
			get { return this.oGIE.OnSelectedItem; }
			set { this.oGIE.OnSelectedItem = value; }
		}

		/// <summary>
		/// ѡ����Ŀǰ���¼�
		/// </summary>
		[DefaultValue("")]
		public string OnSelectItem
		{
			get { return this.oGIE.OnSelectItem; }
			set { this.oGIE.OnSelectItem = value; }
		}

		/// <summary>
		/// ��ǰѡ���Indexֵ
		/// </summary>
		/// <remarks>
		/// ��ǰѡ���Indexֵ
		/// </remarks>
		[DefaultValue(-1)]
		[Browsable(false)]
		public int SelectedIndex
		{
			get { return this.oGIE.SelectedIndex; }
			set { this.oGIE.SelectedIndex = value; }
		}

		/// <summary>
		/// �ؼ��е�ѡ����Ŀ����
		/// </summary>
		/// <remarks>
		/// �ؼ��е�ѡ����Ŀ����
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
		/// �ؼ��Ƿ�ֻ��
		/// </summary>
		/// <remarks>
		/// �ؼ��Ƿ�ֻ��
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
