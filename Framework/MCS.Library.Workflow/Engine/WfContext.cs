using System;
using System.Collections.Generic;
using System.Text;
using MCS.Library.Caching;
using MCS.Library.Workflow.Services;
using MCS.Library.Workflow.Configuration;
using MCS.Library.Expression;

namespace MCS.Library.Workflow.Engine
{
	[Serializable]
	public class WfContext
	{
		private WorkItemQueue _Queue = new WorkItemQueue();
		
		public event CalculateUserFunction ConditionUserFunction;

		private bool _AutoFindPassedTransitions = true;

		private WfContext()
		{
		}

		public static WfContext Current
		{
			get
			{
				object context = null;

				if (ObjectContextCache.Instance.TryGetValue("WfContext", out context) == false)
				{
					context = new WfContext();

					ObjectContextCache.Instance.Add("WfContext", context);
				}

				return (WfContext)context;
			}
			set
			{
				ObjectContextCache.Instance["WfContext"] = value;
			}
		}

		public void CommitProcessInfo()
		{
			WorkflowSettings.GetConfig().Writer.SaveWorkItems(WfContext.Current.Queue.ToArray());

			Queue.Clear();
		}

		public bool AutoFindPassedTransitions
		{
			get { return _AutoFindPassedTransitions; }
			set { _AutoFindPassedTransitions = value; }
		}

		public WorkItemQueue Queue
		{
			get
			{
				return _Queue;
			}
		}

		internal object OnCalculateUserFunction(string funcName, ParamObjectCollection arrParams, object callerContext)
		{
			object result = null;

			if (ConditionUserFunction != null)
				result = ConditionUserFunction(funcName, arrParams, callerContext);

			return result;
		}
	}
}
