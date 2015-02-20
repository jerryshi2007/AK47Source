using System;
using System.Text;
using System.Collections.Generic;

namespace MCS.Library.Caching
{
	internal static class UdpSettingsHelper
	{
        /// <summary>
        /// �����ַ�����ʾ���ԡ�,����һ��˿ں�ת��Ϊ��int�����ʾ��
        /// </summary>
        /// <param name="portsDefine">���ַ�����ʾ��һ��˿ں�</param>
        /// <returns>��int�����ʾ��һ��˿ں�</returns>
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
