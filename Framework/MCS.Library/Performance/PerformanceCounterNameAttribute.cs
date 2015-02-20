using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Core
{
	/// <summary>
	/// 描述性能监视指针名称的Attribute
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class PerformanceCounterNameAttribute : Attribute
	{
		/// <summary>
		/// 构造方法
		/// </summary>
		public PerformanceCounterNameAttribute()
		{
		}

		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="counterName"></param>
		public PerformanceCounterNameAttribute(string counterName)
		{
			this.CounterName = counterName;
		}

		/// <summary>
		/// Counter的名称
		/// </summary>
		public string CounterName
		{
			get;
			set;
		}
	}
}
