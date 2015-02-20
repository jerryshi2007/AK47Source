using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using MCS.Library.Caching;
using MCS.Library.Core;

namespace MCS.Library.Core
{
	/// <summary>
	/// MethodBase相关的扩展方法
	/// </summary>
	public static class MethodExtensions
	{
		/// <summary>
		/// 找到参数名称最匹配的方法
		/// </summary>
		/// <param name="mbs"></param>
		/// <param name="parameterNames"></param>
		/// <returns></returns>
		public static MethodBase GetMatchedMethodInfoByParameterNames(this IEnumerable<MethodBase> mbs, IEnumerable<string> parameterNames)
		{
			MethodBase result = null;

			if (mbs != null)
			{
				int maxMatchLevel = 0;

				foreach (MethodBase mi in mbs)
				{
					int level = mi.GetParamsMatchedLevel(parameterNames);

					if (level > maxMatchLevel)
					{
						maxMatchLevel = level;
						result = mi;
					}
					else
						if (level == maxMatchLevel && result == null)
							result = mi;
				}
			}

			return result;
		}

		/// <summary>
		/// 得到参数名称和方法参数的匹配度
		/// </summary>
		/// <param name="mi"></param>
		/// <param name="parameterNames"></param>
		/// <returns></returns>
		public static int GetParamsMatchedLevel(this MethodBase mi, IEnumerable<string> parameterNames)
		{
			mi.NullCheck("mi");
			parameterNames.NullCheck("parameterNames");

			int result = 0;

			List<string> parameterNameListInMethod = new List<string>();

			CollectMethodParameterNames(mi, parameterNameListInMethod);

			foreach (string pNameInMethod in parameterNameListInMethod)
			{
				if (parameterNames.Any(pn => string.Compare(pn, pNameInMethod, true) == 0))
					result++;
			}

			result -= Math.Abs(parameterNameListInMethod.Count - result);	//方法参数的个数减去匹配程度，形成新的匹配度

			return result;
		}

		/// <summary>
		/// 根据参数，调用方法
		/// </summary>
		/// <param name="mi"></param>
		/// <param name="target"></param>
		/// <param name="reqParams"></param>
		/// <returns></returns>
		public static object Invoke(this MethodBase mi, object target, NameValueCollection reqParams)
		{
			return mi.Invoke(target, PrepareMethodParamValues(mi, reqParams));
		}

		/// <summary>
		/// 根据参数，调用构造方法
		/// </summary>
		/// <param name="ci"></param>
		/// <param name="reqParams"></param>
		/// <returns></returns>
		public static object Invoke(this ConstructorInfo ci, NameValueCollection reqParams)
		{
			return ci.Invoke(PrepareMethodParamValues(ci, reqParams));
		}

		private static object[] PrepareMethodParamValues(MethodBase mi, NameValueCollection reqParams)
		{
			ParameterInfo[] pis = mi.GetParameters();
			object[] paramValues = new object[pis.Length];

			for (int i = 0; i < pis.Length; i++)
			{
				if (IsCollectedParemeterClass(pis[i].ParameterType))
					paramValues[i] = GetCollectedParameterInstance(reqParams, pis[i].ParameterType);
				else
					paramValues[i] = ChangeRequestValueToTargetType(pis[i].Name, reqParams, pis[i].ParameterType);
			}

			return paramValues;
		}

		private static void CollectMethodParameterNames(MethodBase mi, List<string> nameList)
		{
			Dictionary<string, string> nameDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

			mi.GetParameters().ForEach(p =>
			{
				if (IsCollectedParemeterClass(p.ParameterType))
				{
					CollectObjectPropertyNames(p.ParameterType, nameDictionary);
				}
				else
				{
					if (nameDictionary.ContainsKey(p.Name) == false)
						nameDictionary.Add(p.Name, p.Name);
				}
			});

			foreach (KeyValuePair<string, string> kp in nameDictionary)
			{
				nameList.Add(kp.Key);
			}
		}

		/// <summary>
		/// 是否是可收集数据的类型
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private static bool IsCollectedParemeterClass(System.Type type)
		{
			return type.IsEnum == false && type.IsArray == false &&
				type.IsPrimitive == false && type != typeof(string);
		}

		private static void CollectObjectPropertyNames(Type parameterType, Dictionary<string, string> nameDictionary)
		{
			Dictionary<string, PropertyInfo> propertyDict = TypeFlattenHierarchyPropertiesCacheQueue.Instance.GetPropertyDictionary(parameterType);

			propertyDict.ForEach(kp =>
			{
				if (nameDictionary.ContainsKey(kp.Key) == false)
					nameDictionary.Add(kp.Key, kp.Key);
			});
		}

		/// <summary>
		/// 根据Request的参数得到可收集参数的值
		/// </summary>
		/// <param name="reqParams"></param>
		/// <param name="parameterType"></param>
		/// <returns></returns>
		private static object GetCollectedParameterInstance(NameValueCollection reqParams, System.Type parameterType)
		{
			object graph = TypeCreator.CreateInstance(parameterType);

			Dictionary<string, PropertyInfo> propertyDict = TypeFlattenHierarchyPropertiesCacheQueue.Instance.GetPropertyDictionary(parameterType);

			propertyDict.ForEach(kp =>
			{
				PropertyInfo pi = kp.Value;

				if (pi.CanWrite)
				{
					object targetValue = ChangeRequestValueToTargetType(pi.Name, reqParams, pi.PropertyType);

					if (targetValue != null)
					{
						pi.SetValue(graph, targetValue, null);
					}
				}
			});

			return graph;
		}

		private static object ChangeRequestValueToTargetType(string paramName, NameValueCollection reqParams, System.Type targetType)
		{
			object result = null;

			string rValue = reqParams[paramName];

			if (rValue != null)
				result = DataConverter.ChangeType(rValue, targetType);

			return result;
		}
	}
}
