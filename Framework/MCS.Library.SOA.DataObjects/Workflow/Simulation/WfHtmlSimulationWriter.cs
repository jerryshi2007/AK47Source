using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// Html格式的仿真输出器
	/// </summary>
	public class WfHtmlSimulationWriter : IWfSimulationWriter
	{
		#region IWfSimulationWriter Members

		public void Write(IWfProcess process, WfSimulationOperationType operationType, WfSimulationContext context)
		{
			switch (operationType)
			{
				case WfSimulationOperationType.Startup:
					WriteStartupProcessInfo(process, context);
					break;
				case WfSimulationOperationType.MoveTo:
					WriteMoveToInfo(process, context);
					break;
			}
		}

		#endregion

		private void WriteStartupProcessInfo(IWfProcess process, WfSimulationContext context)
		{
			context.Writer.WriteFullBeginTag("div");
			WriteStrongText(context.Writer, "启动流程{0}({1}), ID:{2}", process.Descriptor.Key, process.Descriptor.Name, process.ID);
			context.Writer.WriteEndTag("div");
		}

		private void WriteMoveToInfo(IWfProcess process, WfSimulationContext context)
		{
			IWfActivity currentActivity = WfRuntime.ProcessContext.CurrentActivity;

			StringBuilder strB = new StringBuilder();

			if (currentActivity != null)
				strB.AppendFormat("流转到{0}({1})", currentActivity.Descriptor.Key, currentActivity.Descriptor.Name);

			context.Writer.WriteFullBeginTag("div");
			WriteStrongText(context.Writer, strB.ToString());
			context.Writer.WriteEndTag("div");

			WriteAssignees(currentActivity, context);
		}

		private void WriteAssignees(IWfActivity activity, WfSimulationContext context)
		{
			StringBuilder strB = new StringBuilder();

			foreach (WfAssignee assignee in activity.Assignees)
			{
				if (assignee.User != null)
				{
					if (strB.Length > 0)
						strB.AppendFormat(", ");

					strB.AppendFormat("{0}({1})", assignee.User.DisplayName, assignee.User.ID);
				}
			}

			if (strB.Length > 0)
				WriteTextInTag(context.Writer, "div", "流转给: {0}", strB.ToString());
		}

		#region Html Helper
		private static void WriteStrongText(HtmlTextWriter writer, string format, params string[] args)
		{
			WriteTextInTag(writer, "strong", format, args);
		}

		private static void WriteTextInTag(HtmlTextWriter writer, string tagName, string format, params string[] args)
		{
			string text = string.Format(format, args);

			writer.WriteFullBeginTag(tagName);
			writer.WriteEncodedText(text);
			writer.WriteEndTag(tagName);
		}
		#endregion Html Helper
	}
}
