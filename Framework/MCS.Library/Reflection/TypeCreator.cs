#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library
// FileName	：	TypeCreator.cs
// Remark	：	运用晚绑定方式动态创建一个实例 
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    沈峥	    20070430		创建
// -------------------------------------------------
#endregion
using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using MCS.Library.Properties;
using MCS.Library.Caching;
using MCS.Library.Configuration;
using System.Collections.Specialized;

namespace MCS.Library.Core
{
	/// <summary>
	/// 运用晚绑定方式动态生成实例
	/// </summary>
	/// <remarks>运用晚绑定方式动态生成实例。
	/// </remarks>
	public static class TypeCreator
	{
		private struct TypeInfo
		{
			public string AssemblyName;
			public string TypeName;

			public override string ToString()
			{
				return TypeName + ", " + AssemblyName;
			}
		}

		/// <summary>
		/// 运用后绑定方式动态的创建一个实例。
		/// </summary>
		/// <param name="typeDescription">创建实例的完整类型名称</param>
		/// <param name="constructorParams">创建实例的初始化参数</param>
		/// <returns>实例对象</returns>
		/// <remarks>运用晚绑定方式动态创建一个实例
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\TypeCreatorTest.cs" region = "CreateInstanceTest" lang="cs" title="运用晚绑定创建一个实例" />
		/// <seealso cref="MCS.Library.Logging.LogFilterFactory"/>
		/// <seealso cref="MCS.Library.Logging.LogFormatterFactory"/>
		/// <seealso cref="MCS.Library.Logging.TraceListenerFactory"/>
		/// </remarks>
		public static object CreateInstance(string typeDescription, params object[] constructorParams)
		{
			Type type = GetTypeInfo(typeDescription);

			if (type == null)
				throw new TypeLoadException(string.Format(Resource.TypeLoadException, typeDescription));

			return CreateInstance(type, constructorParams);
		}

		/// <summary>
		/// 根据类型信息，通过反射构造此类型，并且根据构造参数名称找到最匹配的构造方法，然后调用构造方法。会进行类型检查
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="typeDescription"></param>
		/// <param name="constructorParams"></param>
		/// <returns></returns>
		public static T CreateInstance<T>(string typeDescription, params object[] constructorParams)
		{
			object result = CreateInstance(typeDescription, constructorParams);

			(result is T).FalseThrow("无法将类型{0}转换为{1}", typeDescription, typeof(T).AssemblyQualifiedName);

			return (T)result;
		}

		/// <summary>
		/// 根据类型信息创建对象，该对象即使没有公有的构造方法，也可以创建实例
		/// </summary>
		/// <param name="type">创建类型时的类型信息</param>
		/// <param name="constructorParams">创建实例的初始化参数</param>
		/// <returns>实例对象</returns>
		/// <remarks>运用晚绑定方式动态创建一个实例</remarks>
		public static object CreateInstance(System.Type type, params object[] constructorParams)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(type != null, "type");
			ExceptionHelper.FalseThrow<ArgumentNullException>(constructorParams != null, "constructorParams");

			BindingFlags bf = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

			try
			{
				return Activator.CreateInstance(type, bf, null, constructorParams, null);
			}
			catch (TargetInvocationException ex)
			{
				Exception realEx = ExceptionHelper.GetRealException(ex);

				string message = type.AssemblyQualifiedName + ":" + realEx.Message;

				throw new TargetInvocationException(message, realEx);
			}
		}

		/// <summary>
		/// 根据类型信息，通过反射构造此类型，并且根据构造参数名称找到最匹配的构造方法，然后调用构造方法。会进行类型检查
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="type"></param>
		/// <param name="constructorParams"></param>
		/// <returns></returns>
		public static T CreateInstance<T>(System.Type type, params object[] constructorParams)
		{
			object result = CreateInstance(type, constructorParams);

			(result is T).FalseThrow("无法将类型{0}转换为{1}", type.AssemblyQualifiedName, typeof(T).AssemblyQualifiedName);

			return (T)result;
		}

		/// <summary>
		/// 根据类型信息，通过反射构造此类型，并且根据构造参数名称找到最匹配的构造方法，然后调用构造方法
		/// </summary>
		/// <param name="typeDescription"></param>
		/// <param name="constructorParams"></param>
		/// <returns></returns>
		public static object CreateInstance(string typeDescription, NameValueCollection constructorParams)
		{
			Type type = GetTypeInfo(typeDescription);

			if (type == null)
				throw new TypeLoadException(string.Format(Resource.TypeLoadException, typeDescription));

			return CreateInstance(type, constructorParams);
		}

