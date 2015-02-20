using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using MCS.Library.Core;
using MCS.Library.Logging;

namespace MCS.Library.Caching
{
	/// <summary>
	/// ����Udp֪ͨ��CacheDepenedency
	/// </summary>
	public class UdpNotifierCacheDependency : NotifierCacheDependencyBase
	{
		/// <summary>
		/// �õ�������
		/// </summary>
		/// <returns></returns>
		protected override NotifierCacheMonitorBase GetMonitor()
		{
			return UdpNotifierCacheMonitor.Instance;
		}
	}
}
