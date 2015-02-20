using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Core
{
	/// <summary>
	/// 描述性能监视指针信息的属性，用于HitPerformanceCountersBase等类
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
	public sealed class PerformanceCounterDescriptionAttribute : Attribute
	{
		/// <summary>
		/// 构造方法
		/// </summary>
		public PerformanceCounterDescriptionAttribute()
		{
		}

		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="categoryName"></param>
		/// <param name="counterNames">使用逗号分隔的计数器名称</param>
		public PerformanceCounterDescriptionAttribute(string categoryName, string counterNames)
		{
			this.CategoryName = categoryName;
		}

		/// <summary>
		/// 类别的名称
		/// </summary>
		public string CategoryName
		{
			get;
			set;
		}

		/// <summary>
		/// 使用逗号分隔的计数器名称
		/// </summary>
		public string CounterNames
		{
			get;
			set;
		}
	}
}
