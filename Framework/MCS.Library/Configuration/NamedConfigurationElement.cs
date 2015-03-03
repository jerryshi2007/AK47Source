#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library
// FileName	��	NamedConfigurationElement.cs
// Remark	��	Uri���߼�����
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
using MCS.Library.Properties;

namespace MCS.Library.Configuration
{
	/// <summary>
	/// ������Ϊ��ֵ��������
	/// </summary>
	public class NamedConfigurationElement : ConfigurationElement
	{
		/// <summary>
		/// Uri���߼�����
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
		/// ����
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
		/// ����true����Ҫ�ǵ������ļ�����֮��Ϊ�˱��ּ�����
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
		{
			return true;
		}

		/// <summary>
		/// ����true����Ҫ�ǵ������ļ�����֮��Ϊ�˱��ּ�����
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
	/// ������Ϊ��ֵ������Ԫ�ؼ���
	/// </summary>
	/// <typeparam name="T">����Ԫ������</typeparam>
	public abstract class NamedConfigurationElementCollection<T> : ConfigurationElementCollection
		where T : NamedConfigurationElement, new()
	{
		/// <summary>
		/// ������Ż�ȡָ��������Ԫ��
		/// </summary>
		/// <param name="index">���</param>
		/// <returns>����Ԫ��</returns>
		public virtual T this[int index] { get { return (T)base.BaseGet(index); } }

		/// <summary>
		/// �������ƻ�ȡָ��������Ԫ��
		/// </summary>
		/// <param name="name">����</param>
		/// <returns>����Ԫ��</returns>
		public new T this[string name] { get { return (T)base.BaseGet(name); } }

		/// <summary>
		/// �Ƿ����ָ��������Ԫ��
		/// </summary>
		/// <param name="name">����Ԫ������</param>
		/// <returns>�Ƿ����</returns>
		public bool ContainsKey(string name) { return BaseGet(name) != null; }

		/// <summary>
		/// ��鲢�һ�ȡ��Ӧ�����������ڣ����׳��쳣
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public T CheckAndGet(string name)
		{
            return this.CheckAndGet(name, true);
		}

        /// <summary>
        /// ��鲢�һ�ȡ��Ӧ�����������ڣ����׳��쳣
        /// </summary>
        /// <param name="name"></param>
        /// <param name="autoThrow">�Ƿ��Զ��׳��쳣</param>
        /// <returns></returns>
        public T CheckAndGet(string name, bool autoThrow)
        {
            if (this.ContainsKey(name) == false && autoThrow)
                throw new ConfigurationErrorsException(string.Format("�����ҵ�����Ϊ{0}��Ԫ��", name));

            return this[name];
        }

		/// <summary>
		/// �õ�Ԫ�ص�Keyֵ
		/// </summary>
		/// <param name="element">����Ԫ��</param>
		/// <returns>����Ԫ������Ӧ������Ԫ��</returns>
		protected override object GetElementKey(ConfigurationElement element) { return ((T)element).Name; }

		/// <summary>
		/// �����µ�����Ԫ��ʵ��
		/// </summary>
		/// <returns>����Ԫ��ʵ��</returns>
		protected override ConfigurationElement CreateNewElement() { return new T(); }

		/// <summary>
		/// ͨ��name���ֵ��ڲ������ݣ����name�����ڣ����׳��쳣��������������
		/// </summary>
		/// <param name="name">����</param>
		/// <returns>����Ԫ��</returns>
		protected virtual T InnerGet(string name)
		{
			object element = base.BaseGet(name);
			ExceptionHelper.FalseThrow<KeyNotFoundException>(element != null, Resource.CanNotFindNamedConfigElement, name);
			return (T)element;
		}
	}
}