		/// <summary>
		/// 根据类型信息，通过反射构造此类型，并且根据构造参数名称找到最匹配的构造方法，然后调用构造方法。会进行类型检查
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="typeDescription"></param>
		/// <param name="constructorParams"></param>
		/// <returns></returns>
		public static T CreateInstance<T>(string typeDescription, NameValueCollection constructorParams)
		{
			object result = CreateInstance(typeDescription, constructorParams);

			(result is T).FalseThrow("无法将类型{0}转换为{1}", typeDescription, typeof(T).AssemblyQualifiedName);

			return (T)result;
		}

		/// <summary>
		/// 根据类型信息，通过反射构造此类型，并且根据构造参数名称找到最匹配的构造方法，然后调用构造方法
		/// </summary>
		/// <param name="type"></param>
		/// <param name="constructorParams"></param>
		/// <returns></returns>
		public static object CreateInstance(System.Type type, NameValueCollection constructorParams)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(type != null, "type");
			ExceptionHelper.FalseThrow<ArgumentNullException>(constructorParams != null, "constructorParams");

			BindingFlags bf = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

			try
			{
				ConstructorInfo ci = (ConstructorInfo)type.GetConstructors(bf).GetMatchedMethodInfoByParameterNames(constructorParams.AllKeys);

				ExceptionHelper.FalseThrow(ci != null, "不能找到类型{0}的构造方法", type.AssemblyQualifiedName);

				return ci.Invoke(constructorParams);
			}
			catch (TargetInvocationException ex)
			{
				Exception realEx = ExceptionHelper.GetRealException(ex);

				string message = type.AssemblyQualifiedName + ":" + realEx.Message;

				throw new TargetInvocationException(message, realEx);
			}
		}

		/// <summary>
		/// 根据类型描述得到类型对象
		/// </summary>
		/// <param name="typeDescription">完整类型描述，应该是Namespace.ClassName, AssemblyName</param>
		/// <returns>类型对象</returns>
		public static Type GetTypeInfo(string typeDescription)
		{
			ExceptionHelper.CheckStringIsNullOrEmpty(typeDescription, "typeDescription");

			Type result = Type.GetType(typeDescription);

			if (result == null)
			{
				TypeInfo ti = GenerateTypeInfo(typeDescription);

				AssemblyName aName = new AssemblyName(ti.AssemblyName);

				AssemblyMappingConfigurationElement element = AssemblyMappingSettings.GetConfig().Mapping[aName.Name];

				ExceptionHelper.TrueThrow<TypeLoadException>(element == null, "不能找到类型{0}", typeDescription);

				ti.AssemblyName = element.MapTo;

				result = Type.GetType(ti.ToString());

				ExceptionHelper.FalseThrow<TypeLoadException>(result != null, "不能得到类型信息{0}", ti.ToString());
			}

			return result;
		}

		/// <summary>
		/// 根据类型描述试图得到类型对象，如果成功，返回true，否则为false
		/// </summary>
		/// <param name="typeDescription"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool TryGetTypeInfo(string typeDescription, out Type type)
		{
			type = Type.GetType(typeDescription);

			if (type == null)
			{
				TypeInfo ti = GenerateTypeInfo(typeDescription);

				AssemblyName aName = new AssemblyName(ti.AssemblyName);

				AssemblyMappingConfigurationElement element = AssemblyMappingSettings.GetConfig().Mapping[aName.Name];

				if (element != null)
				{
					ti.AssemblyName = element.MapTo;
					type = Type.GetType(ti.ToString());
				}
			}

			return type != null;
		}

		/// <summary>
		/// 得到某个数据类型的缺省值
		/// </summary>
		/// <param name="type">类型</param>
		/// <returns>该类型的缺省值</returns>
		/// <remarks>如果该类型为引用类型，则返回null，否则返回值类型的缺省值。如Int32返回0，DateTime返回DateTime.MinValue</remarks>
		public static object GetTypeDefaultValue(System.Type type)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(type != null, "type");

			object result = null;

			if (type.IsValueType)
			{
				if (TypeDefaultValueCacheQueue.Instance.TryGetValue(type, out result) == false)
				{
					result = TypeCreator.CreateInstance(type);

					TypeDefaultValueCacheQueue.Instance.Add(type, result);
				}
			}
			else
				result = null;

			return result;
		}

		private static TypeInfo GenerateTypeInfo(string typeDescription)
		{
			TypeInfo info = new TypeInfo();

			string[] typeParts = typeDescription.Split(',');

			info.TypeName = typeParts[0].Trim();

			StringBuilder strB = new StringBuilder(256);

			for (int i = 1; i < typeParts.Length; i++)
			{
				if (strB.Length > 0)
					strB.Append(", ");

				strB.Append(typeParts[i]);
			}

			info.AssemblyName = strB.ToString().Trim();

			return info;
		}
	}

	internal class TypeDefaultValueCacheQueue : CacheQueue<Type, object>
	{
		public static readonly TypeDefaultValueCacheQueue Instance = CacheManager.GetInstance<TypeDefaultValueCacheQueue>();
	}
}
