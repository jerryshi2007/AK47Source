using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Library.Core;
using MCS.Web.Library.Script;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	public class WfMatrixDefinitionConverter : JavaScriptConverter
	{
		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			WfMatrixDefinition matrixDefinition = new WfMatrixDefinition();

			matrixDefinition.Description = DictionaryHelper.GetValue(dictionary, "Description", string.Empty);
			matrixDefinition.Enabled = DictionaryHelper.GetValue(dictionary, "Enabled", true);
			matrixDefinition.Key = DictionaryHelper.GetValue(dictionary, "Key", string.Empty);
			matrixDefinition.Name = DictionaryHelper.GetValue(dictionary, "Name", string.Empty);

			WfMatrixDimensionDefinitionCollection dimensions = JSONSerializerExecute.Deserialize<WfMatrixDimensionDefinitionCollection>(dictionary["Dimensions"]);

			matrixDefinition.Dimensions.Clear();
			matrixDefinition.Dimensions.CopyFrom(dimensions);

			return matrixDefinition;
		}
		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			IDictionary<string, object> dictionary = new Dictionary<string, object>();

			WfMatrixDefinition matrixDefinition = (WfMatrixDefinition)obj;
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "Description", matrixDefinition.Description);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "Dimensions", matrixDefinition.Dimensions);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "Enabled", matrixDefinition.Enabled);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "Key", matrixDefinition.Key);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "Name", matrixDefinition.Name);

			return dictionary;
		}

		public override IEnumerable<Type> SupportedTypes
		{
			get { return new Type[] { typeof(WfMatrixDefinition) }; }
		}
	}
}
