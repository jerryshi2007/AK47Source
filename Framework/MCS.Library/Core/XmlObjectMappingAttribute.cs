using System;
using System.Text;
using System.Collections.Generic;

namespace MCS.Library.Core
{
	/// <summary>
	/// Xml节点(属性)和对象属性之间的映射
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class XmlObjectMappingAttribute : System.Attribute
	{
		private string nodeName = string.Empty;
		private XmlNodeMappingType nodeMappingType = XmlNodeMappingType.Attribute;

		/// <summary>
		/// 
		/// </summary>
		public XmlObjectMappingAttribute()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="nodeName"></param>
		public XmlObjectMappingAttribute(string nodeName)
		{
			this.nodeName = nodeName;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="nodeName"></param>
		/// <param name="mappingType"></param>
		public XmlObjectMappingAttribute(string nodeName, XmlNodeMappingType mappingType)
		{
			this.nodeName = nodeName;
			this.nodeMappingType = mappingType;
		}

		/// <summary>
		/// 
		/// </summary>
		public string NodeName
		{
			get { return this.nodeName; }
			set { this.nodeName = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		public XmlNodeMappingType MappingType
		{
			get { return nodeMappingType; }
			set { nodeMappingType = value; }
		}
	}

	/// <summary>
	/// 进行Mapping时忽略的属性
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class NoXmlObjectMappingAttribute : System.Attribute
	{
	}
}
