using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Office.OpenXml.Excel
{
	public sealed class TableCell : CellBase
	{
		internal TableCell()
		{ }

		internal TableCell Clone()
		{
			TableCell tc = new TableCell()
			{
				_Value = this._Value,
				_Style = this._Style
			};

			return tc;
		}

		/// <summary>
		/// 判断是否保存时有必要转换到Sheet Cell里。
		/// </summary>
		/// <returns></returns>
		internal bool IsToSheetCell()
		{
			if (this._Value != null)
				return true;

			if (this._Style != null)
				return true;

			return false;

		}
	}
}
