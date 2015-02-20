#region
// -------------------------------------------------
// Assembly	：	
// FileName	：	DeluxeSelect.cs
// Remark	：  数据选择控件
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		songhaojie	    20070706		创建
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
	/// 选择控件
	/// </summary>
	/// <remarks>选择控件</remarks>
	//引用基础脚本
	[RequiredScript(typeof(ControlBaseScript))]
	//引用本控件脚本，第一项为客户端控件类名称
	[ClientScriptResource("MCS.Web.WebControls.DeluxeSelect", "MCS.Web.WebControls.DeluxeSelect.DeluxeSelect.js")]
	//引用所需的Css
	[ClientCssResource("MCS.Web.WebControls.DeluxeSelect.DeluxeSelect.css")]
	[PersistenceMode(PersistenceMode.InnerProperty)]
	[Designer(typeof(DeluxeSelectDesigner))]
	public class DeluxeSelect : DataBoundScriptControl
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		/// <remarks>数据选择控件的构造函数，继承</remarks>
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
		/// 待选择列表的数据集合
		/// </summary>
		/// <remarks>待选择列表的数据集合</remarks>
		private SelectItemCollection candidateItems;
		/// <summary>
		/// 待选择的项
		/// </summary>
		[Category("Data")]
		[Description("待选择列表的数据集")]
		[ScriptControlProperty]//设置此属性要输出到客户端
		[ClientPropertyName("candidateItems")]//设置此属性对应客户端属性的名称
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
		/// 数据源的ValueFiled
		/// </summary>
		/// <remarks>数据源的ValueFiled</remarks>
		[Category("Data")]
		[Description("数据源的ValueFiled")]
		public string DataSourseValueField
		{
			get { return GetPropertyValue("DataSourseValueField", string.Empty); }
			set { SetPropertyValue("DataSourseValueField", value); }
		}

		/// <summary>
		/// 数据源的TextFiled
		/// </summary>
		/// <remarks>数据源的TextFiled</remarks>
		[Description("数据源的TextFiled")]
		[Category("Data")]
		public string DataSourseTextField
		{
			get { return GetPropertyValue("DataSourseTextField", string.Empty); }
			set { SetPropertyValue("DataSourseTextField", value); }
		}

		/// <summary>
		/// 数据源的SortFiled
		/// </summary>
		/// <remarks>数据源的SortFiled</remarks>
		[Description("数据源的SortFiled")]
		[Category("Data")]
		public string DataSourseSortField
		{
			get { return GetPropertyValue("DataSourseSortField", string.Empty); }
			set { SetPropertyValue("DataSourseSortField", value); }
		}

		/// <summary>
		/// 待选择列表的数据集合
		/// </summary>
		/// <remarks>待选择列表的数据集合</remarks>
		private SelectItemCollection selectedItems;
		/// <summary>
		/// 已选择的项
		/// </summary>
		/// <remarks>已选择的项</remarks>
		[Category("Data")]
		[Description("已选择列表的数据集")]
		[ClientPropertyName("selectedItems")]//设置此属性对应客户端属性的名称
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
		[Description("本次变化的数据集")]
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
		/// 按钮类别数据集合
		/// </summary>
		/// <remarks>按钮类别数据集合</remarks>
		private ButtonItemCollection buttonItems;
		/// <summary>
		/// 按钮项
		/// </summary>
		/// <remarks>按钮项</remarks>
		[Category("Data")]
		[Description("按钮的数据集")]
		[ScriptControlProperty]//设置此属性要输出到客户端
		[ClientPropertyName("buttonItems")]//设置此属性对应客户端属性的名称
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
		/// 待选择列表的Css样式
		/// </summary>
		/// <remarks>待选择列表的Css样式</remarks>
		[Description("待选择列表的样式")]
		[ClientPropertyName("candidateListCssClass")]
		[DefaultValue(""), Category("Appearance")]
		public string CandidateListCssClass
		{
			get { return GetPropertyValue("CandidateListCssClass", ""); }
			set { SetPropertyValue("CandidateListCssClass", value); }
		}

		/// <summary>
		/// 已选择列表的Css样式
		/// </summary>
		/// <remarks>已选择列表的Css样式</remarks>
		[Description("已选择列表的样式")]
		[ScriptControlProperty]//设置此属性要输出到客户端
		[ClientPropertyName("selectedListCssClass")]
		[DefaultValue(""), Category("Appearance")]
		public string SelectedListCssClass
		{
			get { return GetPropertyValue("SelectedListCssClass", ""); }
			set { SetPropertyValue("SelectedListCssClass", value); }
		}

		/// <summary>
		/// '选择'按钮的Css样式
		/// </summary>
		/// <remarks>'选择'按钮的Css样式</remarks>
		[Description("选择按钮的样式")]
		[ScriptControlProperty]//设置此属性要输出到客户端
		[ClientPropertyName("selectButtonCssClass")]
		[DefaultValue(""), Category("Appearance")]
		public string SelectButtonCssClass
		{
			get { return GetPropertyValue("SelectButtonCssClass", string.Empty); }
			set { SetPropertyValue("SelectButtonCssClass", value); }
		}

		/// <summary>
		/// '上下移'按钮的Css样式 
		/// </summary>
		/// <remarks>'上下移'按钮的Css样式</remarks>
		[Description("上下移动按钮的样式")]
		[ScriptControlProperty]//设置此属性要输出到客户端
		[ClientPropertyName("moveButtonCssClass")]
		[DefaultValue(""), Category("Appearance")]
		public string MoveButtonCssClass
		{
			get { return GetPropertyValue("MoveButtonCssClass", string.Empty); }
			set { SetPropertyValue("MoveButtonCssClass", value); }
		}

		/// <summary>
		/// 待选择列表的选择模式（单选\多选）
		/// </summary>
		/// <remarks>待选择列表的选择模式（单选\多选）</remarks>
		[Description("待选择列表的选择模式：true--多选；false--单选")]
		[Category("Behavior")]
		[DefaultValue(false)]
		[ScriptControlProperty]//设置此属性要输出到客户端
		[ClientPropertyName("candidateSelectionMode")]
		public bool CandidateSelectionMode
		{
			get { return GetPropertyValue("CandidateSelectionMode", false); }
			set { SetPropertyValue("CandidateSelectionMode", value); }

		}

		/// <summary>
		/// 待选择列表排序方式（升序\降序）
		/// </summary>
		/// <remarks>待选择列表排序方式（升序\降序）</remarks>
		[Description("待选择列表的排序方式,升序还是降序")]
		[ScriptControlProperty]//设置此属性要输出到客户端
		[ClientPropertyName("candidateListSortDirection")]
		[DefaultValue(0), Category("Behavior")]
		public ListSortDirection CandidateListSortDirection
		{
			get { return GetPropertyValue("CandidateListSortDirection", ListSortDirection.Ascending); }
			set { SetPropertyValue("CandidateListSortDirection", value); }

		}

		/// <summary>
		/// 已选择列表的选择模式（单选\多选）
		/// </summary>
		/// <remarks>已选择列表的选择模式（单选\多选）</remarks>
		[Description("已选择列表的选择模式：true--多选；false--单选")]
		[DefaultValue(false)]
		[Category("Behavior")]
		[ScriptControlProperty]//设置此属性要输出到客户端
		[ClientPropertyName("selectedSelectionMode")]
		public bool SelectedSelectionMode
		{
			get { return GetPropertyValue("SelectedSelectionMode", false); }
			set { SetPropertyValue("SelectedSelectionMode", value); }

		}

		/// <summary>
		/// 已选择列表排序方式（升序\降序）
		/// </summary>
		/// <remarks>已选择列表排序方式（升序\降序）</remarks>
		[Description("已选择列表的排序方式,升序还是降序")]
		[ScriptControlProperty]//设置此属性要输出到客户端
		[ClientPropertyName("selectedListSortDirection")]
		[DefaultValue(0), Category("Behavior")]
		public ListSortDirection SelectedListSortDirection
		{
			get { return GetPropertyValue("SelectedListSortDirection", ListSortDirection.Ascending); }
			set { SetPropertyValue("SelectedListSortDirection", value); }

		}

		/// <summary>
		/// 选择按钮是否显示
		/// </summary>
		/// <remarks>选择按钮是否显示</remarks>
		[Description("是否显示选择按钮：true--显示；false--不显示")]
		[Category("Appearance")]
		[DefaultValue(true)]
		[ScriptControlProperty]//设置此属性要输出到客户端
		[ClientPropertyName("showSelectButton")]
		public bool ShowSelectButton
		{
			get { return GetPropertyValue("ShowSelectButton", true); }
			set { SetPropertyValue("ShowSelectButton", value); }

		}

		/// <summary>
		/// 全部选择按钮是否显示
		/// </summary>
		/// <remarks>全部选择按钮是否显示</remarks>
		[Description("是否显示全部选择按钮：true--显示；false--不显示")]
		[DefaultValue(true)]
		[Category("Appearance")]
		[ScriptControlProperty]//设置此属性要输出到客户端
		[ClientPropertyName("showSelectAllButton")]
		public bool ShowSelectAllButton
		{
			get { return GetPropertyValue("ShowSelectAllButton", true); }
			set { SetPropertyValue("ShowSelectAllButton", value); }

		}

		/// <summary>
		/// 选择按钮的Text
		/// </summary>
		/// <remarks>选择按钮的Text</remarks>
		[Description("选择按钮的Text")]
		[DefaultValue("选择")]
		[Category("Appearance")]
		[ScriptControlProperty]//设置此属性要输出到客户端
		[ClientPropertyName("selectButtonText")]
		public string SelectButtonText
		{
			get { return GetPropertyValue("SelectButtonText", "选择"); }
			set { SetPropertyValue("SelectButtonText", value); }

		}

		/// <summary>
		/// 全部选择按钮的Text
		/// </summary>
		/// <remarks>全部选择按钮的Text</remarks>
		[Description("全部选择按钮的Text")]
		[DefaultValue("全部选择")]
		[Category("Appearance")]
		[ScriptControlProperty]//设置此属性要输出到客户端
		[ClientPropertyName("selectAllButtonText")]
		public string SelectAllButtonText
		{
			get { return GetPropertyValue("SelectAllButtonText", "全部选择"); }
			set { SetPropertyValue("SelectAllButtonText", value); }

		}

		/// <summary>
		/// 取消按钮的Text
		/// </summary>
		/// <remarks>取消按钮的Text</remarks>
		[Description("取消按钮的Text")]
		[DefaultValue("取消")]
		[Category("Appearance")]
		[ScriptControlProperty]//设置此属性要输出到客户端
		[ClientPropertyName("cancelButtonText")]
		public string CancelButtonText
		{
			get { return GetPropertyValue("CancelButtonText", "取消"); }
			set { SetPropertyValue("CancelButtonText", value); }

		}

		/// <summary>
		/// 全部取消按钮的Text
		/// </summary>
		/// <remarks>全部取消按钮的Text</remarks>
		[Description("全部取消按钮的Text")]
		[DefaultValue("全部取消")]
		[Category("Appearance")]
		[ScriptControlProperty]//设置此属性要输出到客户端
		[ClientPropertyName("cancelAllButtonText")]
		public string CancelAllButtonText
		{
			get { return GetPropertyValue("CancelAllButtonText", "全部取消"); }
			set { SetPropertyValue("CancelAllButtonText", value); }

		}

		/// <summary>
		/// 是否允许移动列表数据项
		/// </summary>
		/// <remarks>是否允许移动列表数据项</remarks>
		[Description("是否允许移动列表数据项：true--允许；false--不允许")]
		[DefaultValue(true)]
		[Category("Appearance")]
		[ScriptControlProperty]//设置此属性要输出到客户端
		[ClientPropertyName("moveOption")]
		public bool MoveOption
		{
			get { return GetPropertyValue("MoveOption", true); }
			set { SetPropertyValue("MoveOption", value); }

		}

		/// <summary>
		/// 已选择列表数据项的文本显示格式串
		/// </summary>
		/// <remarks></remarks>
		[Description("已选择列表数据项的文本显示格式串")]
		[DefaultValue("")]
		[Category("Data")]
		[ScriptControlProperty]//设置此属性要输出到客户端
		[ClientPropertyName("selectedFromatString")]
		public string SelectedFromatString
		{
			get { return GetPropertyValue("SelectedFromatString", string.Empty); }
			set { SetPropertyValue("SelectedFromatString", value); }

		}

		#endregion

		#region Methods

		/// <summary>
		/// 构建控件
		/// </summary>
		/// <param name="writer"></param>
		/// <remarks>构建控件</remarks>
		protected override void Render(HtmlTextWriter writer)
		{
			if (this.DesignMode)
			{
				writer.Write(this.GetSelectDesignHTML());
			}
			base.Render(writer);

		}

		/// <summary>
		/// 绑定数据源
		/// </summary>
		/// <param name="dataSource">数据源</param>
		/// <remarks>如果给控件设置了数据源，此方法是将数据源的数据绑定到待选择列表数据集合</remarks>
		protected override void PerformDataBinding(IEnumerable dataSource)
		{
			if (dataSource != null)
			{
				if (this.candidateItems == null)
				{
					//如果数据集合为null，则实例化
					this.candidateItems = new SelectItemCollection();
				}
				//Field是否为空标记
				bool flag = false;
				string propName = this.DataSourseTextField;
				string dataValueField = this.DataSourseValueField;
				string dataSortField = this.DataSourseSortField;

				ICollection iData = dataSource as ICollection;

				if ((propName.Length != 0) || (dataValueField.Length != 0) || dataSortField.Length != 0)
				{
					flag = true;
				}
				//取出dataSourse
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
		/// 清除本次改动的数据记录
		/// </summary>
		public void ClearDeltaItems()
		{
			this.deltaItems.Clear();
		}

		#endregion

		#region ClientState
		/// <summary>
		/// 加载ClientState
		/// </summary>
		/// <param name="clientState">序列化后的clientState</param>
		/// <remarks>加载ClientState</remarks>
		protected override void LoadClientState(string clientState)
		{
			base.LoadClientState(clientState);

			object[] foArray = JSONSerializerExecute.Deserialize<object[]>(clientState);

			if (foArray != null && foArray.Length > 0)
			{
				//已选择列表的Items
				if (foArray[0] != null && foArray.Length > 0)
				{
					this.selectedItems = (SelectItemCollection)JSONSerializerExecute.DeserializeObject(foArray[0], typeof(SelectItemCollection));
				}
				//待选择列表的Items
				if (foArray[1] != null && foArray.Length > 1)
				{
					this.candidateItems = (SelectItemCollection)JSONSerializerExecute.DeserializeObject(foArray[1], typeof(SelectItemCollection));
				}
				//按钮的Items
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
		/// 保存ClientState
		/// </summary>
		/// <returns>序列化后的CLientState字符串</returns>
		/// <remarks>保存ClientState</remarks>
		protected override string SaveClientState()
		{

			object[] foArray = new object[] { this.SelectedItems, this.CandidateItems, this.ButtonItems, this.DeltaItems };

			string fsSerialize = JSONSerializerExecute.Serialize(foArray);

			return fsSerialize;
		}
		#endregion

		#region 设计态 Select
		/// <summary>
		/// 控件的设计态
		/// </summary>
		/// <returns>控件的设计Html</returns>
		/// <remarks>控件的设计态</remarks>
		private string GetSelectDesignHTML()
		{
			StringBuilder strB = new StringBuilder();

			//待选择列表
			string strCandidateItems = "";
			if (this.CandidateItems != null)
			{
				for (int i = 0; i < this.CandidateItems.Count; i++)
				{
					strCandidateItems += "<option>" + this.CandidateItems[i].SelectListBoxText + "</option>";
				}
			}
			//按钮
			string strButtonItems = "";
			if (this.ButtonItems != null)
			{
				for (int i = 0; i < this.ButtonItems.Count; i++)
				{
					strButtonItems += "<input type=\"button\" value=\"" + this.ButtonItems[i].ButtonName + "\" /><br>";
				}
			}
			//已选择列表
			string strSelectedItems = "";
			if (this.SelectedItems != null)
			{
				for (int i = 0; i < this.SelectedItems.Count; i++)
				{
					strSelectedItems += "<option>" + this.SelectedItems[i].SelectListBoxText + "</option>";
				}
			}
			strB.AppendFormat(MCS.Web.WebControls.Properties.Resources.DeluxeSelect, strCandidateItems ?? "未设置值", strButtonItems ?? "未设置值", strSelectedItems ?? "未设置值");
			return strB.ToString();
		}

		#endregion
	}
}
