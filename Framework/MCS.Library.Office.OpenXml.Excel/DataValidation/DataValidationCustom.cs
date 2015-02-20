using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Office.OpenXml.Excel.DataValidation;

namespace MCS.Library.Office.OpenXml.Excel
{
	// IExcelDataValidationWithFormula<IExcelDataValidationFormula>, IExcelDataValidationWithOperator
	public interface IDataValidationCustom : IDataValidationWithFormula<IDataValidationFormula>, IDataValidationWithOperator
	{
	}

	public class DataValidationCustom : DataValidationWithFormula<IDataValidationFormula>, IDataValidationCustom
	{
		internal DataValidationCustom(string address, DataValidationType validationType)
			: base(address, validationType)
		{
			Formula = new DataValidationFormulaCustom();
		}
	}
}
