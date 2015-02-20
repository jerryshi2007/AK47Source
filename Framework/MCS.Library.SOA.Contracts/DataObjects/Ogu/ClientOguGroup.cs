using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MCS.Library.SOA.Contracts.DataObjects
{
    [DataContract(IsReference = true)]
    public class ClientOguGroup : ClientOguObjectBase, IClientGroup
    {

        private ClientOguObjectCollection<ClientOguUser> _members;
        #region IGroup 成员
        [DataMember]
        public ClientOguObjectCollection<ClientOguUser> Members
        {
            get
            {
                if (_members == null)
                    _members = new ClientOguObjectCollection<ClientOguUser>();
                return _members;
            }
            set
            {
                _members = value;
            }
        }


        #endregion

     
    }
}
