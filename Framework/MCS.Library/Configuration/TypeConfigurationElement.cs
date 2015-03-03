#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library
// FileName	：	TypeConfigurationElement.cs
// Remark	：	类型信息的配置项
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    王翔	    20070430		创建
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
    /// 类型信息的配置项
    /// </summary>
    public class TypeConfigurationElement : NamedConfigurationElement
    {
        private Type typeInfo = null;

        /// <summary>
        /// 类型描述信息
        /// </summary>
        /// <remarks>一般采用QualifiedName （QuanlifiedTypeName, AssemblyName）方式</remarks>
        [ConfigurationProperty("type", IsRequired = true)]
        public virtual string Type
        {
            get
            {
                return (string)this["type"];
            }
        }

        /// <summary>
        /// 建立对象的实例
        /// </summary>
        /// <param name="ctorParams">创建实例的初始化参数</param>
        /// <returns>运用晚绑定方式动态创建一个实例</returns>
        public object CreateInstance(params object[] ctorParams)
        {
            return TypeCreator.CreateInstance(GetTypeInfo(), ctorParams);
        }

        /// <summary>
        ///  建立对象的实例同时进行类型检查
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ctorParams"></param>
        /// <returns></returns>
        public T CreateInstance<T>(params object[] ctorParams)
        {
            return TypeCreator.CreateInstance<T>(GetTypeInfo(), ctorParams);
        }

        /// <summary>
        /// 得到System.Type信息
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
    /// 类型的配置元素集合
    /// </summary>
    public class TypeConfigurationCollection : NamedConfigurationElementCollection<TypeConfigurationElement>
    {
        /// <summary>
        /// 找到指定key的类型配置，并且创建指定类型的实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="autoThrow">是否自动抛出异常</param>
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
