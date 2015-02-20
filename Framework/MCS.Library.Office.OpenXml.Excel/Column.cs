using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Office.OpenXml.Excel
{
	public class Column : IIndex
	{
		public Column() { }
		public Column(int columnindex)
		{
			this.Index = columnindex;
		}

		public int Index { get; set; }

		/// <summary>
		/// 设置是否为隐藏列
		/// </summary>
		private bool _Hidden = false;
		public bool Hidden
		{
			get
			{
				return this._Hidden;
			}
			set
			{
				this._Hidden = value;
			}
		}

		/// <summary>
		/// 获取或设置列的宽度
		/// </summary>
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
				if (this._Hidden && value != 0)
				{
					this._Hidden = false;
				}
			}
		}

		internal double VisualWidth
		{
			get
			{
				if (this._Hidden || (Collapsed && OutlineLevel > 0))
				{
					return 0;
				}
				else
				{
					return this._Width;
				}
			}
		}

		public bool BestFit
		{
			get;
			set;
		}

		public bool Collapsed { get; set; }
		public int OutlineLevel { get; set; }
		public bool Phonetic { get; set; }

		internal int _ColumnMax;
		public int ColumnMax
		{
			get { return this._ColumnMax; }
			set
			{
				this._ColumnMax = value;
			}
		}

		///// <summary>
		///// 是否生成列
		///// </summary>
		//public bool IsBuild { get; set; }

		/// <summary>
		/// 获取或设置第一列
		/// </summary>
		private int _ColumnMin;
		public int ColumnMin
		{
			get { return this._ColumnMin; }
		}

		private int _StyleID = 0;
		internal int StyleID
		{
			get
			{
				return this._StyleID;
			}
			set
			{
				this._StyleID = value;
			}
		}

		internal protected CellStyleXmlWrapper _Style;
		public CellStyleXmlWrapper Style
		{
			get
			{
				if (this._Style == null)
				{
					this._Style = new CellStyleXmlWrapper();
				}
				return this._Style;
			}
			set
			{
				this._Style = value;
			}
		}

		public Column Clone(int columnIndex)
		{
			return new Column(columnIndex) { _ColumnMax = this._ColumnMax, _Style = this._Style, _ColumnMin = this._ColumnMin, _Hidden = this._Hidden, _StyleID = this._StyleID, _Width = this._Width };
		}

	}
}
