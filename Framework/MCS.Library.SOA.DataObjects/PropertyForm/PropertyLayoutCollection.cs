using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using MCS.Library.Core;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects
{
    [Serializable]
    [XElementSerializable]
	public class PropertyLayoutCollection : SerializableEditableKeyedDataObjectCollectionBase<string, PropertyLayout>, IXElementSerializable
    {
        public PropertyLayoutCollection()
        {
        }

        public void InitFromLayoutSectionCollection(PropertyLayoutSectionCollection propLayoutSections)
        {
            this.Clear();

            AppendFromLayoutSectionCollection(propLayoutSections);
        }

        public void AppendFromLayoutSectionCollection(PropertyLayoutSectionCollection propLayoutSections)
        {
            foreach (PropertyLayoutSectionDefine layoueSection in propLayoutSections)
            {
                PropertyLayout pl = new PropertyLayout(layoueSection);

                this.Add(pl);
            }
        }

        protected override string GetKeyForItem(PropertyLayout item)
        {
            return item.LayoutSection.DisplayName;
        }

        public void Deserialize(XElement node, XmlDeserializeContext context)
        {
           this.Clear();
			var itemNodes = from vNodes in node.Descendants("I")
							select vNodes;
			if (itemNodes.FirstOrDefault() == null)
				itemNodes = from vNodes in node.Descendants("Item")
							select vNodes;
			foreach (XElement itemNode in itemNodes)
			{
                PropertyLayout pl = new PropertyLayout();
				pl.Deserialize(itemNode, context);
				this.Add(pl);

			}
			string idList = node.Attribute("_pvs", string.Empty);
			if (!idList.IsNullOrEmpty())
			{
				string[] idArray = idList.Split(',');
				foreach (string id in idArray)
				{
					XElement element = new XElement("Custom");
					element.SetAttributeValue("v", id);
                    PropertyLayout pl = new PropertyLayout();
					pl.Deserialize(element, context);
					this.Add(pl);
				}
			}
        }

        public void Serialize(XElement node, XmlSerializeContext context)
        {
            List<int> idList = new List<int>(50);
            foreach (PropertyLayout pl in this)
            {
                int objID = 0;
                var newpl = pl.Clone();
            
                if (context.ObjectContext.TryGetValue(newpl, out objID))
                {
                    idList.Add(objID);
                }
                else
                {
                    XElement itemNode = node.AddChildElement("I");
                    newpl.Serialize(itemNode, context);
                }
            }
            if (idList.Count > 0)
                node.SetAttributeValue("_pvs", string.Join(",", idList.ToArray()));
        }
    }
}
