using System;
using System.Collections.Generic;

namespace MCS.Library.Office.OpenXml.Excel
{
	[AttributeUsageAttribute(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
	public sealed class NotTableColumnAttribute : Attribute
	{
	}

	[AttributeUsageAttribute(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
	public sealed class TableColumnDescriptionAttribute : Attribute
	{
		private string _ColumnName;
		/// <summary>
		/// 列名
		/// </summary>
		public string ColumnName
		{
			get { return this._ColumnName; }
			set { this._ColumnName = value; }
		}

		private int _Index = default(int);
		/// <summary>
		/// 生成序号
		/// </summary>
		public int Index
		{
			get { return this._Index; }
			set { this._Index = value; }
		}

		private string _ColumnFormula;
		/// <summary>
		/// 公式
		/// </summary>
		public string ColumnFormula
		{
			get { return this._ColumnFormula; }
			set { this._ColumnFormula = value; }
		}

		public TableColumnDescriptionAttribute(string name, string formula, int index)
			: this(name)
		{
			this._ColumnFormula = formula;
			this._Index = index;
		}

		public TableColumnDescriptionAttribute(string name, string formula)
			: this(name)
		{
			this._ColumnFormula = formula;
		}

		public TableColumnDescriptionAttribute(string name, int index)
			: this(name)
		{
			this._Index = index;
		}

		public TableColumnDescriptionAttribute(string name)
		{
			this._ColumnName = name;
		}
	}

	[AttributeUsageAttribute(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
	public sealed class TableDescriptionAttribute : Attribute
	{
		private string _TableName;
		/// <summary>
		/// 表名
		/// </summary>
		public string TableName
		{
			get { return this._TableName; }
			set { this._TableName = value; }
		}

		private string _BeginAddress;
		/// <summary>
		/// 开始地址
		/// </summary>
		public string BeginAddress
		{
			get { return this._BeginAddress; }
			set { this._BeginAddress = value; }
		}

		public TableDescriptionAttribute(string tableName, string beginAddress)
		{
			this._TableName = tableName;
			this._BeginAddress = beginAddress;
		}
	}
}
