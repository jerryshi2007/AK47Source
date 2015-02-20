using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects.Workflow;

namespace MCS.Library.SOA.DataObjects.Test.WorkflowTest.ConfigurationTest
{
	class MockWfAction : IWfAction
	{
		public void PrepareAction(WfActionParams actionParams)
		{
			Console.WriteLine("prepare action");
		}

		public void PersistAction(WfActionParams actionParams)
		{
			Console.WriteLine("persist action");
		}

		public void ClearCache()
		{
			Console.WriteLine("clear cache");
		}
	}
}
