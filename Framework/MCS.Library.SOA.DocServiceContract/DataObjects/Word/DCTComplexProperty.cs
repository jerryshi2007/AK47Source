using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MCS.Library.SOA.DocServiceContract
{
    [DataContract]
    public class DCTComplexProperty : DCTDataProperty
    {
        public DCTComplexProperty()
        {
            dataObjects = new DCTWordDataObjectCollection();
        }


        DCTWordDataObjectCollection dataObjects;

        [DataMember]
        public DCTWordDataObjectCollection DataObjects
        {
            get { return dataObjects; }
            set { dataObjects = value; }
        }
    }
}
