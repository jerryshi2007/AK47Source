using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects
{
	/// <summary>
	/// 表示AU对象异常
	/// </summary>
	public class AUObjectException : Exception
	{
		public AUObjectException() : base("管理单元对象的异常") { }
		public AUObjectException(string message) : base(message) { }
		public AUObjectException(string message, Exception inner) : base(message, inner) { }
		protected AUObjectException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }
	}
}
