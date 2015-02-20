using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Principal;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 仿真器
	/// </summary>
	public static class WfSimulator
	{
		/// <summary>
		/// 启动流程仿真
		/// </summary>
		/// <param name="processDescKey"></param>
		/// <param name="simulationContext"></param>
		public static WfSimulationResult StartSimulation(string processDescKey, WfSimulationParameters simulationParameters)
		{
			return StartSimulation(WfProcessDescriptorManager.GetDescriptor(processDescKey), simulationParameters);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="processDesp"></param>
		/// <param name="simulationParameters"></param>
		/// <returns></returns>
		public static WfSimulationResult StartSimulation(IWfProcessDescriptor processDesp, WfSimulationParameters simulationParameters)
		{
			bool oldValue = WfRuntime.ProcessContext.EnableSimulation;

			try
			{
				WfRuntime.ProcessContext.EnableSimulation = true;
				InitSimulationContext(simulationParameters);

				WfStartWorkflowExecutor executor = new WfStartWorkflowExecutor(
					PrepareStartWorkflowParams(processDesp, WfRuntime.ProcessContext.SimulationContext));

				IWfProcess process = null;

				executor.AfterModifyWorkflow += (dataContext =>
				{
					process = dataContext.CurrentProcess;
				});

				executor.Execute();

				return new WfSimulationResult(process) { OutputString = WfRuntime.ProcessContext.SimulationContext.GetOutputString() };
			}
			finally
			{
				WfRuntime.ProcessContext.EnableSimulation = oldValue;
			}
		}

		/// <summary>
		/// 模拟一步流程执行
		/// </summary>
		/// <param name="process"></param>
		/// <param name="simulationContext"></param>
		/// <returns></returns>
		public static WfSimulationResult MoveToSimulation(string processID, WfSimulationParameters simulationParameters)
		{
			bool oldValue = WfRuntime.ProcessContext.EnableSimulation;

			try
			{
				WfRuntime.ProcessContext.EnableSimulation = true;
				InitSimulationContext(simulationParameters);

				IWfProcess process = WfRuntime.GetProcessByProcessID(processID);

				MergeVariablesToApplicationRuntimeParameters(process.ApplicationRuntimeParameters, simulationParameters.Variables);

				InnerMoveToSimulation(process, WfRuntime.ProcessContext.SimulationContext);

				//递归子流程，每个流程执行一步
				return new WfSimulationResult(process) { OutputString = WfRuntime.ProcessContext.SimulationContext.GetOutputString() };
			}
			finally
			{
				WfRuntime.ProcessContext.EnableSimulation = oldValue;
			}
		}

		internal static void WriteSimulationInfo(IWfProcess process, WfSimulationOperationType operationType)
		{
			if (WfRuntime.ProcessContext.EnableSimulation)
			{
				if (operationType == WfSimulationOperationType.MoveTo)
					WfRuntime.ProcessContext.SimulationContext.MoveToCount++;

				WfSimulationSettings.GetConfig().Writers.ForEach(writer => writer.Write(process, operationType, WfRuntime.ProcessContext.SimulationContext));
			}
		}

		private static void InnerMoveToSimulation(IWfProcess process, WfSimulationContext simulationContext)
		{
			if (process.Status == WfProcessStatus.Running)
			{
				if (process.CurrentActivity.Status == WfActivityStatus.Running || process.CurrentActivity.Status == WfActivityStatus.Pending)
				{
					WfTransferParams transferParams = PrepareTransferParams(process, process.CurrentActivity.Assignees.FirstOrDefault(), simulationContext);

					if (transferParams != null)
					{
						WfMoveToExecutor executor = new WfMoveToExecutor(process.CurrentActivity, process.CurrentActivity, transferParams);

						executor.Execute();
					}
				}
			}
		}

		private static void RecursiveSubProcessMoveTo(IWfActivity ownerActivity, WfSimulationContext simulationContext)
		{
			foreach (IWfBranchProcessGroup group in ownerActivity.BranchProcessGroups)
			{
				foreach (IWfProcess process in group.Branches)
					InnerMoveToSimulation(process, simulationContext);
			}
		}

		private static WfProcessStartupParams PrepareStartWorkflowParams(IWfProcessDescriptor processDesp, WfSimulationContext simulationContext)
		{
			WfProcessStartupParams startupParams = new WfProcessStartupParams();

			startupParams.ProcessDescriptor = processDesp;
			startupParams.DefaultTaskTitle = startupParams.ProcessDescriptor.DefaultTaskTitle;

			if (OguUser.IsNotNullOrEmpty(simulationContext.SimulationParameters.Creator))
			{
				startupParams.Creator = simulationContext.SimulationParameters.Creator;
			}
			else
			{
				if (DeluxePrincipal.IsAuthenticated)
					startupParams.Creator = DeluxeIdentity.CurrentUser;
			}

			startupParams.Assignees.Add(startupParams.Creator);
			startupParams.ResourceID = UuidHelper.NewUuidString();

			MergeVariablesToApplicationRuntimeParameters(startupParams.ApplicationRuntimeParameters, simulationContext.SimulationParameters.Variables);

			return startupParams;
		}

		private static WfTransferParams PrepareTransferParams(IWfProcess process, WfAssignee assignee, WfSimulationContext simulationContext)
		{
			IWfActivity originalActivity = process.CurrentActivity;

			IWfTransitionDescriptor transition = FindNextTransitionDescriptor(process);

			WfTransferParams transferParams = null;

			if (transition != null)
			{
				transferParams = new WfTransferParams(transition.ToActivity);

				transferParams.Assignees.CopyFrom(transition.ToActivity.Instance.Candidates);
				transferParams.FromTransitionDescriptor = transition;

				if (assignee != null)
					transferParams.Operator = assignee.User;
			}

			return transferParams;
		}

		private static IWfTransitionDescriptor FindNextTransitionDescriptor(IWfProcess process)
		{
			return process.CurrentActivity.Descriptor.ToTransitions.GetAllCanTransitTransitions().FindHighestPriorityTransition(false);
		}

		private static void InitSimulationContext(WfSimulationParameters simulationParameters)
		{
			WfRuntime.ProcessContext.SimulationContext.Initialize();

			WfRuntime.ProcessContext.SimulationContext.SimulationParameters = simulationParameters;
		}

		private static void MergeVariablesToApplicationRuntimeParameters(Dictionary<string, object> runtimeParameters, WfVariableDescriptorCollection variables)
		{
			foreach (WfVariableDescriptor variable in variables)
				runtimeParameters[variable.Key] = variable.ActualValue;
		}

		private static void MergeVariablesToApplicationRuntimeParameters(WfApplicationRuntimeParameters runtimeParameters, WfVariableDescriptorCollection variables)
		{
			foreach (WfVariableDescriptor variable in variables)
				runtimeParameters[variable.Key] = variable.ActualValue;
		}
	}
}
