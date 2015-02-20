using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	public class WfMatrixDimensionDefinitionConverter : JavaScriptConverter
	{
		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			WfMatrixDimensionDefinition matrixDimensionDefinition = new WfMatrixDimensionDefinition();

			matrixDimensionDefinition.DataType = DictionaryHelper.GetValue(dictionary, "DataType", PropertyDataType.String);
			matrixDimensionDefinition.Description = DictionaryHelper.GetValue(dictionary, "Description", string.Empty);
			matrixDimensionDefinition.DimensionKey = DictionaryHelper.GetValue(dictionary, "DimensionKey", string.Empty);
			matrixDimensionDefinition.MatrixKey = DictionaryHelper.GetValue(dictionary, "MatrixKey", string.Empty);
			matrixDimensionDefinition.Name = DictionaryHelper.GetValue(dictionary, "Name", string.Empty);
			matrixDimensionDefinition.SortOrder = DictionaryHelper.GetValue(dictionary, "SortOrder", 0);

			return matrixDimensionDefinition;
		}

		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			IDictionary<string, object> dictionary = new Dictionary<string, object>();

			WfMatrixDimensionDefinition matrixDimensionDefinition = (WfMatrixDimensionDefinition)obj;
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "DataType", matrixDimensionDefinition.DataType);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "Description", matrixDimensionDefinition.Description);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "DimensionKey", matrixDimensionDefinition.DimensionKey);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "MatrixKey", matrixDimensionDefinition.MatrixKey);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "Name", matrixDimensionDefinition.Name);
			DictionaryHelper.AddNonDefaultValue<string, object>(dictionary, "SortOrder", matrixDimensionDefinition.SortOrder);

			return dictionary;
		}

		public override IEnumerable<Type> SupportedTypes
		{
			get { return new Type[] { typeof(WfMatrixDimensionDefinition) }; }
		}
	}
}
