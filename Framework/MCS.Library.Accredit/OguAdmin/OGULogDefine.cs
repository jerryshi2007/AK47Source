using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Accredit.OguAdmin
{
	/// <summary>
	/// 日志相关数据
	/// </summary>
	public class OGULogDefine
	{
		#region 登录日志(Const Define)
		/// <summary>
		/// 日志中系统登录对应的应用名称
		/// </summary>
		public const string LOGON_APP = "SYS_LOGON";
		/// <summary>
		/// 日志中登录成功类型
		/// </summary>
		public const string LOGON_TYPE_SUCCESS = "SUCCESS_LOGON";
		/// <summary>
		/// 日志中登录失败类型
		/// </summary>
		public const string LOGON_TYPE_FAILURE = "FAILURE_LOGON";
		#endregion

		#region 机构系统日志 (Const Define)
		/// <summary>
		/// 机构人员管理系统对应日志的应用名称
		/// </summary>
		public const string LOG_OGU_APP_NAME = "OGU_ADMIN";
		/// <summary>
		/// 创建新对象
		/// </summary>
		public const string LOG_CREATE_OBJECTS = "CREATE_OBJECTS";
		/// <summary>
		/// 恢复被逻辑删除对象
		/// </summary>
		public const string LOG_FURBISH_DELETE_OBJECTS = "FURBISH_DELETE_OBJECTS";
		/// <summary>
		/// 人员组中增加成员
		/// </summary>
		public const string LOG_GROUP_ADD_USERS = "GROUP_ADD_USERS";
		/// <summary>
		/// 人员组中删除成员
		/// </summary>
		public const string LOG_GROUP_DEL_USERS = "GROUP_DEL_USERS";
		/// <summary>
		/// 初始化用户口令
		/// </summary>
		public const string LOG_INIT_USERS_PWD = "INIT_USERS_PWD";
		/// <summary>
		/// 逻辑删除对象
		/// </summary>
		public const string LOG_LOGIC_DELETE_OBJECTS = "LOGIC_DELETE_OBJECTS";
		/// <summary>
		/// 物理删除对象
		/// </summary>
		public const string LOG_REAL_DELETE_OBJECTS = "REAL_DELETE_OBJECTS";
		/// <summary>
		/// 设置秘书
		/// </summary>
		public const string LOG_SECRETARY_ADD = "SECRETARY_ADD";
		/// <summary>
		/// 删除秘书
		/// </summary>
		public const string LOG_SECRETARY_DEL = "SECRETARY_DEL";
		/// <summary>
		/// 设置用户兼职
		/// </summary>
		public const string LOG_SET_SIDELINE = "SET_SIDELINE";
		/// <summary>
		/// 人员组中人员排序
		/// </summary>
		public const string LOG_SORT_IN_GROUP = "SORT_IN_GROUP";
		/// <summary>
		/// 机构内对象排序
		/// </summary>
		public const string LOG_SORT_IN_ORGANIZATIONS = "SORT_IN_ORGANIZATIONS";
		/// <summary>
		/// 修改对象属性
		/// </summary>
		public const string LOG_UPDATE_OBJECTS = "UPDATE_OBJECTS";
		/// <summary>
		/// 设置主要职务
		/// </summary>
		public const string LOG_SET_MAINDUTY = "SET_MAINDUTY";
		/// <summary>
		/// 系统中对象的移动
		/// </summary>
		public const string LOG_MOVE_OBJECTS = "MOVE_OBJECTS";

		#endregion
	}
}
