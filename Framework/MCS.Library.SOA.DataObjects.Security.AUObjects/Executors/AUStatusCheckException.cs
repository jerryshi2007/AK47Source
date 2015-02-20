using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects.Security.AUObjects.Executors;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects
{
	[Serializable]
	public class AUStatusCheckException : AUObjectException
	{
		private SchemaObjectBase originalData;
		private AUOperationType auOperationType;

		public SchemaObjectBase OriginalData
		{
			get { return originalData; }
		}

		public AUOperationType AUOperationType
		{
			get { return auOperationType; }
		}

		public AUStatusCheckException() : base("对象的状态是无效的不能执行操作") { }
		public AUStatusCheckException(string message) : base(message) { }
		public AUStatusCheckException(string message, Exception inner) : base(message, inner) { }
		protected AUStatusCheckException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }

		public AUStatusCheckException(SchemaObjectBase originalData, Executors.AUOperationType auOperationType)
			: base(GetMessage(originalData, auOperationType))
		{
			this.originalData = originalData;
			this.auOperationType = auOperationType;
		}

		private static string GetMessage(SchemaObjectBase originalData, Executors.AUOperationType auOperationType)
		{
			if (originalData == null)
				throw new ArgumentNullException("originalData");

			return string.Format("ID:为 {0} 的对象的状态不允许执行{1}操作。", originalData.ID, auOperationType.ToString());
		}
	}

}
