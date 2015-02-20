using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MCS.Library.SOA.DocServiceContract.Exceptions
{
    /// <summary>
    /// 目标不存在
    /// </summary>
    public class TargetNotFoundException:Exception
    {
        public TargetNotFoundException()
            : base("Target not found")
        {
        }

        public TargetNotFoundException(string message)
            : base(message)
        {
        }
    }
}