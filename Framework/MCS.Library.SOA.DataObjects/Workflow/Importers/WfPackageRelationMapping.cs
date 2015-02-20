using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MCS.Library.SOA.DataObjects.Workflow.Importers
{
    public class WfPackageRelationMapping
    {
        public WfPackageRelationMapping()
        {
        }

        public WfPackageRelationMapping(XElement node)
        {
            this.MatrixDefID = node.AttributeValue("matrixDefID");
            this.MatrixPath = node.AttributeValue("matrixID");
            this.ProcessDescriptionID = node.AttributeValue("processDescID");
        }

        public string MatrixPath
        {
            get;
            set;
        }

        public string MatrixDefID
        {
            get;
            set;
        }

        public string ProcessDescriptionID
        {
            get;
            set;
        }
    }
}
