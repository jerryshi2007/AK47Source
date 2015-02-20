using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 业务实体的基本要素
	/// </summary>
	public interface IBusinessObject
	{
		string ID { get; set; }
		string Subject { get; set; }
	}
}
