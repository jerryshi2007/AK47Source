using System;
using System.Text;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;

namespace MCS.Library.Core
{
	/// <summary>
	/// 属性的映射方式，节点或属性
	/// </summary>
	public enum XmlNodeMappingType
	{
		/// <summary>
		/// 
		/// </summary>
		Attribute = 0,

		/// <summary>
		/// 
		/// </summary>
		Entity = 1
	}

	/// <summary>
	/// 
	/// </summary>
	[DebuggerDisplay("PropertyName={propertyName}, NodeName={nodeName}")]
	public class XmlObjectMappingItem
	{
		private string propertyName = string.Empty;
		private string subClassPropertyName = string.Empty;
		private string nodeName = string.Empty;
		private string subClassTypeDescription = string.Empty;
		private MemberInfo memberInfo = null;
		private XmlNodeMappingType nodeMappingType = XmlNodeMappingType.Attribute;

		/// <summary>
		/// 
		/// </summary>
		public XmlObjectMappingItem()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="attr"></param>
		public XmlObjectMappingItem(XmlObjectMappingAttribute attr)
		{
			if (attr != null)
			{
				this.nodeName = attr.NodeName;
				this.nodeMappingType = attr.MappingType;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public string PropertyName
		{
			get { return this.propertyName; }
			set { this.propertyName = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		public string SubClassPropertyName
		{
			get { return this.subClassPropertyName; }
			set { this.subClassPropertyName = value; }
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
		public string SubClassTypeDescription
		{
			get { return this.subClassTypeDescription; }
			set { this.subClassTypeDescription = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		public MemberInfo MemberInfo
		{
			get { return this.memberInfo; }
			set { this.memberInfo = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		public XmlNodeMappingType MappingType
		{
			get { return this.nodeMappingType; }
			set { this.nodeMappingType = value; }
		}
	}
}
