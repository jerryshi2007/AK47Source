using System;
using System.Web;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Specialized;
using MCS.Library.Core;
using MCS.Library.Caching;

namespace MCS.Web.Responsive.Library.MVC
{
	/// <summary>
	/// 控制器用到的辅助工具，帮助从request串映射到类的方法
	/// </summary>
	public static class ControllerHelper
	{
		/// <summary>
		/// 根据HttpRequest找到匹配的类方法，然后执行
		/// </summary>
		/// <param name="controller">包含方法的控制器类实例</param>
		public static void ExecuteMethodByRequest(object controller)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(controller != null, "controller");

			MethodBase mi = GetMethodInfoByCurrentUri(controller.GetType());
			ExceptionHelper.FalseThrow(mi != null, "不能处理请求，无法从当前的Request找到匹配的控制器方法");

			mi.Invoke(controller, HttpContext.Current.Request.QueryString);
		}

		/// <summary>
		/// 根据HttpRequest找到匹配的类方法
		/// </summary>
		/// <param name="controllerType">包含方法的控制器类型</param>
		/// <returns>匹配到的方法类型信息</returns>
		public static MethodBase GetMethodInfoByCurrentUri(System.Type controllerType)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(controllerType != null, "controllerType");

			ControllerInfo controllerInfo = null;

			if (ControllerInfoCache.Instance.TryGetValue(controllerType, out controllerInfo) == false)
			{
				controllerInfo = FindControllerMethods(controllerType);

				ControllerInfoCache.Instance.Add(controllerType, controllerInfo);
			}

			MethodBase mi = controllerInfo.ControllerMethods.GetMatchedMethodInfoByParameterNames(HttpContext.Current.Request.QueryString.AllKeys);

			if (mi == null)
				mi = controllerInfo.DefaultMethod;

			return mi;
		}

		/// <summary>
		/// 得到一个类型中，所有标记为ControllerMethod的方法
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private static ControllerInfo FindControllerMethods(System.Type type)
		{
			MethodInfo[] mis = type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

			List<ControllerMethodInfo> methodList = new List<ControllerMethodInfo>();
			MethodInfo defaultMethod = null;

			foreach (MethodInfo mi in mis)
			{
				ControllerMethodAttribute cma = AttributeHelper.GetCustomAttribute<ControllerMethodAttribute>(mi);

				if (cma != null)
				{
					ControllerMethodInfo cmi = new ControllerMethodInfo(mi, cma.ForceIgnoreParameters);

					methodList.Add(cmi);

					if (defaultMethod == null && cma.Default)
						defaultMethod = mi;
				}
			}

			return new ControllerInfo(methodList.ToArray(), defaultMethod);
		}
	}
}