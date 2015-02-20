using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 读写流程数据的接口
	/// </summary>
	public interface IWfProcessPersistManager
	{
		IWfProcess LoadProcessByProcessID(string processID);

		IWfProcess LoadProcessByActivityID(string activityID);

		WfProcessCollection LoadProcessByResourceID(string resourceID);

		WfProcessCollection LoadProcessByOwnerActivityID(string activityID, string templateKey);

		/// <summary>
		/// 读取子流程的信息，只包含状态信息
		/// </summary>
		/// <param name="resourceID"></param>
		/// <param name="templateKey"></param>
		/// <param name="includeAborted">是否包含已经作废的流程</param>
		/// <returns></returns>
		WfProcessCurrentInfoCollection LoadProcessInfoOwnerActivityID(string activityID, string templateKey, bool includeAborted);

		void DeleteProcessByProcessID(string processID);
		void DeleteProcessByActivityID(string activityID);
		void DeleteProcessByResourceID(string resourceID);

		void SaveProcess(IWfProcess process);
	}
}
