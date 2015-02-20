using System;
using System.Collections.Generic;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.Accredit.OguAdmin
{
	/// <summary>
	/// 登录可以使用的条件判断
	/// </summary>
	public enum LogonType
	{
		/// <summary>
		/// 非法方式
		/// </summary>
		[EnumItemDescription("非法方式登录")]
		None = 0,
		/// <summary>
		/// 使用LOGON_NAME登录系统
		/// </summary>
		[EnumItemDescription("LOGON_NAME登录")]
		LOGON_NAME = 1,
		/// <summary>
		/// 使用IC卡登录系统
		/// </summary>
		[EnumItemDescription("IC卡登录")]
		IC_CARD = 2,
		/// <summary>
		/// 使用UsbKey方式登录
		/// </summary>
		[EnumItemDescription("UsbKey方式登录")]
		USB_KEY = 4,
		/// <summary>
		/// 员工号方式
		/// </summary>
		[EnumItemDescription("员工号方式")]
		POSTAL = 8
	}
}
