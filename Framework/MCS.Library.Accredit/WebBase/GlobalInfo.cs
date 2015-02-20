using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Threading;

using MCS.Library.Accredit.OguAdmin.Interfaces;
using MCS.Library.Accredit.WebBase.Interfaces;

namespace MCS.Library.Accredit.WebBase
{
	/// <summary>
	/// 全局信息
	/// </summary>
	public class GlobalInfo
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public GlobalInfo()
		{
		}

		#region Http Enviroment
		/// <summary>
		/// 全局HTTP环境信息的TLS
		/// </summary>
		[ThreadStatic]
		private static LocalDataStoreSlot _HttpEnvSlot = null;

		/// <summary>
		/// 得到并保存当前线程中的Http环境信息
		/// </summary>
		public static IHttpEnvInterface HttpEnvironment
		{
			get
			{
				IHttpEnvInterface result = null;

				if (_HttpEnvSlot != null)
					result = (IHttpEnvInterface)Thread.GetData(_HttpEnvSlot);

				return result;
			}
		}

		/// <summary>
		/// 根据Request对象初始化所需环境信息
		/// </summary>
		/// <param name="request"></param>
		public static void InitHttpEnv(HttpRequest request)
		{
			HttpEnvInfo hei = new HttpEnvInfo(request);

			_HttpEnvSlot = Thread.AllocateDataSlot();
			Thread.SetData(_HttpEnvSlot, hei as IHttpEnvInterface);
		}

		#endregion

		#region User Enviroment
		/// <summary>
		/// 系统登录人的TLS
		/// </summary>
		[ThreadStatic]
		private static LocalDataStoreSlot _LogOnUserInfoSlot;

		/// <summary>
		/// 得到并保存当前线程中的用户信息
		/// </summary>
		public static ILogOnUserInfo UserLogOnInfo
		{
			get
			{
				ILogOnUserInfo result = null;

				if (_LogOnUserInfoSlot != null)
					result = (ILogOnUserInfo)Thread.GetData(_LogOnUserInfoSlot);

				return result;
			}
		}

		/// <summary>
		/// 根据登录用户身份初始化用户信息数据
		/// </summary>
		/// <param name="userInfo"></param>
		public static void InitLogOnUser(ILogOnUserInfo userInfo)
		{
			_LogOnUserInfoSlot = Thread.AllocateDataSlot();

			Thread.SetData(_LogOnUserInfoSlot, userInfo);
		}

		#endregion
	}
}
