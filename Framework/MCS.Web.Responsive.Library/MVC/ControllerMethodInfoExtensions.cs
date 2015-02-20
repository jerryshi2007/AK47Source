using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using MCS.Library.Core;

namespace MCS.Web.Responsive.Library.MVC
{
	internal static class ControllerMethodInfoExtensions
	{
		/// <summary>
		/// 找到参数名称最匹配的方法
		/// </summary>
		/// <param name="mbs"></param>
		/// <param name="parameterNames"></param>
		/// <returns></returns>
		public static MethodBase GetMatchedMethodInfoByParameterNames(this IEnumerable<ControllerMethodInfo> mbs, IEnumerable<string> parameterNames)
		{
			IEnumerable<MethodInfo> methods = mbs.GetNotIgnoredMethods(parameterNames);

			return methods.GetMatchedMethodInfoByParameterNames(parameterNames);
		}

		/// <summary>
		/// 过滤掉存在需要忽略掉的参数的方法
		/// </summary>
		/// <param name="mbs"></param>
		/// <param name="parameterNames"></param>
		/// <returns></returns>
		private static IEnumerable<MethodInfo> GetNotIgnoredMethods(this IEnumerable<ControllerMethodInfo> mbs, IEnumerable<string> parameterNames)
		{
			List<MethodInfo> result = new List<MethodInfo>(10);

			foreach (ControllerMethodInfo mb in mbs)
			{
				bool ignore = false;

				foreach (string paramName in parameterNames)
				{
					if (mb.ForceIgnoreParameters.Contains(paramName))
					{
						ignore = true;
						break;
					}
				}

				if (ignore == false)
					result.Add(mb.ControllerMethod);
			}

			return result;
		}
	}
}
