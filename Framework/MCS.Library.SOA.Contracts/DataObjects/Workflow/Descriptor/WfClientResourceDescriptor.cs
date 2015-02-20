using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace MCS.Library.SOA.Contracts.DataObjects.Workflow
{

    [KnownType(typeof(WfClientUserResourceDescriptor))]
    [KnownType(typeof(WfClientGroupResourceDescriptor))]
    [KnownType(typeof(WfClientOrganizationResourceDescriptor))]
    [KnownType(typeof(WfClientRoleResourceDescriptor))]
    [KnownType(typeof(WfClientDepartmentResourceDescriptor))]
    [KnownType(typeof(WfClientDynamicResourceDescriptor))]
    [KnownType(typeof(WfClientActivityAssigneesResourceDescriptor))]
    [DataContract(IsReference = true)]
    [Serializable]
    public class WfClientResourceDescriptor
    {
       
    }
   

    [DataContract(IsReference = true)]
    public class WfClientUserResourceDescriptor : WfClientResourceDescriptor
    {
        private ClientOguUser _User;

        [DataMember]
        public ClientOguUser User
        {
            get
            {
                if (this._User == null)
                    this._User = new ClientOguUser();

                return this._User;
            }
            set
            {
                this._User = value;
            }
        }
    }

    [DataContract(IsReference = true)]
    public class WfClientGroupResourceDescriptor : WfClientResourceDescriptor
    {
        private ClientOguGroup _Group;

        [DataMember]
        public ClientOguGroup Group
        {
            get
            {
                if (this._Group == null)
                    this._Group = new ClientOguGroup();

                return this._Group;
            }
            set
            {
                this._Group = value;
            }
        }
    }

    [DataContract(IsReference = true)]
    public class WfClientOrganizationResourceDescriptor : WfClientResourceDescriptor
    {
        private ClientOguOrganization _Organization;

        [DataMember]
        public ClientOguOrganization Organization
        {
            get
            {
                if (this._Organization == null)
                    this._Organization = new ClientOguOrganization();

                return this._Organization;
            }
            set
            {
                this._Organization = value;
            }
        }
    }

    [DataContract(IsReference = true)]
    public class WfClientRoleResourceDescriptor : WfClientResourceDescriptor
    {
        private ClientOguRole _Role;

        [DataMember]
        public ClientOguRole Role
        {
            get
            {
                if (this._Role == null)
                    this._Role = new ClientOguRole();

                return this._Role;
            }
            set
            {
                this._Role = value;
            }
        }
    }

    [CollectionDataContract(IsReference = true)]
    public class WfClientResourceDescriptorCollection : ClientCollectionBase<WfClientResourceDescriptor>
    {

    }
}
