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
    [Flags]
    [GeneratedCode("System.Xml", "2.0.50727.312")]
    [XmlType(Namespace = "http://tempuri.org/")]
    public enum SidelineMaskType
    {
        NotSideline = 1,
        Sideline = 2,
        All = 4,
    }
}
