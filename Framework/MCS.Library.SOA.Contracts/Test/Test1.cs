using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace MCS.Library.SOA.Contracts.DataObjects
{
   [CollectionDataContract(IsReference=true)]
    public class ClientStringCollection : EditableKeyedDataObjectCollectionBase<string, ClientPropertyValue>
    {
        protected override string GetKeyForItem(ClientPropertyValue item)
        {
            return item.Definition.Name;
        }
    }
    [DataContract(IsReference = true)]
    public class Main
    {
        private Sub _sub;
        public Main()
        {
            this._sub = new Sub();
            
            this._sub.Main = this;
            this._sub.Table = new ArrayList();
            //this._sub.Table.Add(new Sub());
            //this._sub.Table.Add(new Sub());
            //this._sub.Table.Add(new Sub());
        }
       [DataMember]
      
        public Sub Sub
        {
            get { return _sub; }
            set{_sub=value;}
        }
    }
    [DataContract(IsReference = true)]
    public class Sub
    {
        private ArrayList  _hs;
        private Main _Main;
       [DataMember]
        public Main Main
        {
            get { return _Main; }
            set { _Main = value; }
        }

        [DataMember]
       public ArrayList Table
        {
            get { return _hs; }
            set { _hs = value; }
        }

    }
}