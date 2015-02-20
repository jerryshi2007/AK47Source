using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 通知接口
	/// </summary>
	public interface INotify
	{
		int NotifyID
		{
			get;
			set;
		}

		/// <summary>
		/// 题目
		/// </summary>
		string NotifyTitle
		{
			get;
			set;
		}

		/// <summary>
		/// 题目
		/// </summary>
		string UserTaskGuid
		{
			get;
			set;
		}

		/// <summary>
		/// 下次提醒时间
		/// </summary>
		DateTime LastNotifyTime
		{
			get;
			set;
		}

		/// <summary>
		/// 通知发送频率（秒数，可以为实数）
		/// </summary>
		TimeSpan NotifyFrequence
		{
			get;
			set;
		}

		/// <summary>
		/// 原始的通知发送频率（秒数，可以为实数）
		/// </summary>
		TimeSpan OriginalNotifyFrequence
		{
			get;
			set;
		}

		/// <summary>
		/// 手动设置的通知发送频率（秒数，可以为实数）
		/// </summary>
		bool ManualSetNotifyFrequence
		{
			get;
			set;
		}

		/// <summary>
		/// 发送的目标设备
		/// </summary>
		NotificationTarget SendToDevice
		{
			get;
			set;
		}

		/// <summary>
		/// 通过代办箱ID，得到待办箱对象
		/// </summary>
		IUserTask TaskDesp
		{
			get;
		}

		string NotifyBody
		{
			get;
			set;
		}
	}
}
