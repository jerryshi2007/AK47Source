using System;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace MCS.Library.Passport
{
	/// <summary>
	/// 登录信息的接口
	/// </summary>
	public interface ISignInInfo
	{
		/// <summary>
		/// 登录用户的ID
		/// </summary>
		string UserID
		{
			get;
			set;
		}

		/// <summary>
		/// 扮演前的登录名
		/// </summary>
		string OriginalUserID
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		string Domain
		{
			get;
		}

		/// <summary>
		/// 带域名的用户ID
		/// </summary>
		string UserIDWithDomain
		{
			get;
		}

		/// <summary>
		/// 是否集成认证
		/// </summary>
		bool WindowsIntegrated
		{
			get;
		}

		/// <summary>
		/// 登录的SessionID
		/// </summary>
		string SignInSessionID
		{
			get;
		}

		/// <summary>
		/// 登录的时间
		/// </summary>
 		DateTime SignInTime
		{
			get;
			set;
		}

		/// <summary>
		/// 登录的过期时间
		/// </summary>
		DateTime SignInTimeout
		{
			get;
			set;
		}

		/// <summary>
		/// 认证服务器的域名(或者IP)
		/// </summary>
		string AuthenticateServer
		{
			get;
		}

		/// <summary>
		/// 是否存在登录超时时间（不是日期的最大和最小值）
		/// </summary>
		bool ExistsSignInTimeout
		{
			get;
		}

		/// <summary>
		/// 扩展属性集合（不入库）
		/// </summary>
		Dictionary<string, object> Properties
		{
			get;
		}

		/// <summary>
		/// 将登录信息保存到Cookie中
		/// </summary>
		void SaveToCookie();

		/// <summary>
		/// 将登录信息保存到Xml文档对象中
		/// </summary>
		XmlDocument SaveToXml();

		/// <summary>
		/// SignInInfo是否合法
		/// </summary>
		/// <returns></returns>
		bool IsValid();
	}

	/// <summary>
	/// 应用登录以后生成的Ticket
	/// </summary>
	public interface ITicket
	{
		/// <summary>
		/// 登录的信息
		/// </summary>
		//[NoMapping]
		ISignInInfo SignInInfo
		{
			get;
		}

		/// <summary>
		/// 应用登录以后的应用ID
		/// </summary>
        string AppSignInSessionID
		{
			get;
		}

		/// <summary>
		/// 应用的ID
		/// </summary>
		string AppID
		{
			get;
		}

		/// <summary>
		/// 应用登录的时间
		/// </summary>
		DateTime AppSignInTime
		{
			get;
			set;
		}

		/// <summary>
		/// 应用登录的Session过期时间
		/// </summary>
		DateTime AppSignInTimeout
		{
			get;
			set;
		}

		/// <summary>
		/// 应用登录时的IP地址
		/// </summary>
		string AppSignInIP
		{
			get;
		}

		/// <summary>
		/// 将应用登录信息保存到Cookie中
		/// </summary>
		void SaveToCookie();

		/// <summary>
		/// 将应用登录信息保存到Xml文档对象中
		/// </summary>
		XmlDocument SaveToXml();

		/// <summary>
		/// Ticket是否合法
		/// </summary>
		/// <returns></returns>
		bool IsValid();

		/// <summary>
		/// 生成加密的字符串
		/// </summary>
		/// <returns></returns>
		string ToEncryptString();
	}

	/// <summary>
	/// 登录用户的信息
	/// </summary>
	public interface ISignInUserInfo
	{
		/// <summary>
		/// 登录用户的ID
		/// </summary>
		string UserID
		{
			get;
			set;
		}

		/// <summary>
		/// 用户的域名
		/// </summary>
		string Domain
		{
			get;
			set;
		}

		/// <summary>
		/// 原始的登录ID
		/// </summary>
		string OriginalUserID
		{
			get;
			set;
		}

		/// <summary>
		/// 属性集合
		/// </summary>
		Dictionary<string, object> Properties
		{
			get;
		}
	}
}
