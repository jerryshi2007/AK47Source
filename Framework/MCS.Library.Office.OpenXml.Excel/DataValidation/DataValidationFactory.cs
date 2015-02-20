using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Office.OpenXml.Excel.DataValidation;

namespace MCS.Library.Office.OpenXml.Excel
{
	internal static class DataValidationFactory
	{
		public static ExcelDataValidation Create(DataValidationType type, string address)
		{
			switch (type.Type)
			{
				case ExcelDataValidationType.TextLength:
				case ExcelDataValidationType.Whole:
					return new DataValidationInt(address, type);
				case ExcelDataValidationType.Decimal:
					return new DataValidationDecimal(address, type);
				case ExcelDataValidationType.List:
					return new DataValidationList(address, type);
				case ExcelDataValidationType.DateTime:
					return new DataValidationDateTime(address, type);
				case ExcelDataValidationType.Time:
					return new DataValidationTime(address, type);
				case ExcelDataValidationType.Custom:
					return new DataValidationCustom(address, type);
				default:
					throw new InvalidOperationException("不存在验证: " + type.Type.ToString());
			}
		}
	}
}
