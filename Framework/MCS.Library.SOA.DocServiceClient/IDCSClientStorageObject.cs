using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DocServiceClient
{
    public interface IDCSClientStorageObject
    {
        DCSClient Client { get; set; }

        DCTClientFolder Parent { get; }

        int ID
        {
            get;
            set;
        }

        string Uri
        {
            get;
            set;
        }

        string Name
        {
            get;
            set;
        }

        string AbsoluteUri
        {
            get;
            set;
        }
    }
}
