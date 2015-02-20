using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MCS.Library.Caching;
using MCS.Library.Logging;
using System.Diagnostics;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects.Executors
{
	[DebuggerNonUserCode]
	public sealed class AUExecutorLogContextInfo
	{
		public static TextWriter Writer
		{
			get
			{
				object writer = null;

				if (ObjectContextCache.Instance.TryGetValue("AUExecutorLogContextInfoWriter", out writer) == false)
				{
					StringBuilder strB = new StringBuilder(1024);

					writer = new StringWriter(strB);

					ObjectContextCache.Instance["SCExecutorLogContextInfoWriter"] = writer;
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
					Logger logger = LoggerFactory.Create("AUExecutor");

					if (logger != null)
						logger.Write(strB.ToString(), LogPriority.Normal, 8005, TraceEventType.Information, "AUExecutor上下文信息");
					else
						Debug.Print("没有配置名为AUExecutor的Logger");
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
