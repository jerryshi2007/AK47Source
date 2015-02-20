using System;
using System.Collections.Generic;
using System.Text;
using MCS.Library.Core;

namespace MCS.Web.Responsive.Library.MVC
{
	/// <summary>
	/// 标识出类中的方法是控制器方法
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, Inherited = true)]
	public sealed class ControllerMethodAttribute : Attribute
	{
		private bool isDefault = true;

		/// <summary>
		/// 构造方法
		/// </summary>
		public ControllerMethodAttribute()
		{
		}

		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="defaultMethod"></param>
		public ControllerMethodAttribute(bool defaultMethod)
		{
			this.isDefault = defaultMethod;
		}

		/// <summary>
		/// 构造方法。用逗号或分号分隔的参数名称列表。一旦有这里面的参数，则强制忽略此方法
		/// </summary>
		/// <param name="forceIgnoreParameters"></param>
		public ControllerMethodAttribute(string forceIgnoreParameters)
		{
			this.ForceIgnoreParameters = forceIgnoreParameters;
		}

		/// <summary>
		/// 是否是缺省的方法
		/// </summary>
		public bool Default
		{
			get
			{
				return this.isDefault;
			}
			set
			{
				this.isDefault = value;
			}
		}

		/// <summary>
		/// 用逗号或分号分隔的参数名称列表。一旦有这里面的参数，则强制忽略此方法
		/// </summary>
		public string ForceIgnoreParameters
		{
			get;
			set;
		}
	}
}
