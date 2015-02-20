
using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MCS.Web.WebControls
{
	/// <summary>
	/// Category列模板，用于Repeater
	/// </summary>
	public class CategoryColumn : ITemplate
	{
		#region constructor
		public CategoryColumn(ListItemType itemType, Category category,string cellHeaderCss, string cellCss, string linkCss, string separator, string anyText)
		{
			ItemType = itemType;
			ControlText = category.DataTextField;
			FieldValue = category.CategoryField;
			ControlValue = category.DataValueField;
			LinkCss = linkCss;
            CellHeaderCss = cellHeaderCss;
			CellCss = cellCss;
			Separator = separator;
			AnyText = anyText;
		    ConditionText = category.ConditionText;
		    ConditionValue = category.ConditionValue;
		}
		#endregion

		#region Fields

		/// <summary>
		/// 链接控件显示的文本
		/// </summary>
		public string ControlText { get; set; }

		/// <summary>
		/// 链接控件的值
		/// </summary>
		public string ControlValue { get; set; }

		/// <summary>
		/// 数据库中的字段
		/// </summary>
		public string FieldValue { get; set; }

		/// <summary>
		/// 链接的样式 
		/// </summary>
		public string LinkCss { get; set; }

		/// <summary>
		/// 表格的样式 
		/// </summary>
		public string CellCss { get; set; }

        /// <summary>
        /// 表格头的样式 
        /// </summary>
        public string CellHeaderCss { get; set; }

		/// <summary>
		/// 表头中的分隔符
		/// </summary>
		public string Separator { get; set; }

		/// <summary>
		/// 全部显示的文本
		/// </summary>
		public string AnyText { get; set; }

		/// <summary>
		/// 数据源中指定的Text
		/// </summary>
		public string ConditionText { get; set; }

		/// <summary>
		/// 数据源中指定的Value
		/// </summary>
		public string ConditionValue { get; set; }

		public ListItemType ItemType { get; set; }
		#endregion

		#region ITemplate 成员

		public void InstantiateIn(Control container)
		{
			var placeHolder = new PlaceHolder();
			switch (ItemType)
			{
				case ListItemType.Header:

					var titleCell = new TableCell { CssClass = HttpUtility.HtmlEncode(CellHeaderCss), Text = HttpUtility.HtmlEncode(string.Format("{0}{1}", ControlText, Separator)) };
					var anytextCell = new TableCell { CssClass = HttpUtility.HtmlEncode(CellCss) };
					var link = new LinkButton
					{
						ID = string.Format("any{0}", HttpUtility.HtmlAttributeEncode(ControlValue)),
						Text = HttpUtility.HtmlEncode(AnyText),
						CommandName = HttpUtility.HtmlAttributeEncode(FieldValue),
						CommandArgument = DeluxeSearch.ClearWhereCondition,
						CssClass = HttpUtility.HtmlAttributeEncode(LinkCss)
					};
					anytextCell.Controls.Add(link);
					placeHolder.Controls.Add(titleCell);
					placeHolder.Controls.Add(anytextCell);

					break;
				case ListItemType.Item:
					placeHolder.DataBinding += PlaceHolderDataBinding;
					break;
			}
			container.Controls.Add(placeHolder);
		}

		/// <summary>
		/// DataBinding-create controls and set to value.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void PlaceHolderDataBinding(object sender, EventArgs e)
		{
			var placeHolder = (PlaceHolder)sender;
			var cell = new TableCell { CssClass = HttpUtility.HtmlEncode(CellCss) };
			var repeaterItem = (RepeaterItem)placeHolder.NamingContainer;
			var linkButton = new LinkButton
			{
                ID = HttpUtility.HtmlAttributeEncode(string.Format("{0}_{1}", FieldValue, DataBinder.Eval(repeaterItem.DataItem, ConditionValue))),                
				Text = DataBinder.Eval(repeaterItem.DataItem, ConditionText).ToString(),
				CommandName = FieldValue,
				CommandArgument = DataBinder.Eval(repeaterItem.DataItem, ConditionValue).ToString(),
				CssClass = HttpUtility.HtmlEncode(LinkCss)
			};

			cell.Controls.Add(linkButton);
			placeHolder.Controls.Add(cell);
		}
		#endregion
	}
}
