using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Configuration;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 后台队列保存操作的逻辑
	/// </summary>
	public sealed class WfQueuePersistenceSettings : WfPersistenceSettingsBase
	{
		public static WfQueuePersistenceSettings GetConfig()
		{
			WfQueuePersistenceSettings result = (WfQueuePersistenceSettings)ConfigurationBroker.GetSection("wfQueuePersistenceSettings");

			if (result == null)
				result = new WfQueuePersistenceSettings(true);

			return result;
		}

		private WfQueuePersistenceSettings()
			: base()
		{
		}

		private WfQueuePersistenceSettings(bool useDefault)
			: base(useDefault)
		{
		}
	}
}
