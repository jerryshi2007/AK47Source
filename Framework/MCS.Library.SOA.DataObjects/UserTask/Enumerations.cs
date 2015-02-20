using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 待办的状态
	/// </summary>
	public enum TaskStatus
	{
		[EnumItemDescription("待办事项")]
		Ban = 1,

		[EnumItemDescription("待阅事项")]
		Yue = 2,
	}

	/// <summary>
	/// 系统任务的状态
	/// </summary>
	public enum SysTaskStatus
	{
		/// <summary>
		/// 运行中
		/// </summary>
		[EnumItemDescription("运行中")]
		Running = 0,

		/// <summary>
		/// 已完成
		/// </summary>
		[EnumItemDescription("已完成")]
		Completed = 1,

		/// <summary>
		/// 被终止
		/// </summary>
		[EnumItemDescription("被终止")]
		Aborted = 2,

		/// <summary>
		/// 未运行
		/// </summary>
		[EnumItemDescription("未运行")]
		NotRunning = 3,
	}

	/// <summary>
	/// 任务的级别
	/// </summary>
	public enum TaskLevel
	{
		None = 0,

		VeryLow = 1,

		[EnumItemDescription("提醒消息")]
		Low = 2,		//目前为提醒类消息

		[EnumItemDescription("正常消息")]
		Normal = 3,		//目前为正常消息

		[EnumItemDescription("办理消息")]
		High = 4,

		VeryHigh = 5,
	}

	/// <summary>
	/// 通知的目标
	/// </summary>
	[Flags]
	public enum NotificationTarget
	{
		/// <summary>
		/// 没有任何目标
		/// </summary>
		None = 0,

		/// <summary>
		/// 任务
		/// </summary>
		[EnumItemDescription("待办事项")]
		Task = 1,

		/// <summary>
		/// 电子邮件
		/// </summary>
		[EnumItemDescription("电子邮件")]
		Email = 2,

		/// <summary>
		/// 短消息
		/// </summary>
		[EnumItemDescription("手机短信")]
		SMS = 4,

		/// <summary>
		/// 即时消息
		/// </summary>
		[EnumItemDescription("即时消息")]
		Messenger = 8,

		/// <summary>
		/// 手机邮件审批
		/// </summary>
		[EnumItemDescription("手机邮件审批")]
		MobileApprove = 16,

		/// <summary>
		/// 其他目标
		/// </summary>
		[EnumItemDescription("其它")]
		Other = 256,

		/// <summary>
		/// 所有目标
		/// </summary>
		All = 287,
	}
}
