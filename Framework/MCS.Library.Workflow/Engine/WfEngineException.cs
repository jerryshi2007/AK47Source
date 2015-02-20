using System;
using System.Text;
using System.Collections.Generic;
using MCS.Library.Core;

namespace MCS.Library.Workflow.Engine
{
    /// <summary>
    /// �����������������������쳣��
    /// </summary>
    public class WfEngineException : SystemSupportException
    {
        /// <summary>
        /// 
        /// </summary>
        public WfEngineException()
            : base()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public WfEngineException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public WfEngineException(string message, System.Exception ex)
            : base(message, ex)
        {
        }
    }
}
