using System;
using System.Net;
using System.Windows;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Designer.Commands
{
	public class ConvertFromStringConverter : TypeConverter
	{
		private Type type;

		public ConvertFromStringConverter(Type type)
		{
			this.type = type;
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			string text = value as string;
			if (text != null)
			{
				if (this.type == typeof(bool))
				{
					return bool.Parse(text);
				}
				if (this.type.IsEnum)
				{
					return Enum.Parse(this.type, text, false);
				}
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(string.Concat(new string[]
				{
					"<ContentControl xmlns='http://schemas.microsoft.com/client/2007' xmlns:c='clr-namespace:",
					this.type.Namespace,
					";assembly=",
					this.type.Assembly.FullName.Split(new char[]
					{
						','
					})[0],
					"'>\n"
				}));
				stringBuilder.Append("<c:" + this.type.Name + ">\n");
				stringBuilder.Append(text);
				stringBuilder.Append("</c:" + this.type.Name + ">\n");
				stringBuilder.Append("</ContentControl>");
				ContentControl contentControl = XamlReader.Load(stringBuilder.ToString()) as ContentControl;
				if (contentControl != null)
				{
					return contentControl.Content;
				}
			}
			return base.ConvertFrom(context, culture, value);
		}
	}
}

