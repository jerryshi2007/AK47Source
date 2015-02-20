using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MCS.Library.Data.Mapping;
using MCS.Library.Core;

namespace MCS.Library.Data.DataObjects
{
	/// <summary>
	/// Table中的行
	/// </summary>
	[Serializable]
	public abstract class TableRowBase<TColumnDefinition, TRowValue, TRowValueCollection, TValue>
		where TRowValue : RowValueBase<TColumnDefinition, TValue>
		where TRowValueCollection : RowValueCollectionBase<TColumnDefinition, TRowValue, TValue>, new()
		where TColumnDefinition : ColumnDefinitionBase
	{
		private TRowValueCollection _Values = default(TRowValueCollection);

		/// <summary>
		/// 行中的值集合
		/// </summary>
		[NoMapping]
		public virtual TRowValueCollection Values
		{
			get
			{
				if (this._Values == default(TRowValueCollection))
					this._Values = new TRowValueCollection();

				return this._Values;
			}
		}
	}

	/// <summary>
	/// Table行
	/// </summary>
	/// <typeparam name="TTableRow"></typeparam>
	/// <typeparam name="TColumnDefinition"></typeparam>
	/// <typeparam name="TRowValueCollection"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	/// <typeparam name="TRowValue"></typeparam>
	[Serializable]
	public abstract class TableRowCollectionBase<TTableRow, TColumnDefinition, TRowValue, TRowValueCollection, TValue> : EditableDataObjectCollectionBase<TTableRow>
		where TTableRow : TableRowBase<TColumnDefinition, TRowValue, TRowValueCollection, TValue>
		where TRowValue : RowValueBase<TColumnDefinition, TValue>
		where TRowValueCollection : RowValueCollectionBase<TColumnDefinition, TRowValue, TValue>, new()
		where TColumnDefinition : ColumnDefinitionBase
	{
		/// <summary>
		/// 转换为DataView
		/// </summary>
		/// <param name="columns"></param>
		/// <returns></returns>
		public virtual DataView ToDataView(ColumnDefinitionCollectionBase<TColumnDefinition> columns)
		{
			DataTable table = new DataTable();

			foreach (TColumnDefinition column in columns)
			{
				DataColumn dc = new DataColumn(column.Name, column.RealDataType);

				dc.DefaultValue = column.DefaultValue;
				dc.Caption = column.Caption;

				table.Columns.Add(dc);
			}

			foreach (TTableRow row in this)
			{
				DataRow dr = table.NewRow();

				foreach (TRowValue rv in row.Values)
				{
					TValue dataValue = row.Values.GetValue(rv.Column.Name, (TValue)rv.Column.DefaultValue);
					dr[rv.Column.Name] = DataConverter.ChangeType(dataValue, rv.Column.RealDataType);
				}

				table.Rows.Add(dr);
			}

			return table.DefaultView;
		}
	}
}
