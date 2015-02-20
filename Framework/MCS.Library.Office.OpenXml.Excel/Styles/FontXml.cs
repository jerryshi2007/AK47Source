using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using MCS.Library.Core;
using System.Globalization;

namespace MCS.Library.Office.OpenXml.Excel
{
    public class FontXmlWrapper : StyleXmlBaseWrapper
    {
        internal static readonly string DefaultFontName = "Calibri";

        internal FontXmlWrapper()
        {
            this._Name = FontXmlWrapper.DefaultFontName;
            this._Scheme = string.Empty;
            this._VerticalAlign = string.Empty;
            this._Bold = false;
            this._Italic = false;
            this._Strike = false;
            this._Underline = false; ;

        }

        internal override string Id
        {
            get
            {
                return this.Name + "|" + this.Size + "|" + this.Family + "|" + this.Color.Id + "|" + this.Scheme + "|" + this.Bold.ToString() + "|" + this.Italic.ToString() + "|" + this.Strike.ToString() + "|" + this.VerticalAlign + "|" + this.UnderLine.ToString();
            }
        }

        private string _Name;
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                Scheme = string.Empty;
                this._Name = value;
            }
        }

        private float _Size=11;
        public float Size
        {
            get
            {
                return this._Size;
            }
            set
            {
                this._Size = value;
            }
        }

        private int _Family = int.MinValue;
        public int Family
        {
            get
            {
                return this._Family;
            }
            set
            {
                this._Family = value;
            }
        }

		internal ColorXmlWrapper _Color = null;
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

        private string _Scheme = string.Empty;
        public string Scheme
        {
            get
            {
                return this._Scheme;
            }
            internal set
            {
                this._Scheme = value;
            }
        }

        private int _Charset = int.MinValue;
        public int Charset
        {
            get
            {
                return this._Charset;
            }
            internal set
            {
                this._Charset = value;
            }
        }

        private bool _Bold;
        public bool Bold
        {
            get
            {
                return this._Bold;
            }
            set
            {
                this._Bold = value;
            }
        }

        private bool _Italic;
        public bool Italic
        {
            get
            {
                return this._Italic;
            }
            set
            {
                this._Italic = value;
            }
        }

        private bool _Shadow;
        public bool Shadow
        {
            get { return this._Shadow; }
            set { this._Shadow = value; }
        }

        private bool _Strike;
        public bool Strike
        {
            get
            {
                return this._Strike;
            }
            set
            {
                this._Strike = value;
            }
        }

        private bool _Underline;
        public bool UnderLine
        {
            get
            {
                return this._Underline;
            }
            set
            {
                this._Underline = value;
            }
        }

        private string _VerticalAlign;
        public string VerticalAlign
        {
            get
            {
                return this._VerticalAlign;
            }
            set
            {
                this._VerticalAlign = value;
            }
        }

        public void SetFromFont(System.Drawing.Font Font)
        {
            Name = Font.Name;
            Size = (int)Font.Size;
            Strike = Font.Strikeout;
            Bold = Font.Bold;
            UnderLine = Font.Underline;
            Italic = Font.Italic;
        }

        internal FontXmlWrapper Copy()
        {
            FontXmlWrapper newFont = new FontXmlWrapper();
            newFont.Name = Name;
            newFont.Size = Size;
            newFont.Family = Family;
            newFont.Scheme = Scheme;
            newFont.Bold = Bold;
            newFont.Italic = Italic;
            newFont.UnderLine = UnderLine;
            newFont.Strike = Strike;
            newFont.VerticalAlign = VerticalAlign;
            newFont.Color = Color.Copy();

            return newFont;
        }
    }
}
