using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 仿真输出器的接口
	/// </summary>
	public interface IWfSimulationWriter
	{
		/// <summary>
		/// 输出
		/// </summary>
		/// <param name="simulationParams"></param>
		/// <param name="writer"></param>
		void Write(IWfProcess process, WfSimulationOperationType operationType, WfSimulationContext context);
	}
}
