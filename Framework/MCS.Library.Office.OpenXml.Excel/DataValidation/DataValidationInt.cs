using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Office.OpenXml.Excel.DataValidation;

namespace MCS.Library.Office.OpenXml.Excel
{
	public interface IDataValidationInt : IDataValidationWithFormula2<IDataValidationFormulaInt>, IDataValidationWithOperator
	{

	}

	public class DataValidationInt : DataValidationWithFormula2<IDataValidationFormulaInt>, IDataValidationInt
	{
		internal DataValidationInt(string address, DataValidationType validationType)
			: base(address, validationType)
		{
			Formula = new DataValidationFormulaInt();
			Formula2 = new DataValidationFormulaInt();
		}

	}
}
