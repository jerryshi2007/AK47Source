#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library
// FileName	��	SerializationHelper.cs
// Remark	��	SerializationFormatterTypeö���Ͱ���Soap��Binary��Soap��SOAP��Ϣ��ʽ���룬Binary�Ƕ�������Ϣ��ʽ���� 
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ���	    20070430		����
// -------------------------------------------------
#endregion

using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization.Formatters.Soap;
using System.Runtime.Serialization.Formatters.Binary;

namespace MCS.Library.Core
{
    /// <summary>
    /// ö�����ͣ������Soap��Binary���л����뷽ʽ
    /// </summary>
    /// <remarks>SerializationFormatterTypeö���Ͱ���Soap��Binary��Soap��SOAP��Ϣ��ʽ���룬Binary�Ƕ�������Ϣ��ʽ���롣</remarks>
    public enum SerializationFormatterType
    {
        /// <summary>
        /// SOAP��Ϣ��ʽ����
        /// </summary>
        Soap,

        /// <summary>
        /// ��������Ϣ��ʽ����
        /// </summary>
        Binary
    }

    /// <summary>
    /// ��������ʵ�����л��ͷ����л���
    /// </summary>
    /// <remarks>�������л��ǰѶ������л�ת��Ϊstring���ͣ��������л��ǰѶ����string���ͷ����л�ת��Ϊ��Դ���͡�
    /// </remarks>
    public static class SerializationHelper
    {
        #region Helper method
        /// <summary>
        /// ���մ��л��ı���Ҫ�����ɶ�Ӧ�ı�������
        /// </summary>
        /// <param name="formatterType"></param>
        /// <returns></returns>
        private static IRemotingFormatter GetFormatter(SerializationFormatterType formatterType)
        {
            switch (formatterType)
            {
                case SerializationFormatterType.Binary:
                    return new BinaryFormatter();
                case SerializationFormatterType.Soap:
                    return new SoapFormatter();
                default:
                    throw new NotSupportedException();
            }
        }
        #endregion

        /// <summary>
        /// �Ѷ������л�ת��Ϊ�ַ���
        /// </summary>
        /// <param name="graph">�ɴ��л�����ʵ��</param>
        /// <param name="formatterType">��Ϣ��ʽ�������ͣ�Soap��Binary�ͣ�</param>
        /// <returns>���л�ת�����</returns>
        /// <remarks>����BinaryFormatter��SoapFormatter��Serialize����ʵ����Ҫת�����̡�
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\SerializationHelperTest.cs" region="TestSerializeAndDeserialize" lang="cs" title="�Ѷ���ת��Ϊ�ַ���" />
        /// </remarks>    
        public static string SerializeObjectToString(this object graph, SerializationFormatterType formatterType)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(graph != null, "graph");

            using (MemoryStream memoryStream = new MemoryStream())
            {
                IRemotingFormatter formatter = GetFormatter(formatterType);
                formatter.Serialize(memoryStream, graph);
                Byte[] arrGraph = memoryStream.ToArray();

                return Convert.ToBase64String(arrGraph);
            }
        }

        /// <summary>
        /// �������л�Ϊ�ַ������͵Ķ������л�Ϊָ��������
        /// </summary>
        /// <param name="serializedGraph">�����л�Ϊ�ַ������͵Ķ���</param>
        /// <param name="formatterType">��Ϣ��ʽ�������ͣ�Soap��Binary�ͣ�</param>
        /// <typeparam name="T">����ת���������</typeparam>
        /// <returns>���л�ת�����</returns>
        /// <remarks>����BinaryFormatter��SoapFormatter��Deserialize����ʵ����Ҫת�����̡�
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\SerializationHelperTest.cs" region="TestSerializeAndDeserialize" lang="cs" title="���ַ�������ת��Ϊָ��������" />
        /// </remarks>
        public static T DeserializeStringToObject<T>(this string serializedGraph, SerializationFormatterType formatterType)
        {
            return (T)DeserializeStringToObject(serializedGraph, formatterType);
        }

        /// <summary>
        /// �������л�Ϊ�ַ������͵Ķ������л�Ϊָ��������
        /// </summary>
        /// <typeparam name="T">����ת���������</typeparam>
        /// <param name="serializedGraph">�����л�Ϊ�ַ������͵Ķ���</param>
        /// <param name="formatterType">��Ϣ��ʽ�������ͣ�Soap��Binary�ͣ�</param>
        /// <param name="binder">�����л�ʱ������ת����</param>
        /// <returns>���л�ת����</returns>
        /// <remarks>����BinaryFormatter��SoapFormatter��Deserialize����ʵ����Ҫת�����̡�</remarks>
        public static T DeserializeStringToObject<T>(this string serializedGraph, SerializationFormatterType formatterType, SerializationBinder binder)
        {
            return (T)DeserializeStringToObject(serializedGraph, formatterType, binder);
        }

