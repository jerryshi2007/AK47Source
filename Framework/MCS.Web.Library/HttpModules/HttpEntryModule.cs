using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Configuration;

using MCS.Web.Library;
using MCS.Library.Core;

namespace MCS.Web.Library
{
	/// <summary>
	/// 通过此HttpModule加载deluxe.web/httpModules节点中配置的HttpModule集合
	/// </summary>
	public class HttpEntryModule : IHttpModule
	{
		private Dictionary<string, IHttpModule> _HttpModuleCollection;
		/// <summary>
		/// Init HttpModule
		/// </summary>
		/// <param name="context"></param>
		protected virtual void Init(HttpApplication context)
		{
			_HttpModuleCollection = new Dictionary<string, IHttpModule>();

			HttpModulesSection moduleSection = ConfigSectionFactory.GetHttpModulesSection();

			foreach (HttpModuleAction moduleAction in moduleSection.Modules)
			{
				try
				{
					IHttpModule module = (IHttpModule)TypeCreator.CreateInstance(moduleAction.Type);

					this._HttpModuleCollection.Add(moduleAction.Name, module);

					module.Init(context);
				}
				catch (TypeLoadException ex)
				{
					ex.TryWriteAppLog();
				}
			}
		}

		/// <summary>
		/// Dispose HttpModule
		/// </summary>
		protected virtual void Dispose()
		{
			foreach (IHttpModule module in this._HttpModuleCollection.Values)
			{
				module.Dispose();
			}
		}

		void IHttpModule.Init(HttpApplication context)
		{
			this.Init(context);
		}

		void IHttpModule.Dispose()
		{
			this.Dispose();
		}
	}
}
