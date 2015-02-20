using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.Globalization;
using System.Runtime.Serialization;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	[Serializable]
	[XElementSerializable]
	public class WfActivityCollection : SerializableEditableKeyedDataObjectCollectionBase<string, IWfActivity>, ISimpleXmlSerializer
	{
		public WfActivityCollection()
		{
		}

		public WfActivityCollection(SerializationInfo info, StreamingContext context) :
			base(info, context)
		{
		}

		/// <summary>
		/// 根据Descriptor的Key找到符合条件的Activity
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public IWfActivity FindActivityByDescriptorKey(string key)
		{
			key.CheckStringIsNullOrEmpty("key");

			return this.Find(act => string.Compare(act.Descriptor.Key, key, true) == 0);
		}

		protected override string GetKeyForItem(IWfActivity item)
		{
			return item.ID;
		}

		protected override void OnValidate(object value)
		{
			if (value != null)
			{
				((IWfActivity)value).ID.IsNotEmpty().FalseThrow<WfRuntimeException>(Translator.Translate(
					WfHelper.CultureCategory, "Activity ID不能为空"));
			}
			base.OnValidate(value);
		}

		#region ISimpleXmlSerializer Members

		void ISimpleXmlSerializer.ToXElement(XElement element, string refNodeName)
		{
			foreach (IWfActivity activity in this)
			{
				XElement actElem = element.AddChildElement("Activity");

				((ISimpleXmlSerializer)activity).ToXElement(actElem, refNodeName);
			}
		}

		#endregion
	}
}
