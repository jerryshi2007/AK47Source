using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects;
using MCS.Web.Library.Script;

namespace PermissionCenter.Dialogs
{
	public class ClientSchemaPropertyValue
	{
		#region 属性

		public string PropertyFormID { get; set; }

		/// <summary>
		/// 对应控件属性
		/// </summary>
		public PropertyValueCollection Properties { get; set; }
		#endregion
	}

	public sealed class ClientSchemaPropertyValueConverter : JavaScriptConverter
	{
		#region 属性

		public override IEnumerable<Type> SupportedTypes
		{
			get { return new System.Type[] { typeof(ClientSchemaPropertyValue) }; }
		}

		#endregion
		#region 公开的方法

		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			ClientSchemaPropertyValue clientSchemaProperty = new ClientSchemaPropertyValue();
			clientSchemaProperty.PropertyFormID = DictionaryHelper.GetValue(dictionary, "PropertyFormID", string.Empty);

			if (dictionary.ContainsKey("Properties") == true)
			{
				PropertyValueCollection propCollection = JSONSerializerExecute.Deserialize<PropertyValueCollection>(dictionary["Properties"]);
				clientSchemaProperty.Properties = new PropertyValueCollection();
				clientSchemaProperty.Properties.CopyFrom(propCollection);
			}

			return clientSchemaProperty;
		}

		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			ClientSchemaPropertyValue clientSchemaProperty = (ClientSchemaPropertyValue)obj;

			dictionary.Add("PropertyFormID", clientSchemaProperty.PropertyFormID);
			dictionary.Add("Properties", clientSchemaProperty.Properties);

			return dictionary;
		}

		#endregion
	}
}