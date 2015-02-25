using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Schemas.Configuration;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Configuration;
using MCS.Library.SOA.DataObjects.Security.Permissions;

namespace MCS.Library.SOA.DataObjects.Security.Test
{
	internal static class ConfigurationOutputHelper
	{
		public static void Output(this SchemaPropertyGroupConfigurationElement groupElem, TextWriter writer, int indent = 0)
		{
			string indentChars = new string('\t', indent);

			writer.WriteLine("{0}Name={1}, Desp={2}, DefaultTab={3}",
						indentChars,
						groupElem.Name,
						groupElem.Description,
						groupElem.DefaultTab);

			if (groupElem.AllProperties.Count > 0)
			{
				indent++;

				WriteLine(writer, "Begin Properties", indent);

				indent++;
				groupElem.AllProperties.ForEach<SchemaPropertyDefineConfigurationElement>(p => p.Output(writer, indent));
				indent--;

				WriteLine(writer, "End Properties", indent);
			}
		}

		public static void Output(this SchemaPropertyDefineConfigurationElement propertyElem, TextWriter writer, int indent = 0)
		{
			string indentChars = new string('\t', indent);

			writer.WriteLine("{0}Name={1}, Desp={2}, Type={3}, Category={4}, SnapshotMode={5}",
						indentChars,
						propertyElem.Name,
						propertyElem.Description,
						propertyElem.DataType,
						propertyElem.Category,
						propertyElem.SnapshotMode);
		}

		public static void Output(this SchemaPropertyDefine propertyDefine, TextWriter writer, int indent = 0)
		{
			string indentChars = new string('\t', indent);

			writer.WriteLine("{0}Name={1}, Desp={2}, Type={3}, Category={4}, SnapshotMode={5}",
						indentChars,
						propertyDefine.Name,
						propertyDefine.Description,
						propertyDefine.DataType,
						propertyDefine.Category,
						propertyDefine.SnapshotMode);
		}

		public static void Output(this SCAclPermissionItem permissionItem, TextWriter writer, int indent = 0)
		{
			string indentChars = new string('\t', indent);

			writer.WriteLine("{0}Name={1}, DisplayName={2}, Description={3}",
						indentChars,
						permissionItem.Name,
						permissionItem.DisplayName,
						permissionItem.Description);
		}

		public static void Output(this ObjectSchemaConfigurationElement schemaConfig, TextWriter writer, int indent = 0)
		{
			string indentChars = new string('\t', indent);

			writer.WriteLine("{0}Schema Name={1}", indentChars, schemaConfig.Name);

			if (schemaConfig.Groups.Count > 0)
			{
				indent++;

				WriteLine(writer, "Begin Schemas", indent);

				indent++;
				schemaConfig.Groups.ForEach<ObjectSchemaClassConfigurationElement>(p => p.Output(writer, indent));
				indent--;

				WriteLine(writer, "End Schemas", indent);
			}
		}

		public static void Output(this ObjectSchemaClassConfigurationElement schemaClassConfig, TextWriter writer, int indent = 0)
		{
			string indentChars = new string('\t', indent);

			writer.WriteLine("{0}Group Name={1}", indentChars, schemaClassConfig.GroupName);
		}

		public static void Output(this SchemaInfo schema, TextWriter writer, int indent = 0)
		{
			string indentChars = new string('\t', indent);

			writer.WriteLine("{0}Schema Name={1}, SnapshotTable={2}", indentChars, schema.Name, schema.SnapshotTable);
		}

		public static void Output(this SchemaDefine schema, TextWriter writer, int indent = 0)
		{
			string indentChars = new string('\t', indent);

			writer.WriteLine("{0}Schema Name={1}", indentChars, schema.Name);

			if (schema.Properties.Count > 0)
			{
				indent++;

				WriteLine(writer, "Begin Schemas", indent);

				indent++;
				schema.Properties.ForEach(pd => pd.Output(writer, indent));
				indent--;

				WriteLine(writer, "End Schemas", indent);
			}

			if (schema.PermissionSet.Count > 0)
			{
				indent++;

				WriteLine(writer, "Begin PermissionSet", indent);

				indent++;
				schema.PermissionSet.ForEach(pd => pd.Output(writer, indent));
				indent--;

				WriteLine(writer, "End PermissionSet", indent);
			}
		}

		private static void Write(TextWriter writer, string data, int indent = 0)
		{
			string indentChars = new string('\t', indent);

			writer.Write("{0}{1}", indentChars, data);
		}

		private static void WriteLine(TextWriter writer, string data, int indent = 0)
		{
			string indentChars = new string('\t', indent);

			writer.WriteLine("{0}{1}", indentChars, data);
		}
	}
}
