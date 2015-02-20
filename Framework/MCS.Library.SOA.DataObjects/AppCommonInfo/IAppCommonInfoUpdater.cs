using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects.Workflow;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// AppCommonInfo的更新接口
	/// </summary>
	public interface IAppCommonInfoUpdater
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="aci"></param>
		/// <param name="ignoredUpdateProperties"></param>
		void Update(AppCommonInfo aci, params string[] ignoredUpdateProperties);

		/// <summary>
		/// 更新根据流程对象的RESOURCE_ID更新流程的状态
		/// </summary>
		/// <param name="processes"></param>
		void UpdateProcessStatus(IEnumerable<IWfProcess> processes);
	}
}
