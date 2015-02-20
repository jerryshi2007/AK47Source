using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Core;
using PermissionCenter.Tables;

namespace PermissionCenter.Adapters
{
	internal static class QueryHelper
	{
		public static readonly Dictionary<string, TableBuilderBase> OguSchemaToBuilders =
			new Dictionary<string, TableBuilderBase>(StringComparer.OrdinalIgnoreCase)
				{
					{ "OguObject", new OguObjectTableBuilder() },
					{ "Users", new OguUserTableBuilder() },
					{ "Organizations", new OguOrganizationTableBuilder() },
					{ "Groups", new OguGroupTableBuilder() }
				};

		public static readonly Dictionary<string, AppObjectTableBuilderBase> AppObjectSchemaToBuilders =
			new Dictionary<string, AppObjectTableBuilderBase>(StringComparer.OrdinalIgnoreCase)
				{
					{"Applications", new ApplicationTableBuilder() },
					{"Roles", new RoleTableBuilder() },
					{"Permissions", new PermissionTableBuilder() },
				};

		public static TableBuilderBase GetOguTableBuilder(string[] schemaTypes)
		{
			schemaTypes.NullCheck("schemaTypes");

			TableBuilderBase result = null;

			if (schemaTypes.Length == 0 || schemaTypes.Length > 1)
			{
				result = OguSchemaToBuilders["OguObject"];
			}
			else
			{
				if (OguSchemaToBuilders.TryGetValue(schemaTypes[0], out result) == false)
					result = OguSchemaToBuilders["OguObject"];
			}

			return result;
		}

		public static AppObjectTableBuilderBase GetAppObjectTableBuilder(string[] schemaTypes)
		{
			schemaTypes.NullCheck("schemaTypes");

			AppObjectTableBuilderBase result = null;

			AppObjectSchemaToBuilders.TryGetValue(schemaTypes[0], out result).FalseThrow("不能根据SchemaType为{0}找到对应的TableBuilder", string.Join(",", schemaTypes));

			return result;
		}
	}
}