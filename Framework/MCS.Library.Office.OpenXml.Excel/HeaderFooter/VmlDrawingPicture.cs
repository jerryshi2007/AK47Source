using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using MCS.Library.Core;


namespace MCS.Library.Office.OpenXml.Excel
{
	public sealed class VmlDrawingPicture
	{
		internal string Id
		{
			get;
			set;
		}

		internal VmlDrawingPicture()
		{ }

		public VmlDrawingPicture(Image img)
		{
			this.Image = Image;
		}

		public VmlDrawingPicture(string name, Image img)
		{
			this.Title = name;
			this.Image = img;
		}

		public VmlDrawingPicture(string name, Image img, ImageFormat imageFormat)
		{
			this.Title = name;
			this.Image = img;
			this._ImageFormat = imageFormat;
		}


		//internal string Style
		//{
		//    get { return this._Style ;}
		//    set { this._Style = value; }
		//}

		private double _Width = 0;
		public double Width
		{
			get
			{
				return this._Width;
			}
			set
			{
				this._Width = value;
			}
		}

		private double _Height = 0;
		public double Height
		{
			get
			{
				return this._Height;
			}
			set
			{
				this._Height = value; ;
			}
		}

		public double _Left = 0;
		public double Left
		{
			get
			{
				return this._Left;
			}
			set
			{
				this._Left = value;
			}
		}

		public double _Top = 0;
		public double Top
		{
			get
			{
				return this._Top;
			}
			set
			{
				this._Top = value;
			}
		}

		public string Title
		{
			get;
			set;
		}

		public ImageFormat  _ImageFormat=ImageFormat.Jpeg;
		public ImageFormat ImageFormat
		{
		   get { return this._ImageFormat;}
		   set { this._ImageFormat =value ;}
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
				if (value != null)
				{
					this._Image = value;
					this._Width = this._Image.Width * 72 / this._Image.HorizontalResolution;
					this._Height = this._Image.Height * 72 / this._Image.VerticalResolution;
				}
				else
				{
					this._Image = value;
				}
			}
		}

		internal string RelId
		{
			get;
			set;
		}

		/// <summary>
		/// 定是否将图像显示黑色和白色
		/// </summary>
		public bool BiLevel
		{
			get;
			set;
		}

		/// <summary>
		/// 确定是否图片将显示在灰度模式
		/// </summary>
		public bool GrayScale
		{
			get;
			set;
		}

		private double _Gain = 1;
		/// <summary>
		/// 定义图像中的所有颜色的强度 默认值1
		/// </summary>
		public double Gain
		{
			get { return this._Gain; }
			set
			{
				(value < 0).TrueThrow<ArgumentOutOfRangeException>("定义图像中的所有颜色的强度必须大于0，Gain");
				this._Gain = value;
				this._Gamma = 0;
			}
		}

		private double _Gamma = 0;
		/// <summary>
		/// 定义图像的对比度
		/// </summary>
		public double Gamma
		{
			get { return this._Gamma; }
			set { this._Gamma = value; }
		}

		private double _BlackLevel = 0;
		/// <summary>
		/// 定义图像中的黑色力度 默认值为0
		/// </summary>
		public double BlackLevel
		{
			get { return this._BlackLevel; }
			set { this._BlackLevel = value; }
		}

		internal static double GetFracDT(string v, double def)
		{
			double d;
			if (v.EndsWith("f"))
			{
				v = v.Substring(0, v.Length - 1);
				if (double.TryParse(v, out d))
				{
					d /= 65535;
				}
				else
				{
					d = def;
				}
			}
			else
			{
				if (!double.TryParse(v, out d))
				{
					d = def;
				}
			}
			return d;
		}

		//private void SetStyleProp(string propertyName, string value)
		//{
		//    string style =this.Style;
		//    string newStyle = string.Empty;
		//    bool found = false;
		//    foreach (string prop in style.Split(';'))
		//    {
		//        string[] split = prop.Split(':');
		//        if (split[0] == propertyName)
		//        {
		//            newStyle += propertyName + ":" + value + ";";
		//            found = true;
		//        }
		//        else
		//        {
		//            newStyle += prop + ";";
		//        }
		//    }
		//    if (!found)
		//    {
		//        newStyle += propertyName + ":" + value + ";";
		//    }
		//    this.Style = newStyle;
		//}

		//private double GetStyleProp(string propertyName)
		//{
		//    string style = this.Style;
		//    foreach (string prop in style.Split(';'))
		//    {
		//        string[] split = prop.Split(':');
		//        if (split[0] == propertyName && split.Length > 1)
		//        {
		//            string value = split[1].EndsWith("pt") ? split[1].Substring(0, split[1].Length - 2) : split[1];
		//            double ret;
		//            if (double.TryParse(value, out ret))
		//            {
		//                return ret;
		//            }
		//            else
		//            {
		//                return 0;
		//            }
		//        }
		//    }
		//    return 0;
		//}

		internal string GetStyle(int index)
		{
			StringBuilder styleBuilder = new StringBuilder();
			styleBuilder.Append("position:absolute;");
			styleBuilder.Append("margin-left:0;");
			styleBuilder.Append("margin-top:0;");
			styleBuilder.AppendFormat("width:{0}pt;", this.Width);
			styleBuilder.AppendFormat("height:{0}pt;", this.Height);
			if (this._Left != 0)
			{
				styleBuilder.AppendFormat("left:{0}pt;", this.Left);
			}
			styleBuilder.AppendFormat("z-index:{0}", index);

			return styleBuilder.ToString();
		}

		//private string _Style = "position:absolute;margin-left:0;margin-top:0;width:32.25pt;height:32.25pt; z-index:5";
	}
}
