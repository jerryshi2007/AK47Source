using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using MCS.Library.Caching;
using MCS.Library.Core;

namespace MCS.Library.Office.SpreadSheet
{
	public static class Namespaces
	{
		public const string o = "urn:schemas-microsoft-com:office:office";
		public const string x = "urn:schemas-microsoft-com:office:excel";
		public const string ss = "urn:schemas-microsoft-com:office:spreadsheet";
		public const string c = "urn:schemas-microsoft-com:office:component:spreadsheet";
		public const string html = "http://www.w3.org/TR/REC-html40";
		public const string spreadsheet = "urn:schemas-microsoft-com:office:spreadsheet";
		public const string office = "urn:schemas-microsoft-com:office:office";

		private const string ContextCacheKey = "WorkbookNamespaceManager";

		public static XmlNamespaceManager GetNamespaceManager()
		{
			object data = null;

			ObjectContextCache.Instance.TryGetValue(ContextCacheKey, out data);

			ExceptionHelper.FalseThrow(data != null, "没有初始化NamespaceManager");

			return (XmlNamespaceManager)data;
		}

		public static XmlNamespaceManager InitNamespaceManager(XmlNameTable nameTable)
		{
			XmlNamespaceManager nsmgr = CreateNamespaceManager(nameTable);

			ObjectContextCache.Instance[ContextCacheKey] = nsmgr;

			return nsmgr;
		}

		public static void RemoveNamespaceManager()
		{
			if (ObjectContextCache.Instance.ContainsKey(ContextCacheKey))
				ObjectContextCache.Instance.Remove(ContextCacheKey);
		}

		public static XmlNamespaceManager CreateNamespaceManager(XmlNameTable nameTable)
		{
			XmlNamespaceManager nsmgr = new XmlNamespaceManager(nameTable);

			nsmgr.AddNamespace("o", Namespaces.o);
			nsmgr.AddNamespace("x", Namespaces.x);
			nsmgr.AddNamespace("ss", Namespaces.ss);
			nsmgr.AddNamespace("c", Namespaces.c);
			nsmgr.AddNamespace("html", Namespaces.html);
			nsmgr.AddNamespace("spreadsheet", Namespaces.spreadsheet);
			nsmgr.AddNamespace("office", Namespaces.office);

			return nsmgr;
		}
	}
}
