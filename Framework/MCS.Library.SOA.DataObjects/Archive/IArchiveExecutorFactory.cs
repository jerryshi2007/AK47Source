using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Archive
{
	/// <summary>
	/// 得到归档执行器的工厂
	/// </summary>
	public interface IArchiveExecutorFactory
	{
		/// <summary>
		/// 得到归档执行器
		/// </summary>
		/// <param name="info"></param>
		/// <returns></returns>
		IArchiveExecutor GetArchiveExecutor(ArchiveBasicInfo info);
	}
}
