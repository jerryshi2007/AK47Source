using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Caching;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 与时间点数据相关的操作上下文，主要用于处理VersionStartTime和VersionEndTime相关操作
	/// </summary>
	[ActionContextDescription(Key = "TimePointActionContext")]
	public class DBTimePointActionContext : ActionContextBase<DBTimePointActionContext>
	{
		public DBTimePointActionContext()
		{
		}

		/// <summary>
		/// 获取或设置表示操作的时间点的<see cref="DateTime"/> ，为<see cref="DateTime.MinValue"/>时表示当前时间。
		/// </summary>
		public DateTime TimePoint
		{
			get;
			set;
		}
	}
}
