#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library.Data
// FileName	：	SqlCaluseBuilderBase.cs
// Remark	：	Sql子句构造器的抽象基类
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    张铁军	    20070824		创建
// -------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using MCS.Library.Core;

namespace MCS.Library.Data.Builder
{
	/// <summary>
	/// 所有Sql语句构造项的基类
	/// </summary>
	[Serializable]
	public abstract class SqlClauseBuilderItemBase
	{
		/// <summary>
		/// 得到Data的Sql字符串描述
		/// </summary>
		/// <param name="builder">构造器</param>
		/// <returns>返回将data翻译成sql语句的结果</returns>
		public abstract string GetDataDesp(ISqlBuilder builder);
	}

	/// <summary>
	/// 带数据的Sql语句构造项的基类
	/// </summary>
	[Serializable]
	public class SqlCaluseBuilderItemWithData : SqlClauseBuilderItemBase
	{
		private static DataDescriptionGeneratorBase[] _DataDescriptors = new DataDescriptionGeneratorBase[]{
			NullDescriptionGenerator.Instance,
			ExpressionDescriptionGenerator.Instance,
			DateTimeDescriptionGenerator.Instance,
			DBNullDescriptionGenerator.Instance,
			BooleanDescriptionGenerator.Instance,
			StringDescriptionGenerator.Instance,
			EnumDescriptionGenerator.Instance,
			GuidDescriptionGenerator.Instance,
			BytesDescriptionGenerator.Instance,
			StreamDescriptionGenerator.Instance
		};

		private object data = null;
		private bool isExpression = false;

		/// <summary>
		/// 数据
		/// </summary>
		public object Data
		{
			get { return this.data; }
			set { this.data = value; }
		}

		/// <summary>
		/// 构想项中的Data是否是sql表达式
		/// </summary>
		public bool IsExpression
		{
			get { return this.isExpression; }
			set { this.isExpression = value; }
		}

		/// <summary>
		/// 得到Data的Sql字符串描述
		/// </summary>
		/// <param name="builder">构造器</param>
		/// <returns>返回将data翻译成sql语句的结果</returns>
		public override string GetDataDesp(ISqlBuilder builder)
		{
			string result = string.Empty;

			DataDescriptionGeneratorBase generator = GetDataDescriptor(this);

			if (generator != null)
				result = generator.ToDescription(this, builder);
			else
				result = this.data.ToString();

			return result;
		}
		//public override string GetDataDesp(ISqlBuilder builder)
		//{
		//    string result = string.Empty;

		//    if (this.data == null || this.data is DBNull)
		//        result = "NULL";
		//    else
		//    {
		//        if (this.data is DateTime)
		//        {
		//            DateTime minDate = new DateTime(1753, 1, 1);

		//            if ((DateTime)this.data < minDate)
		//                result = "NULL";
		//            else
		//                result = builder.FormatDateTime((DateTime)this.data);
		//        }
		//        else if (this.data is System.Guid)
		//        {
		//            if ((Guid)this.data == Guid.Empty)
		//                result = "NULL";
		//            else
		//                result = builder.CheckUnicodeQuotationMark(this.data.ToString());
		//        }
		//        else
		//        {
		//            if (this.isExpression == false && (this.data is string || this.data.GetType().IsEnum))
		//                result = builder.CheckUnicodeQuotationMark(this.data.ToString());
		//            else
		//                if (this.data is bool)
		//                    result = ((int)Convert.ChangeType(this.data, typeof(int))).ToString();
		//                else
		//                {
		//                    if (this.data is byte[])
		//                        result = BytesToHexString((byte[])data);
		//                    else
		//                        if (this.data is Stream)
		//                            result = StreamToHexString((Stream)data);
		//                        else
		//                            result = this.data.ToString();
		//                }
		//        }
		//    }

		//    return result;
		//}

		private static DataDescriptionGeneratorBase GetDataDescriptor(SqlCaluseBuilderItemWithData buiderItem)
		{
			DataDescriptionGeneratorBase result = null;

			foreach (DataDescriptionGeneratorBase generator in _DataDescriptors)
			{
				if (generator.IsMatched(buiderItem))
				{
					result = generator;
					break;
				}
			}

			return result;
		}

		//private static string StreamToHexString(Stream stream)
		//{
		//    byte[] buffer = new byte[4096];

		//    StringBuilder strB = new StringBuilder(4096);

		//    using (BinaryReader br = new BinaryReader(stream))
		//    {
		//        int byteRead = br.Read(buffer, 0, buffer.Length);

		//        while (byteRead > 0)
		//        {
		//            for (int i = 0; i < byteRead; i++)
		//            {
		//                if (strB.Length == 0)
		//                    strB.Append("0X");

		//                strB.AppendFormat("{0:X2}", buffer[i]);
		//            }

		//            byteRead = br.Read(buffer, 0, buffer.Length);
		//        }
		//    }

		//    if (strB.Length == 0)
		//        strB.Append("NULL");

		//    return strB.ToString();
		//}

		//private static string BytesToHexString(byte[] data)
		//{
		//    StringBuilder strB = new StringBuilder(4096);

		//    if (data.Length > 0)
		//    {
		//        for (int i = 0; i < data.Length; i++)
		//        {
		//            if (strB.Length == 0)
		//                strB.Append("0X");

		//            strB.AppendFormat("{0:X2}", data[i]);
		//        }
		//    }
		//    else
		//        strB.Append("NULL");

		//    return strB.ToString();
		//}
	}

