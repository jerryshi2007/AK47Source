using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.Security.ADSyncUtilities
{
	/// <summary>
	/// 用于从路径中提取每个名称（从 左 到 右）
	/// </summary>
	/// <remarks>除了转义分隔符之外,路径中不识别其他转义字符</remarks>
	public class PathPartEnumerator : IEnumerable<string>
	{
		private string path;

		public PathPartEnumerator(string path)
		{
			this.path = path;
		}

		public IEnumerator<string> GetEnumerator()
		{
			int i = 0;
			int j = 0;
			int last = 0;

			while (last < path.Length)
			{
				if (i < path.Length && i >= 0)
				{
					i = path.IndexOf('\\', i);
					if (i >= 0 && i < path.Length)
					{
						//遇到分隔符
						if (i + 1 < path.Length)
						{
							j = path.IndexOf('\\', i + 1);
							if (i + 1 == j)
							{
								//遇到双斜线
								i += 2;
								continue;
							}
							else
							{
								yield return path.Substring(last, i - last).Replace("\\\\", "\\");
								last = i + 1;
								i = last + 1;
							}
						}
						else
						{
							yield return path.Substring(last, i - last).Replace("\\\\", "\\");
							last = i + 1;
							i = last + 1;
						}
					}
				}
				else
				{
					break;
				}
			}

			if (last < path.Length)
			{
				yield return path.Substring(last).Replace("\\\\", "\\");
			}
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}
