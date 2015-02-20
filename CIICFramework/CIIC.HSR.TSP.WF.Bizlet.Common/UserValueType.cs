using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CIIC.HSR.TSP.WF.Bizlet.Common
{
    [Serializable]
    [GeneratedCode("System.Xml", "2.0.50727.312")]
    [XmlType(Namespace = "http://tempuri.org/")]
    public enum UserValueType
    {
        LogonName = 1,
        AllPath = 2,
        PersonID = 3,
        ICCode = 4,
        Guid = 8,
        Identity = 16,
    }
}
