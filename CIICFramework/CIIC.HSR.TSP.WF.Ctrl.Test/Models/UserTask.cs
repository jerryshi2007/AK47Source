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

namespace CIIC.HSR.TSP.WF.BizObject{
	/// <summary>
	/// 待办箱实体类
	/// </summary>
	public class UserTask 
	{

		public string TaskID{ get; set; } 
       
		/// <summary>
		/// 应用名称
		/// </summary>
		public string ApplicationName{ get; set; } 

		/// <summary>
		/// 模块名称
		/// </summary>
		public string ProgramName { get; set; } 


		/// <summary>
		/// 标题
		/// </summary>
		public string TaskTitle { get; set; } 


		/// <summary>
		/// 资源ID
		/// </summary>
		public string ResourceID { get; set; } 

		/// <summary>
		/// 发件人ID
		/// </summary>
		public string SourceID { get; set; } 


		/// <summary>
		/// 发件人名称
		/// </summary>
		public string SourceName { get; set; } 

		public string SendToUserID { get; set; } 


		public string SendToUserName { get; set; } 

		public string Body { get; set; } 

		/// <summary>
		/// 消息类型
		/// </summary>
		public int?  Level { get; set; } 

		/// <summary>
		/// 流程ID
		/// </summary>
		public string ProcessID{ get; set; } 


		/// <summary>
		/// 工作流节点ID
		/// </summary>
		public string ActivityID { get; set; } 


		/// <summary>
		/// 待办事项的链接
		/// </summary>
		public string Url { get; set; } 


		/// <summary>
		/// 紧急程度
		/// </summary>
		public int?  Emergency{ get; set; } 


		/// <summary>
		/// 节点名称，例如拟办等
		/// </summary>
		public string Purpose { get; set; } 


		/// <summary>
		/// 待办事项状态
		/// </summary>
		public string Status { get; set; } 


		/// <summary>
		/// 任务开始时间
		/// </summary>
		/// <remarks>一般为文件的拟稿时间，以当前时间为默认值有悖真实情况，是否妥当</remarks>
		public DateTime TaskStartTime { get; set; } 


		/// <summary>
		/// 过期时间
		/// </summary>
		public DateTime ExpireTime { get; set; } 


		/// <summary>
		/// 消息发送时间
		/// </summary>
		public DateTime DeliverTime { get; set; } 


		public DateTime ReadTime { get; set; } 

		/// <summary>
		/// 用户分类
		/// </summary>
        public TaskCategory Category { get; set; } 


		/// <summary>
		/// 消息置顶标志
		/// </summary>
		public int TopFlag { get; set; } 


		/// <summary>
		/// 起草部门的时间
		/// </summary>
		public string DraftDepartmentName { get; set; }

		/// <summary>
		/// 已办任务的办理时间
		/// </summary>

		public DateTime CompletedTime { get; set; }

		public string DraftUserID { get; set; }

		public string DraftUserName { get; set; }

	}
}
