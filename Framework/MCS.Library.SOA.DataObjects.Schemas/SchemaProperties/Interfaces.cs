using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Schemas.SchemaProperties
{
	/// <summary>
	/// 带状态的对象
	/// </summary>
	public interface ISCStatusObject
	{
		SchemaObjectStatus Status
		{
			get;
		}
	}

	/// <summary>
	/// 表示此对象的属性可以动态扩展
	/// </summary>
	public interface IPropertyExtendedObject
	{
		void EnsureExtendedProperties();
	}
}
