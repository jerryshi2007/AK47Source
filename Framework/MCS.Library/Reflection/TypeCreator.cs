#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library
// FileName	��	TypeCreator.cs
// Remark	��	������󶨷�ʽ��̬����һ��ʵ�� 
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ���	    20070430		����
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
	/// ������󶨷�ʽ��̬����ʵ��
	/// </summary>
	/// <remarks>������󶨷�ʽ��̬����ʵ����
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
		/// ���ú�󶨷�ʽ��̬�Ĵ���һ��ʵ����
		/// </summary>
		/// <param name="typeDescription">����ʵ����������������</param>
		/// <param name="constructorParams">����ʵ���ĳ�ʼ������</param>
		/// <returns>ʵ������</returns>
		/// <remarks>������󶨷�ʽ��̬����һ��ʵ��
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\TypeCreatorTest.cs" region = "CreateInstanceTest" lang="cs" title="������󶨴���һ��ʵ��" />
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
		/// ����������Ϣ��ͨ�����乹������ͣ����Ҹ��ݹ�����������ҵ���ƥ��Ĺ��췽����Ȼ����ù��췽������������ͼ��
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="typeDescription"></param>
		/// <param name="constructorParams"></param>
		/// <returns></returns>
		public static T CreateInstance<T>(string typeDescription, params object[] constructorParams)
		{
			object result = CreateInstance(typeDescription, constructorParams);

			(result is T).FalseThrow("�޷�������{0}ת��Ϊ{1}", typeDescription, typeof(T).AssemblyQualifiedName);

			return (T)result;
		}

		/// <summary>
		/// ����������Ϣ�������󣬸ö���ʹû�й��еĹ��췽����Ҳ���Դ���ʵ��
		/// </summary>
		/// <param name="type">��������ʱ��������Ϣ</param>
		/// <param name="constructorParams">����ʵ���ĳ�ʼ������</param>
		/// <returns>ʵ������</returns>
		/// <remarks>������󶨷�ʽ��̬����һ��ʵ��</remarks>
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
		/// ����������Ϣ��ͨ�����乹������ͣ����Ҹ��ݹ�����������ҵ���ƥ��Ĺ��췽����Ȼ����ù��췽������������ͼ��
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="type"></param>
		/// <param name="constructorParams"></param>
		/// <returns></returns>
		public static T CreateInstance<T>(System.Type type, params object[] constructorParams)
		{
			object result = CreateInstance(type, constructorParams);

			(result is T).FalseThrow("�޷�������{0}ת��Ϊ{1}", type.AssemblyQualifiedName, typeof(T).AssemblyQualifiedName);

			return (T)result;
		}

		/// <summary>
		/// ����������Ϣ��ͨ�����乹������ͣ����Ҹ��ݹ�����������ҵ���ƥ��Ĺ��췽����Ȼ����ù��췽��
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
		/// ����������Ϣ��ͨ�����乹������ͣ����Ҹ��ݹ�����������ҵ���ƥ��Ĺ��췽����Ȼ����ù��췽������������ͼ��
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="typeDescription"></param>
		/// <param name="constructorParams"></param>
		/// <returns></returns>
		public static T CreateInstance<T>(string typeDescription, NameValueCollection constructorParams)
		{
			object result = CreateInstance(typeDescription, constructorParams);

			(result is T).FalseThrow("�޷�������{0}ת��Ϊ{1}", typeDescription, typeof(T).AssemblyQualifiedName);

			return (T)result;
		}

		/// <summary>
		/// ����������Ϣ��ͨ�����乹������ͣ����Ҹ��ݹ�����������ҵ���ƥ��Ĺ��췽����Ȼ����ù��췽��
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

				ExceptionHelper.FalseThrow(ci != null, "�����ҵ�����{0}�Ĺ��췽��", type.AssemblyQualifiedName);

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
		/// �������������õ����Ͷ���
		/// </summary>
		/// <param name="typeDescription">��������������Ӧ����Namespace.ClassName, AssemblyName</param>
		/// <returns>���Ͷ���</returns>
		public static Type GetTypeInfo(string typeDescription)
		{
			ExceptionHelper.CheckStringIsNullOrEmpty(typeDescription, "typeDescription");

			Type result = Type.GetType(typeDescription);

			if (result == null)
			{
				TypeInfo ti = GenerateTypeInfo(typeDescription);

				AssemblyName aName = new AssemblyName(ti.AssemblyName);

				AssemblyMappingConfigurationElement element = AssemblyMappingSettings.GetConfig().Mapping[aName.Name];

				ExceptionHelper.TrueThrow<TypeLoadException>(element == null, "�����ҵ�����{0}", typeDescription);

				ti.AssemblyName = element.MapTo;

				result = Type.GetType(ti.ToString());

				ExceptionHelper.FalseThrow<TypeLoadException>(result != null, "���ܵõ�������Ϣ{0}", ti.ToString());
			}

			return result;
		}

		/// <summary>
		/// ��������������ͼ�õ����Ͷ�������ɹ�������true������Ϊfalse
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
		/// �õ�ĳ���������͵�ȱʡֵ
		/// </summary>
		/// <param name="type">����</param>
		/// <returns>�����͵�ȱʡֵ</returns>
		/// <remarks>���������Ϊ�������ͣ��򷵻�null�����򷵻�ֵ���͵�ȱʡֵ����Int32����0��DateTime����DateTime.MinValue</remarks>
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
