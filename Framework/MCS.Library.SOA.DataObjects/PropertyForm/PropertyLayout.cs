using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Xml.Linq;

using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects
{
    [Serializable]
    [XElementSerializable]
    public class PropertyLayout : IXElementSerializable
    {
        internal PropertyLayout()
        { }

        public PropertyLayout(PropertyLayoutSectionDefine define)
        {
            this._LayoutSection = define;
        }

        [XElementFieldSerialize(AlternateFieldName = "_LaSe")]
        private PropertyLayoutSectionDefine _LayoutSection;
        public PropertyLayoutSectionDefine LayoutSection
        {
            get { return this._LayoutSection; }
            set { this._LayoutSection = value; }
        }

        public PropertyLayout Clone()
        {
            PropertyLayout newValue = new PropertyLayout(this._LayoutSection);

            return newValue;
        }

        public void Deserialize(XElement node, XmlDeserializeContext context)
        {
            object pv = null;
            int objID = node.Attribute("v", -1);
            if (context.ObjectContext.TryGetValue(objID, out pv) == true)
            {
                this._LayoutSection = ((PropertyLayout)pv).Clone().LayoutSection;
            }
            else
            {
                this._LayoutSection = new PropertyLayoutSectionDefine();
                this._LayoutSection.Deserialize(node, context);

                objID = node.Attribute("id", -1);
                if (objID > -1)
                {
                    context.ObjectContext.Add(objID, this);
                }
            }
        }

        public void Serialize(XElement node, XmlSerializeContext context)
        {
            PropertyLayout pl = this.Clone();
            int objID = 0;

            if (context.ObjectContext.TryGetValue(pl, out objID) == false)
            {
                objID = context.CurrentID;
                context.ObjectContext.Add(pl, objID);
                node.SetAttributeValue("id", context.CurrentID++);
                pl.LayoutSection.Serialize(node, context);
            }
            else
            {
                node.SetAttributeValue("v", objID);
            }
        }
    }
}
