using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Office.OpenXml.Excel
{
	public sealed class Row : IIndex
	{
		//internal static readonly double _DefaultRowHeight = 15;

		public Row() { }

		public Row(int rowid)
		{
			this.Index = rowid;
		}

		/// <summary>
		/// 表示当前是否为隐藏行
		/// </summary>
		public bool Hidden
		{
			get;
			set;
		}

		private double _Height = ExcelCommon.WorkSheet_DefaultRowHeight;
		/// <summary>
		/// 获取设置行高
		/// </summary>
		public double Height
		{
			get
			{
				return this._Height;
			}
			set
			{
				this._Height = value;
				if (Hidden && value != 0)
				{
					Hidden = false;
				}
			}
		}

		/// <summary>
		/// 行索引
		/// </summary>
		public int Index
		{
			get;
			set;
		}

		public bool Collapsed
		{
			get;
			set;
		}

		public int OutlineLevel
		{
			get;
			set;
		}

		public bool Phonetic
		{
			get;
			set;
		}

		private string _StyleName;
		/// <summary>
		/// 获取样式名称
		/// </summary>
		public string StyleName
		{
			get
			{
				return this._StyleName;
			}
			set
			{
				// this._StyleId = this._Worksheet.Workbook.Styles.GetStyleIdFromName(value);
				this._StyleName = value;
			}
		}

		private int _StyleId = 0;
		/// <summary>
		/// 设置行的样式ID
		/// </summary>
		public int StyleID
		{
			get
			{
				return this._StyleId;
			}
			set
			{
				this._StyleId = value;
			}
		}

		internal CellStyleXmlWrapper _Style;
		public CellStyleXmlWrapper Style
		{
			get
			{
				if (this._Style == null)
					this._Style = new CellStyleXmlWrapper();

				return this._Style;
			}
			set
			{
				this._Style = value;
			}
		}

		public Row Clone(int rowIndex)
		{
			return new Row(rowIndex) { Hidden = this.Hidden, _Height = this._Height, _Style = this._Style, _StyleId = this._StyleId, _StyleName = this._StyleName, Phonetic = this.Phonetic, Collapsed = this.Collapsed, OutlineLevel = this.OutlineLevel };
		}

		public void CopyTo(Row row)
		{
			row.Hidden = this.Hidden;
			row.Height = this._Height;
			row.Style = this._Style;
			row.StyleID = this._StyleId;
			row.StyleName = this._StyleName;
			row.Phonetic = this.Phonetic;
			row.Collapsed = this.Collapsed;
			row.OutlineLevel = this.OutlineLevel;
		}
	}
}
