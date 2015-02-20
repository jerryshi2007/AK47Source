using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using System.Text.RegularExpressions;

namespace MCS.Library.Office.OpenXml.Excel
{
	public struct CellAddress
	{
		public static readonly Regex cellAddress = new Regex(@"(\$)?([A-Z]+)(\$)?([0-9]+)" +
							@"(:((\$)?([A-Z]+)(\$)?([0-9]+)))?$", RegexOptions.Compiled);
		//public CellAddress(string name)
		//{
		//    if (string.IsNullOrEmpty(name))
		//    {
		//        throw new ArgumentNullException("CellAddress Name不能为空");
		//    }

		//    Name = name;
		//    RowIndex = 0;
		//    ColumnIndex = 0;
		//    ParseName();
		//}

		public static implicit operator CellAddress(string address)
		{
			return CellAddress.Parse(address);
		}

		public static CellAddress Parse(string address)
		{
			Match cellMatch = cellAddress.Match(address);
			ExceptionHelper.FalseThrow(cellMatch.Success, "不符合Excel地址规范！{0}", address);
			//, Name = address 
			return new CellAddress() { RowIndex = int.Parse(cellMatch.Groups[4].Value), ColumnIndex = ExcelHelper.ColumnAddressToIndex(cellMatch.Groups[2].Value) };
		}

		public static CellAddress Parse(int columnIndex, int rowIndex)
		{
			ExceptionHelper.TrueThrow<ApplicationException>(rowIndex <= 0 || rowIndex > ExcelCommon.WorkSheet_MaxRows, "行索引超出Excel范围！{0}", rowIndex);
			ExceptionHelper.TrueThrow<ApplicationException>(columnIndex < 1 || columnIndex > ExcelCommon.WorkSheet_MaxColumns, string.Format("不存指定的列名 '{0}'.", columnIndex));

			return new CellAddress() { RowIndex = rowIndex, ColumnIndex = columnIndex };
		}

		//public string Name;

		public int RowIndex { get; set; }

		public int ColumnIndex { get; set; }

		/// <summary>
		/// 返回地址
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("{0}{1}", ExcelHelper.GetColumnLetterFromNumber(this.ColumnIndex), this.RowIndex);
		}

		//private static void ParseName()
		//{
		//    try
		//    {
		//        ExcelHelper.GetRowCol(this.address, out this.RowIndex, out this.ColumnIndex, true);
		//    }
		//    catch (Exception ex)
		//    {
		//        throw new Exception("CellAddress Name解析错误", ex);
		//    }
		//}
	}
}
