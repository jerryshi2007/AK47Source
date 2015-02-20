using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 可复制的业务对象
	/// </summary>
	public interface IClonableBusinessObject
	{
		/// <summary>
		/// 复制出一个新的业务实体
		/// </summary>
		/// <returns></returns>
		IClonableBusinessObject GenerateNewObject();
	}
}