        /// <summary>
        /// �������л�Ϊ�ַ������͵Ķ������л�Ϊָ��������
        /// </summary>
        /// <param name="serializedGraph">�����л�Ϊ�ַ������͵Ķ���</param>
        /// <param name="formatterType">��Ϣ��ʽ�������ͣ�Soap��Binary�ͣ�</param>
        /// <returns>���л�ת�����</returns>
        /// <remarks>����BinaryFormatter��SoapFormatter��Deserialize����ʵ����Ҫת�����̡�
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\SerializationHelperTest.cs" region="TestSerializeAndDeserialize" lang="cs" title="���ַ�������ת��Ϊָ��������" />
        /// </remarks>
        public static object DeserializeStringToObject(this string serializedGraph, SerializationFormatterType formatterType)
        {
            return DeserializeStringToObject(serializedGraph, formatterType, null);
        }

        /// <summary>
        /// �������л�Ϊ�ַ������͵Ķ������л�Ϊָ��������
        /// </summary>
        /// <param name="serializedGraph">�����л�Ϊ�ַ������͵Ķ���</param>
        /// <param name="formatterType">��Ϣ��ʽ�������ͣ�Soap��Binary�ͣ�</param>
        /// <param name="binder"></param>
        /// <returns>���л�ת�����</returns>
        /// <remarks>����BinaryFormatter��SoapFormatter��Deserialize����ʵ����Ҫת�����̡�</remarks>
        public static object DeserializeStringToObject(this string serializedGraph, SerializationFormatterType formatterType, SerializationBinder binder)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(serializedGraph, "serializedGraph");

            Byte[] arrGraph = Convert.FromBase64String(serializedGraph);
            using (MemoryStream memoryStream = new MemoryStream(arrGraph))
            {
                IRemotingFormatter formatter = GetFormatter(formatterType);
                formatter.Binder = binder;

                return formatter.Deserialize(memoryStream);
            }
        }

        /// <summary>
        /// ͨ�����л����ƶ���
        /// </summary>
        /// <param name="graph"></param>
        /// <returns></returns>
        public static object CloneObject(this object graph)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(graph != null, "graph");

            using (MemoryStream memoryStream = new MemoryStream(1024))
            {
                BinaryFormatter formatter = new BinaryFormatter();

                formatter.Serialize(memoryStream, graph);

                memoryStream.Position = 0;

                return formatter.Deserialize(memoryStream);
            }
        }

        /// <summary>
        /// ��һ��Streamת��ΪByte����
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this Stream stream)
        {
            using (MemoryStream ms = new MemoryStream(2048))
            {
                stream.CopyTo(ms);

                return ms.ToArray();
            }
        }
    }

    /// <summary>
    /// ��汾�޹ص�Binder�࣬���ڷ����л�����
    /// </summary>
    public class VersionStrategyBinder : SerializationBinder
    {
        private const string VersionMatchTemplate = @"Version=([0-9.]{1,})(,)";

        private VersionStrategyBinder()
        {
        }

        /// <summary>
        /// ��һʵ��
        /// </summary>
        public static readonly VersionStrategyBinder Instance = new VersionStrategyBinder();

        /// <summary>
        /// ������������������ת����ȥ��Version��Ϣ�����������Ͷ���
        /// </summary>
        /// <param name="assemblyName">AssemblyName</param>
        /// <param name="typeName">���͵�����</param>
        /// <returns>���͵�ʵ��</returns>
        public override Type BindToType(string assemblyName, string typeName)
        {
            typeName = AdjustTypeName(assemblyName, typeName);

            Assembly assembly = Assembly.Load(assemblyName);

            return assembly.GetType(typeName);
        }

        /// <summary>
        /// �����������ƣ�ȥ�������еİ汾��������
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <param name="typeName"></param>
        /// <returns></returns>
        internal static string AdjustTypeName(string assemblyName, string typeName)
        {
            return Regex.Replace(typeName, VersionMatchTemplate, string.Empty, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }
    }

    /// <summary>
    /// δ֪���͵ķ����л���
    /// </summary>
    public class UnknownTypeStrategyBinder : SerializationBinder
    {
        private UnknownTypeStrategyBinder()
        {
        }

        /// <summary>
        /// ��һʵ��
        /// </summary>
        public static readonly UnknownTypeStrategyBinder Instance = new UnknownTypeStrategyBinder();

        /// <summary>
        /// ������������������ת����ȥ��Version��Ϣ�����������Ͷ���
        /// </summary>
        /// <param name="assemblyName">AssemblyName</param>
        /// <param name="typeName">���͵�����</param>
        /// <returns>���͵�ʵ��</returns>
        public override Type BindToType(string assemblyName, string typeName)
        {
            typeName = VersionStrategyBinder.AdjustTypeName(assemblyName, typeName);

            Type result = null;

            try
            {
                Assembly assembly = Assembly.Load(assemblyName);

                result = assembly.GetType(typeName);

                if (result == null)
                    result = typeof(UnknownSerializationType);
            }
            catch (System.Exception)
            {
                result = typeof(UnknownSerializationType);
            }

            return result;
        }
    }
}
