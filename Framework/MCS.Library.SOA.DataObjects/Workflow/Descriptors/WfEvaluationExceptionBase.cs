using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Logging;
using System.Diagnostics;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	[Serializable]
	public abstract class WfEvaluationExceptionBase : SystemSupportException
	{
		private WfConditionDescriptor _Condition = null;

		/// <summary>
		/// 
		/// </summary>
		public WfEvaluationExceptionBase()
			: base()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		public WfEvaluationExceptionBase(string message)
			: base(message)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="condition"></param>
		public WfEvaluationExceptionBase(string message, WfConditionDescriptor condition)
			: base(message)
		{
			this._Condition = condition;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="innerException"></param>
		public WfEvaluationExceptionBase(string message, System.Exception innerException)
			: base(message, innerException)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="innerException"></param>
		/// <param name="condition"></param>
		public WfEvaluationExceptionBase(string message, System.Exception innerException, WfConditionDescriptor condition)
			: base(message, innerException)
		{
			this._Condition = condition;
		}

		/// <summary>
		/// 发生异常的条件源头
		/// </summary>
		public WfConditionDescriptor Condition
		{
			get
			{
				return this._Condition;
			}
		}
	}

	/// <summary>
	/// Evaluation的异常扩展功能
	/// </summary>
	public static class WfEvaluationExceptionExtension
	{
		public static void WriteToLog(this WfEvaluationExceptionBase ex)
		{
			if (ex != null)
			{
				Logger logger = LoggerFactory.Create("WfRuntime");

				if (logger != null)
				{
					StringBuilder strB = new StringBuilder(1024);

					strB.AppendLine(ex.Message);

					if (ex.Condition != null && ex.Condition.Owner != null)
					{
						strB.AppendFormat("Condition Owner Key: {0}\n", ex.Condition.Owner.Key);
						
						if (ex.Condition.Owner.ProcessInstance != null)
							strB.AppendFormat("Condition Process ID: {0}, Key: {1}\n",
								ex.Condition.Owner.ProcessInstance.ID, ex.Condition.Owner.ProcessInstance.Descriptor.Key);
					}

					strB.AppendLine(EnvironmentHelper.GetEnvironmentInfo());
					strB.AppendLine(ex.StackTrace);

					logger.Write(strB.ToString(), LogPriority.Normal, 8004, TraceEventType.Error, "WfRuntime上下文信息");
				}
			}
		}
	}
}
