using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Schemas.Configuration;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.SOA.DataObjects.Security.Configuration;

namespace MCS.Library.SOA.DataObjects.Security
{
	public static class SchemaExtensions
	{
		public static SchemaDefineCollection CreateSchemasDefineFromConfiguration()
		{
			return new SchemaDefineCollection(ObjectSchemaSettings.GetConfig().Schemas);
		}

		public static SchemaInfoCollection CreateSchemasInfoFromConfiguration()
		{
			return new SchemaInfoCollection(ObjectSchemaSettings.GetConfig().Schemas);
		}

		public static SchemaType SchemaCategoryToOguSchemaType(string schemaTypeString)
		{
			SchemaType result = SchemaType.Unspecified;

			switch (schemaTypeString)
			{
				case "Users":
					result = SchemaType.Users;
					break;
				case "Organizations":
					result = SchemaType.Organizations;
					break;
				case "Groups":
					result = SchemaType.Groups;
					break;
			}

			return result;
		}

		public static SchemaObjectBase CreateObject(string schemaTypeString)
		{
			ObjectSchemaSettings settings = ObjectSchemaSettings.GetConfig();

			ObjectSchemaConfigurationElement schemaElement = settings.Schemas[schemaTypeString];
			(schemaElement != null).FalseThrow<NotSupportedException>("不支持的对象类型: {0}", schemaTypeString);

			SchemaObjectBase result = null;

			if (schemaElement.GetTypeInfo() == typeof(SCGenericObject))
			{
				result = new SCGenericObject(schemaTypeString);
			}
			else
			{
				result = (SchemaObjectBase)schemaElement.CreateInstance(schemaTypeString);
				if (result.SchemaType != schemaTypeString)
					throw new SchemaObjectErrorCreationException(string.Format("无法根据指定的SchemaType {0} 创建对应的类型", schemaTypeString));
			}

			return result;
		}

		public static void IfInSnapshot(this SnapshotModeDefinition smd, Action action)
		{
			IfSwitchedOn(smd, SnapshotModeDefinition.IsInSnapshot, action);
		}

		public static void IfInFullTextIndex(this SnapshotModeDefinition smd, Action action)
		{
			IfSwitchedOn(smd, SnapshotModeDefinition.IsFullTextIndexed, action);
		}

		/// <summary>
		/// 如果某个标志位为打开，则执行Action
		/// </summary>
		/// <param name="smd"></param>
		/// <param name="constSmd"></param>
		/// <param name="action"></param>
		public static void IfSwitchedOn(this SnapshotModeDefinition smd, SnapshotModeDefinition constSmd, Action action)
		{
			if ((smd & constSmd) != SnapshotModeDefinition.None && action != null)
				action();
		}

		public static void SchemaToSqlClauseBuilder<T>(this SchemaObjectBase obj, SnapshotModeDefinition constSmd, T builder) where T : SqlClauseBuilderIUW
		{
			if (obj != null && builder != null)
			{
				obj.Schema.Properties.ForEach(pd =>
					pd.SnapshotMode.IfSwitchedOn(constSmd, () =>
						builder.AppendItem(GetFieldName(ORMapping.GetMappingInfo(obj.GetType()), pd), GetPropertyValue(obj, pd))));

				builder.AppendItem("SearchContent", obj.ToFullTextString());
				builder.AppendItem("SchemaType", obj.SchemaType);
				builder.AppendItem("Status", (int)obj.Status);
			}
		}

		public static List<string> SchemaToSqlFields(this SchemaDefine schema, ORMappingItemCollection mapping, params string[] ignoreProperties)
		{
			List<string> result = new List<string>();

			if (schema != null)
			{
				schema.Properties.ForEach(pd =>
					pd.SnapshotMode.IfInSnapshot(() =>
					{
						if (Array.Exists(ignoreProperties, s => s == pd.Name) == false)
							result.Add(GetFieldName(mapping, pd));
					}));

				result.Add("SearchContent");
				result.Add("SchemaType");
			}

			return result;
		}

		private static string GetFieldName(ORMappingItemCollection mapping, SchemaPropertyDefine pd)
		{
			string result = pd.SnapshotFieldName;

			if (result.IsNullOrEmpty())
			{
				result = pd.Name;

				ORMappingItem item = mapping.Find(m => m.PropertyName == pd.Name);

				if (item != null)
					result = item.DataFieldName;
			}

			return result;
		}

		private static object GetPropertyValue(SchemaObjectBase obj, SchemaPropertyDefine pd)
		{
			try
			{
				object defaultValue = TypeCreator.GetTypeDefaultValue(pd.DataType.ToRealType());
				return obj.Properties.GetValue(pd.Name, defaultValue);
			}
			catch (System.Exception ex)
			{
				throw new SystemSupportException(string.Format("生成Snapshot或全文检索时，{0}属性值获取错误: {1}", pd.Name, ex.Message),
					ex);
			}
		}

		[Serializable]
		public class SchemaObjectErrorCreationException : System.Exception
		{
			public SchemaObjectErrorCreationException() : base("创建对象失败") { }
			public SchemaObjectErrorCreationException(string message) : base(message) { }
			public SchemaObjectErrorCreationException(string message, System.Exception inner) : base(message, inner) { }
			protected SchemaObjectErrorCreationException(
			  System.Runtime.Serialization.SerializationInfo info,
			  System.Runtime.Serialization.StreamingContext context)
				: base(info, context) { }
		}
	}
}
