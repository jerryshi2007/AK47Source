using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using MCS.Library.Core;
using System.Globalization;
using System.Drawing;

namespace MCS.Library.Office.OpenXml.Excel
{
    public class FillXmlWrapper : StyleXmlBaseWrapper
    {
        internal FillXmlWrapper()
        {
          
        }

        internal ExcelFillStyle GetPatternType(string patternType)
        {
            if (patternType.IsNullOrEmpty())
            {
                return ExcelFillStyle.None;
            }
            patternType = patternType.Substring(0, 1).ToUpper() + patternType.Substring(1, patternType.Length - 1);
            try
            {
                return (ExcelFillStyle)Enum.Parse(typeof(ExcelFillStyle), patternType);
            }
            catch
            {
                return ExcelFillStyle.None;
            }
        }

        internal override string Id
        {
            get
            {
				StringBuilder strId = new StringBuilder();
				strId.Append(PatternType.ToString());
				strId.Append("|");
				if(this._PatternColor !=null)
				{
					strId.Append(this._PatternColor.Id);
				}
				strId.Append("|");
				if (this._BackgroundColor != null)
				{
					strId.Append(this._BackgroundColor.Id);
				}

				return strId.ToString();
            }
        }

        internal ExcelFillStyle _FillPatternType = ExcelFillStyle.None;
        public ExcelFillStyle PatternType
        {
            get
            {
                return this._FillPatternType;
            }
            set
            {
                this._FillPatternType = value;
            }
        }

		internal ColorXmlWrapper _PatternColor = null;
        public ColorXmlWrapper PatternColor
        {
            get
            {
                if (this._PatternColor == null)
                {
                    this._PatternColor = new ColorXmlWrapper();
                }
                return this._PatternColor;
            }
            internal set
            {
                this._PatternColor = value;
            }
        }

		internal ColorXmlWrapper _BackgroundColor = null;
        public ColorXmlWrapper BackgroundColor
        {
            get
            {
                if (this._BackgroundColor == null)
                {
                    this._BackgroundColor = new ColorXmlWrapper();
                }
                return this._BackgroundColor;
            }
            internal set
            {
                this._BackgroundColor = value;
            }
        }

		/// <summary>
		/// 设置背景着颜色(注意，当ExcelFillStyle.Solid 请不要再设置前景色PatternColor，设置为None将不会填充)
		/// </summary>
		/// <param name="color"></param>
		/// <param name="pattern"></param>
		public void SetBackgroundColor(Color color, ExcelFillStyle pattern)
		{
			if (this._BackgroundColor == null)
			{
				this._BackgroundColor = new ColorXmlWrapper();
			}

			if (pattern == ExcelFillStyle.Solid)
			{
				this._BackgroundColor.Indexed =64;
				if (this._PatternColor == null)
				{
					this._PatternColor = new ColorXmlWrapper();
				}
				this._PatternColor.SetColor(color);
			}
			else
			{
				this._BackgroundColor.SetColor(color);
			}

			this.PatternType = pattern;
		}

        internal FillXmlWrapper Copy()
        {
            FillXmlWrapper newFill = new FillXmlWrapper();
            newFill.PatternType = this._FillPatternType;
            newFill.BackgroundColor = this._BackgroundColor.Copy();
            newFill.PatternColor = this._PatternColor.Copy();

            return newFill;
        }

        internal static string SetPatternString(ExcelFillStyle pattern)
        {
            string newName = Enum.GetName(typeof(ExcelFillStyle), pattern);

            return newName.Substring(0, 1).ToLower() + newName.Substring(1, newName.Length - 1);
        }
    }
}
