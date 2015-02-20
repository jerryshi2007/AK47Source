using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using MCS.Library.Caching;

namespace MCS.Web.Responsive.Library.MVC
{
	internal class ControllerInfo
	{
		private ControllerMethodInfo[] controllerMethods = null;
		private MethodInfo defaultMethod = null;

		public ControllerInfo(ControllerMethodInfo[] mis, MethodInfo defMethod)
		{
			this.controllerMethods = mis;
			this.defaultMethod = defMethod;
		}

		public ControllerMethodInfo[] ControllerMethods
		{
			get
			{
				return controllerMethods;
			}
		}

		public MethodInfo DefaultMethod
		{
			get
			{
				return defaultMethod;
			}
		}
	}

	internal sealed class ControllerInfoCache : CacheQueue<System.Type, ControllerInfo>
	{
		public static readonly ControllerInfoCache Instance = CacheManager.GetInstance<ControllerInfoCache>();
	}
}
