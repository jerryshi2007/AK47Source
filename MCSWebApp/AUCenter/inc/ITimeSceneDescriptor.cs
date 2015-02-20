using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AUCenter
{
	/// <summary>
	/// 指定时间的场景描述符
	/// </summary>
	public interface ITimeSceneDescriptor
	{
		/// <summary>
		/// 获取当前时间的操作场景名
		/// </summary>
		string NormalSceneName { get; }

		/// <summary>
		/// 获取过去时间的操作场景名
		/// </summary>
		string ReadOnlySceneName { get; }
	}

	public interface INormalSceneDescriptor
	{
		void AfterNormalSceneApplied();
	}
}
