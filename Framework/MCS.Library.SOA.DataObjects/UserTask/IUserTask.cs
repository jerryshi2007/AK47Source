using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 用户任务的接口
	/// </summary>
	public interface IUserTask
	{
		/// <summary>
		/// TaskID
		/// </summary>
		string TaskID
		{
			get;
		}

		/// <summary>
		/// 应用名称
		/// </summary>
		string ApplicationName
		{
			get;
			set;
		}

		/// <summary>
		/// 模块名称
		/// </summary>
		string ProgramName
		{
			get;
			set;
		}

		/// <summary>
		/// 题目
		/// </summary>
		string TaskTitle
		{
			get;
			set;
		}

		/// <summary>
		/// 资源ID
		/// </summary>
		string ResourceID
		{
			get;
			set;
		}

		/// <summary>
		/// 发件人ID
		/// </summary>
		string SourceID
		{
			get;
			set;
		}

		/// <summary>
		/// 发件人名称
		/// </summary>
		string SourceName
		{
			get;
			set;
		}

		/// <summary>
		/// 发件人ID
		/// </summary>
		string SendToUserID
		{
			get;
			set;
		}

		string Body
		{
			get;
			set;
		}

		/// <summary>
		/// 消息类型
		/// </summary>
		TaskLevel Level
		{
			get;
			set;
		}

		/// <summary>
		/// 流程ID
		/// </summary>
		string ProcessID
		{
			get;
			set;
		}

		/// <summary>
		/// 工作流节点ID
		/// </summary>
		string ActivityID
		{
			get;
			set;
		}

		/// <summary>
		/// 待办事项的链接
		/// </summary>
		string Url
		{
			get;
			set;
		}

		/// <summary>
		/// 紧急程度
		/// </summary>
		FileEmergency Emergency
		{
			get;
			set;
		}

		/// <summary>
		/// 节点名称，例如拟办等
		/// </summary>
		string Purpose
		{
			get;
			set;
		}

		/// <summary>
		/// 待办事项状态
		/// </summary>
		TaskStatus Status
		{
			get;
			set;
		}

		/// <summary>
		/// 开始时间
		/// </summary>
		DateTime TaskStartTime
		{
			get;
			set;
		}

		/// <summary>
		/// 过期时间
		/// </summary>
		DateTime ExpireTime
		{
			get;
			set;
		}

		DateTime ReadTime
		{
			get;
			set;
		}

		/// <summary>
		/// 用户分类
		/// </summary>
		TaskCategory Category
		{
			get;
			set;
		}

		/// <summary>
		/// 消息置顶标志
		/// </summary>
		int TopFlag
		{
			get;
			set;
		}

		/// <summary>
		/// 起草部门名称
		/// </summary>
		string DraftDepartmentName
		{
			get;
			set;
		}

		/// <summary>
		/// 已办任务的办理时间
		/// </summary>
		DateTime CompletedTime
		{
			get;
			set;
		}

		/// <summary>
		/// 起草人的ID
		/// </summary>
		string DraftUserID
		{
			get;
			set;
		}

		/// <summary>
		/// 起草人的名称
		/// </summary>
		string DraftUserName
		{
			get;
			set;
		}
	}
}
