using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Schemas.Client
{
	[Serializable]
	public class ClientException : Exception
	{
		public ClientException() : base("客户端异常") { }
		public ClientException(string message) : base(message) { }
		public ClientException(string message, Exception inner) : base(message, inner) { }
		protected ClientException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }
	}
}
