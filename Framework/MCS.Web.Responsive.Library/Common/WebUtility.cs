using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using MCS.Library.Core;
using MCS.Web.Responsive.Library.Script;

namespace MCS.Web.Responsive.Library
{
	public static class WebUtility
	{
		private static object _CurrentPageKey = new object();

		/// <summary>
		/// 向页面中增加与scriptType类型相关的脚本
		/// </summary>
		/// <param name="scriptType">脚本相关类型</param>
		public static void RequiredScript(Type scriptType)
		{
			Page page = GetCurrentPage();

			IEnumerable<ScriptReference> srs = Script.ScriptObjectBuilder.GetScriptReferences(scriptType);

			ScriptManager sm = ScriptManager.GetCurrent(page);
			foreach (ScriptReference sr in srs)
			{
				if (sm != null)
					sm.Scripts.Add(sr);
				else
				{
					DeluxeClientScriptManager.RegisterHeaderScript(page, page.ClientScript.GetWebResourceUrl(scriptType, sr.Name));
				}
			}

			Script.ScriptObjectBuilder.RegisterCssReferences(GetCurrentPage(), scriptType);
		}

		/// <summary>
		/// 得到某类型对应的脚本
		/// </summary>
		/// <param name="scriptType">类型信息</param>
		/// <returns></returns>
		public static string GetRequiredScript(Type scriptType)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(scriptType != null, "scriptType");

			List<ResourceEntry> res = Script.ScriptObjectBuilder.GetScriptResourceEntries(scriptType);
			StringBuilder strB = new StringBuilder(1024);

			foreach (ResourceEntry entry in res)
			{
				Page page = GetCurrentPage();

				if (page == null)
					page = new Page();

				string src = page.ClientScript.GetWebResourceUrl(entry.ComponentType, entry.ResourcePath);

				strB.Append(DeluxeClientScriptManager.GetScriptString(src));
				strB.Append("\n");
			}

			return strB.ToString();
		}

		/// <summary>
		/// 获取当前页
		/// </summary>
		/// <returns></returns>
		public static Page GetCurrentPage()
		{
			Page page = (Page)HttpContext.Current.Items[_CurrentPageKey];
			if (page == null)
				page = HttpContext.Current.CurrentHandler as Page;

			//ExceptionHelper.TrueThrow(page == null, "当前没有处理请求的页面！");

			return page;
		}

		/// <summary>
		/// 设置当前页
		/// </summary>
		/// <param name="page"></param>
		public static void SetCurrentPage(Page page)
		{
			HttpContext.Current.Items[_CurrentPageKey] = page;
		}
	}
}
