using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Caching;
using MCS.Library.SOA.DataObjects.Workflow;

namespace MCS.Web.WebControls
{
	/// <summary>
	/// 流程数据收集器
	/// </summary>
	public static class WfApplicationRuntimeParametersCollector
	{
		/// <summary>
		/// 注册收集器
		/// </summary>
		/// <param name="collector"></param>
		public static void RegisterCollector(IWfApplicationRuntimeParametersCollector collector)
		{
			collector.NullCheck("collector");

			List<IWfApplicationRuntimeParametersCollector> collectors =
				(List<IWfApplicationRuntimeParametersCollector>)ObjectContextCache.Instance.GetOrAddNewValue("WfCollectors", (cache, key) =>
				{
					List<IWfApplicationRuntimeParametersCollector> newItem = new List<IWfApplicationRuntimeParametersCollector>();

					cache.Add(key, newItem);

					return newItem;
				});

			collectors.Add(collector);
		}

		/// <summary>
		/// 收集数据
		/// </summary>
		/// <param name="process"></param>
		public static void CollectApplicationData(IWfProcess process)
		{
			process.NullCheck("process");

			List<IWfApplicationRuntimeParametersCollector> collectors =
				(List<IWfApplicationRuntimeParametersCollector>)ObjectContextCache.Instance.GetOrAddNewValue("WfCollectors", (cache, key) =>
				{
					List<IWfApplicationRuntimeParametersCollector> newItem = new List<IWfApplicationRuntimeParametersCollector>();

					cache.Add(key, newItem);

					return newItem;
				});

			collectors.ForEach(c => c.CollectApplicationData(process));
		}
	}
}
