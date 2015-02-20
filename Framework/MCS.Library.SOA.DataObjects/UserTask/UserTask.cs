#region 作者版本
// -------------------------------------------------
// Assembly	：	HB.DataObjects
// FileName	：	UserTask.cs
// Remark	：	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    李苗	    20070723		创建
// -------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using MCS.Library.Data.Mapping;
using MCS.Library;
using MCS.Library.Core;
using System.Web.Script.Serialization;
using MCS.Library.SOA.DataObjects.Workflow;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 待办箱实体类
	/// </summary>
	[Serializable]
	[ORTableMapping("WF.USER_TASK")]
	public class UserTask : IUserTask
	{
		private string taskID = string.Empty;
		private string appID = string.Empty;
		private string progID = string.Empty;

		private string taskTitle = string.Empty;
		private string resourceID = string.Empty;
		private string sourceID = string.Empty;
		private string sourceName = string.Empty;
		private string sendToUserID = string.Empty;

		private string body = string.Empty;

		private string processID = string.Empty;
		private string activityID = string.Empty;
		private TaskLevel level = TaskLevel.Normal;
		private string url = string.Empty;
		private FileEmergency emergency = FileEmergency.None;
		private string purpose = string.Empty;
		private TaskStatus status = TaskStatus.Ban;
		private DateTime taskStartTime = DateTime.MinValue;
		private DateTime expireTime = DateTime.MinValue;
		private DateTime readTime = DateTime.MinValue;

		private TaskCategory category = null;
		private int topFlag = 0;
		private DateTime completedTime = DateTime.MinValue;
		private DateTime deliverTime = DateTime.MinValue;
		private string draftDepartmentName = string.Empty;
		private string draftUserID = string.Empty;
		private string draftUserName = string.Empty;

		private Dictionary<string, object> context = null;
		#region
		/// <summary>
		/// 消息ID
		/// </summary>
		[ORFieldMapping("TASK_GUID")]
		public string TaskID
		{
			get
			{
				return taskID;
			}
			set
			{
				taskID = value;
			}
		}

		/// <summary>
		/// 应用名称
		/// </summary>
		[ORFieldMapping("APPLICATION_NAME")]
		public string ApplicationName
		{
			get
			{
				return this.appID;
			}
			set
			{
				this.appID = value;
			}
		}

		/// <summary>
		/// 模块名称
		/// </summary>
		[ORFieldMapping("PROGRAM_NAME")]
		public string ProgramName
		{
			get
			{
				return this.progID;
			}
			set
			{
				this.progID = value;
			}
		}

		/// <summary>
		/// 标题
		/// </summary>
		[ORFieldMapping("TASK_TITLE")]
		public string TaskTitle
		{
			get
			{
				return this.taskTitle;
			}
			set
			{
				this.taskTitle = value;
			}
		}

		/// <summary>
		/// 资源ID
		/// </summary>
		[ORFieldMapping("RESOURCE_ID")]
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
		/// 发件人ID
		/// </summary>
		[ORFieldMapping("SOURCE_ID")]
		public string SourceID
		{
			get
			{
				return this.sourceID;
			}
			set
			{
				this.sourceID = value;
			}
		}

		/// <summary>
		/// 发件人名称
		/// </summary>
		[ORFieldMapping("SOURCE_NAME")]
		public string SourceName
		{
			get
			{
				return this.sourceName;
			}
			set
			{
				this.sourceName = value;
			}
		}

		[ORFieldMapping("SEND_TO_USER")]
		public string SendToUserID
		{
			get
			{
				return this.sendToUserID;
			}
			set
			{
				this.sendToUserID = value;
			}
		}

		[ORFieldMapping("SEND_TO_USER_NAME")]
		public string SendToUserName
		{
			get;
			set;
		}

		[ORFieldMapping("DATA")]
		public string Body
		{
			get
			{
				return this.body;
			}
			set
			{
				this.body = value;
			}
		}

		/// <summary>
		/// 消息类型
		/// </summary>
		[ORFieldMapping("TASK_LEVEL")]
		public TaskLevel Level
		{
			get
			{
				return this.level;
			}
			set
			{
				this.level = value;
			}
		}

		/// <summary>
		/// 流程ID
		/// </summary>
		[ORFieldMapping("PROCESS_ID")]
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
		/// 工作流节点ID
		/// </summary>
		[ORFieldMapping("ACTIVITY_ID")]
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
		/// 待办事项的链接
		/// </summary>
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

		/// <summary>
		/// 得到绝对路径的Url
		/// </summary>
		[NoMapping]
		public string NormalizedUrl
		{
			get
			{
				return GetNormalizedUrl(this.ApplicationName, this.ProgramName, this.url);
			}
		}

		/// <summary>
		/// 计算绝对路径的Url。如果url不是绝对路径，那么先根据配置文件resourceUriSettings的userTaskRoot项设置。
		/// 如果根路径还是空，则使用WfGlobalParameters的FormBaseUrl
		/// </summary>
		public static string GetNormalizedUrl(string url)
		{
			return GetNormalizedUrl(string.Empty, string.Empty, url);
		}

		/// <summary>
		/// 计算绝对路径的Url。如果url不是绝对路径，那么先根据配置文件resourceUriSettings的userTaskRoot项设置。
		/// 如果根路径还是空，则使用WfGlobalParameters的FormBaseUrl 
		/// </summary>
		/// <param name="appCodeName"></param>
		/// <param name="progCodeName"></param>
		/// <param name="url"></param>
		/// <returns></returns>
		public static string GetNormalizedUrl(string appCodeName, string progCodeName, string url)
		{
			string result = url;

			Uri uri = new Uri(url, UriKind.RelativeOrAbsolute);

			if (uri.IsAbsoluteUri == false)
			{
				ResourceUriSettings settings = ResourceUriSettings.GetConfig();

				string rootPath = string.Empty;

				if (settings.Paths.ContainsKey("userTaskRoot"))
				{
					rootPath = settings.Paths["userTaskRoot"].Uri.OriginalString;
				}
				else
				{
					if (appCodeName.IsNotEmpty() && progCodeName.IsNotEmpty())
						rootPath = WfGlobalParameters.GetValueRecursively(appCodeName, progCodeName, "FormBaseUrl", string.Empty);
					else
						rootPath = WfGlobalParameters.Default.Properties.GetValue("FormBaseUrl", string.Empty);
				}

				if (rootPath.IsNotEmpty())
					result = UriHelper.CombinePath(rootPath, url);
			}

			return result;
		}

		/// <summary>
		/// 紧急程度
		/// </summary>
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

		/// <summary>
		/// 节点名称，例如拟办等
		/// </summary>
		[ORFieldMapping("PURPOSE")]
		public string Purpose
		{
			get
			{
				return this.purpose;
			}
			set
			{
				this.purpose = value;
			}
		}

		/// <summary>
		/// 待办事项状态
		/// </summary>
		[ORFieldMapping("STATUS")]
		public TaskStatus Status
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
		/// 任务开始时间
		/// </summary>
		/// <remarks>一般为文件的拟稿时间，以当前时间为默认值有悖真实情况，是否妥当</remarks>
		[SqlBehavior(BindingFlags = ClauseBindingFlags.All, DefaultExpression = "getdate()")]
		[ORFieldMapping("TASK_START_TIME")]
		public DateTime TaskStartTime
		{
			get
			{
				return this.taskStartTime;
			}
			set
			{
				this.taskStartTime = value;
			}
		}

		/// <summary>
		/// 过期时间
		/// </summary>
		[SqlBehavior(BindingFlags = ClauseBindingFlags.All)]
		[ORFieldMapping("EXPIRE_TIME")]
		public DateTime ExpireTime
		{
			get
			{
				return this.expireTime;
			}
			set
			{
				this.expireTime = value;
			}
		}

		/// <summary>
		/// 消息发送时间
		/// </summary>
		[ORFieldMapping("DELIVER_TIME")]
		[SqlBehavior(BindingFlags = ClauseBindingFlags.All, DefaultExpression = "getdate()")]
		public DateTime DeliverTime
		{
			get
			{
				return this.deliverTime;
			}
			set
			{
				this.deliverTime = value;
			}
		}

		[ORFieldMapping("READ_TIME")]
		[SqlBehavior(BindingFlags = ClauseBindingFlags.All)]
		public DateTime ReadTime
		{
			get
			{
				return this.readTime;
			}
			set
			{
				this.readTime = value;
			}
		}

		/// <summary>
		/// 用户分类
		/// </summary>
		[SubClassORFieldMapping("CategoryID", "CATEGORY_GUID")]
		[SubClassType(typeof(TaskCategory))]
		public TaskCategory Category
		{
			get
			{
				return this.category;
			}
			set
			{
				this.category = value;
			}
		}

		/// <summary>
		/// 消息置顶标志
		/// </summary>
		[ORFieldMapping("TOP_FLAG")]
		public int TopFlag
		{
			get
			{
				return this.topFlag;
			}
			set
			{
				this.topFlag = value;
			}
		}

		/// <summary>
		/// 起草部门的时间
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

		/// <summary>
		/// 已办任务的办理时间
		/// </summary>
		[ORFieldMapping("COMPLETED_TIME")]
		[SqlBehavior(ClauseBindingFlags.Select)]
		public DateTime CompletedTime
		{
			get
			{
				return this.completedTime;
			}
			set
			{
				this.completedTime = value;
			}
		}

		[ORFieldMapping("DRAFT_USER_ID")]
		public string DraftUserID
		{
			get
			{
				return this.draftUserID;
			}
			set
			{
				this.draftUserID = value;
			}
		}

		[ORFieldMapping("DRAFT_USER_NAME")]
		public string DraftUserName
		{
			get
			{
				return this.draftUserName;
			}
			set
			{
				this.draftUserName = value;
			}
		}

		[NoMapping]
		[ScriptIgnore]
		public Dictionary<string, object> Context
		{
			get
			{
				if (this.context == null)
					this.context = new Dictionary<string, object>();

				return this.context;
			}
		}

		public UserTask Clone()
		{
			UserTask newUserTask = new UserTask();
			TaskCategory taskCategory = null;

			if (this.category != null)
			{
				taskCategory = new TaskCategory();
				taskCategory.CategoryID = this.category.CategoryID;
				taskCategory.CategoryName = this.category.CategoryName;
				taskCategory.InnerSortID = this.category.InnerSortID;
				taskCategory.UserID = this.category.UserID;
			}

			newUserTask.ActivityID = this.activityID;
			newUserTask.ApplicationName = this.appID;
			newUserTask.Body = this.body;
			newUserTask.Category = taskCategory;
			newUserTask.Emergency = this.emergency;
			newUserTask.ExpireTime = this.expireTime;
			newUserTask.Level = this.level;
			newUserTask.ProcessID = this.processID;
			newUserTask.ProgramName = this.progID;
			newUserTask.Purpose = this.purpose;
			newUserTask.ReadTime = this.readTime;
			newUserTask.ResourceID = this.resourceID;
			newUserTask.SendToUserID = this.sendToUserID;
			newUserTask.SendToUserName = this.SendToUserName;
			newUserTask.SourceID = this.sourceID;
			newUserTask.SourceName = this.sourceName;
			newUserTask.Status = this.status;
			newUserTask.TaskID = this.taskID;
			newUserTask.TaskStartTime = this.taskStartTime;
			newUserTask.TaskTitle = this.taskTitle;
			newUserTask.TopFlag = this.topFlag;
			newUserTask.Url = this.url;
			newUserTask.DeliverTime = this.deliverTime;
			newUserTask.DraftDepartmentName = this.draftDepartmentName;
			newUserTask.CompletedTime = this.completedTime;
			newUserTask.DraftUserID = this.draftUserID;
			newUserTask.DraftUserName = this.draftUserName;

			return newUserTask;
		}

		#endregion
	}
}
