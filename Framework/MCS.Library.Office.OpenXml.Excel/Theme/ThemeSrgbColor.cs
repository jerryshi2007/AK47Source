using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace MCS.Library.Office.OpenXml.Excel
{
   public class ThemeSrgbColor : IThemeColorScheme, IDisposable
    {
        private ThemeColorScheme _colorScheme;
        private string _parentNodeName;
        private string _val;

        public ThemeSrgbColor(ThemeColorScheme colorScheme,  string parenetNodeName)
        {
            this._colorScheme = colorScheme;
            this._parentNodeName = parenetNodeName;
        }

        public void Dispose()
        {
            this._val = null;
        }

		//public void WriteXml(XmlTextWriter writer)
		//{
		//    writer.WriteStartElement(this._parentNodeName);
		//    writer.WriteStartElement("a:srgbClr");
		//    writer.WriteAttributeString("val", this._val);
		//    writer.WriteEndElement();
		//    writer.WriteEndElement();
		//}

        public string Color
        {
            get
            {
                return this.Val;
            }
        }

        public ThemeColorScheme ColorScheme
        {
            get
            {
                return this._colorScheme;
            }
        }

        public string Val
        {
            get
            {
                return this._val;
            }
            set
            {
                this._val = value;
            }
        }
    }
}
