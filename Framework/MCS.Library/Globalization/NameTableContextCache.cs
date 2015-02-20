using System;
using System.Text;
using System.Threading;
using System.Web.UI;
using System.Collections.Generic;
using MCS.Library.Core;
using MCS.Library.Caching;
using MCS.Library.Globalization;

namespace MCS.Web.Library
{
	/// <summary>
	/// 客户端的名字表
	/// </summary>
	public class DeluxeNameTableContextCache : ContextCacheQueueBase<DeluxeNameTableCacheKey, string>
	{
		/// <summary>
		/// 获取实例
		/// </summary>
		public static DeluxeNameTableContextCache Instance
		{
			get
			{
				return ContextCacheManager.GetInstance<DeluxeNameTableContextCache>();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="category"></param>
		/// <param name="sourceText"></param>
		public void Add(string category, string sourceText)
		{
			DeluxeNameTableCacheKey key = new DeluxeNameTableCacheKey()
			{
				Category = category,
				SourceText = sourceText
			};

			string targetText = Translator.Translate(category, sourceText);

			this.Add(key, targetText);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="category"></param>
		/// <param name="sourceText"></param>
		/// <param name="targetText"></param>
		public void Add(string category, string sourceText, string targetText)
		{
			DeluxeNameTableCacheKey key = new DeluxeNameTableCacheKey()
			{
				Category = category,
				SourceText = sourceText
			};

			this.Add(key, targetText);
		}

		/// <summary>
		/// 注册客户端脚本名称表
		/// </summary>
		/// <param name="page"></param>
		public void RegisterNameTable(Page page)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(page != null, "page");
			Dictionary<string, bool> registeredCategory = new Dictionary<string, bool>();

			if (ObjectContextCache.Instance.ContainsKey("NameTableRegistered") == false)
			{
				page.ClientScript.RegisterClientScriptBlock(typeof(DeluxeNameTableContextCache),
					"DeluxeNameTableContextCache", GetNameTableScript(), true);

				ObjectContextCache.Instance["NameTableRegistered"] = true;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public string GetNameTableScript()
		{
			Dictionary<string, bool> registeredCategory = new Dictionary<string, bool>();

			StringBuilder strB = new StringBuilder();

		    strB.AppendLine();
            strB.AppendFormat("var $language = \"{0}\";\r\n", Thread.CurrentThread.CurrentUICulture.Name);
			strB.AppendLine("var DeluxeNameTable = {category: {}};");
			strB.AppendLine("var $NT = DeluxeNameTable;");
			strB.AppendLine("$NT.getText = function(category, sourceText){ var targetText = sourceText; if ($NT.category[category]) if ($NT.category[category][sourceText]) targetText = $NT.category[category][sourceText]; return targetText; }");
			strB.AppendLine();

			foreach (KeyValuePair<DeluxeNameTableCacheKey, string> kp in this)
			{
				if (registeredCategory.ContainsKey(kp.Key.Category) == false)
				{
					strB.AppendLine("$NT.category[\"{0}\"] = {};".Replace("{0}", kp.Key.Category));
					registeredCategory[kp.Key.Category] = true;
				}

				strB.AppendFormat("$NT.category[\"{0}\"][\"{1}\"] = \"{2}\";\n",
					kp.Key.Category.Replace("\"", "\\\""),
					kp.Key.SourceText.Replace("\"", "\\\""),
					kp.Value.Replace("\"", "\\\""));
			}

			return strB.ToString();
		}
	}

	/// <summary>
	/// 客户端名字表的Key
	/// </summary>
	public struct DeluxeNameTableCacheKey
	{
		/// <summary>
		/// 
		/// </summary>
		public string SourceText { get; set; }
		
		/// <summary>
		/// 
		/// </summary>
		public string Category { get; set; }
	}
}
