#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library.Data
// FileName	：	SqlBuilderBase.cs
// Remark	：	ISqlBuilder的实现基类
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    张铁军	    20070430		创建
// -------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.Data.Builder
{
	/// <summary>
	/// ISqlBuilder的实现基类
	/// </summary>
	public abstract class SqlBuilderBase : ISqlBuilder
	{
		#region ISqlBuilder 成员
		/// <summary>
		/// 检查并修改引号标记
		/// </summary>
		/// <param name="data">需要检查的字符串</param>
		/// <param name="addQuotation">是否添加引号</param>
		/// <returns>返回检查后的字符串</returns>
		public virtual string CheckQuotationMark(string data, bool addQuotation)
		{
			string result = data;

			if (data != null)
			{
				result = data.Replace("'", "''");

				if (addQuotation)
					result = "'" + result + "'";
			}

			return result;
		}

		/// <summary>
		/// 进行单引号检查，如果发现字符串中有单引号，那么替换成两个单引号，防止注入式攻击。然后在头尾各添加一个引号。然后添加Unicode说明
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public virtual string CheckUnicodeQuotationMark(string data)
		{
			return this.CheckQuotationMark(data, true);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="data"></param>
		/// <param name="addQuotation"></param>
		/// <returns></returns>
		public abstract string GetDBStringLengthFunction(string data, bool addQuotation);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="data"></param>
		/// <param name="addQuotation"></param>
		/// <returns></returns>
		public abstract string GetDBByteLengthFunction(string data, bool addQuotation);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="data"></param>
		/// <param name="start"></param>
		/// <param name="length"></param>
		/// <param name="addQuotation"></param>
		/// <returns></returns>
		public abstract string GetDBSubStringStr(string data, int start, int length, bool addQuotation);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		public abstract string FormatDateTime(DateTime dt);

		/// <summary>
		/// 格式化全文检索字符串。默认按照SQL Server的规则，消除双引号。并且将空格替换成逻辑运算符
		/// </summary>
		/// <param name="logicOp">逻辑运算符，默认是AND</param>
		/// <param name="searchText"></param>
		/// <returns></returns>
		public string FormatFullTextString(LogicOperatorDefine logicOp, string searchText)
		{
			StringBuilder result = new StringBuilder();

			string[] parts = null;

			if (searchText.IsNotEmpty())
			{
				searchText = searchText.Replace("\"", "");
				parts = searchText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			}
			else
				parts = new string[0];

			string op = EnumItemDescriptionAttribute.GetAttribute(logicOp).ShortName;

			foreach (string part in parts)
			{
				if (result.Length > 0)
					result.AppendFormat(" {0} ", op);

				result.AppendFormat("\"{0}\"", part);
			}

			return result.ToString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="data"></param>
		/// <param name="nullStr"></param>
		/// <param name="addQuotation"></param>
		/// <returns></returns>
		public abstract string DBNullToString(string data, string nullStr, bool addQuotation);

		/// <summary>
		/// 
		/// </summary>
		public abstract string DBCurrentTimeFunction
		{ get; }

		/// <summary>
		/// 
		/// </summary>
		public abstract string DBStrConcatSymbol
		{ get; }

		/// <summary>
		/// 
		/// </summary>
		public abstract string DBStatementBegin
		{ get; }

		/// <summary>
		/// 
		/// </summary>
		public abstract string DBStatementEnd
		{ get; }

		/// <summary>
		/// 
		/// </summary>
		public abstract string DBStatementSeperator
		{ get; }

		/// <summary>
		/// 将LIKE对应的查询子句转义。将语句中的%、[、_转义
		/// </summary>
		/// <param name="likeString"></param>
		/// <returns></returns>
		public string EscapeLikeString(string likeString)
		{
			string result = likeString;

			if (result.IsNotEmpty())
			{
				result = result.Replace("[", "[[]");
				result = result.Replace("%", "[%]");
				result = result.Replace("_", "[_]");
			}

			return result;
		}
		#endregion

		/// <summary>
		/// 添加引号
		/// </summary>
		/// <param name="data">需要操作的字符串</param>
		/// <param name="addQuotation">添加引号</param>
		/// <returns></returns>
		protected virtual string AddQuotation(string data, bool addQuotation)
		{
			string result = data;

			if (addQuotation)
				result = CheckQuotationMark(data, addQuotation);

			return result;
		}
	}
}
