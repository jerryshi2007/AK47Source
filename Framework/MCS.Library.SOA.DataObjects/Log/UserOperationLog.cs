using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using System.Xml;
using System.Web;
using System.Diagnostics;
using MCS.Library.Data.Mapping;
using MCS.Library.Validation;
using MCS.Library.OGUPermission;
using MCS.Library.Data.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.Principal;

namespace MCS.Library.SOA.DataObjects
{
	public enum OperationType
	{
		[EnumItemDescription("添加文件")]
		Add = 0,

		[EnumItemDescription("修改文件")]
		Update = 1,

		[EnumItemDescription("删除文件")]
		Delete = 2,

		[EnumItemDescription("修改意见")]
		ModifyOpinion = 3,

		[EnumItemDescription("异常处理修改")]
		ExceptionOperation = 4,

		[EnumItemDescription("为流转而打开表单")]
		OpenFormForMove = 5,

		[EnumItemDescription("打开表单")]
		OpenForm = 6,

		[EnumItemDescription("其他修改")]
		Other = 8192
	}

	/// <summary>
	/// 用户操作日志
	/// </summary>
	[Serializable]
	[XElementSerializable]
	[ORTableMapping("WF.USER_OPERATION_LOG")]
	public class UserOperationLog
	{
		#region 私有字段
		private int id = 0;
		//private int sortID;
		private string applicationName;
		private string programName;
		private string resourceID;
		private string processID = string.Empty;
		private string activityID = string.Empty;
		private string activityName;
		private IUser safeNameOperator;
		private IOrganization topDepartment;
		private DateTime? operationDateTime;
		private string operationName;
		private string operationDescription;
		private IUser realUser;
		private string subject;
		//private AppModifyDetail operationDetail;
		//private AppModifyDetail OperationDetail
		//{
		//    get
		//    {
		//        return operationDetail;
		//    }
		//    set
		//    {
		//        operationDetail = value;
		//    }
		//}
		private string correlationID;
		private OperationType operationType = OperationType.Update;

		#endregion

		#region 公共字段
		/// <summary>
		/// 排序ID
		/// </summary>
		[ORFieldMapping("ID", IsIdentity = true, PrimaryKey = true)]
		public int ID
		{
			get
			{
				return this.id;
			}
			set
			{
				this.id = value;
			}
		}

		/// <summary>
		/// 文件ID
		/// </summary>
		[ORFieldMapping("RESOURCE_ID")]
		[StringLengthValidator(0, 36, MessageTemplate = "ResourceID 应为Guid的String类型")]
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

		/// <summary>
		/// 应用名称
		/// </summary>
		[ORFieldMapping("APPLICATION_NAME")]
		[StringLengthValidator(32, MessageTemplate = "ApplicationName 长度不能超过32")]
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

		/// <summary>
		/// 程序名称
		/// </summary>
		[ORFieldMapping("PROGRAM_NAME")]
		[StringLengthValidator(32, MessageTemplate = "ProgramName 长度不能超过32")]
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

		/// <summary>
		/// 流程ID
		/// </summary>
		[ORFieldMapping("PROCESS_ID")]
		[StringLengthValidator(0, 36, MessageTemplate = "ProcessID 应为Guid的String类型")]
		public string ProcessID
		{
			get
			{
				return this.processID;
			}
			set
			{
				this.processID = value;
			}
		}

		/// <summary>
		/// 流程节点ID
		/// </summary>
		[ORFieldMapping("ACTIVITY_ID")]
		[StringLengthValidator(0, 36, MessageTemplate = "ActivityID 应为Guid的String类型")]
		public string ActivityID
		{
			get
			{
				return this.activityID;
			}
			set
			{
				this.activityID = value;
			}
		}

		/// <summary>
		/// 流程节点名称
		/// </summary>
		[ORFieldMapping("ACTIVITY_NAME")]
		[StringLengthValidator(64, MessageTemplate = "ActivityName 长度不能超过64")]
		public string ActivityName
		{
			get
			{
				return this.activityName;
			}
			set
			{
				this.activityName = value;
			}
		}

		/// <summary>
		/// 操作者
		/// </summary>
		[SubClassORFieldMapping("ID", "OPERATOR_ID")]
		[SubClassORFieldMapping("DisplayName", "OPERATOR_NAME")]
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

		/// <summary>
		/// 顶级部门
		/// </summary>
		[SubClassORFieldMapping("ID", "TOP_DEPT_ID")]
		[SubClassORFieldMapping("DisplayName", "TOP_DEPT_NAME")]
		[SubClassType(typeof(OguOrganization))]
		public IOrganization TopDepartment
		{
			get
			{
				return this.topDepartment;
			}
			set
			{
				this.topDepartment = (IOrganization)OguOrganization.CreateWrapperObject(value);
			}
		}

		/// <summary>
		/// 操作时间
		/// </summary>
		[ORFieldMapping("OPERATE_DATETIME")]
		[SqlBehavior(DefaultExpression = "GETDATE()")]
		public DateTime? OperationDateTime
		{
			get
			{
				return this.operationDateTime;
			}
			set
			{
				this.operationDateTime = value;
			}
		}

