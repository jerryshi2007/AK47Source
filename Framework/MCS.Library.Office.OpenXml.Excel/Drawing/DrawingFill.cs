using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using MCS.Library.Core;

namespace MCS.Library.Office.OpenXml.Excel
{
	public sealed class DrawingFill
	{
		private ExcelDrawingFillStyle _FillStyle = ExcelDrawingFillStyle.SolidFill;
		/// <summary>
		/// 目前只实现了 NoFill,SolidFill
		/// </summary>
		public ExcelDrawingFillStyle FillStyle
		{
			get
			{
				return this._FillStyle;
			}
			set
			{
				if (value == ExcelDrawingFillStyle.NoFill || value == ExcelDrawingFillStyle.SolidFill)
				{
					this._FillStyle = value;
				}
			}
		}

		private Color _Color = Color.FromArgb(79, 129, 189);
		public Color Color
		{
			get { return this._Color; }
			set 
			{
				this._FillStyle = ExcelDrawingFillStyle.SolidFill;
				//ExceptionHelper.TrueThrow(this._Style != ExcelDrawingFillStyle.SolidFill,"请先将Sytle设置成SolidFill");
				this._Color = value; 
			}
		}

		public int Transparancy
		{
			get;
			set;
		}
	}
}
