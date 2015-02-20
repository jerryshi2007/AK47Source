#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library.Data
// FileName	：	TSqlBuilder.cs
// Remark	：	基于T-SQL的ISqlBuilder的实现类
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
    /// 基于T-SQL的ISqlBuilder的实现类
    /// </summary>
    public class TSqlBuilder : SqlBuilderBase
    {
        /// <summary>
        /// TSqlBuilder的实例
        /// </summary>
        public static readonly TSqlBuilder Instance = new TSqlBuilder();

        private TSqlBuilder()
        {
        }

		/// <summary>
		/// 进行单引号检查，如果发现字符串中有单引号，那么替换成两个单引号，防止注入式攻击。然后在头尾各添加一个引号。然后添加Unicode前缀N
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public override string CheckUnicodeQuotationMark(string data)
		{
			string result = CheckQuotationMark(data, true);

            if (result != null)
                result = 'N' + result;

            return result;
		}

        /// <summary>
        /// 获取数据库同步时间函数
        /// </summary>
        public override string DBCurrentTimeFunction
        {
            get
            {
                return "GETDATE()";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="nullStr"></param>
        /// <param name="addQuotation"></param>
        /// <returns></returns>
        public override string DBNullToString(string data, string nullStr, bool addQuotation)
        {
            return string.Format("ISNULL({0}, {1})", AddQuotation(data, addQuotation), nullStr);
        }

        /// <summary>
        /// 
        /// </summary>
        public override string DBStatementBegin
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override string DBStatementEnd
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override string DBStatementSeperator
        {
            get
            {
                return "\n";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override string DBStrConcatSymbol
        {
            get
            {
                return "+";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public override string FormatDateTime(DateTime dt)
        {
            return this.AddQuotation(dt.ToString("yyyyMMdd HH:mm:ss.fff"), true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="addQuotation"></param>
        /// <returns></returns>
        public override string GetDBByteLengthFunction(string data, bool addQuotation)
        {
            return string.Format("DATALENGTH({0})", AddQuotation(data, addQuotation));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="addQuotation"></param>
        /// <returns></returns>
        public override string GetDBStringLengthFunction(string data, bool addQuotation)
        {
            return string.Format("LEN({0})", AddQuotation(data, addQuotation));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="addQuotation"></param>
        /// <returns></returns>
        public override string GetDBSubStringStr(string data, int start, int length, bool addQuotation)
        {
            return string.Format("SUBSTRING({0}, {1}, {2})", AddQuotation(data, addQuotation), start, length);
        }
    }
}
