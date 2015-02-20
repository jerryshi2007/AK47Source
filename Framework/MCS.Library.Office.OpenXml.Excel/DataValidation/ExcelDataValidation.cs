using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.Office.OpenXml.Excel.DataValidation
{
	public abstract class ExcelDataValidation : IDataValidation
	{
		internal ExcelDataValidation(string address, DataValidationType validationType)
		{
			this.Address = Range.Parse(address);
			this.ValidationType = validationType;
		}

		public Range Address
		{
			get;
			private set;
		}

		public DataValidationType ValidationType
		{
			get;
			private set;
		}

		private ExcelDataValidationWarningStyle _ErrorStyle = ExcelDataValidationWarningStyle.undefined;
		public ExcelDataValidationWarningStyle ErrorStyle
		{
			get
			{
				return this._ErrorStyle;
			}
			set
			{
				this._ErrorStyle = value;
			}
		}

		public bool? AllowBlank
		{
			get;
			set;
		}

		public bool? ShowInputMessage
		{
			get;
			set;
		}

		public bool? ShowErrorMessage
		{
			get;
			set;
		}

		public string ErrorTitle
		{
			get;
			set;
		}

		public string Error
		{
			get;
			set;
		}

		public string PromptTitle
		{
			get;
			set;
		}

		public string Prompt
		{
			get;
			set;
		}

		public bool AllowsOperator
		{
			get;
			private set;
		}

		public abstract void Validate();

		private ExcelDataValidationOperator _Operator;
		public ExcelDataValidationOperator Operator
		{
			get
			{
				return this._Operator;
			}
			set
			{
				//ExceptionHelper.FalseThrow<InvalidOperationException>(ValidationType.AllowOperator, "当前类型不允许设置");
				this._Operator = value;
			}
		}
	}


}
