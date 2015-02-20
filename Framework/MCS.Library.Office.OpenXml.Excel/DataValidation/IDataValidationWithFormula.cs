using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Office.OpenXml.Excel.DataValidation
{
	public interface IDataValidationWithFormula<T> : IDataValidation
	   where T : IDataValidationFormula
	{
		T Formula { get; }
	}

	public interface IDataValidationWithFormula2<T> : IDataValidationWithFormula<T>
		where T : IDataValidationFormula
	{
		T Formula2 { get; }
	}

	public interface IDataValidationFormulaWithValue<T> : IDataValidationFormula
	{
		T Value { get; set; }
	}
}
