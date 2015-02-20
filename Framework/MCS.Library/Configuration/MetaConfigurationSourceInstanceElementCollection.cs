#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library
// FileName	：	MetaConfigurationSourceInstanceElementCollection.cs
// Remark	：	SourceMappings instances configuratoin element collection
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    王翔	    20070430		创建
// -------------------------------------------------
#endregion
#region using
using System;
using System.Collections.Generic;
using System.Text;

using System.Configuration;
using System.Diagnostics;

using MCS.Library.Core;
using MCS.Library.Configuration.Accessories;
#endregion
namespace MCS.Library.Configuration
{
    /// <summary>
    /// SourceMappings实例的配置元素集合
    /// </summary>
    [ConfigurationCollection(typeof(MetaConfigurationSourceInstanceElement),
    CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
    class MetaConfigurationSourceInstanceElementCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Public const
        /// </summary>
        public const string Name = "instances";

        /// <summary>
        /// 由索引获取MetaConfigurationSourceInstanceElement
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public MetaConfigurationSourceInstanceElement this[int index]
        {
            get
            {
                return (MetaConfigurationSourceInstanceElement)base.BaseGet(index);
            }
        }

        /// <summary>
        /// 由名称获取MetaConfigurationSourceInstanceElement
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public new MetaConfigurationSourceInstanceElement this[string name]
        {
            get
            {
                return base.BaseGet(name) as MetaConfigurationSourceInstanceElement;
            }
        }

        /// <summary>
        /// 根据应用的类型过滤实例的配置
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public IEnumerable<MetaConfigurationSourceInstanceElement> FilterByInstanceMode(InstanceMode mode)
        {
            List<MetaConfigurationSourceInstanceElement> result = new List<MetaConfigurationSourceInstanceElement>();

            foreach (MetaConfigurationSourceInstanceElement inst in this)
            {
                if (inst.GetMode() == mode)
                    result.Add(inst);
            }

            return result;
        }

        /// <summary>
        /// 创建新的配置元素实例
        /// </summary>
        /// <returns></returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new MetaConfigurationSourceInstanceElement();
        }

        /// <summary>
        /// 由键值获取配置元素
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as MetaConfigurationSourceInstanceElement).Name;
        }

        /// <summary>
        /// 参数appPath为外面传入访问地址的绝对路径
        /// mode属性是用来区别web访问和winForm类型的，程序只对匹配mode下的配置型进行匹配
        /// 以减少匹配的工作量，提高匹配的效率
        /// 返回值是匹配成功的全局配置文件的路径和文件名称（即匹配项对应的path属性的值）
        /// 如果未找到匹配的全局配置文件，则返回值为""
        /// </summary>
        /// <param name="appPath">(疑问地点？？？？？？不知道该参数是何用途)</param>
        /// <returns>匹配成功的全局配置文件的路径和文件名称（即匹配项对应的path属性的值）</returns>
        public string GetMatchedPath(string appPath)
        {
            string path = string.Empty;
            PathMatchStrategyContext context = new PathMatchStrategyContext();

            // 对于 windows 应用，如果有匹配的 assembly 内容，则直接使用文件内容匹配
            if (EnvironmentHelper.Mode == InstanceMode.Windows)
            {
                context.Strategy = new BestFileNameMatchStrategy(this);
                path = context.DoAction();

                if (path.IsNotEmpty())
                    return path;
            }

            // 如果非 windows 应用，或者 windows 应用中没有匹配的 assembly 指定，则采用目录匹配算法
            context.Strategy = new BestDirectoryMatchStrategy(this);
            path = context.DoAction();

            return path;
        }
    }
}
