using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.Office.OpenXml.Excel
{
	public sealed class ExcelChartTitle
	{
		public string Text
		{
			get
			{
				return this.RichText.Text;
			}
			set
			{
				this.RichText.Text = value;
			}
		}

		private ParagraphCollection _RichText = null;
		public ParagraphCollection RichText
		{
			get
			{
				if (this._RichText == null)
					this._RichText = new ParagraphCollection();

				return _RichText;
			}
		}

		internal DrawingBorder _Border = null;
		public DrawingBorder Border
		{
			get
			{
				if (this._Border == null)
					this._Border = new DrawingBorder();

				return this._Border;
			}
		}

		internal DrawingFill _Fill = null;
		public DrawingFill Fill
		{
			get
			{
				if (this._Fill == null)
					this._Fill = new DrawingFill();

				return this._Fill;
			}
		}

		public Paragraph Font
		{
			get
			{
				if (this._RichText == null || this._RichText.Count == 0)
				{
					this.RichText.Add("");
				}
				return this.RichText[0];
			}
		}

		public bool Overlay
		{
			get;
			set;
		}

		public bool AnchorCtr
		{
			get;
			set;
		}

		private ExcelTextAnchoringType _Anchor = ExcelTextAnchoringType.Top;
		public ExcelTextAnchoringType Anchor
		{
			get
			{
				return this._Anchor;
			}
			set
			{
				this._Anchor = value;
			}
		}

		private ExcelTextVerticalType _TextVerTical = ExcelTextVerticalType.Horizontal;
		public ExcelTextVerticalType TextVertical
		{
			get
			{
				return this._TextVerTical;
			}
			set
			{
				this._TextVerTical = value;
			}
		}

		private ExcelTextHorzOverflowType _TextHorz = default(ExcelTextHorzOverflowType);
		public ExcelTextHorzOverflowType TextHorz
		{
			get
			{
				return this._TextHorz;
			}
			set
			{
				this._TextHorz = value;
			}
		}

		private double _Rotation = default(double);
		public double Rotation
		{
			get
			{
				return this._Rotation;
			}
			set
			{
				ExceptionHelper.TrueThrow<ArgumentOutOfRangeException>(value < 0 || value > 360, "超出范围");
				this._Rotation = value;
			}
		}
	}
}
