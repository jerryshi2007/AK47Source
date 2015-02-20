using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.Expression
{
	/// <summary>
	/// 得到内置函数信息类
	/// </summary>
	public static class BuiltInFunctionHelper
	{
		/// <summary>
		/// 执行函数
		/// </summary>
		/// <param name="funcName"></param>
		/// <param name="target"></param>
		/// <param name="arrParams"></param>
		/// <param name="callerContext"></param>
		/// <returns></returns>
		public static object ExecuteFunction(string funcName, object target, ParamObjectCollection arrParams, object callerContext)
		{
			target.NullCheck("target");

			BuiltInFunctionInfoCollection funcsInfo = GetBuiltInFunctionsInfo(target.GetType());

			object result = null;

			if (funcsInfo.Contains(funcName))
				result = funcsInfo[funcName].ExecuteFunction(target, arrParams, callerContext);

			return result;
		}

		/// <summary>
		/// 执行静态函数
		/// </summary>
		/// <param name="funcName"></param>
		/// <param name="type"></param>
		/// <param name="arrParams"></param>
		/// <param name="callerContext"></param>
		/// <returns></returns>
		public static object ExecuteFunction(string funcName, Type type, ParamObjectCollection arrParams, object callerContext)
		{
			type.NullCheck("type");

			BuiltInFunctionInfoCollection funcsInfo = GetBuiltInFunctionsInfo(type);

			object result = null;

			if (funcsInfo.Contains(funcName))
				result = funcsInfo[funcName].ExecuteFunction(null, arrParams, callerContext);

			return result;
		}

		/// <summary>
		/// 从类型上获取内置表达式函数集合
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static BuiltInFunctionInfoCollection GetBuiltInFunctionsInfo(Type type)
		{
			type.NullCheck("type");

			return BuiltInFunctionInfoCache.Instance.GetOrAddNewValue(type, (cache, key) =>
			{
				BuiltInFunctionInfoCollection info = BuildBuiltInFunctionsInfo(type);

				cache.Add(key, info);

				return info;
			});
		}

		/// <summary>
		/// 从类型上获取内置表达式函数集合
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static BuiltInFunctionInfoCollection BuildBuiltInFunctionsInfo(Type type)
		{
			type.NullCheck("type");

			MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

			BuiltInFunctionInfoCollection result = new BuiltInFunctionInfoCollection();

			foreach (MethodInfo method in methods)
			{
				BuiltInFunctionAttribute attr = AttributeHelper.GetCustomAttribute<BuiltInFunctionAttribute>(method);

				if (attr != null)
				{
					if (attr.FunctionName.IsNullOrEmpty())
						attr.FunctionName = method.Name;

					if (result.Contains(attr.FunctionName) == false)
						result.Add(new BuiltInFunctionInfo(attr, method));
				}
			}

			return result;
		}
	}
}
