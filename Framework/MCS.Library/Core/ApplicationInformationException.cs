using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Core
{
	/// <summary>
	/// 应用程序提示信息类的异常
	/// </summary>
	[Serializable]
	public class ApplicationInformationException : ApplicationException
	{
		/// <summary>
        /// ApplicationInformationException的缺省构造函数
        /// </summary>
        /// <remarks>ApplicationInformationException的缺省构造函数.
        /// </remarks>
        public ApplicationInformationException()
        {
        }

        /// <summary>
        /// ApplicationInformationException的带错误消息参数的构造函数
        /// </summary>
        /// <param name="strMessage">错误消息串</param>
        /// <remarks>ApplicationInformationException的带错误消息参数的构造函数,该错误消息将在消息抛出异常时显示出来。
        /// <seealso cref="MCS.Library.Expression.ExpTreeExecutor"/>
        /// </remarks>
        public ApplicationInformationException(string strMessage)
            : base(strMessage)
        {
        }

        /// <summary>
        /// ApplicationInformationException的构造函数。
        /// </summary>
        /// <param name="strMessage">错误消息串</param>
        /// <param name="ex">导致该异常的异常</param>
        /// <remarks>该构造函数把导致该异常的异常记录了下来。
        /// </remarks>
        public ApplicationInformationException(string strMessage, Exception ex)
            : base(strMessage, ex)
        {
        }
	}
}
