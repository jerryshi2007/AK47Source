using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.Office.OpenXml.Excel
{
	public class DefinedNameCollection : ExcelCollectionBase<string, DefinedName>
	{
		internal WorkSheet _Worksheet;

		public DefinedNameCollection(WorkSheet worksheet)
		{
			this._Worksheet = worksheet;
		}

		public DefinedName Add(string Address, string name, bool IsHidden = false)
		{
			DefinedName nameRang = new DefinedName(name, this._Worksheet)
			{
				Address = Range.Parse(this._Worksheet, Address),
				IsNameHidden = IsHidden
			};
			base.Add(nameRang);

			return nameRang;
		}

		internal void AddRangeByDescription(TableDescription tbDesp)
		{
			int col = tbDesp.BeginAddress.ColumnIndex;
			int row = tbDesp.BeginAddress.RowIndex;

			foreach (TableColumnDescription tcDesp in tbDesp.AllColumns)
			{
				DefinedName nameRang = new DefinedName(tcDesp.PropertyName, this._Worksheet) { Address = new Range() { _WorkSheet = this._Worksheet, StartRow = row, StartColumn = col, EndColumn = col, EndRow = row }, NameValue = tcDesp.ColumnName };
				base.Add(nameRang);
				col++;
			}
		}

		internal void AddRange(IEnumerable<DefinedName> listNameRange)
		{
			listNameRange.ForEach(nameRange =>
			{
				InnerDict.Add(nameRange.Name, nameRange);
			});
		}

		public new DefinedName this[string name]
		{
			get
			{
				if (base.ContainsKey(name))
				{
					return base[name];
				}
				else
				{
					return null;
				}
			}
		}

		protected override string GetKeyForItem(DefinedName item)
		{
			return item.Name;
		}
	}
}
