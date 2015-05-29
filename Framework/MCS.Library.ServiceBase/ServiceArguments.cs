using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Services
{
	/// <summary>
	/// 服务参数
	/// </summary>
	public class ServiceArguments
	{
		/// <summary>
		/// 服务名称
		/// </summary>
		public string ServiceName
		{
			get;
			set;
		}

		/// <summary>
		/// 启动模式
		/// </summary>
		public ServiceEntryType EntryType
		{
			get;
			set;
		}

		public string Port
		{
			get;
			set;
		}

		/// <summary>
		/// 获取当前的参数
		/// </summary>
		public static ServiceArguments Current
		{
			get
			{
				ServiceArguments result = (ServiceArguments)AppDomain.CurrentDomain.GetData("ServiceArguments");

				lock (typeof(ServiceArguments))
				{
					if (result == null)
					{
						result = new ServiceArguments();

						AppDomain.CurrentDomain.SetData("ServiceArguments", result);
					}
				}

				return result;
			}
		}
	}
}
