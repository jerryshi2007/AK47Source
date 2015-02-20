using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 用于Variable的DataType枚举的额外声明
	/// </summary>
	[AttributeUsageAttribute(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
	public class DataTypeDescriptionAttribute : Attribute
	{
		/// <summary>
		/// 构造方法
		/// </summary>
		public DataTypeDescriptionAttribute()
		{
		}

		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="typeDescription">类型描述</param>
		public DataTypeDescriptionAttribute(string typeDescription)
		{
			this.TypeDescription = typeDescription;
		}

		/// <summary>
		/// 类型描述类型
		/// </summary>
		public string TypeDescription
		{
			get;
			set;
		}
	}
}
