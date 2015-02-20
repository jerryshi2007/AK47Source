using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.Contracts.DataObjects
{
	/// <summary>
	/// 参照TypeCode枚举值
	/// </summary>
	public enum ClientPropertyDataType
	{
		DataObject = 1,
		Boolean = 3,
		Integer = 9,
		Decimal = 15,
		DateTime = 16,
		String = 18,
		Enum = 20
	}
}
