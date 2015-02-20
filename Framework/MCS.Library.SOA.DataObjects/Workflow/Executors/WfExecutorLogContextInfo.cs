using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using MCS.Library.Logging;
using MCS.Library.Caching;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// Executor日志输出上下文类，直到执行了CommitInfoToLogger，才向真正的Logger输出
	/// </summary>
	public sealed class WfExecutorLogContextInfo
	{
		public static TextWriter Writer
		{
			get
			{
				object writer = null;

				if (ObjectContextCache.Instance.TryGetValue("ContextInfoWriter", out writer) == false)
				{
					StringBuilder strB = new StringBuilder(1024);

					writer = new StringWriter(strB);

					ObjectContextCache.Instance["ContextInfoWriter"] = writer;
				}

				return (TextWriter)writer;
			}
		}

		/// <summary>
		/// 提交到真正的Logger
		/// </summary>
		public static void CommitInfoToLogger()
		{
			StringBuilder strB = ((StringWriter)Writer).GetStringBuilder();

			if (strB.Length > 0)
			{
				try
				{
					Logger logger = LoggerFactory.Create("WfExecutor");

					if (logger != null)
						logger.Write(strB.ToString(), LogPriority.Normal, 8002, TraceEventType.Information, "WfExecutor上下文信息");
				}
				catch (System.Exception)
				{
				}
				finally
				{
					strB.Clear();
				}
			}
		}
	}
}
