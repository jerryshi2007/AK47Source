using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Office.OpenXml.Excel.DataValidation
{
	public interface IDataValidationFormula
	{
		string GetValueAsString();
		string Formula { get; set; }
	}

	public interface IDataValidationFormulaInt : IDataValidationFormulaWithValue<int?>
	{
	}

	public interface IDataValidationFormulaDecimal : IDataValidationFormulaWithValue<double?>
	{
	}

	public interface IDataValidationFormulaDateTime : IDataValidationFormulaWithValue<DateTime?>
	{
	}

	public interface IDataValidationFormulaTime : IDataValidationFormulaWithValue<TimeWrapper>
	{
	}

	public interface IDataValidationFormulaList : IDataValidationFormula
	{
		IList<string> Values { get; }
	}
}
