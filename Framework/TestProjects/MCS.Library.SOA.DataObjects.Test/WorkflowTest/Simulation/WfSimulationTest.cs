using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Workflow;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MCS.Library.SOA.DataObjects.Test.WorkflowTest.Simulation
{
	[TestClass]
	public class WfSimulationTest
	{
		[TestMethod]
		[TestCategory(ProcessTestHelper.Simulation)]
		public void SimpleProcessSimulation()
		{
			IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateSimpleProcessDescriptor();

			processDesp.Activities["NormalActivity"].Resources.Add(new WfUserResourceDescriptor((IUser)OguObjectSettings.GetConfig().Objects["approver1"].Object));
			processDesp.Activities["NormalActivity"].Resources.Add(new WfUserResourceDescriptor((IUser)OguObjectSettings.GetConfig().Objects["approver2"].Object));

			WfSimulationParameters simulationParameters = new WfSimulationParameters();
			WfSimulationResult result = WfSimulator.StartSimulation(processDesp, new WfSimulationParameters());

			Console.WriteLine(WfRuntime.ProcessContext.SimulationContext.GetOutputString());

			while (result.ProcessStatus != WfProcessStatus.Completed)
			{
				result = WfSimulator.MoveToSimulation(result.ProcessID, simulationParameters);

				Console.WriteLine(WfRuntime.ProcessContext.SimulationContext.GetOutputString());
			}
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Simulation)]
		public void BranchProcessSimulation()
		{
			IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateSimpleProcessDescriptorWithBranchTemplate();

			WfSimulationParameters simulationParameters = new WfSimulationParameters();
			WfSimulationResult result = WfSimulator.StartSimulation(processDesp, new WfSimulationParameters());

			Console.WriteLine(WfRuntime.ProcessContext.SimulationContext.GetOutputString());

			while (result.ProcessStatus != WfProcessStatus.Completed)
			{
				result = WfSimulator.MoveToSimulation(result.ProcessID, simulationParameters);

				Console.WriteLine(WfRuntime.ProcessContext.SimulationContext.GetOutputString());
			}
		}

		[TestMethod]
		[TestCategory(ProcessTestHelper.Simulation)]
		public void ConditionProcessSimulation()
		{
			IWfProcessDescriptor processDesp = WfProcessTestCommon.CreateSimpleProcessDescriptorWithCondition();

			WfSimulationParameters simulationParameters = new WfSimulationParameters();

			simulationParameters.Variables.Add(new WfVariableDescriptor("Amount", "10000", DataType.Int));

			WfSimulationResult result = WfSimulator.StartSimulation(processDesp, new WfSimulationParameters());

			Console.WriteLine(WfRuntime.ProcessContext.SimulationContext.GetOutputString());

			while (result.ProcessStatus != WfProcessStatus.Completed)
			{
				result = WfSimulator.MoveToSimulation(result.ProcessID, simulationParameters);

				Console.WriteLine(WfRuntime.ProcessContext.SimulationContext.GetOutputString());
			}
		}
	}
}
