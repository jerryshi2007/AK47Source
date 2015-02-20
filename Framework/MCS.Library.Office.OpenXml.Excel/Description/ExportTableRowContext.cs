using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Office.OpenXml.Excel
{
	/// <summary>
	/// 导出数据返回值
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class ExportTableRowContext<T> where T : class
	{
		private T _CurrentObject;

		/// <summary>
		/// 当前对象定义
		/// </summary>
		public T CurrentObject
		{
			get
			{
				return this._CurrentObject;
			}
		}

		public ExportTableRowContext(T objcurrent)
		{
			this._CurrentObject = objcurrent;
		}

		/// <summary>
		/// 当前工作表名称
		/// </summary>
		public string CurrentTableName
		{
			get;
			internal set;
		}

		private Dictionary<string, string> _PropertyCellAddressMapping;
		/// <summary>
		///  属性与工作表地址映射关系
		/// </summary>
		public Dictionary<string, string> PropertyCellAddressMapping
		{
			get
			{
				if (this._PropertyCellAddressMapping == null)
					this._PropertyCellAddressMapping = new Dictionary<string, string>();

				return this._PropertyCellAddressMapping;
			}
		}
	}

	/// <summary>
	/// 验证错误时的后续操作模式
	/// </summary>
	public enum ValidationErrorStopMode
	{
		/// <summary>
		/// 不再进下一行获取
		/// </summary>
		Stop,

		/// <summary>
		/// 跳过
		/// </summary>
		Continue
	}

	public sealed class ExcportRowContext
	{
		/// <summary>
		/// 行号
		/// </summary>
		public int RowIndex { get; internal set; }

		private ExportRowCellCollection _PropertyDescriptions = null;
		public ExportRowCellCollection PropertyDescriptions
		{
			get
			{
				if (this._PropertyDescriptions == null)
					this._PropertyDescriptions = new ExportRowCellCollection();

				return this._PropertyDescriptions;
			}
		}
	}

	public sealed class ExportRowResult<T>
	{
		private T _CurrentObject;

		/// <summary>
		/// 当前对象定义
		/// </summary>
		public T CurrentObject
		{
			get
			{
				return this._CurrentObject;
			}
		}

		/// <summary>
		/// 当处理数据集合用这个
		/// </summary>
		/// <param name="obj"></param>
		public ExportRowResult(T obj)
		{
			this._CurrentObject = obj;
		}

		public ExportRowResult()
		{
		}

		/// <summary>
		/// 已经成功验证过的
		/// </summary>
		public bool Validated { get; set; }

		private StringBuilder _ErrorLog = new StringBuilder();

		/// <summary>
		/// 当前条件错误日制记录
		/// </summary>
		public StringBuilder ErrorLog
		{
			get
			{
				return this._ErrorLog;
			}
		}
	}

	public sealed class ExportCellDescription
	{
		public ExportCellDescription(string propertyName)
		{
			this.PropertyName = propertyName;
		}

		public string TableColumnName { get; internal set; }

		public string PropertyName { get; internal set; }

		public string Address { get; internal set; }

		public Object Value { get; internal set; }
	}

	public class ExportRowCellCollection : ExcelCollectionBase<string, ExportCellDescription>
	{
		protected override string GetKeyForItem(ExportCellDescription item)
		{
			return item.PropertyName;
		}
	}

	public class CreatingDataCellParameters<T>
	{
		public T CurrentObj { get; internal set; }

		public Object PropertyValue { get; internal set; }

		public string ColumnName { get; internal set; }

		public int RowIndex { get; internal set; }

		public CreatingDataCellParameters(T currentObj, object propertyValue)
		{
			this.CurrentObj = currentObj;
			this.PropertyValue = propertyValue;
		}

		public CreatingDataCellParameters(T currentObj, object propertyValue, string columnName, int rowIndex)
			: this(currentObj, propertyValue)
		{
			this.ColumnName = columnName;
			this.RowIndex = rowIndex;
		}
	}

	public delegate void CreatingDataCellAction<T>(CellBase cell, CreatingDataCellParameters<T> parameters);

	public delegate T GetEmptyObjEntity<T>();

	public delegate ExportRowResult<T> ExportTableRow<T>(ExcportRowContext rowContext);

	public delegate void SetObjEntityPropertyValue<T>(ref T objEntity, Object propertyValue, string columnName, string propertyName);

	/// <summary>
	/// 将数据转换成Collection 参数列表
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public sealed class SpreadGetTableCollectionParams<T>
	{
		private GetEmptyObjEntity<T> _IintObj = null;
		/// <summary>
		/// 初始化每一个对象
		/// </summary>
		public GetEmptyObjEntity<T> IintObj
		{
			get
			{
				return this._IintObj;
			}
		}

		/// <summary>
		/// 设置对象的每一个属性值
		/// </summary>
		private SetObjEntityPropertyValue<T> _SetObjProperty = null;
		/// <summary>
		/// 设置对象的每一个属性值
		/// </summary>
		public SetObjEntityPropertyValue<T> SetObjProperty
		{
			get
			{
				return this._SetObjProperty;
			}
		}

		public SpreadGetTableCollectionParams(GetEmptyObjEntity<T> initObj, SetObjEntityPropertyValue<T> setProperty)
		{
			this._IintObj = initObj;
			this._SetObjProperty = setProperty;
		}

		public SpreadGetTableCollectionParams(ExportTableRow<T> exportRow, ValidationErrorStopMode validationOperator)
		{
			this._ValidationOperator = validationOperator;
			this._ExportRow = exportRow;
		}

		private ExportTableRow<T> _ExportRow = null;
		/// <summary>
		/// 导出行的方法。
		/// </summary>
		public ExportTableRow<T> ExportRow
		{
			get
			{
				return this._ExportRow;
			}
		}

		private ValidationErrorStopMode _ValidationOperator = ValidationErrorStopMode.Continue;

		/// <summary>
		/// 当验证不通过时，是否继续
		/// </summary>
		public ValidationErrorStopMode ValidationOperator
		{
			get
			{
				return this._ValidationOperator;
			}
		}
	}

}
