using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.Office.OpenXml.Excel.DataValidation
{
	internal abstract class BaseDataValidationFormula
	{
		protected ExcelDataValidationFormulaState State
		{
			get;
			set;
		}

		private string _Formula;
		public string Formula
		{
			get
			{
				return this._Formula;
			}
			set
			{
				ExceptionHelper.TrueThrow<InvalidOperationException>(!string.IsNullOrEmpty(value) && value.Length > 255, "一个DataValidation公式的长度不能超过255个字符");
				ResetValue();
				State = ExcelDataValidationFormulaState.Formula;
				this._Formula = value;
			}
		}

		internal abstract void ResetValue();

		//internal abstract string GetValueAsString();
	}

	internal abstract class BaseDataValidationFormulaValue<T> : BaseDataValidationFormula
	{
		private T _Value;
		public T Value
		{
			get
			{
				return _Value;
			}
			set
			{
				State = ExcelDataValidationFormulaState.Value;
				this._Value = value;
			}
		}

		internal override void ResetValue()
		{
			Value = default(T);
		}
	}
}
