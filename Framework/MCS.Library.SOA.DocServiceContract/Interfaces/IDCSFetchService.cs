using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using MCS.Library.SOA.DocServiceContract;

namespace MCS.Library.SOA.DocServiceContract
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IDCSFetchService" in both code and config file together.
    [ServiceContract]
    public interface IDCSFetchService
    {
        [OperationContract]
        BaseCollection<DCTFile> DCMQueryDocByField(BaseCollection<DCTFileField> fields);

        [OperationContract]
        BaseCollection<DCTFile> DCMQueryDocByCaml(string camlText);

        [OperationContract]
        BaseCollection<DCTSearchResult> DCMSearchDoc(string[] keyWords);
    }
}
