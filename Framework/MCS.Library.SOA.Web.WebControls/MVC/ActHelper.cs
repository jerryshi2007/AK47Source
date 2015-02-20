using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Web;
using MCS.Library.Core;
using MCS.Library.Caching;

namespace MCS.Web.Library.MVC
{
	/// <summary>
	/// 帮助读取场景信息
	/// </summary>
	public static class ActHelper
	{
		/// <summary>
		/// 从虚路径加载场景信息
		/// </summary>
		/// <param name="virtualPath">场景文件的虚路径</param>
		/// <returns>幕的集合</returns>
		public static ActCollection GetActs(string virtualPath)
		{
			ActCollection result = null;

			string filePath = HttpContext.Current.Server.MapPath(virtualPath).ToLower();

			if (ActCache.Instance.TryGetValue(filePath, out result) == false)
			{
				XmlDocument xmlDoc = XmlHelper.LoadDocument(filePath);

				result = LoadActs(xmlDoc);

				FileCacheDependency dependency = new FileCacheDependency(filePath);
				ActCache.Instance.Add(filePath, result, dependency);
			}

			return result;
		}

		/// <summary>
		/// 从xml string加载场景信息
		/// add by 徐文卓
		/// </summary>
		/// <param name="virtualPath">xmlstr</param>
		/// <returns>幕的集合</returns>
		public static ActCollection GetActsFromStr(string xmlstr)
		{
			ActCollection result = null;
			if (xmlstr.Length <= 0) return result;
			if (ActCache.Instance.TryGetValue(xmlstr, out result) == false)
			{
				XmlDocument xmlDoc = new XmlDocument();
				xmlDoc.LoadXml(xmlstr);
				result = LoadActs(xmlDoc);

				ActCache.Instance.Add(xmlstr, result, null);
			}

			return result;
		}

		private static ActCollection LoadActs(XmlDocument xmlDoc)
		{
			ActCollection acts = new ActCollection();

			foreach (XmlElement elem in xmlDoc.DocumentElement.SelectNodes("Act"))
				acts.Add(new Act(elem));

			return acts;
		}
	}

	internal sealed class ActCache : CacheQueue<string, ActCollection>
	{
		public static readonly ActCache Instance = CacheManager.GetInstance<ActCache>();
	}
}
