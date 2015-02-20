using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using MCS.Library.Core;

namespace MCS.Library.Office.OpenXml.Excel
{
	public class ExcelPicture : ExcelDrawing
	{
		internal ExcelPicture(WorkSheet worksheet)
			: base(worksheet)
		{

		}

		private ImageFormat _ImageFormat = ImageFormat.Jpeg;
		public ImageFormat ImageFormat
		{
			get { return this._ImageFormat; }
			set { this._ImageFormat = value; }
		}

		private Image _Image;
		public Image Image
		{
			get
			{
				return this._Image;
			}
			set
			{
				this._Image = value;
				this.ContentType = string.Empty;
			}
		}

		internal string ContentType
		{
			get;
			set;
		}

		private ExcelDrawingFillStyle _Style = ExcelDrawingFillStyle.SolidFill;
		public ExcelDrawingFillStyle Style
		{
			get { return this._Style; }
			set { this._Style = value; }
		}

		internal DrawingBorder _Border;
		public DrawingBorder Border
		{
			get
			{
				if (this._Border == null)
				{
					this._Border = new DrawingBorder();
				}

				return this._Border;
			}
		}

		internal DrawingFill _Fill;
		public DrawingFill Fill
		{
			get
			{
				if (this._Fill == null)
				{
					this._Fill = new DrawingFill();
				}

				return this._Fill;
			}
		}


		internal void SetPosDefaults(Image image)
		{
			SetPixelWidth(image.Width, image.HorizontalResolution);
			SetPixelHeight(image.Height, image.VerticalResolution);
		}


		//public void testReadStyle()
		//{ 
		//  EnumItemDescriptionAttribute.GetDescription((ExcelDraiwingFillStyle)typeCode));
		//}
	}
}
