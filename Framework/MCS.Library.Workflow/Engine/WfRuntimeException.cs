using System;
using System.Text;
using System.Collections.Generic;
using MCS.Library.Core;

namespace MCS.Library.Workflow.Engine
{
	public enum WfRuntimeErrorType
	{
		ProcessError,
		ActivityError
	}

	/// <summary>
	/// 运行时异常
	/// </summary>
	public class WfRuntimeException : SystemSupportException
	{
		public WfRuntimeErrorType ErrorType { get; set; }

		/// <summary>
        /// 
        /// </summary>
        public WfRuntimeException()
            : base()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public WfRuntimeException(string message)
            : base(message)
        {
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="errType"></param>
		/// <param name="message"></param>
		public WfRuntimeException(WfRuntimeErrorType errType, string message)
			: base(message)
		{
			this.ErrorType = errType;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
		public WfRuntimeException(string message, System.Exception ex)
            : base(message, ex)
        {
        }

		public WfRuntimeException(WfRuntimeErrorType errType,  string message, System.Exception ex)
			: base(message, ex)
		{
			this.ErrorType = errType;
		}
	}
}
