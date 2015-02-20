using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MCS.Library.Core;

namespace MCS.Library.Data.DataObjects
{
	/// <summary>
	/// 基于Table的数据类型定义
	/// </summary>
	[Serializable]
	public abstract class TableBase<TTableRow, TRowValue, TRowValueCollection, TTableRowCollection, TValue, TColumnDefinition, TColumnDefinitionCollection>
		where TColumnDefinition : ColumnDefinitionBase
		where TColumnDefinitionCollection : ColumnDefinitionCollectionBase<TColumnDefinition>, new()
		where TTableRow : TableRowBase<TColumnDefinition, TRowValue, TRowValueCollection, TValue>, new()
		where TRowValue : RowValueBase<TColumnDefinition, TValue>
		where TRowValueCollection : RowValueCollectionBase<TColumnDefinition, TRowValue, TValue>, new()
		where TTableRowCollection : TableRowCollectionBase<TTableRow, TColumnDefinition, TRowValue, TRowValueCollection, TValue>, new()
	{
		private TColumnDefinitionCollection _Columns = default(TColumnDefinitionCollection);
		private TTableRowCollection _Rows = default(TTableRowCollection);

		/// <summary>
		/// 行信息
		/// </summary>
		public TTableRowCollection Rows
		{
			get
			{
				if (this._Rows == default(TTableRowCollection))
					this._Rows = new TTableRowCollection();

				return this._Rows;
			}
		}

		/// <summary>
		/// 列定义
		/// </summary>
		public TColumnDefinitionCollection Columns
		{
			get
			{
				if (this._Columns == default(TColumnDefinitionCollection))
					this._Columns = new TColumnDefinitionCollection();

				return this._Columns;
			}
		}

		/// <summary>
		/// 转换到DataView类型
		/// </summary>
		/// <returns></returns>
		public virtual DataView ToDataView()
		{
			return this.Rows.ToDataView(this.Columns);
		}
	}
}
