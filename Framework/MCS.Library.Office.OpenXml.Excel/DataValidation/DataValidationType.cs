using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Office.OpenXml.Excel
{
	public class DataValidationType
	{
		private DataValidationType(ExcelDataValidationType validationType, bool allowOperator, string schemaName)
		{
			this.Type = validationType;
			this.SchemaName = schemaName;
			this.AllowOperator = allowOperator;
		}


		internal bool AllowOperator
		{
			get;
			private set;
		}

		public ExcelDataValidationType Type
		{
			get;
			private set;
		}

		internal string SchemaName
		{
			get;
			private set;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is DataValidationType))
			{
				return false;
			}
			return ((DataValidationType)obj).Type == Type;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		private static DataValidationType _Whole;
		/// <summary>
		/// 整数
		/// </summary>
		public static DataValidationType Whole
		{
			get
			{
				if (_Whole == null)
				{
					_Whole = new DataValidationType(ExcelDataValidationType.Whole, true, ExcelCommon.DataValidationSchemaNames.Whole);
				}
				return _Whole;
			}
		}


		private static DataValidationType _List;
		/// <summary>
		/// 列表
		/// </summary>
		public static DataValidationType List
		{
			get
			{
				if (_List == null)
				{
					_List = new DataValidationType(ExcelDataValidationType.List, false, ExcelCommon.DataValidationSchemaNames.List);
				}
				return _List;
			}
		}

		private static DataValidationType _decimal;
		/// <summary>
		/// 小数
		/// </summary>
		public static DataValidationType Decimal
		{
			get
			{
				if (_decimal == null)
				{
					_decimal = new DataValidationType(ExcelDataValidationType.Decimal, true, ExcelCommon.DataValidationSchemaNames.Decimal);
				}
				return _decimal;
			}
		}

		private static DataValidationType _TextLength;
		public static DataValidationType TextLength
		{
			get
			{
				if (_TextLength == null)
				{
					_TextLength = new DataValidationType(ExcelDataValidationType.TextLength, true, ExcelCommon.DataValidationSchemaNames.TextLength);
				}
				return _TextLength;
			}
		}

		private static DataValidationType _DateTime;
		public static DataValidationType DateTime
		{
			get
			{
				if (_DateTime == null)
				{
					_DateTime = new DataValidationType(ExcelDataValidationType.DateTime, true, ExcelCommon.DataValidationSchemaNames.Date);
				}
				return _DateTime;
			}
		}

		private static DataValidationType _Time;
		public static DataValidationType Time
		{
			get
			{
				if (_Time == null)
				{
					_Time = new DataValidationType(ExcelDataValidationType.Time, true, ExcelCommon.DataValidationSchemaNames.Time);
				}
				return _Time;
			}
		}

		private static DataValidationType _Custom;
		/// <summary>
		/// 自定义
		/// </summary>
		public static DataValidationType Custom
		{
			get
			{
				if (_Custom == null)
				{
					_Custom = new DataValidationType(ExcelDataValidationType.Custom, false, ExcelCommon.DataValidationSchemaNames.Custom);
				}
				return _Custom;
			}
		}

		internal static DataValidationType GetBySchemaName(string schemaName)
		{
			switch (schemaName)
			{
				case ExcelCommon.DataValidationSchemaNames.Whole:
					return DataValidationType.Whole;
				case ExcelCommon.DataValidationSchemaNames.Decimal:
					return DataValidationType.Decimal;
				case ExcelCommon.DataValidationSchemaNames.List:
					return DataValidationType.List;
				case ExcelCommon.DataValidationSchemaNames.TextLength:
					return DataValidationType.TextLength;
				case ExcelCommon.DataValidationSchemaNames.Date:
					return DataValidationType.DateTime;
				case ExcelCommon.DataValidationSchemaNames.Time:
					return DataValidationType.Time;
				case ExcelCommon.DataValidationSchemaNames.Custom:
					return DataValidationType.Custom;
				default:
					throw new ArgumentException("无效的类型" + schemaName);
			}
		}

	}
}
