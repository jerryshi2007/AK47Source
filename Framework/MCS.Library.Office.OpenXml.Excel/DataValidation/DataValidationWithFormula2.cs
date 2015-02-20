using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Office.OpenXml.Excel.DataValidation
{
	public class DataValidationWithFormula<T> : ExcelDataValidation
		where T : IDataValidationFormula
	{
		internal DataValidationWithFormula(string address, DataValidationType validationType): base(address,validationType)
		{ 
		 
		}

		public T Formula
		{
			get;
			internal set;
		}

		public override void Validate()
		{
		//    base.Validate();
		}
	}

	public class DataValidationWithFormula2<T> : DataValidationWithFormula<T>
		   where T : IDataValidationFormula
	{

		internal DataValidationWithFormula2(string address, DataValidationType validationType)
			: base(address, validationType)
		{ 
		}

		public T Formula2
		{
			get;
			internal set;
		}
	}
}
