using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.Builder;
using MCS.Library.SOA.DataObjects.Schemas.Configuration;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Security.Configuration;

namespace MCS.Library.SOA.DataObjects.Security.Validators
{
	public static class CodeNameUniqueValidatorFacade
	{
		public static bool Validate(string codeName, string objectID, string objectSchemaType, string parentID, bool normalOnly, bool ignoreVersions, DateTime timePoint)
		{
			var schemaConfig = ObjectSchemaSettings.GetConfig().Schemas[objectSchemaType];
			IEnumerable<string> schemaNames = GetScopeSchemas(schemaConfig.CodeNameKey);

			switch (schemaConfig.CodeNameValidationMethod)
			{
				case SchemaObjectCodeNameValidationMethod.ByCodeNameKey:
					return ValidateCodeName(codeName, schemaNames.ToArray(), objectID, normalOnly, ignoreVersions, timePoint);
				case SchemaObjectCodeNameValidationMethod.ByContainerAndCodeNameKey:
					return ValidateCodeNameWithContainer(codeName, schemaNames.ToArray(), objectID, parentID, normalOnly, ignoreVersions, timePoint);
				default:
					throw new NotSupportedException("不支持的验证方式");
			}
		}

		internal static bool ValidateCodeName(string codeName, string[] schemaNames, string objectID, bool normalOnly, bool ignoreVersions, DateTime timePoint)
		{
			var objects = SchemaObjectAdapter.Instance.LoadByCodeNameAndSchema(schemaNames, new string[] { codeName }, normalOnly, ignoreVersions, DateTime.MinValue);

			bool exist = false;

			foreach (string schemaName in schemaNames)
			{
				foreach (SchemaObjectBase o in objects)
				{
					if (o.ID != objectID && o.SchemaType == schemaName)
					{
						exist = true;
						break;
					}
				}
			}

			return exist == false;
		}

		internal static bool ValidateCodeNameWithContainer(string codeName, string[] schemaNames, string objectID, string parentID, bool normalOnly, bool ignoreVersions, DateTime timePoint)
		{
			var objects = SchemaObjectAdapter.Instance.LoadByCodeNameAndSchema(schemaNames.ToArray(), new string[] { codeName }, true, false, DateTime.MinValue);

			bool result = false;
			if (objects.Count == 0)
			{
				result = true;
			}
			else
			{
				WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();

				where.AppendItem("ContainerID", parentID);

				if (normalOnly)
					where.AppendItem("Status", (int)SchemaObjectStatus.Normal);

				InSqlClauseBuilder inSql = new InSqlClauseBuilder("MemberID");
				inSql.AppendItem((from o in objects select o.ID).ToArray());

				// 只取当前关系
				var relations = SCMemberRelationAdapter.Instance.Load(new ConnectiveSqlClauseCollection(where, inSql), timePoint);

				bool exist = false;

				foreach (string schemaName in schemaNames)
				{
					foreach (SCMemberRelation o in relations)
					{
						if (o.ID != objectID && o.MemberSchemaType == schemaName)
						{
							exist = true;
							break;
						}
					}
				}

				result = exist == false;
			}

			return result;
		}

		internal static IEnumerable<string> GetScopeSchemas(string codeNameKey)
		{
			foreach (ObjectSchemaConfigurationElement schemaElem in ObjectSchemaSettings.GetConfig().Schemas)
			{
				if (schemaElem.CodeNameKey == codeNameKey)
				{
					yield return schemaElem.Name;
				}
			}
		}

		internal static List<string> GetScopeSchemas2(string CodeNameKey)
		{
			List<string> result = new List<string>();

			SchemaInfoCollection schemas = SchemaInfo.FilterByCodeNameKey(CodeNameKey);

			foreach (SchemaInfo item in schemas)
			{
				result.Add(item.Name);
			}

			return result;
		}

	}
}
