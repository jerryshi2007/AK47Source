using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 扩展的流程信息保存器，它的执行将发生在流程保存之后
	/// </summary>
	public interface IWfExtraProcessPersistManager
	{
		/// <summary>
		/// 保存数据
		/// </summary>
		/// <param name="process">流程对象</param>
		/// <param name="context">上下文参数</param>
		void SaveData(IWfProcess process, Dictionary<object, object> context);

		/// <summary>
		/// 删除数据
		/// </summary>
		/// <param name="processesInfo">流程信息</param>
		/// <param name="context"></param>
		void DeleteData(WfProcessCurrentInfoCollection processesInfo, Dictionary<object, object> context);
	}

	/// <summary>
	/// 扩展的流程信息保存器集合
	/// </summary>
	public class WfExtraProcessPersistManagerCollection : EditableDataObjectCollectionBase<IWfExtraProcessPersistManager>
	{
		public void SaveData(IWfProcess process, Dictionary<object, object> context)
		{
			this.ForEach(p => p.SaveData(process, context));
		}

		public void DeleteData(WfProcessCurrentInfoCollection processesInfo, Dictionary<object, object> context)
		{
			this.ForEach(p => p.DeleteData(processesInfo, context));
		}
	}
}
