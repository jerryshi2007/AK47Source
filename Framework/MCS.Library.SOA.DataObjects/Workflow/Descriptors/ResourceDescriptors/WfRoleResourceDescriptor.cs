using System;
using System.Collections.Generic;
using System.Xml.Linq;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Workflow.Builders;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    [Serializable]
    [XElementSerializable]
    public class WfRoleResourceDescriptor : WfResourceDescriptor, IWfCreateActivityParamsGenerator
    {
        private IRole _Role = null;

        public static readonly WfRoleResourceDescriptor EmptyInstance = new WfRoleResourceDescriptor();

        public IRole Role
        {
            get { return this._Role; }
            set { this._Role = (IRole)SOARole.CreateWrapperObject(value); }
        }

        public WfRoleResourceDescriptor() { }

        public WfRoleResourceDescriptor(IRole role)
        {
            role.NullCheck("role");
            this._Role = (IRole)SOARole.CreateWrapperObject(role);
        }

        protected internal override void FillUsers(OguDataCollection<IUser> users)
        {
            if (this._Role != null)
            {
                SOARoleContext.DoAction(this._Role, this.ProcessInstance, (context) =>
                {
                    foreach (IOguObject obj in this._Role.ObjectsInRole)
                    {
                        FillObjectToUsers(users, obj);
                    }
                });
            }
        }

        private void FillObjectToUsers(OguDataCollection<IUser> users, IOguObject obj)
        {
            OguBase wrappedObj = (OguBase)OguBase.CreateWrapperObject(obj);
            wrappedObj.FullPath = obj.FullPath;

            switch (obj.ObjectType)
            {
                case SchemaType.Users:
                    if (users.Exists(u => string.Compare(u.ID, obj.ID, true) == 0) == false)
                        users.Add((IUser)wrappedObj);
                    break;
                case SchemaType.Groups:
                    IGroup group = (IGroup)wrappedObj;
                    group.Members.ForEach(u =>
                    {
                        if (users.Exists(ul => string.Compare(ul.ID, u.ID, true) == 0) == false)
                            users.Add(u);
                    });
                    break;
                case SchemaType.Organizations:
                case SchemaType.OrganizationsInRole:
                    IOrganization dept = (IOrganization)obj;
                    dept.Children.ForEach(o => FillObjectToUsers(users, o));
                    break;
            }
        }

        public void Fill(WfCreateActivityParamCollection capc, PropertyDefineCollection definedProperties)
        {
            definedProperties.NullCheck("definedProperties");

            if (this._Role != null)
            {
                SOARoleContext.DoAction(this._Role, this.ProcessInstance, (context) =>
                    ((SOARole)(this._Role)).FillCreateActivityParams(capc, definedProperties)
                );
            }
        }

        public bool UseCreateActivityParams
        {
            get
            {
                bool result = false;

                if (this._Role != null)
                    result = ((SOARole)(this._Role)).MatrixType == WfMatrixType.ActivityMatrix;

                return result;
            }
        }

        protected override void ToXElement(XElement element)
        {
            if (this._Role != null)
                ((ISimpleXmlSerializer)this._Role).ToXElement(element, string.Empty);
        }
    }
}
