using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DocServiceContract.DataObjects
{
        
    [Serializable]
    [Flags]
    public enum DCTWordPropertyType
    {

        DCTSimpleProperty = 0,

        DCTComplexProperty = 1,

    }
}
