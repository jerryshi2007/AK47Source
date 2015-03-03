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
            get;
            set;
        }

        public string DisplayName
        {
            get;
            set;
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
