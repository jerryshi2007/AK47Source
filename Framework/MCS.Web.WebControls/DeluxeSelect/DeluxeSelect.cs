#region
// -------------------------------------------------
// Assembly	��	
// FileName	��	DeluxeSelect.cs
// Remark	��  ����ѡ��ؼ�
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		songhaojie	    20070706		����
// -------------------------------------------------
#endregion

using System;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Web.UI.WebControls.WebParts;
using System.Text;
using System.Collections.Generic;
using MCS.Web.WebControls;
using MCS.Web.Library.Script;
using System.Web.Util;
using System.Collections;
using System.Drawing.Design;

#region [ Resources ]

[assembly: System.Web.UI.WebResource("MCS.Web.WebControls.DeluxeSelect.DeluxeSelect.js", "text/javascript")]
[assembly: System.Web.UI.WebResource("MCS.Web.WebControls.DeluxeSelect.DeluxeSelect.css", "text/css")]

#endregion

namespace MCS.Web.WebControls
{
	/// <summary>
	/// ѡ��ؼ�
	/// </summary>
	/// <remarks>ѡ��ؼ�</remarks>
	//���û����ű�
	[RequiredScript(typeof(ControlBaseScript))]
	//���ñ��ؼ��ű�����һ��Ϊ�ͻ��˿ؼ�������
	[ClientScriptResource("MCS.Web.WebControls.DeluxeSelect", "MCS.Web.WebControls.DeluxeSelect.DeluxeSelect.js")]
	//���������Css
	[ClientCssResource("MCS.Web.WebControls.DeluxeSelect.DeluxeSelect.css")]
	[PersistenceMode(PersistenceMode.InnerProperty)]
	[Designer(typeof(DeluxeSelectDesigner))]
	public class DeluxeSelect : DataBoundScriptControl
	{
		/// <summary>
		/// ���캯��
		/// </summary>
		/// <remarks>����ѡ��ؼ��Ĺ��캯�����̳�</remarks>
		public DeluxeSelect()
			: base(true, HtmlTextWriterTag.Div)
		{
			//this.Width = Unit.Percentage(100);
			//this.Height = Unit.Percentage(100);
			this.Width = Unit.Pixel(350);
			this.Height = Unit.Pixel(200);
		}

		#region Property

