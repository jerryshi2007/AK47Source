using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects.Security.Actions;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Security.Executors
{
	/// <summary>
	/// Executor执行时，状态检查出现异常时的异常类
	/// </summary>
	public class SCStatusCheckException : System.Exception
	{
		private SchemaObjectBase _RelativeObject = null;
		private SCOperationType _OperationType = SCOperationType.None;

		/// <summary>
		/// 
		/// </summary>
		public SCStatusCheckException()
			: base()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		public SCStatusCheckException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="innerException"></param>
		public SCStatusCheckException(string message, System.Exception innerException) :
			base(message, innerException)
		{
		}

		public SCStatusCheckException(SchemaObjectBase relativeObject, SCOperationType opType)
			: base(string.Format("对象\"{0}\"的状态不是正常状态，不能执行{1}操作", relativeObject.ToSimpleObject().Name, EnumItemDescriptionAttribute.GetDescription(opType)))
		{
			this._RelativeObject = relativeObject;
			this._OperationType = opType;
		}
	}
}
