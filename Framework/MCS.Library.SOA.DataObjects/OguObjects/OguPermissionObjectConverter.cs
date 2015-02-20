using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Library.Core;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects
{
	public abstract class OguPermissionObjectConverter<T> : JavaScriptConverter where T : OguPermissionObjectBase
	{
		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			T obj = CreateObject();

			obj.ID = DictionaryHelper.GetValue(dictionary, "id", string.Empty);
			obj.Name = DictionaryHelper.GetValue(dictionary, "name", string.Empty);
			obj.CodeName = DictionaryHelper.GetValue(dictionary, "codeName", string.Empty);
			obj.Description = DictionaryHelper.GetValue(dictionary, "description", string.Empty);

			return obj;
		}

		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			Dictionary<string, object> dict = new Dictionary<string, object>();

			T permissionObj = (T)obj;

			DictionaryHelper.AddNonDefaultValue<string, object>(dict, "id", permissionObj.ID);
			DictionaryHelper.AddNonDefaultValue<string, object>(dict, "name", permissionObj.Name);
			DictionaryHelper.AddNonDefaultValue<string, object>(dict, "codeName", permissionObj.CodeName);
			DictionaryHelper.AddNonDefaultValue<string, object>(dict, "description", permissionObj.Description);
			DictionaryHelper.AddNonDefaultValue<string, object>(dict, "__type", obj.GetType().AssemblyQualifiedName);

			return dict;
		}

		protected abstract T CreateObject();
	}
}
