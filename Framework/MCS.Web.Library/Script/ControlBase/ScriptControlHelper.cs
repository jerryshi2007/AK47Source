using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using MCS.Library.Core;
using MCS.Web.Library.Resources;

namespace MCS.Web.Library.Script
{
	/// <summary>
	/// 
	/// </summary>
	public static class ScriptControlHelper
	{
		internal static string _S_PageLevelKey = "PageLevelKey";
		internal static string _S_PageUniqueIDKey = "PageUniqueID";

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sm"></param>
		/// <param name="page"></param>
		public static void EnsureScriptManager(ref ScriptManager sm, Page page)
		{
			if (sm == null)
			{
				sm = ScriptManager.GetCurrent(page);
				if (sm == null)
				{
					ExceptionHelper.TrueThrow(page.Form.Controls.IsReadOnly, DeluxeWebResource.E_NoScriptManager);

					sm = new ScriptManager();

					//根据应用的Debug状态来决定ScriptManager的状态 2008-9-18
					bool debug = WebUtility.IsWebApplicationCompilationDebug();
					sm.ScriptMode = debug ? ScriptMode.Debug : ScriptMode.Release;

					sm.EnableScriptGlobalization = true;
					page.Form.Controls.Add(sm);
				}
			}
			else
			{
				ExceptionHelper.FalseThrow(sm.EnableScriptGlobalization, "页面中ScriptManger对象中属性EnableScriptGlobalization值应该设置为True！");
			}

			if (sm != null)
			{
				sm.AsyncPostBackError -= sm_AsyncPostBackError;
				sm.AsyncPostBackError += new EventHandler<AsyncPostBackErrorEventArgs>(sm_AsyncPostBackError);
			}
		}

		private static void sm_AsyncPostBackError(object sender, AsyncPostBackErrorEventArgs e)
		{
			ScriptManager sm = (ScriptManager)sender;

			if (e.Exception != null)
				sm.AsyncPostBackErrorMessage = e.Exception.GetRealException().Message;
		}

