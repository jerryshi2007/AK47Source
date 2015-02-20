#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library.Data
// FileName	：	ISqlBuilder.cs
// Remark	：	定义了一些SQL语句通用语法接口。
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    张铁军	    20070430		创建
// -------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Data.Builder
{
    /// <summary>
    /// 定义了一些SQL语句通用语法接口，可以由不同的实现类来实现
    /// </summary>
    public interface ISqlBuilder
    {
        /// <summary>
        /// 进行单引号检查，如果发现字符串中有单引号，那么替换成两个单引号，防止注入式攻击
        /// </summary>
        /// <param name="data">字符串的值</param>
        /// <param name="addQuotation">返回值是否在data的两端添加单引号</param>
        /// <returns>检查后的字符串</returns>
        string CheckQuotationMark(string data, bool addQuotation);

		/// <summary>
		/// 进行单引号检查，如果发现字符串中有单引号，那么替换成两个单引号，防止注入式攻击。然后在头尾各添加一个引号。然后添加Unicode说明
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		string CheckUnicodeQuotationMark(string data);

        /// <summary>
        /// 数据库端返回当前时间的函数名称
        /// </summary>
        string DBCurrentTimeFunction
        {
            get;
        }

        /// <summary>
        /// 返回数据库中获得字符串长度的函数名称
        /// </summary>
        /// <param name="data">字段、变量或常量的名称</param>
        /// <param name="addQuotation">data参数是否需要加引号</param>
        /// <returns>字符串长度的函数名称</returns>
        string GetDBStringLengthFunction(string data, bool addQuotation);

        /// <summary>
        /// 返回数据库中获得字节长度的函数名称
        /// </summary>
        /// <param name="data">字段、变量或常量的名称</param>
        /// <param name="addQuotation">data参数是否需要加引号</param>
        /// <returns>字节长度的函数名称</returns>
        string GetDBByteLengthFunction(string data, bool addQuotation);

        /// <summary>
        /// 返回数据库中SubString函数的字符串
        /// </summary>
        /// <param name="data">字段、变量或常量的名称</param>
        /// <param name="start">起始位</param>
        /// <param name="length">长度</param>
        /// <param name="addQuotation">data参数是否需要加引号</param>
        /// <returns>SubString函数的字符串</returns>
        string GetDBSubStringStr(string data, int start, int length, bool addQuotation);

        /// <summary>
        /// 将DateTime格式化为数据库所识别的日期格式
        /// </summary>
        /// <param name="dt">日期</param>
        /// <returns>数据库中的日期常量表示</returns>
        string FormatDateTime(DateTime dt);

		/// <summary>
		/// 将LIKE对应的查询子句转义
		/// </summary>
		/// <param name="likeString"></param>
		/// <returns></returns>
		string EscapeLikeString(string likeString);

		/// <summary>
		/// 格式化全文检索字符串。默认按照SQL Server的规则，消除双引号。并且将空格替换成逻辑运算符
		/// </summary>
		/// <param name="logicOp">逻辑运算符，默认是AND</param>
		/// <param name="searchText"></param>
		/// <returns></returns>
		string FormatFullTextString(LogicOperatorDefine logicOp, string searchText);

        /// <summary>
        /// SQL语句中，字符串之间的连接符
        /// </summary>
        string DBStrConcatSymbol
        {
            get;
        }

        /// <summary>
        /// 得到SQL Server中的ISNULL或Oracle中的NVL
        /// </summary>
        /// <param name="data">需要检查的值</param>
        /// <param name="nullStr">如果data为null, 则转化成的字符串</param>
        /// <param name="addQuotation">data参数是否需要加引号</param>
        /// <returns>得到SQL Server中的ISNULL或Oracle中的NVL</returns>
        string DBNullToString(string data, string nullStr, bool addQuotation);

        /// <summary>
        /// 批量SQL的开始标识，SQL Server中没有，Oracle中是BEGIN
        /// </summary>
        string DBStatementBegin
        {
            get;
        }

        /// <summary>
        /// 批量SQL的结束标识，SQL Server中没有，Oracle中是BEGIN
        /// </summary>
        string DBStatementEnd
        {
            get;
        }

        /// <summary>
        /// SQL语句之间的分隔符，SQL Server中是“;”或CR/LF，Oracle中是“;”+CR/LF
        /// </summary>
        string DBStatementSeperator
        {
            get;
        }
    }
}
