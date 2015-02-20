using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Office.OpenXml.Excel.DataValidation;

namespace MCS.Library.Office.OpenXml.Excel
{
	public interface IDataValidationDecimal : IDataValidationWithFormula2<IDataValidationFormulaDecimal>, IDataValidationWithOperator
	{
	}

	public class DataValidationDecimal : DataValidationWithFormula2<IDataValidationFormulaDecimal>, IDataValidationDecimal
	{
		internal DataValidationDecimal(string address, DataValidationType validationType)
			: base(address, validationType)
		{
			Formula = new DataValidationFormulaDecimal();
			Formula2 = new DataValidationFormulaDecimal();
		}
	}
}
