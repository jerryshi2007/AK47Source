#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library.Data
// FileName	：	PlSqlBuilder.cs
// Remark	：	基于PL/SQL的ISqlBuilder的实现类
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
    /// 基于PL/SQL的ISqlBuilder的实现类
    /// </summary>
    public class PlSqlBuilder : SqlBuilderBase
    {
        /// <summary>
        /// PlSql的实例
        /// </summary>
        public static readonly PlSqlBuilder Instance = new PlSqlBuilder();

        private PlSqlBuilder()
        {
        }
        /// <summary>
        ///  数据库端返回当前时间的函数名称
        /// </summary>
        public override string DBCurrentTimeFunction
        {
            get
            {
                return "SYSDATE";
            }
        }
        /// <summary>
        /// 重写将空数据转化成字符串格式
        /// </summary>
        /// <param name="data">字符串数据</param>
        /// <param name="nullStr">空字符串</param>
        /// <param name="addQuotation">是否增加引用</param>
        /// <returns>返回转化后的字符串</returns>
        public override string DBNullToString(string data, string nullStr, bool addQuotation)
        {
            return string.Format("NVL({0}, {1})", AddQuotation(data, addQuotation), nullStr);
        }
        /// <summary>
        /// 批量SQL的开始标识，SQL Server中没有，Oracle中是BEGIN
        /// </summary>
        public override string DBStatementBegin
        {
            get
            {
                return "BEGIN\n";
            }
        }
        /// <summary>
        /// 批量SQL的结束标识，SQL Server中没有，Oracle中是END
        /// </summary>
        public override string DBStatementEnd
        {
            get
            {
                return "END\n";
            }
        }
        /// <summary>
        /// QL语句之间的分隔符，SQL Server中是“;”或CR/LF，Oracle中是“;”+CR/LF
        /// </summary>
        public override string DBStatementSeperator
        {
            get
            {
                return ";\n";
            }
        }
        /// <summary>
        /// SQL语句中，字符串之间的连接符
        /// </summary>
        public override string DBStrConcatSymbol
        {
            get
            {
                return "||";
            }
        }
        /// <summary>
        /// 将DateTime格式化为数据库所识别的日期格式
        /// </summary>
        /// <param name="dt">时间类型的数据</param>
        /// <returns>数据库所识别的日期格式</returns>
        public override string FormatDateTime(DateTime dt)
        {
			return string.Format("TO_DATE({0}, 'YYYY-MM-DD HH24:MI:SS')", AddQuotation(dt.ToString("yyyy-MM-dd HH:mm:ss"), true));
        }

        /// <summary>
        /// 返回数据库中获得字节长度的函数名称
        /// </summary>
        /// <param name="data">字符串数据</param>
        /// <param name="addQuotation">是否添加</param>
        /// <returns>返回数据库中获得字节长度的函数名称</returns>
        public override string GetDBByteLengthFunction(string data, bool addQuotation)
        {
            return string.Format("LENGTHB({0})", AddQuotation(data, addQuotation));
        }
        /// <summary>
        /// 返回数据库中获得字符串长度的函数名称
        /// </summary>
        /// <param name="data"></param>
        /// <param name="addQuotation"></param>
        /// <returns>返回数据库中获得字符串长度的函数名称</returns>
        public override string GetDBStringLengthFunction(string data, bool addQuotation)
        {
            return string.Format("LENGTH({0})", AddQuotation(data, addQuotation));
        }
        /// <summary>
        /// 返回数据库中SubString函数的字符串
        /// </summary>
        /// <param name="data">需要格式化的数据</param>
        /// <param name="start">开始位置</param>
        /// <param name="length">结束位置</param>
        /// <param name="addQuotation">是否作为字符串参数执行</param>
        /// <returns>返回数据库中SubString函数的字符串</returns>
        public override string GetDBSubStringStr(string data, int start, int length, bool addQuotation)
        {
            return string.Format("SUBSTR({0}, {1}, {2})", AddQuotation(data, addQuotation), start, length);
        }
    }
}
