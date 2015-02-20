using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Office.OpenXml.Excel.DataValidation;

namespace MCS.Library.Office.OpenXml.Excel
{
	public interface IDataValidationTime : IDataValidationWithFormula2<IDataValidationFormulaTime>, IDataValidationWithOperator
	{

	}

	public class DataValidationTime : DataValidationWithFormula2<IDataValidationFormulaTime>, IDataValidationTime
	{
		internal DataValidationTime(string address, DataValidationType validationType)
			: base(address, validationType)
		{
			Formula = new DataValidationFormulaTime();
			Formula2 = new DataValidationFormulaTime();
		}
	}
}
