using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Office.OpenXml.Excel.DataValidation;

namespace MCS.Library.Office.OpenXml.Excel
{
	public interface IDataValidationDateTime : IDataValidationWithFormula2<IDataValidationFormulaDateTime>, IDataValidationWithOperator
	{
	}

	public class DataValidationDateTime : DataValidationWithFormula2<IDataValidationFormulaDateTime>, IDataValidationDateTime
	{
		internal DataValidationDateTime(string address, DataValidationType validationType)
			: base(address, validationType)
		{
			Formula = new DataValidationFormulaDateTime();
			Formula2 = new DataValidationFormulaDateTime();
		}
	}
}
