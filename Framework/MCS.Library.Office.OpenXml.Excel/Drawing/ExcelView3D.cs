using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.Office.OpenXml.Excel
{
	public sealed class ExcelView3D
	{
		public decimal Perspective
		{
			get;
			set; 
		}

		public decimal RotX
		{
			get;
			set; 
		}

		public decimal RotY
		{
			get;
			set; 
		}

		public bool RightAngleAxes
		{
			get;
			set;
		}

		private int _DepthPercent = int.MinValue;
		public int DepthPercent
		{
			get
			{
				return this._DepthPercent;
			}
			set
			{
				ExceptionHelper.TrueThrow<ArgumentOutOfRangeException>(value < 0 || value > 2000, "值必须介于0至2000之间");
				this._DepthPercent = value;
			}
		}

		private int _HeightPercent = int.MinValue ;
		public int HeightPercent
		{
			get
			{
				return this._HeightPercent;
			}
			set
			{
				ExceptionHelper.TrueThrow<ArgumentOutOfRangeException>(value < 5 || value > 500, "值必须介于5至500之间");
				this._HeightPercent = value;
			}
		}
	}
}
