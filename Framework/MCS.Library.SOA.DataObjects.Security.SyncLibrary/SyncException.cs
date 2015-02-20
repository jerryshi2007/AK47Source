using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Security.SyncLibrary
{
    [Serializable]
    public class SyncException : Exception
    {
        public SyncException() : base("同步时出现错误") { }
        public SyncException(string message) : base(message) { }
        public SyncException(string message, Exception inner) : base(message, inner) { }
        protected SyncException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
