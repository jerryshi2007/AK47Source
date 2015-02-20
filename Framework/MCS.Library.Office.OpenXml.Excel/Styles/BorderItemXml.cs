using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using MCS.Library.Core;

namespace MCS.Library.Office.OpenXml.Excel
{
    public class BorderItemXmlWrapper : StyleXmlBaseWrapper
    {
        internal BorderItemXmlWrapper()
        {
        }

        internal ExcelBorderStyle GetBorderStyle(string style)
        {
            if (style.IsNullOrEmpty())
            {
                return ExcelBorderStyle.None;
            }
            string sInStyle = style.Substring(0, 1).ToUpper() + style.Substring(1, style.Length - 1);
            try
            {
                return (ExcelBorderStyle)Enum.Parse(typeof(ExcelBorderStyle), sInStyle);
            }
            catch
            {
                return ExcelBorderStyle.None;
            }
        }

        private ExcelBorderStyle _BorderStyle = ExcelBorderStyle.None;
        public ExcelBorderStyle Style
        {
            get
            {
                return this._BorderStyle;
            }
            set
            {
                this._BorderStyle = value;
                this.Exists = true;
            }
        }

        internal ColorXmlWrapper _Color;
        private const string _colorPath = "d:color";
        public ColorXmlWrapper Color
        {
            get
            {
                if (this._Color == null)
                {
                    this._Color = new ColorXmlWrapper();
                }
                return this._Color;
            }
            internal set
            {
                this._Color = value;
            }
        }
        internal override string Id
        {
            get { return SetBorderString(Style) + "|" + Color.Id; }
        }

        internal BorderItemXmlWrapper Copy()
        {
            BorderItemXmlWrapper borderItem = new BorderItemXmlWrapper();
            borderItem.Style = this._BorderStyle;
            borderItem.Color = this._Color.Copy();

            return borderItem;
        }

		internal string SetBorderString(ExcelBorderStyle Style)
        {
            string newName = Enum.GetName(typeof(ExcelBorderStyle), Style);

            return newName.Substring(0, 1).ToLower() + newName.Substring(1, newName.Length - 1);
        }

        public bool Exists { get; private set; }
    }
}
