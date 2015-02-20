using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Principal;

namespace MCS.Library.Accredit.OguAdmin.Interfaces
{
	/// <summary>
	/// ILogOnUserInfo 的摘要说明。
	/// </summary>
	public interface ILogOnUserInfo : IPrincipal
	{
		/// <summary>
		/// 用户本身对应的标识
		/// </summary>
		string UserGuid
		{
			get;
		}

		/// <summary>
		/// 用户的登录名
		/// </summary>
		string UserLogOnName
		{
			get;
		}

		/// <summary>
		/// 当前用户的行政级别
		/// </summary>
		IRankDefine RankDefine
		{
			get;
		}

		/// <summary>
		/// 当前用户所处于的所有可用的机构人员关系
		/// </summary>
		IOuUsers[] OuUsers
		{
			get;
		}
	}
}
