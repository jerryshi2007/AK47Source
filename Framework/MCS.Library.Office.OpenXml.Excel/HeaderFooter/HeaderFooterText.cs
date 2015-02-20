using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace MCS.Library.Office.OpenXml.Excel
{
	public class HeaderFooterText
	{
		private HeaderFooter _HeaderFooter;
		private string code = string.Empty;

		internal HeaderFooterText(HeaderFooter headerFooter, string text)
		{
			this._HeaderFooter = headerFooter;
			this.code = text;
		}

		private VmlDrawingPicture _LeftImag = null;
		public VmlDrawingPicture LeftImag
		{
			get
			{
				return this._LeftImag;
			}
			set
			{
				if (value == null)
				{
					this._LeftAlignedText.Replace(ExcelCommon.HeaderFooter.Image, null);
					this._LeftImag = value;
				}
				else
				{
					if (!this._LeftAlignedText.Contains(ExcelCommon.HeaderFooter.Image))
					{
						this._LeftAlignedText += ExcelCommon.HeaderFooter.Image;
					}
					this._LeftImag = value;
					this._LeftImag.Id = string.Concat("L", this.code);
				}

			}
		}

		private string _LeftAlignedText = string.Empty;
		/// <summary>
		/// 获取或设置页眉或页脚的左侧文本
		/// </summary>
		public string LeftAlignedText
		{
			get { return this._LeftAlignedText; }
			set
			{ this._LeftAlignedText = string.Concat(this._LeftAlignedText, value); }
		}

		private VmlDrawingPicture _CenteredImag = null;
		public VmlDrawingPicture CenteredImag
		{
			get
			{
				return this._CenteredImag;
			}
			set
			{
				if (value == null)
				{
					this._CenteredText.Replace(ExcelCommon.HeaderFooter.Image, null);
					this._CenteredImag = value;
				}
				else
				{
					if (!this._CenteredText.Contains(ExcelCommon.HeaderFooter.Image))
					{
						this._CenteredText = string.Concat(this._CenteredText, ExcelCommon.HeaderFooter.Image);
					}
					this._CenteredImag = value;
					this._CenteredImag.Id = string.Concat("C", this.code);
				}

			}
		}

		private string _CenteredText = string.Empty;
		/// <summary>
		///  获取或设置页眉或页脚的中间文本
		/// </summary>
		public string CenteredText
		{
			get { return this._CenteredText; }
			set { this._CenteredText = string.Concat(this._CenteredText, value); }
		}

		private VmlDrawingPicture _RightAlignedImag = null;
		public VmlDrawingPicture RightAlignedImag
		{
			get
			{
				return this._RightAlignedImag;
			}
			set
			{
				if (value == null)
				{
					this._RightAlignedText.Replace(ExcelCommon.HeaderFooter.Image, string.Empty);
					this._RightAlignedImag = value;
				}
				else
				{
					if (!this._RightAlignedText.Contains(ExcelCommon.HeaderFooter.Image))
					{
						this._RightAlignedText += ExcelCommon.HeaderFooter.Image;
					}
					this._RightAlignedImag = value;
					this._RightAlignedImag.Id = string.Concat("R", this.code);
				}

			}
		}

		private string _RightAlignedText = string.Empty;
		/// <summary>
		/// 获取或设置右侧文本
		/// </summary>
		public string RightAlignedText
		{
			get { return this._RightAlignedText; }
			set { this._RightAlignedText = string.Concat(this._RightAlignedText, value); }
		}
	}
}
