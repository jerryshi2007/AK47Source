using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.Core;
using MCS.Web.Library.Script;

namespace MCS.Web.Responsive.WebControls
{
	public class ClientGridColumnConverter : JavaScriptConverter
	{
		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			ClientGridColumn column = new ClientGridColumn();

			column.DataField = DictionaryHelper.GetValue(dictionary, "dataField", string.Empty);
			column.HeaderText = DictionaryHelper.GetValue(dictionary, "headerText", string.Empty);
			column.HeaderTips = DictionaryHelper.GetValue(dictionary, "headerTips", string.Empty);
			column.HeaderTipsStyle = DictionaryHelper.GetValue(dictionary, "headerTipsStyle", "{color:Red}");
			column.SortExpression = DictionaryHelper.GetValue(dictionary, "sortExpression", string.Empty);
			column.SelectColumn = DictionaryHelper.GetValue(dictionary, "selectColumn", false);
			column.ShowSelectAll = DictionaryHelper.GetValue(dictionary, "showSelectAll", false);

			column.DataType = DictionaryHelper.GetValue(dictionary, "dataType", DataType.String);
			column.MaxLength = DictionaryHelper.GetValue(dictionary, "maxLength", 0);
			column.FormatString = DictionaryHelper.GetValue(dictionary, "formatString", string.Empty);
			column.EditorStyle = DictionaryHelper.GetValue(dictionary, "editorStyle", string.Empty);
			column.EditorTooltips = DictionaryHelper.GetValue(dictionary, "editorTooltips", string.Empty);
			column.EditorReadOnly = DictionaryHelper.GetValue(dictionary, "editorReadOnly", false);
			column.EditorEnabled = DictionaryHelper.GetValue(dictionary, "editorEnabled", true);
			column.Visible = DictionaryHelper.GetValue(dictionary, "visible", true);
			column.IsDynamicColumn = DictionaryHelper.GetValue(dictionary, "isDynamicColumn", false);
			column.AutoBindingValidation = DictionaryHelper.GetValue(dictionary, "autoBindingValidation", false);
			column.IsFixedLine = DictionaryHelper.GetValue(dictionary, "isFixedLine", false);
			column.IsStatistic = DictionaryHelper.GetValue(dictionary, "isStatistic", false);

			if (dictionary.ContainsKey("editTemplate"))
			{
				column.EditTemplate = JSONSerializerExecute.Deserialize<ClientGridColumnEditTemplate>(dictionary["editTemplate"]);
			}

			return column;
		}

		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			ClientGridColumn column = (ClientGridColumn)obj;

			Dictionary<string, object> dictionary = new Dictionary<string, object>();

			DictionaryHelper.AddNonDefaultValue(dictionary, "dataField", column.DataField);
			DictionaryHelper.AddNonDefaultValue(dictionary, "headerText", column.HeaderText);
			DictionaryHelper.AddNonDefaultValue(dictionary, "headerTips", column.HeaderTips);
			DictionaryHelper.AddNonDefaultValue(dictionary, "headerTipsStyle", column.HeaderTipsStyle);
			DictionaryHelper.AddNonDefaultValue(dictionary, "sortExpression", column.SortExpression);
			DictionaryHelper.AddNonDefaultValue(dictionary, "selectColumn", column.SelectColumn);
			DictionaryHelper.AddNonDefaultValue(dictionary, "showSelectAll", column.ShowSelectAll);
			DictionaryHelper.AddNonDefaultValue(dictionary, "editTemplate", column.EditTemplate);

			DictionaryHelper.AddNonDefaultValue(dictionary, "dataType", column.DataType);
			DictionaryHelper.AddNonDefaultValue(dictionary, "maxLength", column.MaxLength);
			DictionaryHelper.AddNonDefaultValue(dictionary, "formatString", column.FormatString);
			DictionaryHelper.AddNonDefaultValue(dictionary, "editorStyle", column.EditorStyle);
			DictionaryHelper.AddNonDefaultValue(dictionary, "editorTooltips", column.EditorTooltips);
			DictionaryHelper.AddNonDefaultValue(dictionary, "editorReadOnly", column.EditorReadOnly);
			DictionaryHelper.AddNonDefaultValue(dictionary, "editorEnabled", column.EditorEnabled);
			DictionaryHelper.AddNonDefaultValue(dictionary, "visible", column.Visible);
			DictionaryHelper.AddNonDefaultValue(dictionary, "tag", column.Tag);
			DictionaryHelper.AddNonDefaultValue(dictionary, "isDynamicColumn", column.IsDynamicColumn);
			DictionaryHelper.AddNonDefaultValue(dictionary, "autoBindingValidation", column.AutoBindingValidation);
			DictionaryHelper.AddNonDefaultValue(dictionary, "isFixedLine", column.IsFixedLine);
			DictionaryHelper.AddNonDefaultValue(dictionary, "isStatistic", column.IsStatistic);

			if (string.IsNullOrEmpty(column.HeaderStyle) == false)
				dictionary.Add("headerStyle", serializer.DeserializeObject(column.HeaderStyle));

			if (string.IsNullOrEmpty(column.ItemStyle) == false)
				dictionary.Add("itemStyle", serializer.DeserializeObject(column.ItemStyle));

			if (string.IsNullOrEmpty(column.FooterStyle) == false)
				dictionary.Add("footerStyle", serializer.DeserializeObject(column.FooterStyle));


			return dictionary;
		}

		public override IEnumerable<Type> SupportedTypes
		{
			get
			{
				return new System.Type[] { typeof(ClientGridColumn) };
			}
		}
	}
}
