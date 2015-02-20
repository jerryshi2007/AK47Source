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
	/// �������õ��ĸ������ߣ�������request��ӳ�䵽��ķ���
	/// </summary>
	public static class ControllerHelper
	{
		/// <summary>
		/// ����HttpRequest�ҵ�ƥ����෽����Ȼ��ִ��
		/// </summary>
		/// <param name="controller">���������Ŀ�������ʵ��</param>
		public static void ExecuteMethodByRequest(object controller)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(controller != null, "controller");

			MethodBase mi = GetMethodInfoByCurrentUri(controller.GetType());
			ExceptionHelper.FalseThrow(mi != null, "���ܴ��������޷��ӵ�ǰ��Request�ҵ�ƥ��Ŀ���������");

			mi.Invoke(controller, HttpContext.Current.Request.QueryString);
		}

		/// <summary>
		/// ����HttpRequest�ҵ�ƥ����෽��
		/// </summary>
		/// <param name="controllerType">���������Ŀ���������</param>
		/// <returns>ƥ�䵽�ķ���������Ϣ</returns>
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
		/// �õ�һ�������У����б��ΪControllerMethod�ķ���
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