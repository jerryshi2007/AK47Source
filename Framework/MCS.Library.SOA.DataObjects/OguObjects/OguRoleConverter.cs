using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using System.Web.Script.Serialization;

namespace MCS.Library.SOA.DataObjects
{
	public class OguRoleConverter : JavaScriptConverter
	{
		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			string fullCodeName = (string)dictionary["fullCodeName"];

			OguRole role = new OguRole(fullCodeName);

			role.ID = DictionaryHelper.GetValue(dictionary, "id", string.Empty);
			role.Name = DictionaryHelper.GetValue(dictionary, "name", string.Empty);
			role.CodeName = DictionaryHelper.GetValue(dictionary, "codeName", string.Empty);
			role.Description = DictionaryHelper.GetValue(dictionary, "description", string.Empty);

			return role;
		}

		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			Dictionary<string, object> dict = new Dictionary<string, object>();

			IRole role = (IRole)obj;

			string roleID = string.Empty;
			string roleName = string.Empty;
			string roleCodeName = string.Empty;
			string roleDesc = string.Empty;

			try
			{
				roleID = role.ID;
				roleName = role.Name;
				roleCodeName = role.CodeName;
				roleDesc = role.Description;
			}
			catch (SystemSupportException)
			{
				roleName = "未能在授权系统中找到角色：" + role.FullCodeName;
			}
			catch (Exception)
			{
				throw;
			}
			finally
			{
				DictionaryHelper.AddNonDefaultValue<string, object>(dict, "id", roleID);
				DictionaryHelper.AddNonDefaultValue<string, object>(dict, "fullCodeName", role.FullCodeName);
				DictionaryHelper.AddNonDefaultValue<string, object>(dict, "name", roleName);
				DictionaryHelper.AddNonDefaultValue<string, object>(dict, "codeName", roleCodeName);
				DictionaryHelper.AddNonDefaultValue<string, object>(dict, "description", roleDesc);
				DictionaryHelper.AddNonDefaultValue<string, object>(dict, "__type", obj.GetType().AssemblyQualifiedName);
			}

			return dict;
		}

		public override IEnumerable<Type> SupportedTypes
		{
			get
			{
				return new System.Type[] { typeof(OguRole) };
			}
		}
	}
}
