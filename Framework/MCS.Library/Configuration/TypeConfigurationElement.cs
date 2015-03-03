#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library
// FileName	��	TypeConfigurationElement.cs
// Remark	��	������Ϣ��������
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ����	    20070430		����
// -------------------------------------------------
#endregion
using System;
using System.Text;
using System.Configuration;
using System.Collections.Generic;
using MCS.Library.Core;

namespace MCS.Library.Configuration
{
    /// <summary>
    /// ������Ϣ��������
    /// </summary>
    public class TypeConfigurationElement : NamedConfigurationElement
    {
        private Type typeInfo = null;

        /// <summary>
        /// ����������Ϣ
        /// </summary>
        /// <remarks>һ�����QualifiedName ��QuanlifiedTypeName, AssemblyName����ʽ</remarks>
        [ConfigurationProperty("type", IsRequired = true)]
        public virtual string Type
        {
            get
            {
                return (string)this["type"];
            }
        }

        /// <summary>
        /// ���������ʵ��
        /// </summary>
        /// <param name="ctorParams">����ʵ���ĳ�ʼ������</param>
        /// <returns>������󶨷�ʽ��̬����һ��ʵ��</returns>
        public object CreateInstance(params object[] ctorParams)
        {
            return TypeCreator.CreateInstance(GetTypeInfo(), ctorParams);
        }

        /// <summary>
        ///  ���������ʵ��ͬʱ�������ͼ��
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ctorParams"></param>
        /// <returns></returns>
        public T CreateInstance<T>(params object[] ctorParams)
        {
            return TypeCreator.CreateInstance<T>(GetTypeInfo(), ctorParams);
        }

        /// <summary>
        /// �õ�System.Type��Ϣ
        /// </summary>
        /// <returns></returns>
        public Type GetTypeInfo()
        {
            if (this.typeInfo == null)
            {
                if (this.Type.IsNotEmpty())
                    this.typeInfo = TypeCreator.GetTypeInfo(this.Type);
            }

            return this.typeInfo;
        }
    }

    /// <summary>
    /// ���͵�����Ԫ�ؼ���
    /// </summary>
    public class TypeConfigurationCollection : NamedConfigurationElementCollection<TypeConfigurationElement>
    {
        /// <summary>
        /// �ҵ�ָ��key���������ã����Ҵ���ָ�����͵�ʵ��
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="autoThrow">�Ƿ��Զ��׳��쳣</param>
        /// <returns></returns>
        public T CheckAndGetInstance<T>(string key, bool autoThrow = true) where T : class
        {
            TypeConfigurationElement element = this.CheckAndGet(key, autoThrow);

            T instance = default(T);

            if (element != null)
                instance = (T)element.CreateInstance();

            return instance;
        }
    }
}
