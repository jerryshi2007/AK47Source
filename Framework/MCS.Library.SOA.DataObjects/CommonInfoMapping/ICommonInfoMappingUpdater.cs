using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// CommonInfoMapping更新接口
	/// </summary>
	public interface ICommonInfoMappingUpdater
	{
		void Update(CommonInfoMappingCollection cimItems);
	}
}
