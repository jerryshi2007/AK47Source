using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace MCS.Library.Office.OpenXml.Excel
{
    public class ThemeSysColor : IThemeColorScheme, IDisposable
    {
        private string _parentNodeName;

        public ThemeSysColor(ThemeColorScheme colorScheme, string parenetNodeName)
        {
            this._ColorScheme = colorScheme;
            this._parentNodeName = parenetNodeName;
        }

        public void Dispose()
        {
            this._val = null;
            this._LastClr = null;
        }

		//public void WriteXml(XmlTextWriter writer)
		//{
		//    writer.WriteStartElement(this._parentNodeName);
		//    writer.WriteStartElement("a:sysClr");
		//    writer.WriteAttributeString("val", this._val);
		//    writer.WriteAttributeString("lastClr", this._LastClr);
		//    writer.WriteEndElement();
		//    writer.WriteEndElement();
		//}

        public string Color
        {
            get
            {
                return this.LastClr;
            }
        }

        private ThemeColorScheme _ColorScheme;
        public ThemeColorScheme ColorScheme
        {
            get
            {
                return this._ColorScheme;
            }
        }

        private string _LastClr;
        public string LastClr
        {
            get
            {
                return this._LastClr;
            }
            set
            {
                this._LastClr = value;
            }
        }

        private string _val;
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
