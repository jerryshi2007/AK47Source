using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 保存TimePoint的实现器
	/// </summary>
	public class TimePointPersister : IPersistTimePoint
	{
		public DateTime LoadTimePoint(string key)
		{
			UserSettings settings = UserSettings.GetSettings(key);

			return settings.GetPropertyValue("CommonSettings", "SimulatedTime", DateTime.MinValue);
		}

		public void SaveTimePoint(string key, DateTime simulatedTime)
		{
			UserSettings settings = UserSettings.LoadSettings(key);

			settings.Categories["CommonSettings"].Properties.SetValue("SimulatedTime", simulatedTime);

			settings.Update();
		}
	}
}
