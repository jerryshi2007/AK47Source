using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Caching;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	public class WfApplicationParametersContext : IDisposable
	{
		private WfApplicationRuntimeParameters _Parameters = null;

		private WfApplicationParametersContext()
		{
		}

		public static WfApplicationParametersContext CreateContext(IDictionary<string, object> parameters)
		{
			ObjectContextCache.Instance.ContainsKey(CacheKey).TrueThrow("WfApplicationParametersContext已经在使用中，不能嵌套使用");

			WfApplicationParametersContext context = new WfApplicationParametersContext();

			if (parameters != null)
				context.ApplicationRuntimeParameters.CopyFrom(parameters);

			ObjectContextCache.Instance.Add(CacheKey, context);

			return context;
		}

		public WfApplicationRuntimeParameters ApplicationRuntimeParameters
		{
			get
			{
				if (this._Parameters == null)
					this._Parameters = new WfApplicationRuntimeParameters();

				return this._Parameters;
			}
		}

		private static string CacheKey
		{
			get
			{
				return typeof(WfApplicationParametersContext).Name;
			}
		}

		public static WfApplicationParametersContext Current
		{
			get
			{
				return GetCurrentContext();
			}
		}

		private static WfApplicationParametersContext GetCurrentContext()
		{
			object context = null;

			ObjectContextCache.Instance.TryGetValue(CacheKey, out context);

			return (WfApplicationParametersContext)context;
		}

		public void Dispose()
		{
			object context = null;

			if (ObjectContextCache.Instance.TryGetValue(CacheKey, out context))
			{
				ObjectContextCache.Instance.Remove(CacheKey);
			}
		}
	}
}
