using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Office.OpenXml.Excel
{
	/// <summary>
	/// 公式表
	/// </summary>
	public class Formula
	{
		private ExcelCellFormulaType _FormulaType = ExcelCellFormulaType.Normal;

		//t
		/// <summary>
		/// 公式类型
		/// </summary>
		public ExcelCellFormulaType FormulaType
		{
			get { return this._FormulaType; }
			set { this._FormulaType = value; }
		}


		private string _FormulaValue = string.Empty;
		/// <summary>
		/// 公式
		/// </summary>
		public string FormulaValue
		{
			get { return this._FormulaValue; }
			set { this._FormulaValue = value; }
		}

		//aca 
		/// <summary>
		/// true表示 只适用于数组公式,应计算在整个阵列,
		/// </summary>
		public bool AlwaysCalculateArray { get; set; }

		//ref
		/// <summary>
		/// 引用地址
		/// </summary>
		public Range Address { get; set; }

		//dt2D 
		/// <summary>
		/// 二维数据表。仅适用于数据表功能。写单元格的数据表中的公式。
		/// </summary>
		public bool DataTable2D { get; set; }

		//dtr
		/// <summary>
		/// 一维数据表, 只适用于数据表功能 
		/// </summary>
		public bool DataTableRow { get; set; }

		//del1
		/// <summary>
		/// 适用于数据表中的公式而已。只在单元格数据表的计算公式。
		/// </summary>
		public bool Input1Deleted { get; set; }

		//del2
		/// <summary>
		/// 第二个输入单元格已被删除
		/// </summary>
		public bool Input2Deleted { get; set; }

		//r1
		/// <summary>
		/// 数据表的第一个输入单元格。仅适用于数据表数组TABLE()
		/// </summary>
		public string DataTableCell1 { get; set; }

		/// <summary>
		/// 数据表的第一个输入细胞。仅适用于数据表数组"TABLE()"写在表中，只计算主要单元格
		/// </summary>
		public string InputCell2 { get; set; }

		//si
		/// <summary>
		/// 只适用于 FormulaType Shared (组ID）
		/// </summary>
		public int SharedIndex { get; set; }

		//ca
		/// <summary>
		/// 下次运行，表示该公式要重新计算(例如:=RAND())
		/// </summary>
		public bool CalculateCell { get; set; }

		//bx
		/// <summary>
		/// 公式分配一个名称的值
		/// </summary>
		public bool AssignsValueToName { get; set; }
	}
}
