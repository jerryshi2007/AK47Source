using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.OA.CommonPages.AppTrace
{
	[Serializable]
	public class OperationDeniedException : Exception
	{
		public OperationDeniedException() : base("操作被拒绝") { }
		public OperationDeniedException(string message) : base(message) { }
		public OperationDeniedException(string message, Exception inner) : base(message, inner) { }
		protected OperationDeniedException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }
	}
}
