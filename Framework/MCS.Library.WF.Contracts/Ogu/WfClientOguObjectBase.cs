using MCS.Library.Passport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Ogu
{
    [Serializable]
    [DataContract]
    public abstract class WfClientOguObjectBase : ITicketToken
    {
        private string _Name = null;
        private string _DisplayName = null;

        public WfClientOguObjectBase(ClientOguSchemaType schemaType)
        {
            this.ObjectType = schemaType;
        }

        public WfClientOguObjectBase(string id, ClientOguSchemaType schemaType)
            : this(schemaType)
        {
            this.ID = id;
        }

        public WfClientOguObjectBase(string id, string name, ClientOguSchemaType schemaType)
            : this(schemaType)
        {
            this.ID = id;
            this.Name = name;
        }

        public ClientOguSchemaType ObjectType
        {
            get;
            protected set;
        }

        public string ID
        {
            get;
            set;
        }

        public string Name
        {
            get
            {
                if (this._Name == null)
                    this._Name = this._DisplayName;

                return this._Name;
            }
            set
            {
                this._Name = value;
            }
        }

        public string DisplayName
        {
            get
            {
                if (this._DisplayName == null)
                    this._DisplayName = this._Name;

                return this._DisplayName;
            }
            set
            {
                this._DisplayName = value;
            }
        }

        public static WfClientOguObjectBase CreateWrapperObject(string id, string name, ClientOguSchemaType schemaType)
        {
            WfClientOguObjectBase result = null;

            switch (schemaType)
            {
                case ClientOguSchemaType.Organizations:
                    result = new WfClientOrganization(id, name);
                    break;
                case ClientOguSchemaType.Users:
                    result = new WfClientUser(id, name);
                    break;
                case ClientOguSchemaType.Groups:
                    result = new WfClientGroup(id, name);
                    break;
                default:
                    throw new ApplicationException(string.Format("schemaType错误{0}", schemaType));
            }

            return result;
        }
    }
}
