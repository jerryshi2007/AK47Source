using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using MCS.Library.Configuration;
using MCS.Library.SOA.DataObjects.Workflow.Runtime;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 流程扩展数据保存
	/// </summary>
	public sealed class WfExtraPersistenceSettings : WfPersistenceSettingsBase
	{
		public static WfExtraPersistenceSettings GetConfig()
		{
			WfExtraPersistenceSettings result = (WfExtraPersistenceSettings)ConfigurationBroker.GetSection("wfExtraPersistenceSettings");

			if (result == null)
				result = new WfExtraPersistenceSettings(true);

			return result;
		}

		private WfExtraPersistenceSettings()
			: base()
		{
		}

		private WfExtraPersistenceSettings(bool useDefault)
			: base(useDefault)
		{
		}

		protected override WfExtraProcessPersistManagerCollection GetDefaultPersisters()
		{
			WfExtraProcessPersistManagerCollection result = new WfExtraProcessPersistManagerCollection();

			result.Add(new WfCurrentAssigneesPersistManager());
			result.Add(new WfProcessRelativeParamsPersistManager());

			return result;
		}
	}
}
