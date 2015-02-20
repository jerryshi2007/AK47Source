using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Core
{
	/// <summary>
	/// 保存TimePoint的接口
	/// </summary>
	public interface IPersistTimePoint
	{
		/// <summary>
		/// 加载TimePoint
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		DateTime LoadTimePoint(string key);

		/// <summary>
		/// 保存TimePoint
		/// </summary>
		/// <param name="key"></param>
		/// <param name="simulatedTime"></param>
		void SaveTimePoint(string key, DateTime simulatedTime);
	}
}
