using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using MCS.Library.Core;
using MCS.Library.Caching;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Archive;
using MCS.Library.SOA.DataObjects.Workflow;

namespace MCS.Web.WebControls
{
	public class ArchiveRedirectModule : IHttpModule
	{
		#region IHttpModule Members

		public void Dispose()
		{
		}

		public void Init(HttpApplication context)
		{
			context.PreRequestHandlerExecute += new EventHandler(context_PreRequestHandlerExecute);
			context.EndRequest += new EventHandler(context_EndRequest);
		}

		private void context_PreRequestHandlerExecute(object sender, EventArgs e)
		{
			string resourceID = HttpContext.Current.Request.QueryString["resourceID"];
			string materialID = HttpContext.Current.Request.QueryString["materialID"];

			if (string.Compare(HttpContext.Current.Request.RequestType, "GET", true) == 0)
			{
				ArchiveSettings settings = ArchiveSettings.GetConfig();

				if (string.IsNullOrEmpty(resourceID) == false)
				{
					if (HttpContext.Current.Request.QueryString["isNew"] == null)
					{
						if (settings.AutoRedirectToArchiveDB)
						{
							if (OriginalDataExists(resourceID) == false)
								SetConnectionMappings();
						}
					}
				}

				if (materialID.IsNotEmpty() && ArchiveContext.Current.InArchiveMode == false)
				{
					if (OriginalMaterialContentExists(materialID) == false)
						SetConnectionMappings();
				}

				HttpContext.Current.Items["AppPathMappingContexts"] =
					settings.AppPathMappings.CreateAppPathMappingContexts();
			}
		}

		private void context_EndRequest(object sender, EventArgs e)
		{
			ArchiveConnectionMappingContextCollection contexts =
					(ArchiveConnectionMappingContextCollection)HttpContext.Current.Items["ConnectionMappingContexts"];

			if (contexts != null)
				contexts.Dispose();
		}

		private static void SetConnectionMappings()
		{
			HttpContext.Current.Items["ConnectionMappingContexts"] =
									ArchiveSettings.GetConfig().ConnectionMappings.CreateConnectionMappingContexts();

			ArchiveContext.Current.InArchiveMode = true;
		}

		private bool OriginalDataExists(string resourceID)
		{
			bool result = false;

			try
			{
				WfProcessCollection processes = WfRuntime.GetProcessByResourceID(resourceID);

				result = processes.Count > 0;

				if (result == false)
					result = AppCommonInfoAdapter.Instance.ExistsAndNotArchived(resourceID);
			}
			catch (System.Exception)
			{
			}

			return result;
		}

		private bool OriginalMaterialContentExists(string materialID)
		{
			bool result = false;

			try
			{
				result = MaterialContentAdapter.Instance.Exists(b => b.AppendItem("CONTENT_ID", materialID));
			}
			catch (System.Exception)
			{
			}

			return result;
		}
		#endregion
	}

	/// <summary>
	/// 当前是否处于归档状态
	/// </summary>
	public class ArchiveContext
	{
		private ArchiveContext()
		{
		}

		public bool InArchiveMode
		{
			get
			{
				bool result = false;

				object data = HttpContext.Current.Items["InArchiveMode"];

				if (data != null)
				{
					result = (bool)data;
				}

				return result;
			}
			internal set
			{
				HttpContext.Current.Items["InArchiveMode"] = value;
			}
		}

		public static ArchiveContext Current
		{
			get
			{
				object current = null;

				if (ObjectContextCache.Instance.TryGetValue("ArchiveContext", out current) == false)
				{
					current = new ArchiveContext();

					ObjectContextCache.Instance.Add("ArchiveContext", current);
				}

				return (ArchiveContext)current;
			}
			set
			{
				ObjectContextCache.Instance["ArchiveContext"] = value;
			}
		}
	}
}
