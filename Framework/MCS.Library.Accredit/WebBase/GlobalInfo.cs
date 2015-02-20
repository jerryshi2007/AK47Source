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
	/// ȫ����Ϣ
	/// </summary>
	public class GlobalInfo
	{
		/// <summary>
		/// ���캯��
		/// </summary>
		public GlobalInfo()
		{
		}

		#region Http Enviroment
		/// <summary>
		/// ȫ��HTTP������Ϣ��TLS
		/// </summary>
		[ThreadStatic]
		private static LocalDataStoreSlot _HttpEnvSlot = null;

		/// <summary>
		/// �õ������浱ǰ�߳��е�Http������Ϣ
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
		/// ����Request�����ʼ�����軷����Ϣ
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
		/// ϵͳ��¼�˵�TLS
		/// </summary>
		[ThreadStatic]
		private static LocalDataStoreSlot _LogOnUserInfoSlot;

		/// <summary>
		/// �õ������浱ǰ�߳��е��û���Ϣ
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
		/// ���ݵ�¼�û���ݳ�ʼ���û���Ϣ����
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
