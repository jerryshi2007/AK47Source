using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Data.Builder
{
	/// <summary>
	/// 带版本信息的数据实体
	/// </summary>
	public interface IVersionDataObject
	{
		string ID { get; }
		DateTime VersionStartTime { get; }
		DateTime VersionEndTime { get; }
	}
}