	/// <summary>
	/// In操作的语句项
	/// </summary>
	[Serializable]
	public class SqlCaluseBuilderItemInOperator : SqlCaluseBuilderItemWithData
	{
	}

	/// <summary>
	/// 适合INSERT、UPDATE、WHERE的每一个构造项，包括字段名称和字段的值等内容
	/// </summary>
	[Serializable]
	public class SqlClauseBuilderItemIUW : SqlCaluseBuilderItemWithData
	{
		private string operation = SqlClauseBuilderBase.EqualTo;

		/// <summary>
		/// 构造方法
		/// </summary>
		public SqlClauseBuilderItemIUW()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		private string dataField = string.Empty;

		/// <summary>
		/// Sql语句中的字段名
		/// </summary>
		public string DataField
		{
			get { return this.dataField; }
			set
			{
				ExceptionHelper.TrueThrow<ArgumentException>(string.IsNullOrEmpty(value), "DataField属性不能为空或空字符串");
				this.dataField = value;
			}
		}

		/// <summary>
		/// 字段和数据之间的操作符
		/// </summary>
		public string Operation
		{
			get { return this.operation; }
			set { this.operation = value; }
		}
	}

	/// <summary>
	/// 适合UPDATE、WHERE的每一个构造项，包括字段名称和字段的值等内容
	/// </summary>
	[Serializable]
	public class SqlClauseBuilderItemUW : SqlClauseBuilderItemIUW
	{
		private string template = string.Empty;

		/// <summary>
		/// 表达式模板。如果没有提供，则使用默认模板(${DataField}$ ${Operation}$ ${Data}$)。
		/// </summary>
		public string Template
		{
			get { return this.template; }
			set { this.template = value; }
		}

		/// <summary>
		/// 默认的表达式模板
		/// </summary>
		private const string DefaultTemplate = "${DataField}$ ${Operation}$ ${Data}$";

		/// <summary>
		/// 根据模板生成SQL子句
		/// </summary>
		/// <param name="strB"></param>
		/// <param name="builder"></param>
		internal void ToSqlString(StringBuilder strB, ISqlBuilder builder)
		{
			Regex reg = new Regex(@"\$\{[0-9 a-z A-Z]*?\}\$");

			string t = DefaultTemplate;

			if (this.template.IsNotEmpty())
				t = this.template;

			string replacedExp = reg.Replace(t, m =>
			{
				string paramName = m.Value.Substring(2, m.Length - 4);

				return GetParameterValue(paramName, builder);
			});

			strB.Append(replacedExp);
		}

		private string GetParameterValue(string paramName, ISqlBuilder builder)
		{
			string result = string.Empty;

			switch (paramName.ToLower())
			{
				case "datafield":
					result = this.DataField;
					break;
				case "operation":
					result = this.Operation;
					break;
				case "data":
					result = this.GetDataDesp(builder);
					break;
			}

			return result;
		}
	}

	/// <summary>
	/// 构造排序表达式的构造项
	/// </summary>
	[Serializable]
	public class SqlClauseBuilderItemOrd : SqlClauseBuilderItemBase
	{
		/// <summary>
		/// 
		/// </summary>
		private FieldSortDirection sortDirection = FieldSortDirection.Ascending;
		/// <summary>
		/// 
		/// </summary>
		private string dataField = string.Empty;

		/// <summary>
		/// 构造方法
		/// </summary>
		public SqlClauseBuilderItemOrd()
		{
		}

		/// <summary>
		/// Sql语句中的字段名
		/// </summary>
		public string DataField
		{
			get { return this.dataField; }
			set
			{
				ExceptionHelper.TrueThrow<ArgumentException>(string.IsNullOrEmpty(value), "DataField属性不能为空或空字符串");
				this.dataField = value;
			}
		}

		/// <summary>
		/// 排序方向
		/// </summary>
		public FieldSortDirection SortDirection
		{
			get
			{
				return this.sortDirection;
			}
			set
			{
				this.sortDirection = value;
			}
		}

		/// <summary>
		/// 得到Data的Sql字符串描述
		/// </summary>
		/// <param name="builder">构造器</param>
		/// <returns>返回将data翻译成sql语句的结果</returns>
		public override string GetDataDesp(ISqlBuilder builder)
		{
			string result = string.Empty;

			if (this.sortDirection == FieldSortDirection.Descending)
				result = "DESC";

			return result;
		}

		/// <summary>
		/// 生成SQL子句（“字段 ASC|DESC”）
		/// </summary>
		/// <param name="strB"></param>
		/// <param name="builder"></param>
		internal void ToSqlString(StringBuilder strB, ISqlBuilder builder)
		{
			strB.Append(this.DataField);

			string desp = this.GetDataDesp(builder);

			if (false == string.IsNullOrEmpty(desp))
				strB.Append(" " + desp);
		}
	}

	/// <summary>
	/// 字段的排序方向定义
	/// </summary>
	public enum FieldSortDirection
	{
		/// <summary>
		/// 升序
		/// </summary>
		Ascending,

		/// <summary>
		/// 降序
		/// </summary>
		Descending
	}

	/// <summary>
	/// 逻辑运算符
	/// </summary>
	public enum LogicOperatorDefine
	{
		/// <summary>
		/// “与”操作
		/// </summary>
		[EnumItemDescription(Description = "“与”操作", ShortName = "AND")]
		And,

		/// <summary>
		/// “或”操作
		/// </summary>
		[EnumItemDescription(Description = "“或”操作", ShortName = "OR")]
		Or
	}
}
