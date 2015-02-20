using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Archive
{
	/// <summary>
	/// 归档操作细节的接口定义。这是每一类具体归档数据操作的实现类。每一个具体操作与事务无关，由外层控制
	/// </summary>
	public interface IArchiveOperation
	{
		/// <summary>
		/// 加载原始数据
		/// </summary>
		/// <param name="info"></param>
		void LoadOriginalData(ArchiveBasicInfo info);

		/// <summary>
		/// 保存归档数据
		/// </summary>
		/// <param name="info"></param>
		void SaveArchiveData(ArchiveBasicInfo info);

		/// <summary>
		/// 删除原始数据
		/// </summary>
		/// <param name="info"></param>
		void DeleteOriginalData(ArchiveBasicInfo info);
	}
}
