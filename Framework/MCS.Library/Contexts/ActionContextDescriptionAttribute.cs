using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Core
{
	/// <summary>
	/// 描述ActionContext的Key信息
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class ActionContextDescriptionAttribute : Attribute
	{
		/// <summary>
		/// 构造方法
		/// </summary>
		public ActionContextDescriptionAttribute()
		{
		}

		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="key"></param>
		public ActionContextDescriptionAttribute(string key)
		{
			this.Key = key;
		}

		/// <summary>
		/// Key值，这个值用于ObjectContextQueue中Key值
		/// </summary>
		public string Key
		{
			get;
			set;
		}
	}
}
