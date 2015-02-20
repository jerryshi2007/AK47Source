using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.Mapping;
using System.Diagnostics;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Security.Actions;
using MCS.Library.Principal;
using MCS.Library.OGUPermission;
using MCS.Library.Data.DataObjects;
using MCS.Library.SOA.DataObjects.Security.AUObjects.Executors;
using MCS.Library.SOA.DataObjects.Security.AUObjects.Actions;
using MCS.Library.SOA.DataObjects.Schemas.Actions;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects.Logs
{
	[Serializable]
	[ORTableMapping("SC.OperationLog")]
	public class AUOperationLog
	{
		/// <summary>
		/// 根据环境信息初始化Log。
		/// 初始化的属性包括：CorrelationID、RequestContextString、Operator、RealOperator
		/// </summary>
		/// <returns></returns>
		public static AUOperationLog CreateLogFromEnvironment()
		{
			AUOperationLog log = new AUOperationLog();

			log.CorrelationID = Trace.CorrelationManager.ActivityId.ToString();
			log.RequestContextString = EnvironmentHelper.GetEnvironmentInfo();
			log.CreateTime = SCActionContext.Current.TimePoint;

			if (DeluxePrincipal.IsAuthenticated)
			{
				log.Operator = DeluxeIdentity.CurrentUser;
				log.RealOperator = DeluxeIdentity.CurrentRealUser;
			}

			return log;
		}

		[ORFieldMapping("ID", PrimaryKey = true, IsIdentity = true)]
		public int ID { get; set; }

		[ORFieldMapping("ResourceID")]
		public string ResourceID { get; set; }

		[ORFieldMapping("CorrelationID")]
		public string CorrelationID { get; set; }

		[ORFieldMapping("Category")]
		public string Category { get; set; }

		private IUser safeNameOperator = null;

		/// <summary>
		/// 操作者
		/// </summary>
		[SubClassORFieldMapping("ID", "OperatorID")]
		[SubClassORFieldMapping("DisplayName", "OperatorName")]
		[SubClassType(typeof(OguUser))]
		public IUser Operator
		{
			get
			{
				return this.safeNameOperator;
			}
			set
			{
				this.safeNameOperator = (IUser)OguUser.CreateWrapperObject(value);
			}
		}

		private IUser realUser = null;

		[SubClassORFieldMapping("ID", "RealOperatorID")]
		[SubClassORFieldMapping("DisplayName", "RealOperatorName")]
		[SubClassType(typeof(OguUser))]
		public IUser RealOperator
		{
			get
			{
				return this.realUser;
			}
			set
			{
				this.realUser = (IUser)OguUser.CreateWrapperObject(value);
			}
		}

		/// <summary>
		/// 客户端类型
		/// </summary>
		[ORFieldMapping("RequestContext")]
		public string RequestContextString { get; set; }

		[ORFieldMapping("Subject")]
		public string Subject { get; set; }

		[ORFieldMapping("SchemaType")]
		public string SchemaType { get; set; }

		[ORFieldMapping("OperationType")]
		[SqlBehavior(EnumUsage = EnumUsageTypes.UseEnumString)]
		public AUOperationType OperationType { get; set; }

		[ORFieldMapping("CreateTime")]
		[SqlBehavior(DefaultExpression = "GETDATE()")]
		public DateTime CreateTime { get; set; }

		[ORFieldMapping("SearchContent")]
		public string SearchContent { get; set; }
	}
}