		/// <summary>
		/// 操作名称
		/// </summary>
		[ORFieldMapping("OPERATE_NAME")]
		[StringLengthValidator(32, MessageTemplate = "OperationName 长度不能超过32")]
		public string OperationName
		{
			get
			{
				return this.operationName;
			}
			set
			{
				this.operationName = value;
			}
		}

		[ORFieldMapping("OPERATE_TYPE")]
		public OperationType OperationType
		{
			get
			{
				return this.operationType;
			}
			set
			{
				this.operationType = value;
			}
		}

		/// <summary>
		/// 操作描述
		/// </summary>
		[ORFieldMapping("OPERATE_DESCRIPTION")]
		[StringLengthValidator(1024, MessageTemplate = "OperationDescription 长度不能超过1024")]
		public string OperationDescription
		{
			get
			{
				return this.operationDescription;
			}
			set
			{
				this.operationDescription = value;
			}
		}

		/// <summary>
		/// 客户端类型
		/// </summary>
		[ORFieldMapping("HTTP_CONTEXT")]
		public string HttpContextString
		{
			get
			{
				return GetHttpContextXml();
			}
		}

		/// <summary>
		/// 真实用户
		/// </summary>
		[SubClassORFieldMapping("ID", "REAL_USER_ID")]
		[SubClassORFieldMapping("DisplayName", "REAL_USER_NAME")]
		[SubClassType(typeof(OguUser))]
		public IUser RealUser
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
		/// 文件名称
		/// </summary>
		[ORFieldMapping("SUBJECT")]
		[StringLengthValidator(512, MessageTemplate = "Subject 长度不能超过512")]
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

		/// <summary>
		/// 相关ID
		/// </summary>
		[ORFieldMapping("CORRELATION_ID")]
		[StringLengthValidator(0, 36, MessageTemplate = "CorrelationID 应为Guid的String类型")]
		public string CorrelationID
		{
			get
			{
				if (string.IsNullOrEmpty(correlationID))
				{
					CorrelationManager cm = Trace.CorrelationManager;
					this.correlationID = cm.ActivityId.ToString();
				}
				return this.correlationID;
			}
		}

		/// <summary>
		/// 传送目节点信息
		/// </summary>
		[NoMapping]
		public string TargetDescription
		{
			get;
			set;
		}
		#endregion

		#region 构造函数
		public UserOperationLog()
		{

		}
		#endregion

		#region 公共静态方法
		/// <summary>
		/// 从Activity构造
		/// </summary>
		/// <param name="activity"></param>
		/// <returns></returns>
		public static UserOperationLog FromActivity(IWfActivity activity)
		{
			UserOperationLog log = new UserOperationLog();

			log.ResourceID = activity.Process.ResourceID;
			log.ApplicationName = activity.Process.Descriptor.ApplicationName;
			log.ProgramName = activity.Process.Descriptor.ProgramName;

			log.ProcessID = activity.Process.ID;
			log.ActivityID = activity.ID;

			if (activity.Descriptor.TaskTitle.IsNotEmpty())
				log.Subject = activity.Descriptor.TaskTitle;
			else
				log.Subject = activity.Process.Descriptor.DefaultTaskTitle;

			log.Subject = activity.Process.ApplicationRuntimeParameters.GetMatchedString(log.Subject);

			log.ActivityName = activity.Descriptor.Name;

			if (DeluxePrincipal.IsAuthenticated)
			{
				log.Operator = DeluxeIdentity.CurrentUser;
				log.RealUser = DeluxeIdentity.CurrentRealUser;
			}

			log.TopDepartment = activity.RootActivity.Process.OwnerDepartment;

			return log;
		}
		#endregion

		#region 私有方法
		private string GetHttpContextXml()
		{
			string str = string.Empty;

			if (EnvironmentHelper.Mode == InstanceMode.Web)
			{
				XmlDocument xmlDoc = XmlHelper.CreateDomDocument("<HttpContext />");
				XmlNode xmlNode = xmlDoc.DocumentElement;

				if (HttpContext.Current != null)
				{
					HttpRequest request = HttpContext.Current.Request;

					string userAgent = string.Empty;
					if (request.UserAgent != null)
					{
						userAgent = request.UserAgent;
					}

					XmlHelper.AppendNode(xmlNode, "ContentEncoding", request.ContentEncoding.ToString());
					XmlHelper.AppendNode(xmlNode, "ContentLength", request.ContentLength.ToString());
					XmlHelper.AppendNode(xmlNode, "Url", request.Url.ToString());
					XmlHelper.AppendNode(xmlNode, "UserAgent", userAgent);
					XmlHelper.AppendNode(xmlNode, "UserHostAddress", request.UserHostAddress);
					XmlHelper.AppendNode(xmlNode, "UserHostName", request.UserHostName);
				}
			}

			return str;
		}
		#endregion
	}

	/// <summary>
	/// 用户操作日志的集合
	/// </summary>
	[Serializable]
	[XElementSerializable]
	public class UserOperationLogCollection : EditableDataObjectCollectionBase<UserOperationLog>
	{
	}
}
