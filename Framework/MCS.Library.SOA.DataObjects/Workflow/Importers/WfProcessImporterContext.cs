using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MCS.Library.SOA.DataObjects.Workflow.Importers
{
    internal class WfProcessImporterContext
    {
        public const string WF_PROCESS_SUFFIX = "_proc.xml";
        public const string WF_MATRIX_SUFFIX = "_mtrx.xlsx";
        public const string WF_MATRIXDEF_SUFFIX = "_mtrxdef.xml";
        public const string WF_MAPPING = "mapping.map";

        private List<PackagePart> _MatrixDefParts = new List<PackagePart>();
        private List<PackagePart> _MatrixParts = new List<PackagePart>();
        private List<PackagePart> _ProcessDescParts = new List<PackagePart>();
        private Dictionary<string, WfPackageRelationMapping> _MappingInfo = new Dictionary<string, WfPackageRelationMapping>();
        private Dictionary<string, WfMatrixDefinition> _MatrixDefinitions = new Dictionary<string, WfMatrixDefinition>();

        public WfProcessImporterContext(IEnumerable<PackagePart> packageParts)
        {
            InitRelativeParts(packageParts);
            InitMappings(packageParts);
            InitMatrixDefinitions(this.MatrixDefParts);
        }

        public List<PackagePart> MatrixDefParts
        {
            get
            {
                return this._MatrixDefParts;
            }
        }

        public List<PackagePart> MatrixParts
        {
            get
            {
                return this._MatrixParts;
            }
        }

        public List<PackagePart> ProcessDescParts
        {
            get
            {
                return this._ProcessDescParts;
            }
        }

        public Dictionary<string, WfPackageRelationMapping> MappingInfo
        {
            get
            {
                return this._MappingInfo;
            }
        }

        public Dictionary<string, WfMatrixDefinition> MatrixDefinitions
        {
            get
            {
                return this._MatrixDefinitions;
            }
        }

        private void InitRelativeParts(IEnumerable<PackagePart> packageParts)
        {
            foreach (PackagePart part in packageParts)
            {
                if (MatchAndFillPart(part, this.ProcessDescParts, WF_PROCESS_SUFFIX))
                    continue;

                if (MatchAndFillPart(part, this.MatrixDefParts, WF_MATRIXDEF_SUFFIX))
                    continue;

                if (MatchAndFillPart(part, this.MatrixParts, WF_MATRIX_SUFFIX))
                    continue;
            }
        }

        private static bool MatchAndFillPart(PackagePart sourcePart, IList<PackagePart> targetParts, string matchString)
        {
            bool matched = sourcePart.Uri.ToString().EndsWith(matchString);

            if (matched)
                targetParts.Add(sourcePart);

            return matched;
        }

        private void InitMappings(IEnumerable<PackagePart> packageParts)
        {
            var mapPart = packageParts.FirstOrDefault(p => p.Uri.ToString().EndsWith(WF_MAPPING));

            if (mapPart != null)
            {
                using (Stream stream = mapPart.GetStream())
                {
                    XDocument doc = XDocument.Load(stream);

                    foreach (XElement node in doc.Root.Nodes())
                    {
                        WfPackageRelationMapping mapping = new WfPackageRelationMapping(node);

                        this.MappingInfo.Add(mapping.MatrixPath, mapping);
                    }
                }
            }
        }

        private void InitMatrixDefinitions(IEnumerable<PackagePart> matrixDefParts)
        {
            XElementFormatter formatter = new XElementFormatter();
            formatter.OutputShortType = false;

            foreach (PackagePart part in matrixDefParts)
            {
                XDocument xDoc = XDocument.Load(part.GetStream());
                WfMatrixDefinition matrixDef = (WfMatrixDefinition)formatter.Deserialize(xDoc.Root);

                this.MatrixDefinitions.Add(matrixDef.Key, matrixDef);
            }
        }
    }
}
