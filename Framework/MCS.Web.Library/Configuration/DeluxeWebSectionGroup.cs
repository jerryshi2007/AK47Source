using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace MCS.Web.Library
{
	/// <summary>
	/// DeluxeWeb������
	/// </summary>
    public sealed class DeluxeWebSectionGroup : ConfigurationSectionGroup
    {
    }

    /// <summary>
    /// �ļ����ýڼ���
    /// </summary>
    public class FilePathConfigElementCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new FilePathConfigElement();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((FilePathConfigElement)element).Path;
        }
    }

    /// <summary>
    /// �ļ����ý�
    /// </summary>
    public class FilePathConfigElement : ConfigurationElement
    {
        /// <summary>
        /// �ļ�·��
        /// </summary>
        [ConfigurationProperty("path", DefaultValue = "")]
        public string Path
        {
            get
            {
                return (string)this["path"];
            }
        }
    }
}
