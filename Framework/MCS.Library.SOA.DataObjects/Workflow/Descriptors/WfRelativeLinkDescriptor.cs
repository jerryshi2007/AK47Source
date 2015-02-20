using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 相关链接的实现类
	/// </summary>
	[Serializable]
	[XElementSerializable]
	public class WfRelativeLinkDescriptor : WfKeyedDescriptorBase, IWfRelativeLinkDescriptor, ISimpleXmlSerializer
	{
		public WfRelativeLinkDescriptor()
		{
		}

		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="key"></param>
		public WfRelativeLinkDescriptor(string key)
			: base(key)
		{
		}

		#region IWfRelativeLinkDescriptor Members

		public string Url
		{
			get;
			set;
		}

		public string Category
		{
			get;
			set;
		}

		#endregion


		#region ISimpleXmlSerializer Members

		void ISimpleXmlSerializer.ToXElement(XElement element, string refNodeName)
		{
			if (refNodeName.IsNotEmpty())
				element = element.AddChildElement(refNodeName);

			element.SetAttributeValue("key", this.Key);
			element.SetAttributeValue("category", this.Category);
			element.SetAttributeValue("url", this.Url);
		}

		#endregion
	}

	/// <summary>
	/// 相关链接的集合
	/// </summary>
	[Serializable]
	[XElementSerializable]
	public class WfRelativeLinkDescriptorCollection : WfKeyedDescriptorCollectionBase<IWfRelativeLinkDescriptor>, ISimpleXmlSerializer
	{
		public WfRelativeLinkDescriptorCollection()
			: base(null)
		{
		}

		public WfRelativeLinkDescriptorCollection(IWfDescriptor owner)
			: base(owner)
		{
		}

		protected WfRelativeLinkDescriptorCollection(SerializationInfo info, StreamingContext context) :
			base(info, context)
		{
		}

		public WfRelativeLinkDescriptorCollection FilterByCategory(string category)
		{
			category.CheckStringIsNullOrEmpty("category");

			WfRelativeLinkDescriptorCollection result = new WfRelativeLinkDescriptorCollection();

			result.CopyFrom(this.FindAll(link => link.Category == category));

			return result;
		}

		#region ISimpleXmlSerializer Members

		void ISimpleXmlSerializer.ToXElement(XElement element, string refNodeName)
		{
			if (refNodeName.IsNotEmpty())
				element = element.AddChildElement(refNodeName);

			foreach (IWfRelativeLinkDescriptor relLink in this)
				((ISimpleXmlSerializer)relLink).ToXElement(element, "RelativeLink");
		}

		#endregion
	}
}
