using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Expression
{
	/// <summary>
	/// 内置函数的属性说明
	/// </summary>
	[Serializable]
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public class BuiltInFunctionAttribute : Attribute
	{
		/// <summary>
		/// 构造方法
		/// </summary>
		public BuiltInFunctionAttribute()
		{

		}

		/// <summary>
		/// 构造方法
		/// </summary>
		public BuiltInFunctionAttribute(string funcName)
		{
			this.FunctionName = funcName;
		}

		/// <summary>
		/// 构造方法
		/// </summary>
		public BuiltInFunctionAttribute(string funcName, string description)
		{
			this.FunctionName = funcName;
			this.Description = description;
		}

		/// <summary>
		/// 函数名称
		/// </summary>
		public string FunctionName
		{
			get;
			set;
		}

		/// <summary>
		/// 函数说明
		/// </summary>
		public string Description
		{
			get;
			set;
		}
	}
}
