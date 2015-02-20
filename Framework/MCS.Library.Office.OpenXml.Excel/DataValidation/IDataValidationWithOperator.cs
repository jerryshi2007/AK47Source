using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Office.OpenXml.Excel.DataValidation
{
	public interface IDataValidationWithOperator
	{
		ExcelDataValidationOperator Operator { get; set; }
	}
}
