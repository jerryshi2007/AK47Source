using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Office.OpenXml.Excel.DataValidation;

namespace MCS.Library.Office.OpenXml.Excel
{
	public class DataValidationCollection : ExcelCollectionBase<Range, IDataValidation>
	{
		private WorkSheet WrokSheet;

		public DataValidationCollection(WorkSheet worksheet)
		{
			this.WrokSheet = worksheet;
		}

		public IDataValidationInt AddIntegerValidation(string address)
		{
			DataValidationInt item = new DataValidationInt(address, DataValidationType.Whole);
			this.Add(item);

			return item;
		}

		public IDataValidationInt AddTextLengthValidation(string address)
		{
			DataValidationInt item = new DataValidationInt(address, DataValidationType.TextLength);
			this.Add(item);

			return item;
		}

		public IDataValidationDecimal AddDecimalValidation(string address)
		{
			DataValidationDecimal item = new DataValidationDecimal(address, DataValidationType.Decimal);
			this.Add(item);

			return item;
		}

		public IDataValidationDateTime AddDateTimeValidation(string address)
		{
			DataValidationDateTime item = new DataValidationDateTime(address, DataValidationType.DateTime);
			this.Add(item);

			return item;
		}

		public IDataValidationTime AddTimeValidation(string address)
		{
			DataValidationTime item = new DataValidationTime(address, DataValidationType.Time);
			this.Add(item);

			return item;
		}

		public IDataValidationList AddListValidation(string address)
		{
			DataValidationList item = new DataValidationList(address, DataValidationType.List);
			this.Add(item);

			return item;
		}

		public IDataValidationCustom AddCustomValidation(string address)
		{
			DataValidationCustom item = new DataValidationCustom(address, DataValidationType.Custom);
			this.Add(item);

			return item;
		}

		public IDataValidation this[string address]
		{
			get
			{
				return base[Range.Parse(this.WrokSheet, address)];
			}
		}

		protected override Range GetKeyForItem(IDataValidation item)
		{
			return item.Address;
		}
	}
}
