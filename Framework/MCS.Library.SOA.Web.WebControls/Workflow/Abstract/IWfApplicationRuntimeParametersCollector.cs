using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects.Workflow;

namespace MCS.Web.WebControls
{
	/// <summary>
	/// 流程应用参数的收集器
	/// </summary>
	public interface IWfApplicationRuntimeParametersCollector
	{
		/// <summary>
		/// 收集数据
		/// </summary>
		/// <param name="process"></param>
		void CollectApplicationData(IWfProcess process);
	}
}
