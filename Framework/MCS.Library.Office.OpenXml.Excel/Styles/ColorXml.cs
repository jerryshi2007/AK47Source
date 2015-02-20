using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using MCS.Library.Core;
using System.Globalization;

namespace MCS.Library.Office.OpenXml.Excel
{
	public class ColorXmlWrapper : StyleXmlBaseWrapper
	{
		internal ColorXmlWrapper()
		{
			//this._Auto = string.Empty;
			//this._Theme = string.Empty;
			//this._Tint = 0;
			//this._Rgb = string.Empty;
			//this._Indexed = int.MinValue;
		}

		internal override string Id
		{
			get
			{
				return this._Auto + "|" + this._Theme + "|" + this._Tint + "|" + this._Rgb + "|" + this._Indexed;
			}
		}

		private bool _Auto = false;
		public bool Auto
		{
			get
			{
				return this._Auto;
			}
			set
			{
				this._Theme = string.Empty;
				this._Tint = 0;
				this._Indexed = int.MinValue;
				this._Rgb = string.Empty;

				this._Auto = value;
			}
		}

		private string _Theme = string.Empty;
		public string Theme
		{
			get
			{
				return this._Theme;
			}
			internal set
			{
				this._Theme = value;
			}
		}


		private decimal _Tint = 0;
		public decimal Tint
		{
			get
			{
				return this._Tint;
			}
			internal set
			{
				this._Tint = value;
			}
		}


		private string _Rgb = string.Empty;
		public string Rgb
		{
			get
			{
				return this._Rgb;
			}
			set
			{
				this._Rgb = value;
				this._Indexed = int.MinValue;
				this._Auto = false;
			}
		}

		private int _Indexed = int.MinValue;
		public int Indexed
		{
			get
			{
				return this._Indexed;
			}
			set
			{
				//ExceptionHelper.TrueThrow(value < 0 || value > 65, "超出了索引范围{0}到{1}", 1, 64);
				this._Indexed = value;

				this._Theme = string.Empty;
				this._Tint = decimal.MaxValue;
				this._Rgb = string.Empty;
				this._Auto = false;
			}
		}

		public void SetColor(System.Drawing.Color color)
		{
			this._Theme = string.Empty;
			this._Tint = 0;
			this._Indexed = int.MinValue;
			this._Rgb = color.ToArgb().ToString("X");
		}

		internal ColorXmlWrapper Copy()
		{
			return new ColorXmlWrapper() { _Indexed = this.Indexed, _Tint = this.Tint, _Rgb = this.Rgb, _Theme = this.Theme, _Auto = this.Auto };
		}
	}
}
