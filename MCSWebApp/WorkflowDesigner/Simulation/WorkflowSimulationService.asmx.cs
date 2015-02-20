using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library.Script;

namespace WorkflowDesigner.Simulation
{
	/// <summary>
	/// Summary description for WorkflowSimulationService
	/// </summary>
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.ComponentModel.ToolboxItem(false)]
	[System.Web.Script.Services.ScriptService]
	public class WorkflowSimulationService : System.Web.Services.WebService
	{
		public WorkflowSimulationService()
		{
			JSONSerializerExecute.RegisterConverter(typeof(WfSimulationParametersConverter));
		}
		
		[WebMethod]
		public WfSimulationResult StartSimulation(string processDescKey, string simulationParametersJson)
		{
			return WfSimulator.StartSimulation(processDescKey, JSONSerializerExecute.Deserialize<WfSimulationParameters>(simulationParametersJson));
		}

		[WebMethod]
		public WfSimulationResult MoveToSimulation(string processID, string simulationParametersJson)
		{
			return WfSimulator.MoveToSimulation(processID, JSONSerializerExecute.Deserialize<WfSimulationParameters>(simulationParametersJson));
		}
	}
}
