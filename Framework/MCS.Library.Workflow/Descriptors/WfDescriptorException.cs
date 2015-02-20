using System;
using System.Text;
using System.Collections.Generic;
using MCS.Library.Core;

namespace MCS.Library.Workflow.Descriptors
{
    public class WfDescriptorException : SystemSupportException
    {
        public WfDescriptorException()
            : base()
        {
        }

        public WfDescriptorException(string message)
            : base(message)
        {
        }

        public WfDescriptorException(string message, System.Exception ex)
            : base(message, ex)
        {
        }
    }
}
