using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Schemas.Client
{
	[Serializable]
	public class ObjectNotFoundException : ClientException
	{
		public ObjectNotFoundException() : base("对象未找到") { }
		public ObjectNotFoundException(string message) : base(message) { }
		public ObjectNotFoundException(string message, Exception inner) : base(message, inner) { }
		protected ObjectNotFoundException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }
	}
}
