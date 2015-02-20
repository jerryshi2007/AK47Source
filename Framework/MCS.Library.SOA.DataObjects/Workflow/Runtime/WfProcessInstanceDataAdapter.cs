using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.Mapping;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 直接读写流程信息表的访问类
	/// </summary>
	public class WfProcessInstanceDataAdapter : UpdatableAndLoadableAdapterBase<WfProcessInstanceData, WfProcessInstanceDataCollection>
	{
		public static readonly WfProcessInstanceDataAdapter Instance = new WfProcessInstanceDataAdapter();

		private WfProcessInstanceDataAdapter()
		{
		}

		public WfProcessInstanceDataCollection LoadByResourceID(string resourceID)
		{
			resourceID.CheckStringIsNullOrEmpty("resourceID");

			return LoadByInBuilder(b =>
			{
				b.DataField = "RESOURCE_ID";
				b.AppendItem(resourceID);
			});
		}

		protected override ORMappingItemCollection GetMappingInfo(Dictionary<string, object> context)
		{
			ORMappingItemCollection result = base.GetMappingInfo(context).Clone();

			result["DATA"].BindingFlags = ClauseBindingFlags.All;
			result["UPDATE_TAG"].BindingFlags = ClauseBindingFlags.All;
			result["CREATE_TIME"].BindingFlags = ClauseBindingFlags.All;

			return result;
		}
	}
}
