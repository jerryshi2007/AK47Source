#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library
// FileName	：	NamedConfigurationElement.cs
// Remark	：	Uri的逻辑名称
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
using MCS.Library.Properties;

namespace MCS.Library.Configuration
{
	/// <summary>
	/// 以名字为键值的配置项
	/// </summary>
	public class NamedConfigurationElement : ConfigurationElement
	{
		/// <summary>
		/// Uri的逻辑名称
		/// </summary>
		[ConfigurationProperty("name", IsRequired = true, IsKey = true)]
		public virtual string Name
		{
			get
			{
				return (string)this["name"];
			}
		}

		/// <summary>
		/// 描述
		/// </summary>
		[ConfigurationProperty("description", DefaultValue = "")]
		public virtual string Description
		{
			get
			{
				return (string)this["description"];
			}
		}

		/// <summary>
		/// 返回true。主要是当配置文件改了之后为了保持兼容性
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
		{
			return true;
		}

		/// <summary>
		/// 返回true。主要是当配置文件改了之后为了保持兼容性
		/// </summary>
		/// <param name="elementName"></param>
		/// <param name="reader"></param>
		/// <returns></returns>
		protected override bool OnDeserializeUnrecognizedElement(string elementName, System.Xml.XmlReader reader)
		{
			return true;
		}
	}

	/// <summary>
	/// 以名字为键值的配置元素集合
	/// </summary>
	/// <typeparam name="T">配置元素类型</typeparam>
	public abstract class NamedConfigurationElementCollection<T> : ConfigurationElementCollection
		where T : NamedConfigurationElement, new()
	{
		/// <summary>
		/// 按照序号获取指定的配置元素
		/// </summary>
		/// <param name="index">序号</param>
		/// <returns>配置元素</returns>
		public virtual T this[int index] { get { return (T)base.BaseGet(index); } }

		/// <summary>
		/// 按照名称获取指定的配置元素
		/// </summary>
		/// <param name="name">名称</param>
		/// <returns>配置元素</returns>
		public new T this[string name] { get { return (T)base.BaseGet(name); } }

		/// <summary>
		/// 是否包含指定的配置元素
		/// </summary>
		/// <param name="name">配置元素名称</param>
		/// <returns>是否包含</returns>
		public bool ContainsKey(string name) { return BaseGet(name) != null; }

		/// <summary>
		/// 检查并且获取对应的项，如果不存在，则抛出异常
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public T CheckAndGet(string name)
		{
			if (this.ContainsKey(name) == false)
				throw new ConfigurationErrorsException(string.Format("不能找到名称为{0}的元素", name));

			return this[name];
		}

		/// <summary>
		/// 得到元素的Key值
		/// </summary>
		/// <param name="element">配置元素</param>
		/// <returns>配置元素所对应的配置元素</returns>
		protected override object GetElementKey(ConfigurationElement element) { return ((T)element).Name; }

		/// <summary>
		/// 生成新的配置元素实例
		/// </summary>
		/// <returns>配置元素实例</returns>
		protected override ConfigurationElement CreateNewElement() { return new T(); }

		/// <summary>
		/// 通过name在字典内查找数据，如果name不存在，则抛出异常，本方法可重载
		/// </summary>
		/// <param name="name">名称</param>
		/// <returns>配置元素</returns>
		protected virtual T InnerGet(string name)
		{
			object element = base.BaseGet(name);
			ExceptionHelper.FalseThrow<KeyNotFoundException>(element != null, Resource.CanNotFindNamedConfigElement, name);
			return (T)element;
		}
	}
}
