using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects
{
	[Serializable]
	[ORTableMapping("WF.APPLICATIONS_COMMON_INFO")]
	public class AppCommonInfo
	{
		private string applicationName = string.Empty;
		private string programName = string.Empty;
		private string resourceID = string.Empty;
		private IOrganization topOU = null;
		private IUser creator = null;
		private IOrganization department = null;
		private DateTime createTime = DateTime.MinValue;
		private string subject = string.Empty;
		private string content = string.Empty;
		private FileEmergency emergency = FileEmergency.None;
		private FileSecret secret = FileSecret.None;
		private string snTitle = string.Empty;
		private string sn_no = string.Empty;
		private string sn_year = string.Empty;
		private string barCode = string.Empty;
		private int register_no = 0;
		private ArchiveStatus status = ArchiveStatus.UnArchive;
		private CompleteFlag completed = CompleteFlag.NoComplete;
		private string url = string.Empty;
		private string draftDepartmentName = string.Empty;

		/// <summary>
		/// 同步流程的属性到Completed Flag
		/// </summary>
		public void SyncProcessStatus(IWfProcess process)
		{
			process.NullCheck("process");

			this.Completed = ConvertProcessStatusToCompletedFlag(process.Status);
		}

		public static CompleteFlag ConvertProcessStatusToCompletedFlag(WfProcessStatus status)
		{
			CompleteFlag result = CompleteFlag.NoComplete;

			switch (status)
			{
				case WfProcessStatus.Aborted:
					result = CompleteFlag.IsCancelled;
					break;
				case WfProcessStatus.Completed:
					result = CompleteFlag.IsComplete;
					break;
			}

			return result;
		}

		#region 公有属性
		[ORFieldMapping("APPLICATION_NAME", IsNullable = false)]
		public string ApplicationName
		{
			get
			{
				return this.applicationName;
			}
			set
			{
				this.applicationName = value;
			}
		}

		[ORFieldMapping("PROGRAM_NAME")]
		public string ProgramName
		{
			get
			{
				return this.programName;
			}
			set
			{
				this.programName = value;
			}
		}

		[ORFieldMapping("RESOURCE_ID", PrimaryKey = true)]
		public string ResourceID
		{
			get
			{
				return this.resourceID;
			}
			set
			{
				this.resourceID = value;
			}
		}

		[SubClassORFieldMapping("ID", "TOPOU_ID", IsNullable = false)]
		[SubClassORFieldMapping("DisplayName", "TOPOU_NAME")]
		[SubClassType(typeof(OguOrganization))]
		public IOrganization TopOU
		{
			get
			{
				return this.topOU;
			}
			set
			{
				this.topOU = (IOrganization)OguBase.CreateWrapperObject(value);
			}
		}

		[SubClassORFieldMapping("ID", "CREATOR_ID", IsNullable = false)]
		[SubClassORFieldMapping("DisplayName", "CREATOR_NAME")]
		[SubClassType(typeof(OguUser))]
		public IUser Creator
		{
			get
			{
				return this.creator;
			}
			set
			{
				this.creator = (IUser)OguBase.CreateWrapperObject(value);
			}
		}

		[SubClassORFieldMapping("ID", "DEPT_ID", IsNullable = false)]
		[SubClassORFieldMapping("DisplayName", "DEPT_NAME")]
		[SubClassType(typeof(OguOrganization))]
		public IOrganization Department
		{
			get
			{
				return this.department;
			}
			set
			{
				this.department = (IOrganization)OguBase.CreateWrapperObject(value);
			}
		}

		[ORFieldMapping("CREATE_TIME")]
		[SqlBehavior(BindingFlags = ClauseBindingFlags.Select, DefaultExpression = "getdate()")]
		public DateTime CreateTime
		{
			get
			{
				return this.createTime;
			}
			set
			{
				this.createTime = value;
			}
		}

		[ORFieldMapping("SUBJECT", IsNullable = false)]
		public string Subject
		{
			get
			{
				return this.subject;
			}
			set
			{
				this.subject = value;
			}
		}

		[ORFieldMapping("CONTENT")]
		public string Content
		{
			get
			{
				return this.content;
			}
			set
			{
				this.content = value;
			}
		}

		[ORFieldMapping("URL")]
		public string Url
		{
			get
			{
				return this.url;
			}
			set
			{
				this.url = value;
			}
		}

		[ORFieldMapping("EMERGENCY")]
		public FileEmergency Emergency
		{
			get
			{
				return this.emergency;
			}
			set
			{
				this.emergency = value;
			}
		}

		[ORFieldMapping("SECRET")]
		[SqlBehavior(EnumUsage = EnumUsageTypes.UseEnumValue)]
		public FileSecret Secret
		{
			get
			{
				return this.secret;
			}
			set
			{
				this.secret = value;
			}
		}

		[ORFieldMapping("SN_TITLE")]
		public string SNTitle
		{
			get
			{
				return this.snTitle;
			}
			set
			{
				this.snTitle = value;
			}
		}

		[ORFieldMapping("SN_YEAR")]
		public string SN_Year
		{
			get
			{
				return this.sn_year;
			}
			set
			{
				this.sn_year = value;
			}
		}

		[ORFieldMapping("SN_NO")]
		public string SN_NO
		{
			get
			{
				return this.sn_no;
			}
			set
			{
				this.sn_no = value;
			}
		}

		[ORFieldMapping("BARCODE")]
		public string BarCode
		{
			get
			{
				return this.barCode;
			}
			set
			{
				this.barCode = value;
			}
		}

		/// <summary>
		/// 登记号（如：收文号）
		/// </summary>
		[ORFieldMapping("REGISTER_NO")]
		public int RegisterNO
		{
			get
			{
				return this.register_no;
			}
			set
			{
				this.register_no = value;
			}
		}

		/// <summary>
		/// 归档状态
		/// </summary>
		[ORFieldMapping("ARCHIVE_STATUS")]
		public ArchiveStatus Status
		{
			get
			{
				return this.status;
			}
			set
			{
				this.status = value;
			}
		}

		/// <summary>
		/// 是否已办结
		/// </summary>
		[ORFieldMapping("COMPLETED_FLAG")]
		public CompleteFlag Completed
		{
			get
			{
				return this.completed;
			}
			set
			{
				this.completed = value;
			}
		}

		/// <summary>
		/// 起草部门的名称
		/// </summary>
		[ORFieldMapping("DRAFT_DEPARTMENT_NAME")]
		public string DraftDepartmentName
		{
			get
			{
				return this.draftDepartmentName;
			}
			set
			{
				this.draftDepartmentName = value;
			}
		}
		#endregion
	}

	public class AppCommonInfoCollection : EditableDataObjectCollectionBase<AppCommonInfo>
	{
		internal void LoadFromDataView(DataView dv)
		{
			this.Clear();

			ORMapping.DataViewToCollection(this, dv);
		}
	}
}
