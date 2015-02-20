using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCS.Library.SOA.DataObjects.Workflow;

namespace MCS.Library.SOA.DataObjects.Test.WorkflowTest.ConfigurationTest
{
	public class ProcessStatusChangeFakeAction : IWfAction
	{
		public void PrepareAction(WfActionParams actionParams)
		{
		}

		public void PersistAction(WfActionParams actionParams)
		{
			actionParams.Context.StatusChangedProcesses.ForEach(process =>
			{
				Console.WriteLine("状态改变的流程{0},Status={1}", process.ID, process.Status);
			});
		}

		public void ClearCache()
		{
		}
	}
}
