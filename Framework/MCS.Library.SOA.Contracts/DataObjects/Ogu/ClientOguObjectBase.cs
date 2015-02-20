using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Runtime.Serialization;

namespace MCS.Library.SOA.Contracts.DataObjects
{
   
    [KnownType(typeof(ClientOguGroup))]
    [KnownType(typeof(ClientOguUser))]
    [KnownType(typeof(ClientOguOrganization))]
    [KnownType(typeof(ClientOguRole))]
    [DataContract(IsReference = true)]
    public class ClientOguObjectBase : IClientOguObject
    {
        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string DisplayName { get; set; }

        [DataMember]
        public string FullPath { get; set; }

        [DataMember]
        public string GlobalSortID { get; set; }

        [DataMember]
        public string ID { get; set; }

        [DataMember]
        public int Levels { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public ClientObjectSchemaType ObjectType { get; set; }

      
        [DataMember]
        public string Tag { get; set; }
    }

    [CollectionDataContract(IsReference = true)]
    public class ClientOguObjectCollection<T> : EditableKeyedDataObjectCollectionBase<string, T> where T : IClientOguObject
    {
        public ClientOguObjectCollection()
        { }
        protected override string GetKeyForItem(T item)
        {
            return item.ID;
        }
    }
}
