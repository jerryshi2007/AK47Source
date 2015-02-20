using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Office.OpenXml.Excel
{
	internal class Formulas
	{
		//aca  (只适用与数组公式) Always Calculate Array  bool
		//bx (Assigns Value to Name) (指定一个公式分配名称的值） bool
		internal string Ca { get; set; }//(Calculate Cell) (表是这个公式需要重新计算 RAND() )  bool
		//del1 (Input 1 Deleted) bool
		//del2 (Input 2 Deleted) bool
		internal string Dt2D { get; set; }//(Data Table 2-D) 二维数据表。仅适用于数据表功能。写在单元格的数据表中的公式。
		internal string Dtr { get; set; }//(Data Table Row) 如果为True -维数据表，只适用于一行或一列。 
		internal string R1 { get; set; }//(Data Table Cell 1)
		internal string R2 { get; set; }//(Input Cell 2)
		//ref (Range of Cells) (共享公式中,指定单元格地址范围,
		//si (Shared Group Index) (共享公式索引）
		//t 类型 FormulasType

		internal int Index { get; set; }
		internal Range Address { get; set; }
		internal bool IsArray { get; set; }
		internal string Formula { get; set; }
		//internal int StartRow { get; set; }
		//internal int StartCol { get; set; }
	}

}
