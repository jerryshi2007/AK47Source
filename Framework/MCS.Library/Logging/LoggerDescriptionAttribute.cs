using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Logging
{
	/// <summary>
	/// Logger描述信息的属性
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Assembly, AllowMultiple = false, Inherited = true)]
	public class LoggerDescriptionAttribute : Attribute
	{
		/// <summary>
		/// 构造方法
		/// </summary>
		public LoggerDescriptionAttribute()
		{
		}

		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="name"></param>
		public LoggerDescriptionAttribute(string name)
		{
			this.Name = name;
		}

		/// <summary>
		/// Logger的名称
		/// </summary>
		public string Name
		{
			get;
			set;
		}
	}

	/// <summary>
	/// Log的源描述信息
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Assembly, AllowMultiple = false, Inherited = true)]
	public class LogSourceAttribute : Attribute
	{
		/// <summary>
		/// 构造方法
		/// </summary>
		public LogSourceAttribute()
		{
		}

		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="name"></param>
		public LogSourceAttribute(string name)
		{
			this.Name = name;
		}

		/// <summary>
		/// Logger的名称
		/// </summary>
		public string Name
		{
			get;
			set;
		}
	}
}
