using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using System.Globalization;

namespace MCS.Library.Office.OpenXml.Excel
{
	public struct Range
	{
		internal WorkSheet _WorkSheet;

		public static Range Parse(WorkSheet workSheet, int startColumn, int startRow, int endColumn, int endRow)
		{
			return new Range() { _WorkSheet = workSheet, StartRow = startRow, StartColumn = startColumn, EndRow = endRow, EndColumn = endColumn };
		}

		public static Range Parse(WorkSheet workSheet, string rangeAddress)
		{
			rangeAddress.IsNullOrEmpty().TrueThrow<ArgumentNullException>("Rang地址不能为空");
			int _StartRow, _StartColumn, _EndRow, _EndColumn;
			ExcelHelper.GetRowColFromAddress(rangeAddress, out _StartRow, out _StartColumn, out  _EndRow, out _EndColumn);
			return new Range() { _WorkSheet = workSheet, StartRow = _StartRow, StartColumn = _StartColumn, EndRow = _EndRow, EndColumn = _EndColumn };
		}

		public static Range Parse(string rangeAddress)
		{
			rangeAddress.IsNullOrEmpty().TrueThrow<ArgumentNullException>("Rang地址不能为空");
			int _StartRow, _StartColumn, _EndRow, _EndColumn;
			ExcelHelper.GetRowColFromAddress(rangeAddress, out _StartRow, out _StartColumn, out  _EndRow, out _EndColumn);

			return new Range() { StartRow = _StartRow, StartColumn = _StartColumn, EndRow = _EndRow, EndColumn = _EndColumn };
		}

		public static Range Parse(int startColumn, int startRow, int endColumn, int endRow)
		{
			return new Range() { StartRow = startRow, StartColumn = startColumn, EndRow = endRow, EndColumn = endColumn };
		}

		private int _StartRow;

		public int StartRow
		{
			get
			{
				if (this._StartRow == default(int))
				{
					return -1;
				}
				return this._StartRow;
			}
			set
			{
				this._StartRow = value;
			}
		}

		private int _StartColumn;

		public int StartColumn
		{
			get
			{
				if (this._StartColumn == default(int))
				{
					return -1;
				}

				return this._StartColumn;
			}
			set
			{
				this._StartColumn = value;
			}
		}

		private int _EndRow;
		public int EndRow
		{
			get
			{
				if (this._EndRow == default(int))
				{
					return -1;
				}


				return this._EndRow;
			}
			set
			{
				this._EndRow = value;
			}
		}

		private int _EndColumn;

		public int EndColumn
		{
			get
			{
				if (this._EndColumn == default(int))
				{
					return -1;
				}

				return this._EndColumn;
			}
			set
			{
				this._EndColumn = value;
			}
		}


		/// <summary>
		/// 转换在NameRange地址
		/// </summary>
		/// <returns></returns>
		public string ToDefinedNameAddress()
		{
			StringBuilder result = new StringBuilder();
			//if (!string.IsNullOrEmpty(this._SheetNme))
			if (this._WorkSheet != null)
			{
				result.Append("'");
				result.Append(this._WorkSheet.Name);
				result.Append("'!$");
			}
			result.Append(ExcelHelper.GetColumnLetterFromNumber(this.StartColumn));
			result.Append("$");
			result.Append(this.StartRow.ToString(CultureInfo.InvariantCulture));
			if (this.StartColumn != this.EndColumn || this.StartRow != this.EndRow)
			{
				result.Append(":");
				result.Append("$");
				result.Append(ExcelHelper.GetColumnLetterFromNumber(this.EndColumn));
				result.Append("$");
				result.Append(this.EndRow.ToString(CultureInfo.InvariantCulture));
			}

			return result.ToString();
		}

		/// <summary>
		/// 包换工作表名称地址
		/// </summary>
		/// <returns></returns>
		public string ToFullAddress()
		{
			StringBuilder result = new StringBuilder();
			if (this._WorkSheet != null)
			{
				result.Append(this._WorkSheet.Name);
				result.Append("!");
			}
			if (this.StartColumn != -1 || this.StartRow != -1)
			{
				result.Append(ExcelHelper.GetColumnLetterFromNumber(this.StartColumn));
				result.Append(this.StartRow.ToString(CultureInfo.InvariantCulture));
				if (this.StartColumn != this.EndColumn || this.StartRow != this.EndRow || this._EndColumn != -1 || this._EndRow != -1)
				{
					result.Append(":");
					result.Append(ExcelHelper.GetColumnLetterFromNumber(this.EndColumn));
					result.Append(this.EndRow.ToString(CultureInfo.InvariantCulture));
				}
			}

			return result.ToString();
		}

		/// <summary>
		/// 不包函工作薄名称
		/// </summary>
		/// <returns></returns>
		public string ToAddress()
		{
			StringBuilder result = new StringBuilder();
			if (this.StartColumn != -1 || this.StartRow != -1)
			{
				result.Append(ExcelHelper.GetColumnLetterFromNumber(this.StartColumn));
				result.Append(this.StartRow.ToString(CultureInfo.InvariantCulture));

				if (this.StartColumn != this.EndColumn || this.StartRow != this.EndRow || this._EndColumn != -1 || this._EndRow != -1)
				{
					result.Append(":");
					result.Append(ExcelHelper.GetColumnLetterFromNumber(this.EndColumn));
					result.Append(this.EndRow.ToString(CultureInfo.InvariantCulture));
				}

			}

			return result.ToString();
		}

		/// <summary>
		/// 转换成地址
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			StringBuilder result = new StringBuilder();
			if (this._WorkSheet != null)
			{
				result.Append(this._WorkSheet.Name);
				result.Append("!$");
			}
			if (this.StartColumn != -1 || this.StartRow != -1)
			{
				result.Append(ExcelHelper.GetColumnLetterFromNumber(this.StartColumn));
				result.Append("$");
				result.Append(this.StartRow.ToString(CultureInfo.InvariantCulture));
				if (this.StartColumn != this.EndColumn || this.StartRow != this.EndRow || this._EndColumn != -1 || this._EndRow != -1)
				{
					result.Append(":");
					result.Append("$");
					result.Append(ExcelHelper.GetColumnLetterFromNumber(this.EndColumn));
					result.Append("$");
					result.Append(this.EndRow.ToString(CultureInfo.InvariantCulture));
				}
			}

			return result.ToString();
		}

		public static bool operator !=(Range R1, Range R2)
		{
			return !SelfCompare(R1, R2);
		}

		public static bool operator ==(Range R1, Range R2)
		{
			return SelfCompare(R1, R2);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Range))
			{
				return false;
			}

			return SelfCompare((Range)obj, this);
		}

		public override int GetHashCode()
		{
			return string.Format("{0}|{1}|{2}|{3}", this.StartColumn, this.StartRow, this.EndColumn, this.EndRow).GetHashCode();
		}

		private static bool SelfCompare(Range R1, Range R2)
		{
			return ((R1.EndRow == R2.EndRow) && (R1.EndColumn == R2.EndColumn) && (R1.StartColumn == R2.StartColumn) && (R1.StartRow == R2.StartRow) && (R1.StartRow > 0) && (R1.StartColumn > 0) && (R2.EndRow > 0) && (R1.EndColumn > 0));
		}

		/// <summary>
		/// 验证是否是合法Range
		/// </summary>
		/// <returns></returns>
		public bool ValidatorRange()
		{
			bool reslut = true;

			if (this.StartRow > 0 && this.StartColumn > 0 && this.EndColumn >= this.StartColumn && this.EndRow >= this.StartRow)
			{
				reslut = true;
			}

			return reslut;
		}

	}
}
