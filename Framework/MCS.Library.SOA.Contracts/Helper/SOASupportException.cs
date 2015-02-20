using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.SOA.Contracts
{
    /// <summary>
    /// 定制的异常类
    /// </summary>
    /// <remarks>定制的异常类,这种异常类会提醒前端程序显示出技术支持信息的提示信息，该类继承自ApplicationException类。
    /// </remarks>
    public class SOASupportException : ApplicationException
    {
        /// <summary>
        /// SystemSupportException的缺省构造函数
        /// </summary>
        /// <remarks>SystemSupportException的缺省构造函数.
        /// </remarks>
        public SOASupportException()
        {
        }

        /// <summary>
        /// SystemSupportException的带错误消息参数的构造函数
        /// </summary>
        /// <param name="strMessage">错误消息串</param>
        /// <remarks>SystemSupportException的带错误消息参数的构造函数,该错误消息将在消息抛出异常时显示出来。
        /// <seealso cref="MCS.Library.Expression.ExpTreeExecutor"/>
        /// </remarks>
        public SOASupportException(string strMessage)
            : base(strMessage)
        {
        }

        /// <summary>
        /// SystemSupportException的构造函数。
        /// </summary>
        /// <param name="strMessage">错误消息串</param>
        /// <param name="ex">导致该异常的异常</param>
        /// <remarks>该构造函数把导致该异常的异常记录了下来。
        /// </remarks>
        public SOASupportException(string strMessage, Exception ex)
            : base(strMessage, ex)
        {
        }
    }
}
