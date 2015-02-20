using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using MCS.Library.Office.OpenXml.Excel.Theme;

namespace MCS.Library.Office.OpenXml.Excel
{
    public class ThemeElements : IDisposable
    {
        private AppTheme _Theme;

        public ThemeElements(AppTheme theme)
        {
            this._Theme = theme;
        }

        private ThemeColorScheme _ColorScheme;
        public ThemeColorScheme ColorScheme
        {
            get
            {
                if (this._ColorScheme == null)
                {
                    this._ColorScheme = new ThemeColorScheme(this);
                }
                return this._ColorScheme;
            }
        }

        private ThemeFontScheme _FontScheme;
        public ThemeFontScheme FontScheme
        {
            get
            {
                if (this._FontScheme == null)
                {
                    this._FontScheme = new ThemeFontScheme(this);
                }
                return this._FontScheme;
            }
        }

        private ThemeFormatScheme _FormatScheme;
        public ThemeFormatScheme FormatScheme
        {
            get
            {
                if (this._FormatScheme == null)
                {
                    this._FormatScheme = new ThemeFormatScheme(this);
                }

                return this._FormatScheme;
            }
        }

        public void Dispose()
        {
            if (this._ColorScheme != null)
            {
                this._ColorScheme.Dispose();
                this._ColorScheme = null;
            }
            if (this._FontScheme != null)
            {
                this._FontScheme.Dispose();
                this._FontScheme = null;
            }
            if (this._FormatScheme != null)
            {
                this._FormatScheme.Dispose();
                this._FormatScheme = null;
            }
        }
    }
}
