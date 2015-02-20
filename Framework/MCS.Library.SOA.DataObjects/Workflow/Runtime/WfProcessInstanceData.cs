using System;
using System.Xml.Linq;
using MCS.Library.Core;
using MCS.Library.Data.Mapping;
using MCS.Library.OGUPermission;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	[ORTableMapping("WF.PROCESS_INSTANCES")]
	[Serializable]
	public class WfProcessInstanceData
	{
		private bool _Committed = true;

		[ORFieldMapping("INSTANCE_ID", PrimaryKey = true)]
		public string InstanceID
		{
			get;
			set;
		}

		[ORFieldMapping("CURRENT_ACTIVITY_ID")]
		public string CurrentActivityID
		{
			get;
			set;
		}

		[ORFieldMapping("OWNER_ACTIVITY_ID")]
		public string OwnerActivityID
		{
			get;
			set;
		}

		[ORFieldMapping("OWNER_TEMPLATE_KEY")]
		public string OwnerTemplateKey
		{
			get;
			set;
		}

		[ORFieldMapping("SEQUENCE")]
		public int Sequence
		{
			get;
			set;
		}

		[ORFieldMapping("RESOURCE_ID")]
		public string ResourceID
		{
			get;
			set;
		}

		[ORFieldMapping("STATUS")]
		[SqlBehavior(EnumUsage = EnumUsageTypes.UseEnumString)]
		public WfProcessStatus Status
		{
			get;
			set;
		}

		[ORFieldMapping("DESCRIPTOR_KEY")]
		public string DescriptorKey
		{
			get;
			set;
		}

		[ORFieldMapping("PROCESS_NAME")]
		public string ProcessName
		{
			get;
			set;
		}

		[ORFieldMapping("APPLICATION_NAME")]
		public string ApplicationName
		{
			get;
			set;
		}

		[ORFieldMapping("PROGRAM_NAME")]
		public string ProgramName
		{
			get;
			set;
		}

		[ORFieldMapping("START_TIME")]
		public DateTime StartTime
		{
			get;
			set;
		}

		[ORFieldMapping("END_TIME")]
		public DateTime EndTime
		{
			get;
			set;
		}

		//[SqlBehavior(BindingFlags = ClauseBindingFlags.Select | ClauseBindingFlags.Where)]
		[ORFieldMapping("DATA")]
		public string Data
		{
			get;
			set;
		}

		/// <summary>
		/// 流程的扩展描述信息
		/// </summary>
		[ORFieldMapping("EXT_DATA")]
		public string ExtData
		{
			get;
			set;
		}

		[ORFieldMapping("BIN_DATA")]
		public byte[] BinaryData
		{
			get;
			set;
		}

		[ORFieldMapping("CREATE_TIME")]
		[SqlBehavior(BindingFlags = ClauseBindingFlags.Select | ClauseBindingFlags.Where)]
		public DateTime CreateTime
		{
			get;
			set;
		}

		private IUser _Creator = null;

		[SubClassORFieldMapping("ID", "CREATOR_ID")]
		[SubClassSqlBehavior("ID", ClauseBindingFlags.Insert)]
		[SubClassORFieldMapping("DisplayName", "CREATOR_NAME")]
		[SubClassSqlBehavior("DisplayName", ClauseBindingFlags.Insert)]
		[SubClassORFieldMapping("FullPath", "CREATOR_PATH")]
		[SubClassSqlBehavior("FullPath", ClauseBindingFlags.Insert)]
		[SubClassType(typeof(OguUser))]
		public IUser Creator
		{
			get
			{
				return this._Creator;
			}
			set
			{
				this._Creator = (IUser)OguUser.CreateWrapperObject(value);
			}
		}

		private IOrganization _Department = null;

		[SubClassORFieldMapping("ID", "DEPARTMENT_ID")]
		[SubClassORFieldMapping("DisplayName", "DEPARTMENT_NAME")]
		[SubClassORFieldMapping("FullPath", "DEPARTMENT_PATH")]
		[SubClassType(typeof(OguOrganization))]
		public IOrganization Department
		{
			get
			{
				return this._Department;
			}
			set
			{
				this._Department = value;
			}
		}

		[ORFieldMapping("UPDATE_TAG")]
		[SqlBehavior(BindingFlags = ClauseBindingFlags.Select | ClauseBindingFlags.Where)]
		public int UpdateTag
		{
			get;
			set;
		}

		/// <summary>
		/// 流程是否是提交的。如果为False，则表示是用户打开表单，启动了流程，但是没有保存和流转
		/// </summary>
		[ORFieldMapping("COMMITTED")]
		public bool Committed
		{
			get
			{
				return this._Committed;
			}
			set
			{
				this._Committed = value;
			}
		}

		/// <summary>
		/// 从流程对象生成入库的对象
		/// </summary>
		/// <param name="process"></param>
		/// <returns></returns>
		public static WfProcessInstanceData FromProcess(IWfProcess process)
		{
			process.NullCheck("process");

			WfProcessInstanceData instanceData = new WfProcessInstanceData();

			instanceData.InstanceID = process.ID;
			instanceData.ResourceID = process.ResourceID;
			instanceData.Status = process.Status;
			instanceData.StartTime = process.StartTime;
			instanceData.EndTime = process.EndTime;
			instanceData.ApplicationName = process.Descriptor.ApplicationName;
			instanceData.ProcessName = GetRuntimeProcessName(process);
			process.Context["RuntimeProcessName"] = instanceData.ProcessName;

			instanceData.DescriptorKey = process.Descriptor.Key;
			instanceData.ProgramName = process.Descriptor.ProgramName;
			instanceData._Creator = process.Creator;
			instanceData._Department = process.OwnerDepartment;
			instanceData.Sequence = process.Sequence;
			instanceData.OwnerActivityID = ((WfProcess)process).OwnerActivityID;
			instanceData.OwnerTemplateKey = ((WfProcess)process).OwnerTemplateKey;
			instanceData.Committed = process.Committed;
			instanceData.ExtData = process.Context.GetValue("SerilizationExtData", GetDefaultExtData(process));

			XElementFormatter formatter = new XElementFormatter();

			formatter.OutputShortType = WorkflowSettings.GetConfig().OutputShortType;

			instanceData.Data = formatter.Serialize(process).ToString();

			if (process.CurrentActivity != null)
				instanceData.CurrentActivityID = process.CurrentActivity.ID;

			return instanceData;
		}

		private static string GetRuntimeProcessName(IWfProcess process)
		{
			string runtimeProcessName = process.Descriptor.Properties.GetValue("RuntimeProcessName", string.Empty);

			if (runtimeProcessName.IsNotEmpty())
				runtimeProcessName = process.ApplicationRuntimeParameters.GetMatchedString(runtimeProcessName);

			if (runtimeProcessName.IsNullOrEmpty())
				runtimeProcessName = process.Descriptor.Name;

			return runtimeProcessName;
		}

		private static string GetDefaultExtData(IWfProcess process)
		{
			XElement result = XElement.Parse("<ExtData />");

			string encodingName = "gb2312";

			if (process.LoadingType == DataLoadingType.Memory)
				encodingName = "utf-8";

			result.SetAttributeValue("encoding", encodingName);

			return result.ToString();
		}
	}

	[Serializable]
	public class WfProcessInstanceDataCollection : EditableDataObjectCollectionBase<WfProcessInstanceData>
	{
	}
}
