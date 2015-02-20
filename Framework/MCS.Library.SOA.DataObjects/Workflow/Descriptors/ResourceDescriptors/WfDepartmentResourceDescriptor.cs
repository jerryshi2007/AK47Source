using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MCS.Library.Core;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    [Serializable]
    [XElementSerializable]
    public class WfDepartmentResourceDescriptor : WfResourceDescriptor
    {
        private IOrganization _Department = null;

        public static readonly WfDepartmentResourceDescriptor EmptyInstance = new WfDepartmentResourceDescriptor();

        public WfDepartmentResourceDescriptor()
        {
        }

        public WfDepartmentResourceDescriptor(IOrganization org)
        {
            this._Department = (IOrganization)OguOrganization.CreateWrapperObject(org);
        }

        public IOrganization Department
        {
            get { return this._Department; }
            set { this._Department = (IOrganization)OguOrganization.CreateWrapperObject(value); }
        }

        protected internal override void FillUsers(OguDataCollection<IUser> users)
        {
            if (this._Department != null)
            {
                OguObjectCollection<IUser> children = this._Department.GetAllChildren<IUser>(true);

                children.ForEach(u => users.Add(u));
            }
        }

        protected override void ToXElement(XElement element)
        {
            if (this._Department != null)
                ((ISimpleXmlSerializer)this._Department).ToXElement(element, string.Empty);
        }
    }
}
