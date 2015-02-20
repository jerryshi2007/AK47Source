using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.PermissionBridge
{
    [Serializable]
    public class UserNotFoundException : ObjectNotFoundException
    {
        public UserNotFoundException() { }
        public UserNotFoundException(string message) : base(message) { }
        public UserNotFoundException(string message, Exception inner) : base(message, inner) { }
        protected UserNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
