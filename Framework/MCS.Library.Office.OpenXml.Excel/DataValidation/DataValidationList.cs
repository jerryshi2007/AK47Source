using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Office.OpenXml.Excel.DataValidation;

namespace MCS.Library.Office.OpenXml.Excel
{
	public interface IDataValidationList : IDataValidationWithFormula<IDataValidationFormulaList>
	{
	}

	public class DataValidationList : DataValidationWithFormula<IDataValidationFormulaList>, IDataValidationList
	{
		internal DataValidationList(string address, DataValidationType validationType)
			: base(address, validationType)
		{
			Formula = new DataValidationFormulaList();
		}
	}
}
