using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.DirectoryServices;

namespace MCS.Library
{
	internal static class SearchResultHelper
	{
		/// <summary>
		/// 将SearchResult转换为列表
		/// </summary>
		/// <param name="src"></param>
		/// <returns></returns>
		public static List<SearchResult> ToList(this SearchResultCollection src)
		{
			List<SearchResult> result = new List<SearchResult>();

			if (src != null)
			{
				using (src)
				{
					foreach (SearchResult sr in src)
						result.Add(sr);
				}
			}

			return result;
		}
	}
}