		/// <summary>
		/// ��ѡ���б�����ݼ���
		/// </summary>
		/// <remarks>��ѡ���б�����ݼ���</remarks>
		private SelectItemCollection candidateItems;
		/// <summary>
		/// ��ѡ�����
		/// </summary>
		[Category("Data")]
		[Description("��ѡ���б�����ݼ�")]
		[ScriptControlProperty]//���ô�����Ҫ������ͻ���
		[ClientPropertyName("candidateItems")]//���ô����Զ�Ӧ�ͻ������Ե�����
		[Editor("System.Web.UI.Design.WebControls.ListItemsCollectionEditor,System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(System.Drawing.Design.UITypeEditor))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		public SelectItemCollection CandidateItems
		{
			get
			{
				if (this.candidateItems == null)
				{
					this.candidateItems = new SelectItemCollection();
				}
				return this.candidateItems;
			}

		}

		/// <summary>
		/// ����Դ��ValueFiled
		/// </summary>
		/// <remarks>����Դ��ValueFiled</remarks>
		[Category("Data")]
		[Description("����Դ��ValueFiled")]
		public string DataSourseValueField
		{
			get { return GetPropertyValue("DataSourseValueField", string.Empty); }
			set { SetPropertyValue("DataSourseValueField", value); }
		}

		/// <summary>
		/// ����Դ��TextFiled
		/// </summary>
		/// <remarks>����Դ��TextFiled</remarks>
		[Description("����Դ��TextFiled")]
		[Category("Data")]
		public string DataSourseTextField
		{
			get { return GetPropertyValue("DataSourseTextField", string.Empty); }
			set { SetPropertyValue("DataSourseTextField", value); }
		}

		/// <summary>
		/// ����Դ��SortFiled
		/// </summary>
		/// <remarks>����Դ��SortFiled</remarks>
		[Description("����Դ��SortFiled")]
		[Category("Data")]
		public string DataSourseSortField
		{
			get { return GetPropertyValue("DataSourseSortField", string.Empty); }
			set { SetPropertyValue("DataSourseSortField", value); }
		}

		/// <summary>
		/// ��ѡ���б�����ݼ���
		/// </summary>
		/// <remarks>��ѡ���б�����ݼ���</remarks>
		private SelectItemCollection selectedItems;
		/// <summary>
		/// ��ѡ�����
		/// </summary>
		/// <remarks>��ѡ�����</remarks>
		[Category("Data")]
		[Description("��ѡ���б�����ݼ�")]
		[ClientPropertyName("selectedItems")]//���ô����Զ�Ӧ�ͻ������Ե�����
		[Editor("System.Web.UI.Design.WebControls.ListItemsCollectionEditor,System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(System.Drawing.Design.UITypeEditor))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		public SelectItemCollection SelectedItems
		{
			get
			{
				if (this.selectedItems == null)
				{
					this.selectedItems = new SelectItemCollection();
				}
				return this.selectedItems;
			}
		}

		DeltaItemCollection deltaItems = new DeltaItemCollection();

		/// <summary>
		/// deltaItems
		/// </summary>
		[Category("Data")]
		[Description("���α仯�����ݼ�")]
		[ClientPropertyName("deltaItems")]
		[Browsable(false)]
		public DeltaItemCollection DeltaItems
		{
			get
			{
				if (this.deltaItems == null)
					this.deltaItems = new DeltaItemCollection();

				return this.deltaItems;
			}
		}


		/// <summary>
		/// ��ť������ݼ���
		/// </summary>
		/// <remarks>��ť������ݼ���</remarks>
		private ButtonItemCollection buttonItems;
		/// <summary>
		/// ��ť��
		/// </summary>
		/// <remarks>��ť��</remarks>
		[Category("Data")]
		[Description("��ť�����ݼ�")]
		[ScriptControlProperty]//���ô�����Ҫ������ͻ���
		[ClientPropertyName("buttonItems")]//���ô����Զ�Ӧ�ͻ������Ե�����
		[Editor("System.Web.UI.Design.WebControls.ListItemsCollectionEditor,System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(System.Drawing.Design.UITypeEditor))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		public ButtonItemCollection ButtonItems
		{
			get
			{
				if (this.buttonItems == null)
				{
					this.buttonItems = new ButtonItemCollection();
				}
				return this.buttonItems;
			}

		}

		/// <summary>
		/// ��ѡ���б��Css��ʽ
		/// </summary>
		/// <remarks>��ѡ���б��Css��ʽ</remarks>
		[Description("��ѡ���б����ʽ")]
		[ClientPropertyName("candidateListCssClass")]
		[DefaultValue(""), Category("Appearance")]
		public string CandidateListCssClass
		{
			get { return GetPropertyValue("CandidateListCssClass", ""); }
			set { SetPropertyValue("CandidateListCssClass", value); }
		}

		/// <summary>
		/// ��ѡ���б��Css��ʽ
		/// </summary>
		/// <remarks>��ѡ���б��Css��ʽ</remarks>
		[Description("��ѡ���б����ʽ")]
		[ScriptControlProperty]//���ô�����Ҫ������ͻ���
		[ClientPropertyName("selectedListCssClass")]
		[DefaultValue(""), Category("Appearance")]
		public string SelectedListCssClass
		{
			get { return GetPropertyValue("SelectedListCssClass", ""); }
			set { SetPropertyValue("SelectedListCssClass", value); }
		}

		/// <summary>
		/// 'ѡ��'��ť��Css��ʽ
		/// </summary>
		/// <remarks>'ѡ��'��ť��Css��ʽ</remarks>
		[Description("ѡ��ť����ʽ")]
		[ScriptControlProperty]//���ô�����Ҫ������ͻ���
		[ClientPropertyName("selectButtonCssClass")]
		[DefaultValue(""), Category("Appearance")]
		public string SelectButtonCssClass
		{
			get { return GetPropertyValue("SelectButtonCssClass", string.Empty); }
			set { SetPropertyValue("SelectButtonCssClass", value); }
		}

		/// <summary>
		/// '������'��ť��Css��ʽ 
		/// </summary>
		/// <remarks>'������'��ť��Css��ʽ</remarks>
		[Description("�����ƶ���ť����ʽ")]
		[ScriptControlProperty]//���ô�����Ҫ������ͻ���
		[ClientPropertyName("moveButtonCssClass")]
		[DefaultValue(""), Category("Appearance")]
		public string MoveButtonCssClass
		{
			get { return GetPropertyValue("MoveButtonCssClass", string.Empty); }
			set { SetPropertyValue("MoveButtonCssClass", value); }
		}

		/// <summary>
		/// ��ѡ���б��ѡ��ģʽ����ѡ\��ѡ��
		/// </summary>
		/// <remarks>��ѡ���б��ѡ��ģʽ����ѡ\��ѡ��</remarks>
		[Description("��ѡ���б��ѡ��ģʽ��true--��ѡ��false--��ѡ")]
		[Category("Behavior")]
		[DefaultValue(false)]
		[ScriptControlProperty]//���ô�����Ҫ������ͻ���
		[ClientPropertyName("candidateSelectionMode")]
		public bool CandidateSelectionMode
		{
			get { return GetPropertyValue("CandidateSelectionMode", false); }
			set { SetPropertyValue("CandidateSelectionMode", value); }

		}

		/// <summary>
		/// ��ѡ���б�����ʽ������\����
		/// </summary>
		/// <remarks>��ѡ���б�����ʽ������\����</remarks>
		[Description("��ѡ���б������ʽ,�����ǽ���")]
		[ScriptControlProperty]//���ô�����Ҫ������ͻ���
		[ClientPropertyName("candidateListSortDirection")]
		[DefaultValue(0), Category("Behavior")]
		public ListSortDirection CandidateListSortDirection
		{
			get { return GetPropertyValue("CandidateListSortDirection", ListSortDirection.Ascending); }
			set { SetPropertyValue("CandidateListSortDirection", value); }

		}

		/// <summary>
		/// ��ѡ���б��ѡ��ģʽ����ѡ\��ѡ��
		/// </summary>
		/// <remarks>��ѡ���б��ѡ��ģʽ����ѡ\��ѡ��</remarks>
		[Description("��ѡ���б��ѡ��ģʽ��true--��ѡ��false--��ѡ")]
		[DefaultValue(false)]
		[Category("Behavior")]
		[ScriptControlProperty]//���ô�����Ҫ������ͻ���
		[ClientPropertyName("selectedSelectionMode")]
		public bool SelectedSelectionMode
		{
			get { return GetPropertyValue("SelectedSelectionMode", false); }
			set { SetPropertyValue("SelectedSelectionMode", value); }

		}

		/// <summary>
		/// ��ѡ���б�����ʽ������\����
		/// </summary>
		/// <remarks>��ѡ���б�����ʽ������\����</remarks>
		[Description("��ѡ���б������ʽ,�����ǽ���")]
		[ScriptControlProperty]//���ô�����Ҫ������ͻ���
		[ClientPropertyName("selectedListSortDirection")]
		[DefaultValue(0), Category("Behavior")]
		public ListSortDirection SelectedListSortDirection
		{
			get { return GetPropertyValue("SelectedListSortDirection", ListSortDirection.Ascending); }
			set { SetPropertyValue("SelectedListSortDirection", value); }

		}

		/// <summary>
		/// ѡ��ť�Ƿ���ʾ
		/// </summary>
		/// <remarks>ѡ��ť�Ƿ���ʾ</remarks>
		[Description("�Ƿ���ʾѡ��ť��true--��ʾ��false--����ʾ")]
		[Category("Appearance")]
		[DefaultValue(true)]
		[ScriptControlProperty]//���ô�����Ҫ������ͻ���
		[ClientPropertyName("showSelectButton")]
		public bool ShowSelectButton
		{
			get { return GetPropertyValue("ShowSelectButton", true); }
			set { SetPropertyValue("ShowSelectButton", value); }

		}

		/// <summary>
		/// ȫ��ѡ��ť�Ƿ���ʾ
		/// </summary>
		/// <remarks>ȫ��ѡ��ť�Ƿ���ʾ</remarks>
		[Description("�Ƿ���ʾȫ��ѡ��ť��true--��ʾ��false--����ʾ")]
		[DefaultValue(true)]
		[Category("Appearance")]
		[ScriptControlProperty]//���ô�����Ҫ������ͻ���
		[ClientPropertyName("showSelectAllButton")]
		public bool ShowSelectAllButton
		{
			get { return GetPropertyValue("ShowSelectAllButton", true); }
			set { SetPropertyValue("ShowSelectAllButton", value); }

		}

		/// <summary>
		/// ѡ��ť��Text
		/// </summary>
		/// <remarks>ѡ��ť��Text</remarks>
		[Description("ѡ��ť��Text")]
		[DefaultValue("ѡ��")]
		[Category("Appearance")]
		[ScriptControlProperty]//���ô�����Ҫ������ͻ���
		[ClientPropertyName("selectButtonText")]
		public string SelectButtonText
		{
			get { return GetPropertyValue("SelectButtonText", "ѡ��"); }
			set { SetPropertyValue("SelectButtonText", value); }

		}

		/// <summary>
		/// ȫ��ѡ��ť��Text
		/// </summary>
		/// <remarks>ȫ��ѡ��ť��Text</remarks>
		[Description("ȫ��ѡ��ť��Text")]
		[DefaultValue("ȫ��ѡ��")]
		[Category("Appearance")]
		[ScriptControlProperty]//���ô�����Ҫ������ͻ���
		[ClientPropertyName("selectAllButtonText")]
		public string SelectAllButtonText
		{
			get { return GetPropertyValue("SelectAllButtonText", "ȫ��ѡ��"); }
			set { SetPropertyValue("SelectAllButtonText", value); }

		}

		/// <summary>
		/// ȡ����ť��Text
		/// </summary>
		/// <remarks>ȡ����ť��Text</remarks>
		[Description("ȡ����ť��Text")]
		[DefaultValue("ȡ��")]
		[Category("Appearance")]
		[ScriptControlProperty]//���ô�����Ҫ������ͻ���
		[ClientPropertyName("cancelButtonText")]
		public string CancelButtonText
		{
			get { return GetPropertyValue("CancelButtonText", "ȡ��"); }
			set { SetPropertyValue("CancelButtonText", value); }

		}

		/// <summary>
		/// ȫ��ȡ����ť��Text
		/// </summary>
		/// <remarks>ȫ��ȡ����ť��Text</remarks>
		[Description("ȫ��ȡ����ť��Text")]
		[DefaultValue("ȫ��ȡ��")]
		[Category("Appearance")]
		[ScriptControlProperty]//���ô�����Ҫ������ͻ���
		[ClientPropertyName("cancelAllButtonText")]
		public string CancelAllButtonText
		{
			get { return GetPropertyValue("CancelAllButtonText", "ȫ��ȡ��"); }
			set { SetPropertyValue("CancelAllButtonText", value); }

		}

		/// <summary>
		/// �Ƿ������ƶ��б�������
		/// </summary>
		/// <remarks>�Ƿ������ƶ��б�������</remarks>
		[Description("�Ƿ������ƶ��б������true--����false--������")]
		[DefaultValue(true)]
		[Category("Appearance")]
		[ScriptControlProperty]//���ô�����Ҫ������ͻ���
		[ClientPropertyName("moveOption")]
		public bool MoveOption
		{
			get { return GetPropertyValue("MoveOption", true); }
			set { SetPropertyValue("MoveOption", value); }

		}

		/// <summary>
		/// ��ѡ���б���������ı���ʾ��ʽ��
		/// </summary>
		/// <remarks></remarks>
		[Description("��ѡ���б���������ı���ʾ��ʽ��")]
		[DefaultValue("")]
		[Category("Data")]
		[ScriptControlProperty]//���ô�����Ҫ������ͻ���
		[ClientPropertyName("selectedFromatString")]
		public string SelectedFromatString
		{
			get { return GetPropertyValue("SelectedFromatString", string.Empty); }
			set { SetPropertyValue("SelectedFromatString", value); }

		}

		#endregion

		#region Methods

		/// <summary>
		/// �����ؼ�
		/// </summary>
		/// <param name="writer"></param>
		/// <remarks>�����ؼ�</remarks>
		protected override void Render(HtmlTextWriter writer)
		{
			if (this.DesignMode)
			{
				writer.Write(this.GetSelectDesignHTML());
			}
			base.Render(writer);

		}

		/// <summary>
		/// ������Դ
		/// </summary>
		/// <param name="dataSource">����Դ</param>
		/// <remarks>������ؼ�����������Դ���˷����ǽ�����Դ�����ݰ󶨵���ѡ���б����ݼ���</remarks>
		protected override void PerformDataBinding(IEnumerable dataSource)
		{
			if (dataSource != null)
			{
				if (this.candidateItems == null)
				{
					//������ݼ���Ϊnull����ʵ����
					this.candidateItems = new SelectItemCollection();
				}
				//Field�Ƿ�Ϊ�ձ��
				bool flag = false;
				string propName = this.DataSourseTextField;
				string dataValueField = this.DataSourseValueField;
				string dataSortField = this.DataSourseSortField;

				ICollection iData = dataSource as ICollection;

				if ((propName.Length != 0) || (dataValueField.Length != 0) || dataSortField.Length != 0)
				{
					flag = true;
				}
				//ȡ��dataSourse
				foreach (object objData in dataSource)
				{
					SelectItem item = new SelectItem();
					if (flag)
					{
						if (propName.Length > 0)
						{
							item.SelectListBoxText = DataBinder.GetPropertyValue(objData, propName, null);
						}
						if (dataValueField.Length > 0)
						{
							item.SelectListBoxValue = DataBinder.GetPropertyValue(objData, dataValueField, null);
						}
						if (dataSortField.Length > 0)
						{
							item.SelectListBoxSortColumn = DataBinder.GetPropertyValue(objData, dataSortField, null);
						}
					}
					else
					{
						item.SelectListBoxText = objData.ToString();
						item.SelectListBoxValue = objData.ToString();
						item.SelectListBoxSortColumn = objData.ToString();
					}
					this.candidateItems.Add(item);
				}
			}
		}

		/// <summary>
		/// ������θĶ������ݼ�¼
		/// </summary>
		public void ClearDeltaItems()
		{
			this.deltaItems.Clear();
		}

		#endregion

		#region ClientState
		/// <summary>
		/// ����ClientState
		/// </summary>
		/// <param name="clientState">���л����clientState</param>
		/// <remarks>����ClientState</remarks>
		protected override void LoadClientState(string clientState)
		{
			base.LoadClientState(clientState);

			object[] foArray = JSONSerializerExecute.Deserialize<object[]>(clientState);

			if (foArray != null && foArray.Length > 0)
			{
				//��ѡ���б��Items
				if (foArray[0] != null && foArray.Length > 0)
				{
					this.selectedItems = (SelectItemCollection)JSONSerializerExecute.DeserializeObject(foArray[0], typeof(SelectItemCollection));
				}
				//��ѡ���б��Items
				if (foArray[1] != null && foArray.Length > 1)
				{
					this.candidateItems = (SelectItemCollection)JSONSerializerExecute.DeserializeObject(foArray[1], typeof(SelectItemCollection));
				}
				//��ť��Items
				if (foArray[2] != null && foArray.Length > 2)
				{
					this.buttonItems = (ButtonItemCollection)JSONSerializerExecute.DeserializeObject(foArray[2], typeof(ButtonItemCollection));
				}

				//deltaItems
				if (foArray[3] != null && foArray.Length > 3)
				{
					this.deltaItems = (DeltaItemCollection)JSONSerializerExecute.DeserializeObject(foArray[3], typeof(DeltaItemCollection));
				}
			}

		}

		/// <summary>
		/// ����ClientState
		/// </summary>
		/// <returns>���л����CLientState�ַ���</returns>
		/// <remarks>����ClientState</remarks>
		protected override string SaveClientState()
		{

			object[] foArray = new object[] { this.SelectedItems, this.CandidateItems, this.ButtonItems, this.DeltaItems };

			string fsSerialize = JSONSerializerExecute.Serialize(foArray);

			return fsSerialize;
		}
		#endregion

		#region ���̬ Select
		/// <summary>
		/// �ؼ������̬
		/// </summary>
		/// <returns>�ؼ������Html</returns>
		/// <remarks>�ؼ������̬</remarks>
		private string GetSelectDesignHTML()
		{
			StringBuilder strB = new StringBuilder();

			//��ѡ���б�
			string strCandidateItems = "";
			if (this.CandidateItems != null)
			{
				for (int i = 0; i < this.CandidateItems.Count; i++)
				{
					strCandidateItems += "<option>" + this.CandidateItems[i].SelectListBoxText + "</option>";
				}
			}
			//��ť
			string strButtonItems = "";
			if (this.ButtonItems != null)
			{
				for (int i = 0; i < this.ButtonItems.Count; i++)
				{
					strButtonItems += "<input type=\"button\" value=\"" + this.ButtonItems[i].ButtonName + "\" /><br>";
				}
			}
			//��ѡ���б�
			string strSelectedItems = "";
			if (this.SelectedItems != null)
			{
				for (int i = 0; i < this.SelectedItems.Count; i++)
				{
					strSelectedItems += "<option>" + this.SelectedItems[i].SelectListBoxText + "</option>";
				}
			}
			strB.AppendFormat(MCS.Web.WebControls.Properties.Resources.DeluxeSelect, strCandidateItems ?? "δ����ֵ", strButtonItems ?? "δ����ֵ", strSelectedItems ?? "δ����ֵ");
			return strB.ToString();
		}

		#endregion
	}
}
