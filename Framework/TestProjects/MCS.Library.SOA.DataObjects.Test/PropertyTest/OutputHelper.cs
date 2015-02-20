using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MCS.Library.SOA.DataObjects.Test
{
	public static class OutputHelper
	{
		public static void Output(this PropertyDefineConfigurationElement propertyElem, TextWriter writer, int indent = 0)
		{
			PropertyDefine prop = new PropertyDefine(propertyElem);

			Output(prop, writer, indent);
		}

		public static void Output(this PropertyDefine property, TextWriter writer, int indent = 0)
		{
			string indentChars = new string('\t', indent);

			writer.WriteLine("{0}Name={1}, Desp={2}, Type={3}, Category={4}",
						indentChars,
						property.Name,
						property.Description,
						property.DataType,
						property.Category);

			if (property.Validators.Count > 0)
			{
				indent++;

				WriteLine(writer, "Begin Validators", indent);

				indent++;
				property.Validators.ForEach(p => p.Output(writer, indent));
				indent--;

				WriteLine(writer, "End Validators", indent);
			}
		}

		public static void Output(this PropertyValue property, TextWriter writer, int indent = 0)
		{
			string indentChars = new string('\t', indent);

			writer.WriteLine("{0}Name={1}, Desp={2}, Type={3}, Category={4}",
						indentChars,
						property.Definition.Name,
						property.Definition.Description,
						property.Definition.DataType,
						property.Definition.Category);
		}

		public static void Output(this PropertyValidatorDescriptor propertyValidator, TextWriter writer, int indent = 0)
		{
			string indentChars = new string('\t', indent);

			writer.WriteLine("{0}Name={1}, Type={2}, MessageTemplate={3}",
						indentChars,
						propertyValidator.Name,
						propertyValidator.TypeDescription,
						propertyValidator.MessageTemplate);

			if (propertyValidator.Parameters.Count > 0)
			{
				indent++;

				WriteLine(writer, "Begin Parameters", indent);

				indent++;
				propertyValidator.Parameters.ForEach(p => p.Output(writer, indent));
				indent--;

				WriteLine(writer, "End Parameters", indent);
			}
		}

		public static void Output(this PropertyValidatorParameterDescriptor pvp, TextWriter writer, int indent = 0)
		{
			string indentChars = new string('\t', indent);

			writer.WriteLine("{0}ParamName={1}, DataType={2}, ParamValue={3}",
						indentChars,
						pvp.ParamName,
						pvp.DataType,
						pvp.ParamValue);
		}

		public static void Output(this DataFieldDefine dfd, TextWriter writer, int indent = 0)
		{
			Write(writer, string.Empty, indent);

			writer.WriteLine("Name={0}, Type={1}, Default={2}",
						dfd.Name,
						dfd.DataType,
						dfd.DefaultValue
						);

			indent++;

			WriteLine(writer, "Begin Properties", indent);

			indent++;
			dfd.Properties.ForEach(p => p.Output(writer, indent));
			indent--;

			WriteLine(writer, "End Properties", indent);
		}

		public static void Output(this DataObjectDefine dod, TextWriter writer, int indent = 0)
		{
			Write(writer, string.Empty, indent);

			writer.WriteLine("Name={0}, Desp={1}",
						dod.Name,
						dod.Description
						);

			indent++;

			WriteLine(writer, "Begin Fields", indent);

			indent++;
			dod.Fields.ForEach(f => f.Output(writer, indent));
			indent--;

			WriteLine(writer, "End Fields", indent);
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
