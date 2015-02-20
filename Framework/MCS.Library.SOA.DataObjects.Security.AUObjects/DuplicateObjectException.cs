using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects
{
	/// <summary>
	/// 表示对象重复
	/// </summary>
	[Serializable]
	public class DuplicateObjectException : AUObjectValidationException
	{
		public DuplicateObjectException() : base("取得的对象不唯一。") { }
		public DuplicateObjectException(string message) : base(message) { }
		public DuplicateObjectException(string message, Exception inner) : base(message, inner) { }
		protected DuplicateObjectException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }
	}
}