		/// <summary>
		/// 得到页面Page类型
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static Type GetBasePageTypeInfo(Type type)
		{
			Type baseType = null;

			while (type != null)
			{
				if (type == typeof(Page))
				{
					baseType = type;
					break;
				}

				type = type.BaseType;
			}

			return baseType;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="cache"></param>
		/// <returns></returns>
		public static int GetPageLevel(PageRenderModePageCache cache)
		{
			int level = cache.GetValue<int>(_S_PageLevelKey, 0);
			return level;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="cache"></param>
		/// <param name="level"></param>
		public static void SetPageLevel(PageRenderModePageCache cache, int level)
		{
			cache[_S_PageLevelKey] = level;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="cache"></param>
		/// <returns></returns>
		public static string GetPageUniqueID(PageRenderModePageCache cache)
		{
			string id = cache.GetValue<string>(_S_PageUniqueIDKey, string.Empty);

			return id == null ? string.Empty : (string)id;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="cache"></param>
		/// <param name="id"></param>
		public static void SetPageUniqueID(PageRenderModePageCache cache, string id)
		{
			cache[_S_PageUniqueIDKey] = id;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ctr"></param>
		/// <param name="renderMode"></param>
		public static void CheckOnlyRenderSelf(Control ctr, ControlRenderMode renderMode)
		{
			if (renderMode.OnlyRenderSelf && renderMode.UseNewPage && ctr.Page.Items[WebUtility.PageRenderControlItemKey] != ctr)
			{
				Page currentPage = ctr.Page;
				ctr.Parent.Controls.GetType().GetMethod("SetCollectionReadOnly", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(ctr.Parent.Controls, new object[1] { null });

				Page page = new Page();

				PageRenderModePageCache currentPageCache = PageRenderModeHelper.GetPageRenderModeCache(currentPage);
				PageRenderModePageCache pageCache = PageRenderModeHelper.GetPageRenderModeCache(page);

				SetPageLevel(pageCache, GetPageLevel(currentPageCache) + 1);
				string currentPageUniqueID = GetPageUniqueID(currentPageCache);

				if (currentPageUniqueID != string.Empty)
					currentPageUniqueID += ",";

				SetPageUniqueID(pageCache, string.Format("{0}{1}", GetPageUniqueID(currentPageCache), ctr.UniqueID));

				page.AppRelativeVirtualPath = ctr.Page.AppRelativeVirtualPath;
				page.EnableEventValidation = false;

				InitNewPageContent(page, ctr);

				WebUtility.AttachPageModules(page);

				page.ProcessRequest(HttpContext.Current);

				HttpContext.Current.Response.End();
			}
		}

		private static void InitNewPageContent(Page page, Control ctr)
		{
			HtmlGenericControl html = new HtmlGenericControl("html");
			WebUtility.SetCurrentPage(page);
			page.Items[WebUtility.PageRenderControlItemKey] = ctr;
			page.Controls.Add(html);

			HtmlHead head = new HtmlHead();

			html.Controls.Add(head);
			HtmlGenericControl body = new HtmlGenericControl("body");
			html.Controls.Add(body);

			HtmlForm form = new HtmlForm();
			form.ID = "pageForm";
			form.Controls.Add(ctr);
			body.Controls.Add(form);

			page.PreRenderComplete += new EventHandler(dialogPage_PreRenderComplete);
		}

		/// <summary>
		/// 对话框状态下，根据ViewState属性生成客户端清理ViewState的脚本
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private static void dialogPage_PreRenderComplete(object sender, EventArgs e)
		{
			Page page = (Page)sender;
			Control ctr = (Control)page.Items[WebUtility.PageRenderControlItemKey];

			if (page.IsCallback == false)
			{
				if (ctr.EnableViewState == false)
					DeluxeClientScriptManager.RegisterStartupScript(page, "document.getElementById('__VIEWSTATE').value = '';");
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ctr"></param>
		/// <returns></returns>
		public static ControlRenderMode GetControlRenderMode(Control ctr)
		{
			PageRenderMode pageRenderMode = WebUtility.GetRequestPageRenderMode();
			ControlRenderMode renderMode = new ControlRenderMode(pageRenderMode);

			PageRenderModePageCache currentPageCache = PageRenderModeHelper.GetPageRenderModeCache(ctr.Page);
			PageRenderModePageCache requestPageCache = renderMode.PageCache;

			int currentPageLevel = GetPageLevel(currentPageCache);
			string currentPageUniqueID = GetPageUniqueID(currentPageCache);
			int requestPageLevel = GetPageLevel(requestPageCache);
			string requestPageUniqueID = GetPageUniqueID(requestPageCache);

			if (requestPageLevel == currentPageLevel)
			{
				if (ctr.UniqueID == pageRenderMode.RenderControlUniqueID)
					renderMode.OnlyRenderSelf = true;
			}
			else if (requestPageLevel > currentPageLevel)
			{
				string id = requestPageUniqueID.Split(',')[currentPageLevel];

				if (ctr.UniqueID == id)
					renderMode.OnlyRenderSelf = true;
			}

			return renderMode;
		}

		internal static IEnumerable<ScriptReference> GetScriptReferences(Type controlType, string scriptPath)
		{
			List<ScriptReference> refs = new List<ScriptReference>();

			refs.AddRange(ScriptObjectBuilder.GetScriptReferences(controlType));

			if (scriptPath.Length > 0)
			{
				refs.Add(new ScriptReference(scriptPath));
			}

			lock (controlType)
			{
				if (ScriptControlSection.GetSection().UseScriptReferencesInAssembly == false)
				{
					refs = ConvertScriptReferenceToLocalScript(refs);
				}

				foreach (ScriptReference sr in refs)
				{
					if (sr.Path.IndexOf("ClientScriptCacheability.None") == 0)
						sr.Path = string.Empty;
				}
			}

			return refs;
		}

		private static List<ScriptReference> ConvertScriptReferenceToLocalScript(List<ScriptReference> refs)
		{
			List<ScriptReference> result = new List<ScriptReference>();

			foreach (ScriptReference sr in refs)
			{
				ScriptReference existedRef = null;

				//Path的ClientScriptCacheability.None是个约定，表示不能够进行文件缓存
				if (sr.Path.IndexOf("ClientScriptCacheability.None") == 0)
				{
					existedRef = sr;
				}
				else
				{
					if (ConvertedScriptReferencesCache.Instance.TryGetValue(sr.Name, out existedRef) == false)
					{
						existedRef = ConvertOneScriptToLocalScript(sr);
						ConvertedScriptReferencesCache.Instance.Add(existedRef.Name, existedRef);
					}
				}

				result.Add(existedRef);
			}

			return result;
		}

		private static ScriptReference ConvertOneScriptToLocalScript(ScriptReference originalScript)
		{
			ScriptReference result = originalScript;

			if (string.IsNullOrEmpty(originalScript.Assembly) == false)
			{
				Assembly assembly = Assembly.Load(originalScript.Assembly);

				byte[] buffer = new byte[4096];

				string targetVFilePath = ScriptControlSection.GetSection().LocalScriptVirtualDir;

				targetVFilePath = Path.Combine(targetVFilePath, originalScript.Name);
				targetVFilePath = targetVFilePath.Replace('\\', '/');

				string targetFilePath = HttpContext.Current.Server.MapPath(targetVFilePath);

				string targetDir = Path.GetDirectoryName(targetFilePath);

				if (Directory.Exists(targetDir) == false)
					Directory.CreateDirectory(targetDir);

				if (FileTimeMatched(assembly.Location, targetFilePath) == false)
				{
					lock (typeof(ScriptControlHelper))
					{
						using (Stream stream = assembly.GetManifestResourceStream(originalScript.Name))
						{
							using (FileStream fs = new FileStream(targetFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
							{
								stream.CopyTo(fs);
							}

							File.SetLastWriteTime(targetFilePath, File.GetLastAccessTime(assembly.Location));
						}
					}
				}

				result = new ScriptReference();
				result.Path = targetVFilePath;
			}

			return result;
		}

		private static bool FileTimeMatched(string srcFile, string destFile)
		{
			bool result = false;

			FileInfo srcInfo = new FileInfo(srcFile);
			FileInfo destInfo = new FileInfo(destFile);

			if (srcInfo.Exists && destInfo.Exists)
			{
				DateTime srcModifiedTime = srcInfo.LastWriteTime;
				DateTime descModifiedTime = destInfo.LastWriteTime;

				result = Math.Abs((srcModifiedTime - descModifiedTime).TotalSeconds) <= 2;
			}

			return result;
		}
	}
}
