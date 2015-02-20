using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Caching;

namespace MCS.Library.Office.OpenXml.Excel
{
	internal class TableDescriptionCacheQueue : CacheQueue<Type, TableDescription>
	{
		public static readonly TableDescriptionCacheQueue Instance = CacheManager.GetInstance<TableDescriptionCacheQueue>();

		private TableDescriptionCacheQueue()
		{
		}
	}
}
