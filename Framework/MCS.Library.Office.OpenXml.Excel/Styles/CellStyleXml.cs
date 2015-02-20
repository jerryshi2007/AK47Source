using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Globalization;
using MCS.Library.Core;
using MCS.Library.Office.OpenXml.Excel.Styles;

namespace MCS.Library.Office.OpenXml.Excel
{
    public class CellStyleXmlWrapper : StyleXmlBaseWrapper
    {
        internal static ExcelHorizontalAlignment GetHorizontalAlign(string align)
        {
            if (align.IsNullOrEmpty())
                return ExcelHorizontalAlignment.Left;

            align = align.Substring(0, 1).ToUpper() + align.Substring(1, align.Length - 1);
            try
            {
                return (ExcelHorizontalAlignment)Enum.Parse(typeof(ExcelHorizontalAlignment), align);
            }
            catch
            {
                return ExcelHorizontalAlignment.Left;
            }
        }

        internal static ExcelVerticalAlignment GetVerticalAlign(string align)
        {
            if (align.IsNullOrEmpty())
                return ExcelVerticalAlignment.Bottom;

            align = align.Substring(0, 1).ToUpper() + align.Substring(1, align.Length - 1);
            try
            {
                return (ExcelVerticalAlignment)Enum.Parse(typeof(ExcelVerticalAlignment), align);
            }
            catch
            {
                return ExcelVerticalAlignment.Bottom;
            }
        }

        public int _XfID;
        public int XfId
        {
            get
            {
                return this._XfID;
            }
            set
            {
                this._XfID = value;
            }
        }

        private int _NumFmtId;
        internal int NumberFormatId
        {
            get
            {
                return this._NumFmtId;
            }
            set
            {
                this._NumFmtId = value;
            }
        }

        private NumberFormatXmlWrapper _NumberFormat;
        public NumberFormatXmlWrapper NumberFormat
        {
            get
            {
                if (this._NumberFormat == null)
                    this._NumberFormat = new NumberFormatXmlWrapper(true);

                return this._NumberFormat;
            }
            internal set
            {
                this._NumberFormat = value;
            }
        }

        private int _FontId;
        internal int FontId
        {
            get
            {
                return this._FontId;
            }
            set
            {
                this._FontId = value;
            }
        }

        private int _FillId;
        internal int FillId
        {
            get
            {
                return this._FillId;
            }
            set
            {
                this._FillId = value;
            }
        }

        private int _BorderId;
        internal int BorderId
        {
            get
            {
                return this._BorderId;
            }
            set
            {
                this._BorderId = value;
            }
        }

        private bool IsBuildIn
        {
            get;
            set;
        }

        public bool _ApplyBorder;
        public bool ApplyBorder
        {
            get { return this._ApplyBorder; }
            set { this._ApplyBorder = value; }
        }

        public bool _ApplyAlignment;
        public bool ApplyAlignment
        {
            get { return this._ApplyAlignment; }
            set { this._ApplyAlignment = value; }
        }

        private FontXmlWrapper _Font;
        public FontXmlWrapper Font
        {
            get
            {
                if (this._Font == null)
                {
                    this._Font = new FontXmlWrapper();
                }
                return this._Font;
            }
            set
            {
                this._Font = value;
            }
        }

        private FillXmlWrapper _Fill;
        public FillXmlWrapper Fill
        {
            get
            {
                if (this._Fill == null)
                {
                    this._Fill = new FillXmlWrapper();
                }
                return this._Fill;
            }
            set
            {
                this._Fill = value;
            }
        }

        private BorderXmlWrapper _Border;
        public BorderXmlWrapper Border
        {
            get
            {
                if (this._Border == null)
                {
                    this._Border = new BorderXmlWrapper();
                }
                return this._Border;
            }
            set
            {
                this._Border = value;
            }
        }

        private const string horizontalAlignPath = "d:alignment/@horizontal";
        private ExcelHorizontalAlignment _HorizontalAlignment = ExcelHorizontalAlignment.Left;
        public ExcelHorizontalAlignment HorizontalAlignment
        {
            get
            {
                return this._HorizontalAlignment;
            }
            set
            {
                this._HorizontalAlignment = value;
            }
        }

        private const string verticalAlignPath = "d:alignment/@vertical";
        private ExcelVerticalAlignment _VerticalAlignment = ExcelVerticalAlignment.Bottom;
        public ExcelVerticalAlignment VerticalAlignment
        {
            get
            {
                return this._VerticalAlignment;
            }
            set
            {
                this._VerticalAlignment = value;
            }
        }

        private const string wrapTextPath = "d:alignment/@wrapText";
        private bool _WrapText = false;
        public bool WrapText
        {
            get
            {
                return this._WrapText;
            }
            set
            {
                this._WrapText = value;
            }
        }

        private const string textRotationPath = "d:alignment/@textRotation";
        private int _TextRotation = 0;
        public int TextRotation
        {
            get
            {
                return this._TextRotation;
            }
            set
            {
                this._TextRotation = value;
            }
        }

        private const string lockedPath = "d:protection/@locked";
        private bool _Locked = true;
        public bool Locked
        {
            get
            {
                return this._Locked;
            }
            set
            {
                this._Locked = value;
            }
        }

        private const string hiddenPath = "d:protection/@hidden";
        private bool _Hidden = false;
        public bool Hidden
        {
            get
            {
                return this._Hidden;
            }
            set
            {
                this._Hidden = value;
            }
        }

        private const string readingOrderPath = "d:alignment/@readingOrder";
        private bool _ReadingOrder = false;
        public bool ReadingOrder
        {
            get
            {
                return this._ReadingOrder;
            }
            set
            {
                this._ReadingOrder = value;
            }
        }

        private const string shrinkToFitPath = "d:alignment/@shrinkToFit";
        private bool _ShrinkToFit = false;
        public bool ShrinkToFit
        {
            get
            {
                return this._ShrinkToFit;
            }
            set
            {
                this._ShrinkToFit = value;
            }
        }

        private const string indentPath = "d:alignment/@indent";
        private int _Indent = 0;
        public int Indent
        {
            get
            {
                return this._Indent;
            }
            set
            {
                this._Indent = value;
            }
        }

        internal override string Id
        {
            get
            {
                return XfId + "|" + NumberFormat.Id.ToString() + "|" + this.Font.Id + "|" + this.Fill.Id + "|" + this.Border.Id + "|" + VerticalAlignment.ToString() + "|" + HorizontalAlignment.ToString() + "|" + WrapText.ToString() + "|" + ReadingOrder.ToString() + "|" + IsBuildIn.ToString() + "|" + TextRotation.ToString() + "|" + Locked.ToString() + "|" + Hidden.ToString() + "|" + ShrinkToFit.ToString() + "|" + Indent.ToString();
            }
        }

        private void SetBorderItem(BorderItemXmlWrapper excelBorderItem, ExcelStyleProperty styleProperty, object value)
        {
            if (styleProperty == ExcelStyleProperty.Style)
            {
                excelBorderItem.Style = (ExcelBorderStyle)value;
            }
            else if (styleProperty == ExcelStyleProperty.Color)
            {
                ExceptionHelper.TrueThrow(excelBorderItem.Style == ExcelBorderStyle.None, "不能设置边框样式为None");

                excelBorderItem.Color.Rgb = value.ToString();
            }
        }

        private string SetAlignString(Enum align)
        {
            string newName = Enum.GetName(align.GetType(), align);

            return newName.Substring(0, 1).ToLower() + newName.Substring(1, newName.Length - 1);
        }
    }
}
