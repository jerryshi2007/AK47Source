using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using System.Transactions;
using MCS.Library.Data;
using MCS.Library.Data.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;


namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 搜索映射表
	/// </summary>
	[Serializable]
	[ORTableMapping("WF.COMMON_INFO_MAPPING")]
    [TenantRelativeObject]
	public class CommonInfoMapping
	{
		[ORFieldMapping("COMMON_INFO_ID")]
		public string CommonInfoID
		{
			get;
			set;
		}

		[ORFieldMapping("RESOURCE_ID", PrimaryKey = true)]
		public string ResourceID
		{
			get;
			set;
		}

		[ORFieldMapping("PROCESS_ID", PrimaryKey = true)]
		public string ProcessID
		{
			get;
			set;
		}
	}

	/// <summary>
	/// 搜索映射信息表条目的集合
	/// </summary>
	[Serializable]
	public class CommonInfoMappingCollection : EditableDataObjectCollectionBase<CommonInfoMapping>
	{
		public void FromProcesses(IEnumerable<IWfProcess> processItems)
		{
			this.Clear();

			foreach (WfProcess process in processItems)
			{
				CommonInfoMapping cim = new CommonInfoMapping();
				cim.ResourceID = process.ResourceID;
				cim.ProcessID = process.ID;
				cim.CommonInfoID = process.SearchID;

				this.Add(cim);
			}
		}
	}

}
