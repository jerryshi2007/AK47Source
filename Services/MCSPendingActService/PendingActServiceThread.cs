using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Services;
using MCS.Library.SOA.DataObjects.Workflow;
using System.Threading;
using System.Configuration;
using MCS.Library.Configuration;
using MCS.Library.Core;
using System.Security.Principal;
using MCS.Library.Principal;

namespace MCSPendingActService
{
	public class PendingActServiceThread : ThreadTaskBase
	{
		public override void OnThreadTaskStart()
		{
			Run();
		}

		private void Run()
		{
			PendingActServiceConfigurationElement pendingActServiceParam = PendingActServiceSettings.GetConfig().PendingActServices[this.Params.Name];

			pendingActServiceParam.NullCheck(string.Format("pendingActServiceConfig配置节错误，没有找到线程{0}对应的节点.", this.Params.Name));

			WfPendingActivityInfoCollection pendingActs = LoadPendingActs(pendingActServiceParam.ApplicationName, pendingActServiceParam.ProgramName);

			if (pendingActs.Count > 0)
			{
				Thread.CurrentPrincipal = new DeluxePrincipal(new DeluxeIdentity(pendingActServiceParam.Operator));

				//WfActivity中CanMoveTo没有判断StartTime。我希望在workflowSettings加开关，是否使用它。
				foreach (var pendingAct in pendingActs)
				{
					try
					{
						WfRuntime.ProcessPendingActivity(pendingAct);
					}
					catch (System.Exception ex)
					{
						this.Params.Log.Write(ex);
					}
					finally
					{
						WfRuntime.ClearCache();
					}
				}
			}
		}

		private static WfPendingActivityInfoCollection LoadPendingActs(string appName, string progName)
		{
			WfPendingActivityInfoCollection result = null;

			if (appName.ToLower() == "all" && progName.ToLower() == "all")
			{
				result = WfPendingActivityInfoAdapter.Instance.LoadAll();
			}
			else
			{
				//当appName和progName都是空时，相当于all。返回所有记录
				result = WfPendingActivityInfoAdapter.Instance.Load(appName, progName);	//注意PendingActivity表关于AppName和progName的索引
			}

			return result;
		}
	}
}
