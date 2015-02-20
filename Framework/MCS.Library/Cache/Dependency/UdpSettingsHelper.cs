using System;
using System.Text;
using System.Collections.Generic;

namespace MCS.Library.Caching
{
	internal static class UdpSettingsHelper
	{
        /// <summary>
        /// 将由字符串表示，以“,”的一组端口号转换为由int数组表示。
        /// </summary>
        /// <param name="portsDefine">由字符串表示的一组端口号</param>
        /// <returns>以int数组表示的一组端口号</returns>
		public static int[] GetPorts(string portsDefine)
		{
			List<int> portList = new List<int>();

			string[] ports = portsDefine.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

			for (int i = 0; i < ports.Length; i++)
			{
				int port = -1;

				if (int.TryParse(ports[i], out port))
					portList.Add(port);
				else
				{
					string[] segments = ports[i].Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);

					int start = -1;
					int end = -1;

					if (segments.Length > 0)
					{
						int.TryParse(segments[0], out start);

						if (segments.Length > 1)
							int.TryParse(segments[1], out end);

						if (start >= 0)
							for (int j = start; j < end + 1; j++)
								portList.Add(j);
					}
				}
			}

			return portList.ToArray();
		}
	}
}
