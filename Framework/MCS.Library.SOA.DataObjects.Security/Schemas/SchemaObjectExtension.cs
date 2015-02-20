using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects.Security
{
	public static class SchemaObjectExtension
	{
		public static IOguObject ToOguObject(this SCSimpleObject obj)
		{
			OguBase result = null;

			string category = SchemaDefine.GetSchemaConfig(obj.SchemaType).Category;
			SchemaType schemaType = SchemaExtensions.SchemaCategoryToOguSchemaType(category);

			if (schemaType != OGUPermission.SchemaType.Unspecified)
			{
				result = (OguBase)OguBase.CreateWrapperObject(obj.ID, schemaType);
				result.Name = obj.Name;
				result.DisplayName = obj.DisplayName;
			}

			return result;
		}

		public static UserInfoExtendDataObject ToUserExtendedInfo(this SchemaObjectBase obj)
		{
			obj.NullCheck("obj");

			UserInfoExtendDataObject extendedInfo = new UserInfoExtendDataObject();

			extendedInfo.ID = obj.ID;
			extendedInfo.IMAddress = obj.Properties.GetValue("Sip", string.Empty);
			extendedInfo.IntranetEmail = obj.Properties.GetValue("Mail", string.Empty);
			extendedInfo.InternetEmail = obj.Properties.GetValue("Mail", string.Empty);
			extendedInfo.LogonName = obj.Properties.GetValue("CodeName", string.Empty);
			extendedInfo.Mobile = obj.Properties.GetValue("MP", string.Empty);
			extendedInfo.OfficeTel = obj.Properties.GetValue("WP", string.Empty);
			extendedInfo.DisplayName = obj.Properties.GetValue("Display", string.Empty);

			return extendedInfo;
		}
	}
}
