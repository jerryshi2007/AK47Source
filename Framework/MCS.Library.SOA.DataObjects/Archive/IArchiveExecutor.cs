using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Archive
{
	/// <summary>
	/// 归档操作的执行器
	/// </summary>
	public interface IArchiveExecutor
	{
		/// <summary>
		/// 归档
		/// </summary>
		void Archive(ArchiveBasicInfo info);

		/// <summary>
		/// 删除
		/// </summary>
		/// <param name="info"></param>
		void Delete(ArchiveBasicInfo info);
	}
}
